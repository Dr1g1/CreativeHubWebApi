using CreativeHubWebApp.Models;
using CreativeHubWebApp.Settings;
using Microsoft.Extensions.Options; // omogucava automatsko citanje konfiguracija
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace CreativeHubWebApp.Repositories
{

    public class MongoContext
    {
        public IMongoClient Client { get; }// konekcija ka mongodb serveru
        public IMongoDatabase Database { get; }// konkretno koja je baza sa kojom cemo da radimo

        // mongosettins objekat - koristi konfiguraciju iz appsettings.json
        public MongoContext(IOptions<MongoSettings> options)
        {
            var settings = options.Value;
            Client = new MongoClient(settings.ConnectionString);
            Database = Client.GetDatabase(settings.DatabaseName);// kreiranje klijenta i povezivanje na mongodb server
        }

        // kolekcija users 
        public IMongoCollection<User> Users => Database.GetCollection<User>("users");
        // kolekcija resources
        public IMongoCollection<Resource> Resources => Database.GetCollection<Resource>("resources");
        //objekat preko kog cemo da uploadujemo i skidamo fajlove; bucket je gridfs naziv za skladiste fajlova; on interno koristi one fs.files i fs.chunks kolekcije
        public IGridFSBucket GridFs => new GridFSBucket(Database);
        public IMongoCollection<Review> Reviews => Database.GetCollection<Review>("reviews");
        public IMongoCollection<ResourceCollection> Collections =>
            Database.GetCollection<ResourceCollection>("collections");
    }
}