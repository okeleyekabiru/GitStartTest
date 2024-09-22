using Authentication.API.Domain.Entities;
using Authentication.API.Model;
using Authentication.API.Repository;
using GitStartFramework.Shared.Exceptions;
using GitStartFramework.Shared.Model;
using Microsoft.AspNetCore.Identity;

namespace Authentication.API.Services
{
    public interface IUserService
    {
        Task<Response<AuthResponse>> LoginUserAsync(string email, string password);

        Task<Response<User>> UpdateUserAsync(Guid userId, string userName, string email, string newPassword);

        Task<Response<bool>> DeleteUserAsync(Guid userId);

        Task<Response<User>> RegisterUserAsync(string userName, string email, string password);
    }

    public class UserService(IPasswordHasher<User> passwordHasher, IUserRepository userRepository, ITokenService tokenService) : IUserService
    {
        public async Task<Response<User>> RegisterUserAsync(string userName, string email, string password)
        {
            if (await IsExistingUser(userRepository, email))
            {
                throw new BadRequestException("Email is already registered.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = userName,
                Email = email,
                DateCreated = DateTime.UtcNow,
                IsActive = true
            };

            user.PasswordHash = passwordHasher.HashPassword(user, password);

            await userRepository.AddAsync(user);

            return Response<User>.Success(user);
        }

        public async Task<Response<AuthResponse>> LoginUserAsync(string email, string password)
        {
            var user = await userRepository.GetUserByEmailAsync(email);

            if (user == null || !passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password).Equals(PasswordVerificationResult.Success))
            {
                throw new BadRequestException("Invalid email or password.");
            }

            var token = tokenService.GenerateToken(user);
            return Response<AuthResponse>.Success(new AuthResponse
            {
                AuthToken = token,
            });
        }

        public async Task<Response<User>> UpdateUserAsync(Guid userId, string userName, string email, string newPassword)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            // Check if the email is already used by another user
            if (await IsExistingUser(userRepository, email) && user.Email != email)
            {
                throw new BadRequestException("Email is already registered by another user.");
            }

            // Update user details
            user.Username = userName ?? user.Username;
            user.Email = email ?? user.Email;

            // If a new password is provided, hash and update it
            if (!string.IsNullOrEmpty(newPassword))
            {
                user.PasswordHash = passwordHasher.HashPassword(user, newPassword);
            }

            await userRepository.UpdateAsync(user);
            return Response<User>.Success(user);
        }

        public async Task<Response<bool>> DeleteUserAsync(Guid userId)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }
            await userRepository.DeleteAsync(user);

            return Response<bool>.Success(true);
        }

        private static async Task<bool> IsExistingUser(IUserRepository userRepository, string email)
        {
            return await userRepository.AnyAsync(e => e.Email == email);
        }
    }
}