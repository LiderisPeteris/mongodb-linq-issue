using Example.App;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Diagnostics;

IConfiguration configuration = new ConfigurationBuilder()
.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
.AddEnvironmentVariables()
.Build();

var connectionString = configuration.GetConnectionString("ExampleDb");

var settings = MongoClientSettings.FromConnectionString(connectionString);

//settings.LinqProvider = LinqProvider.V3;
settings.LinqProvider = LinqProvider.V2;

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.CSharpLegacy));

var mongoClient = new MongoClient(settings);
var database = mongoClient.GetDatabase("ExampleDb");
var collection = database.GetCollection<ExampleCollection>("ExampleCollectionEntries");

var anyDocument = await GetAnyDocumentAsync(mongoClient, collection);


if (anyDocument == null)
{
    await CreateExampleCollectionIndexesAsync(mongoClient, collection);
}

var filter = Builders<ExampleCollection>.Filter;

var filterDefinitions = filter.And(
        filter.Eq(c => c.Year, 2024),
        filter.In(c => c.Week, new[] { 1, 2, 3 }
        ));

IAggregateFluent<ProjectedExample> query;

if (settings.LinqProvider == LinqProvider.V3)
{
    query = collection
         .Aggregate(new AggregateOptions() { AllowDiskUse = true })
         .Match(filter.Or(filterDefinitions))
         .Unwind<ExampleCollection, ExampleSummaryMetrics>(s => s.ExampleMetricsList)
         .Group(c => new
         {
             c.ExampleMetricsList.PrimaryGroupingId,
             c.ExampleMetricsList.LocationCode,
         },
         group => new ProjectedExample
         {
             BucketId = group.First().BucketId,
             LocationCode = group.First().ExampleMetricsList.LocationCode,
             DeviceId = group.First().ExampleMetricsList.DeviceId.ToString(),
             SecondaryGroupingId = group.First().ExampleMetricsList.SecondaryGroupingId,
             SecondaryGroupingName = group.First().ExampleMetricsList.SecondaryGroupingName,
             PrimaryGroupingId = group.First().ExampleMetricsList.PrimaryGroupingId,
             PrimaryGroupName = group.First().ExampleMetricsList.PrimaryGroupName,
             MeasurementOne = group.Sum(c => c.ExampleMetricsList.MeasurementOne),
             MeasurementTwo = group.Sum(c => c.ExampleMetricsList.MeasurementTwo),
             MeasurementThree = group.Sum(c => c.ExampleMetricsList.MeasurementThree),
             CounterOne = group.Sum(c => c.ExampleMetricsList.CounterOne),
             CounterTwo = group.Sum(c => c.ExampleMetricsList.CounterTwo),
             CounterThree = group.Sum(c => c.ExampleMetricsList.CounterThree),
             CustomEvents = group.Select(c => c.ExampleMetricsList.CustomEvents),
         }
         );
}
else
{
    query = collection
         .Aggregate(new AggregateOptions() { AllowDiskUse = true })
         .Match(filter.Or(filterDefinitions))
         .Unwind<ExampleCollection, ExampleSummaryMetrics>(s => s.ExampleMetricsList)
         .Group(c => new
         {
             c.ExampleMetricsList.PrimaryGroupingId,
             c.ExampleMetricsList.LocationCode,
         },
         group => new
         {
             BucketId = group.First().BucketId,
             LocationCode = group.First().ExampleMetricsList.LocationCode,
             DeviceId = group.First().ExampleMetricsList.DeviceId,
             SecondaryGroupingId = group.First().ExampleMetricsList.SecondaryGroupingId,
             SecondaryGroupingName = group.First().ExampleMetricsList.SecondaryGroupingName,
             PrimaryGroupingId = group.First().ExampleMetricsList.PrimaryGroupingId,
             PrimaryGroupName = group.First().ExampleMetricsList.PrimaryGroupName,
             MeasurementOne = group.Sum(c => c.ExampleMetricsList.MeasurementOne),
             MeasurementTwo = group.Sum(c => c.ExampleMetricsList.MeasurementTwo),
             MeasurementThree = group.Sum(c => c.ExampleMetricsList.MeasurementThree),
             CounterOne = group.Sum(c => c.ExampleMetricsList.CounterOne),
             CounterTwo = group.Sum(c => c.ExampleMetricsList.CounterTwo),
             CounterThree = group.Sum(c => c.ExampleMetricsList.CounterThree),
             CustomEvents = group.Select(c => c.ExampleMetricsList.CustomEvents),
         })
         .Project(grouped =>
          new ProjectedExample()
          {
              BucketId = grouped.BucketId,
              LocationCode = grouped.LocationCode,
              DeviceId = grouped.DeviceId.ToString(),
              SecondaryGroupingId = grouped.SecondaryGroupingId,
              SecondaryGroupingName = grouped.SecondaryGroupingName,
              PrimaryGroupingId = grouped.PrimaryGroupingId,
              PrimaryGroupName = grouped.PrimaryGroupName,
              MeasurementOne = grouped.MeasurementOne,
              MeasurementTwo = grouped.MeasurementTwo,
              MeasurementThree = grouped.MeasurementThree,
              CounterOne = grouped.CounterOne,
              CounterTwo = grouped.CounterTwo,
              CounterThree = grouped.CounterThree,
              CustomEvents = grouped.CustomEvents,
          }
        );
}

for (int i = 0; i < 5; i++)
{

    var timer = new Stopwatch();
    timer.Start();
    var result = await query.ToListAsync();
    timer.Stop();
    Console.WriteLine($"time spent {timer.Elapsed} to retrieve {result.Count()} result count.");

}

async Task CreateExampleCollectionIndexesAsync(MongoClient mongoClient, IMongoCollection<ExampleCollection> collection)
{
    Console.WriteLine("Collection is empty, executing index creation and seed.");
    var exampleDocs = ExampleCollectionSeed.GetSeedDocs();
    var options = new InsertManyOptions { BypassDocumentValidation = true };
    await collection.InsertManyAsync(exampleDocs, options);

    await CreateIndex(
        collection,
        (build) => build.Ascending(g => g.BucketId));

    await CreateIndex(
        collection,
        (build) => build
            .Ascending(g => g.Year)
            .Ascending(g => g.Week)
            .Ascending(g => g.BucketId)
        ,
        options => options.Name = "Example_Index");

    await CreateIndex(
        collection,
        (build) => build.Ascending(g => g.ExampleGuid));
}

async Task<ExampleCollection> GetAnyDocumentAsync(MongoClient mongoClient, IMongoCollection<ExampleCollection> collection)
{
    var filterBuilder = Builders<ExampleCollection>.Filter;
    var optionsExists = new FindOptions<ExampleCollection> { Limit = 1 };
    var filterExists = filterBuilder.Exists(d => d.Id);
    var result = await collection.FindAsync(filterExists, new FindOptions<ExampleCollection> { Limit = 1 });
    return await result.FirstOrDefaultAsync();
}

async Task<string> CreateIndex<TDocument>(
   IMongoCollection<TDocument> collection,
   Func<IndexKeysDefinitionBuilder<TDocument>, IndexKeysDefinition<TDocument>> build,
   Action<CreateIndexOptions> configure = null,
   bool isUnique = false
   )
{
    var indexDefinitionBuilder = Builders<TDocument>.IndexKeys;
    var indexDefinition = build(indexDefinitionBuilder);

    var options = new CreateIndexOptions
    {
        Background = true,
        Unique = isUnique,
    };

    configure?.Invoke(options);

    var indexModel = new CreateIndexModel<TDocument>(indexDefinition, options);
    return await collection.Indexes.CreateOneAsync(indexModel);
}
