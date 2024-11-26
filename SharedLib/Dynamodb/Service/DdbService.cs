using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using SharedLib.Dynamodb.Models;
using System.Reflection;

namespace SharedLib.Dynamodb.Service
{
    public interface IDbService
    {
        public Task<T> SaveAsync<T>(T model) where T : DdbBase;
        public Task<T> SaveAsync<T>(string pk, string sk, T model) where T : DdbBase;
        public Task<T> FindAsync<T>(string pk);
        public Task<T> FindAsync<T>(string pk, string sk);
    }

    public class DdbService : IDbService
    {
        private readonly IDynamoDBContext _ddbContext;

        public DdbService(IDynamoDBContext ddbContext)
        {
            _ddbContext = ddbContext;
        }

        public async Task<T> SaveAsync<T>(T model) where T : DdbBase
        {
            try
            {
                string pk = model.GetPKString();
                model.PK = pk;
                model.SK = pk;

                await _ddbContext.SaveAsync(model);

                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<T> SaveAsync<T>(string pk, string sk, T model) where T : DdbBase
        {
            try
            {
                model.PK = pk;
                model.SK = sk;

                await _ddbContext.SaveAsync(model);

                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<T> FindAsync<T>(string pk)
        {
            try
            {
                var loadedData = await _ddbContext.LoadAsync<T>(pk, pk, CancellationToken.None);

                return loadedData;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<T> FindAsync<T>(string pk, string sk)
        {
            try
            {
                var loadedData = await _ddbContext.LoadAsync<T>(pk, sk, CancellationToken.None);

                return loadedData;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
