using Amazon.DynamoDBv2.DataModel;
using SharedLib.Dynamodb.Models;

namespace SharedLib.Dynamodb.Service
{
    public interface IDbService
    {
        public Task<T> SaveAsync<T>(T model) where T : DdbBase;
        public Task<T> FindAsync<T>(string pk);
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
    }
}
