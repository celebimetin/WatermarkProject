using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AzureStorageLibrary
{
    public interface INoSqlStorage<Tentity>
    {
        Task<Tentity> AddAsync(Tentity entity);
        Task<Tentity> UpdateAsync(Tentity entity);
        Task DeleteAsync(string rowKey, string partitionKey);
        Task<Tentity> GetAsync(string rowKey, string partitionKey);
        IQueryable<Tentity> GetAll();
        IQueryable<Tentity> Query(Expression<Func<Tentity, bool>> exception);
    }
}