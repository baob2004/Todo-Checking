using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Entities;
using api.Interfaces;

namespace api.Repositories
{
    public class TodoRepository : GenericRepository<TodoItem>, ITodoRepository
    {

        public TodoRepository(AppDbContext db) : base(db)
        {
        }

        public IQueryable<TodoItem> QueryByUser(string userId)
        {
            return _set.Where(t => t.UserId == userId).AsQueryable();
        }
    }
}