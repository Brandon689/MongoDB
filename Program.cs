using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MongoDBExample
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<BsonDocument> _collection;

    public MongoDBExample(string connectionString, string databaseName, string collectionName)
    {
        try
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
            _collection = _database.GetCollection<BsonDocument>(collectionName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to MongoDB: {ex.Message}");
            throw;
        }
    }

    public async Task InsertDocumentAsync(BsonDocument document)
    {
        try
        {
            await _collection.InsertOneAsync(document);
            Console.WriteLine("Document inserted successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inserting document: {ex.Message}");
        }
    }

    public async Task<List<BsonDocument>> FindDocumentsAsync(FilterDefinition<BsonDocument> filter)
    {
        try
        {
            var result = await _collection.Find(filter).ToListAsync();
            Console.WriteLine($"Found {result.Count} documents");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error finding documents: {ex.Message}");
            return new List<BsonDocument>();
        }
    }

    public async Task UpdateDocumentAsync(FilterDefinition<BsonDocument> filter, UpdateDefinition<BsonDocument> update)
    {
        try
        {
            var result = await _collection.UpdateOneAsync(filter, update);
            Console.WriteLine($"Modified {result.ModifiedCount} document(s)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating document: {ex.Message}");
        }
    }

    public async Task DeleteDocumentAsync(FilterDefinition<BsonDocument> filter)
    {
        try
        {
            var result = await _collection.DeleteOneAsync(filter);
            Console.WriteLine($"Deleted {result.DeletedCount} document(s)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting document: {ex.Message}");
        }
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        string connectionString = "mongodb://localhost:27017";
        string databaseName = "myDatabase";
        string collectionName = "myCollection";

        var mongoExample = new MongoDBExample(connectionString, databaseName, collectionName);

        // Insert a document
        var document = new BsonDocument
        {
            { "name", "John Doe" },
            { "age", 30 },
            { "city", "New York" }
        };
        await mongoExample.InsertDocumentAsync(document);

        // Find documents
        var filter = Builders<BsonDocument>.Filter.Eq("name", "John Doe");
        var foundDocuments = await mongoExample.FindDocumentsAsync(filter);

        // Update a document
        var update = Builders<BsonDocument>.Update.Set("age", 31);
        await mongoExample.UpdateDocumentAsync(filter, update);

        // Delete a document
        await mongoExample.DeleteDocumentAsync(filter);

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}