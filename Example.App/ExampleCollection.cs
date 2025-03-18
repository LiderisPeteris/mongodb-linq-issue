using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Example.App
{
    public class ExampleCollection
    {
        public ObjectId Id { get; set; }
        public long BucketId { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
        [BsonIgnoreIfNull]
        public Guid ExampleGuid { get; set; }
        public IEnumerable<ExampleMetrics> ExampleMetricsList { get; set; }
    }

    public class ExampleMetrics
    {
        public string LocationCode { get; set; }
        public long DeviceId { get; set; }
        public string PrimaryGroupingId { get; set; }
        public string PrimaryGroupName { get; set; }
        public string SecondaryGroupingId { get; set; }
        public string SecondaryGroupingName { get; set; }
        public decimal MeasurementOne { get; set; }
        public decimal MeasurementTwo { get; set; }
        public decimal MeasurementThree { get; set; }
        public long CounterOne { get; set; }
        public long CounterTwo { get; set; }
        public long CounterThree { get; set; }
        [BsonIgnoreIfNull]
        public CustomEvent[] CustomEvents { get; set; }
    }

    public class CustomEvent
    {
        [BsonElement("Id")]
        public long CustomEventId { get; set; }
        public decimal CustomMesurmentOne { get; set; }
        public decimal CustomMesurmentTwo { get; set; }
    }
}
