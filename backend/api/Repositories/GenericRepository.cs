using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using api.Data;
using api.Interfaces;
using api.Interfaces.Common;
using api.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _db;
        protected readonly DbSet<T> _set;
        public GenericRepository(AppDbContext db)
        {
            _db = db;
            _set = _db.Set<T>();
        }
        public async Task AddAsync(T entity)
        {
            await _set.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _set.AddRangeAsync(entities);
        }

        public async Task<int> CountAsync()
        {
            return await _set.CountAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _set.FindAsync(id) != null;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            bool asNoTracking = true,
            CancellationToken cancellationToken = default,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _set;

            if (asNoTracking)
                query = query.AsNoTracking();

            if (includes is { Length: > 0 })
                query = ApplyIncludes(query, includes);

            if (predicate is not null)
                query = query.Where(predicate);

            if (orderBy is not null)
                query = orderBy(query);

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<T?> GetAsync(
            Expression<Func<T, bool>> predicate,
            bool asNoTracking = true,
            CancellationToken cancellationToken = default,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _set;

            if (asNoTracking)
                query = query.AsNoTracking();

            if (includes is { Length: > 0 })
                query = ApplyIncludes(query, includes);

            return await query.FirstOrDefaultAsync(predicate, cancellationToken);
        }


        public void Remove(T entity)
        {
            _set.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _set.RemoveRange(entities);
        }

        public void Update(T entity)
        {
            _set.Update(entity);
        }

        private static IQueryable<T> ApplyIncludes(IQueryable<T> query, params Expression<Func<T, object>>[] includes)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query;
        }

    }
}