using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Interfaces;
using api.Interfaces.Common;

namespace api.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;

        private readonly ITodoRepository _todoRepository;
        public ITodoRepository TodoRepository => _todoRepository;

        public UnitOfWork(AppDbContext db, ITodoRepository todoRepository)
        {
            _db = db;
            _todoRepository = todoRepository;
        }


        public ValueTask DisposeAsync()
        {
            return _db.DisposeAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }
    }
}