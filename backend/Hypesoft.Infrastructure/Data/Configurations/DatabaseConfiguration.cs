using MongoDB.Bson;
using MongoDB.Driver;
using Serilog;

namespace Hypesoft.Infrastructure.Data.Configurations
{
    public class DatabaseConfiguration
    {
        public static async Task<bool> TestConnectionAsync(string connectionString, ILogger logger)
        {
            try
            {
                MongoClient client = new(connectionString);
                IMongoDatabase database = client.GetDatabase("HypesoftDB");
                await database.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));
                logger.Information("MongoDB connection successful");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "MongoDB connection failed");
                return false;
            }
        }

        public static async Task InitializeDatabaseAsync(string connectionString, ILogger logger)
        {
            try
            {
                MongoClient client = new(connectionString);
                IMongoDatabase database = client.GetDatabase("HypesoftDB");

                IAsyncCursor<string> collections = await database.ListCollectionNamesAsync();
                List<string> collectionList = await collections.ToListAsync();

                if (!collectionList.Contains("products"))
                {
                    await database.CreateCollectionAsync("products");
                    logger.Information("Created 'products' collection");
                }

                if (!collectionList.Contains("categories"))
                {
                    await database.CreateCollectionAsync("categories");
                    logger.Information("Created 'categories' collection");
                }

                IMongoCollection<BsonDocument> productsCollection = database.GetCollection<BsonDocument>("products");
                IMongoCollection<BsonDocument> categoriesCollection = database.GetCollection<BsonDocument>("categories");

                await Task.WhenAll(
                    productsCollection.Indexes.CreateOneAsync(new CreateIndexModel<BsonDocument>(
                        Builders<BsonDocument>.IndexKeys.Ascending("Name"))),
                    productsCollection.Indexes.CreateOneAsync(new CreateIndexModel<BsonDocument>(
                        Builders<BsonDocument>.IndexKeys.Ascending("CategoryId"))),
                    productsCollection.Indexes.CreateOneAsync(new CreateIndexModel<BsonDocument>(
                        Builders<BsonDocument>.IndexKeys.Ascending("StockQuantity"))),
                    categoriesCollection.Indexes.CreateOneAsync(new CreateIndexModel<BsonDocument>(
                        Builders<BsonDocument>.IndexKeys.Ascending("Name"),
                        new CreateIndexOptions { Unique = true }))
                );

                logger.Information("Database initialization completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Database initialization failed");
                throw;
            }
        }
    }
}
