using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Hypesoft.Infrastructure.HealthChecks
{
    public class MongoDbHealthCheck : IHealthCheck
    {
        private readonly IMongoDatabase _database;

        public MongoDbHealthCheck(string connectionString)
        {
            MongoClient client = new(connectionString);
            _database = client.GetDatabase("HypesoftDB");
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _database.RunCommandAsync<BsonDocument>(
                    new BsonDocument("ping", 1),
                    cancellationToken: cancellationToken);

                return HealthCheckResult.Healthy("MongoDB is responsive");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    "MongoDB is not responsive",
                    ex);
            }
        }
    }
}
