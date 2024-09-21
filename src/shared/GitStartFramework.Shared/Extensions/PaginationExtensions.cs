using GitStartFramework.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GitStartFramework.Shared.Extensions
{
    public static class PaginationExtensions
    {
        public static PaginatedResult<T> GetPaged<T>(this IQueryable<T> query, int page, int pageSize) where T : class
        {
            var result = new PaginatedResult<T>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.TotalItems = query.Count();
            result.TotalPages = (int)Math.Ceiling(result.TotalItems / (double)pageSize);

            var skip = (page - 1) * pageSize;
            result.Items = query.Skip(skip).Take(pageSize).ToList();

            return result;
        }

        public static PaginatedResult<T> GetPaged<T>(this IEnumerable<T> query, int page, int pageSize) where T : class
        {
            var result = new PaginatedResult<T>
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = query.Count()
            };
            result.TotalPages = (int)Math.Ceiling(result.TotalItems / (double)pageSize);

            var skip = (page - 1) * pageSize;
            result.Items = query.Skip(skip).Take(pageSize).ToList();

            return result;
        }
    }
}