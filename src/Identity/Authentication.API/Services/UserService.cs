using Authentication.API.Domain.Entities;
using Authentication.API.Model;
using Authentication.API.Repository;
using GitStartFramework.Shared.Exceptions;
using GitStartFramework.Shared.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Authentication.API.Services
{
    public interface IUserService
    {
        Task<Response<AuthResponse>> LoginUserAsync(string email, string password);

        Task<Response<User>> RegisterUserAsync(string userName, string email, string password);
    }

    public class UserService(IPasswordHasher<User> passwordHasher, IUserRepository userRepository, ITokenService tokenService) : IUserService
    {
        public async Task<Response<User>> RegisterUserAsync(string userName, string email, string password)
        {
            if (await IsExistingUser(userRepository, email))
            {
                throw new BadHttpRequestException("Email is already registered.");
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

        private static async Task<bool> IsExistingUser(IUserRepository userRepository, string email)
        {
            return await userRepository.AnyAsync(e => e.Email == email);
        }
    }
}