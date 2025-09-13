using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Interfaces.Common
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        ITodoRepository TodoRepository { get; }
        Task<int> SaveChangesAsync();
    }
}