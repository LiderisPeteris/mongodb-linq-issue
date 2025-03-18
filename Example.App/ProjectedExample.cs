using MongoDB.Bson.Serialization.Attributes;

namespace Example.App
{
    public class ProjectedExample
    {
        public long BucketId { get; set; }
        public string LocationCode { get; set; }
        public string DeviceId { get; set; }
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
        public IEnumerable<CustomEvent[]> CustomEvents { get; set; }
    }
}
