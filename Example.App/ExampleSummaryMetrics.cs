using MongoDB.Bson;

namespace Example.App
{
    public class ExampleSummaryMetrics
    {
        public ObjectId Id { get; set; }
        public long BucketId { get; set; }
        public ExampleMetrics ExampleMetricsList { get; set; }
    }
}
