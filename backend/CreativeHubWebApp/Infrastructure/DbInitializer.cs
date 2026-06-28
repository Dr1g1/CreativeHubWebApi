using CreativeHubWebApp.Models;
using CreativeHubWebApp.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CreativeHubWebApp.Infrastructure
{
    public class DbInitializer
    {
        private readonly MongoContext _ctx;
        public DbInitializer(MongoContext ctx) => _ctx = ctx;

        public async Task InitAsync()
        {
            // validacija za reviews mora da ide od 1-5
            var existing = await (await _ctx.Database.ListCollectionNamesAsync()).ToListAsync(); // uzimamo imena svih kolekcija iz baze
            if (!existing.Contains("reviews"))
            {
                // ako kolekcija reviews ne postoji onda je pravimo
                // definisemo JSON schemu a to znaci kao pattern za dokument ili sablon
                var validator = new BsonDocument("$jsonSchema", new BsonDocument
                {
                    { "bsonType", "object" }, // svaki dokument je bson objekat
                    { "required", new BsonArray { "ResourceId", "UserId", "Rating" } }, // polja koja mora mo da imamo
                    { "properties", new BsonDocument
                        {
                            { "Rating", new BsonDocument // pravila za polje rating
                                {
                                    { "bsonType", "int" },
                                    { "minimum", 1 },
                                    { "maximum", 5 }
                                }
                            }
                        }
                    }
                });
                // kreiramo kolekciju u koju cemo da ubacujemo kao dokumente za reviews
                await _ctx.Database.CreateCollectionAsync("reviews",
                    new CreateCollectionOptions<BsonDocument> { Validator = validator });
            }

            // createmany - pravi vise indeksa odjednom
            await _ctx.Users.Indexes.CreateManyAsync(new[]
            {
                //index na username
                new CreateIndexModel<User>(
                    Builders<User>.IndexKeys.Ascending(u => u.Username),
                    new CreateIndexOptions { Unique = true }),
                //index na email
                new CreateIndexModel<User>(
                    Builders<User>.IndexKeys.Ascending(u => u.Email),
                    new CreateIndexOptions { Unique = true }),
            });
            await _ctx.Resources.Indexes.CreateManyAsync(new[]
            {
                new CreateIndexModel<Resource>(
                    Builders<Resource>.IndexKeys.Ascending(r => r.Tags)),
                new CreateIndexModel<Resource>(
                    Builders<Resource>.IndexKeys.Ascending(r => r.Type)
                                                .Descending(r => r.Downloads)),
                new CreateIndexModel<Resource>(
                    Builders<Resource>.IndexKeys.Text(r => r.Title)
                                                .Text(r => r.Description)),
                new CreateIndexModel<Resource>(
                    Builders<Resource>.IndexKeys.Descending(r => r.CreatedAt)),
            });
            // indeks na recnzije on je compound unique sto znaci da jedan korisnik moze da ostavi jednu recenziju po resursu
            await _ctx.Reviews.Indexes.CreateOneAsync(new CreateIndexModel<Review>(
                Builders<Review>.IndexKeys.Ascending(r => r.ResourceId).Ascending(r => r.UserId),
                new CreateIndexOptions { Unique = true }));
            // partial indeks samo za javne kolekcije
            await _ctx.Collections.Indexes.CreateOneAsync(new CreateIndexModel<ResourceCollection>(
                Builders<ResourceCollection>.IndexKeys.Ascending(c => c.OwnerId),
                new CreateIndexOptions<ResourceCollection>
                {
                    // indeksira samo dokumente gde je IsPublic=true
                    PartialFilterExpression = Builders<ResourceCollection>.Filter.Eq(c => c.IsPublic, true)
                }));
        }
    }
}