using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Entities;
using api.Interfaces.Common;
using api.Repositories;

namespace api.Interfaces
{
    public interface ITodoRepository : IGenericRepository<TodoItem>
    {
        IQueryable<TodoItem> QueryByUser(string userId);
    }
}