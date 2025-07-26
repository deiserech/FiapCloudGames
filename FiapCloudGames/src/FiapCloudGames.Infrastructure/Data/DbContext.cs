       using MongoDB.Driver;
       
       namespace FiapCloudGames.Infrastructure.Data
       {
           public class DbContext
           {
               private readonly IMongoDatabase _database;
       
               public DbContext(string connectionString, string databaseName)
               {
                   var client = new MongoClient(connectionString);
                   _database = client.GetDatabase(databaseName);
               }
       
               public IMongoCollection<User> Clientes => _database.GetCollection<User>("users");
           }
}      