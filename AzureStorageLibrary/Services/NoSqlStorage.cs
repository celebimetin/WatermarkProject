using Microsoft.Azure.Cosmos.Table;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AzureStorageLibrary.Services
{
    public class NoSqlStorage<Tentity> : INoSqlStorage<Tentity> where Tentity : TableEntity, new()
    {
        private readonly CloudTableClient _client;
        private readonly CloudTable _table;

        public NoSqlStorage()
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(ConnectionStrings.AzureStorageConnectionString);

            _client = cloudStorageAccount.CreateCloudTableClient();
            _table = _client.GetTableReference(typeof(Tentity).Name);
            _table.CreateIfNotExistsAsync();
        }

        public async Task<Tentity> AddAsync(Tentity entity)
        {
            var operation = TableOperation.InsertOrMerge(entity);
            var execute = await _table.ExecuteAsync(operation);
            return execute.Result as Tentity;
        }

        public async Task DeleteAsync(string rowKey, string partitionKey)
        {
            var entity = await GetAsync(rowKey, partitionKey);
            var operation = TableOperation.Delete(entity);
            await _table.ExecuteAsync(operation);
        }

        public async Task<Tentity> GetAsync(string rowKey, string partitionKey)
        {
            var operation = TableOperation.Retrieve<Tentity>(partitionKey, rowKey);
            var execute = await _table.ExecuteAsync(operation);
            return execute.Result as Tentity;
        }

        public IQueryable<Tentity> GetAll()
        {
            return _table.CreateQuery<Tentity>().AsQueryable();
        }

        public IQueryable<Tentity> Query(Expression<Func<Tentity, bool>> exception)
        {
            return _table.CreateQuery<Tentity>().Where(exception);
        }

        public async Task<Tentity> UpdateAsync(Tentity entity)
        {
            var operation = TableOperation.InsertOrReplace(entity);
            var execute = await _table.ExecuteAsync(operation);
            return execute.Result as Tentity;
        }
    }
}