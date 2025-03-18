using MongoDB.Driver;

namespace Example.App
{
    class ExampleCollectionSeed
    {
        public static List<ExampleCollection> GetSeedDocs()
        {
            var options = new InsertManyOptions { BypassDocumentValidation = true };
            var bucketIds = new[] { 931250159, 959248196, 953457465, 368118733, 897471069, 265741841, 231457339, 642702135, 638492621, 205433077, 769068984, 413289225, 144879849, 727681823, 455027588, 696728538, 451770194, 830746321, 415705583, 190335668, 481341638, 663500975, 824757149, 906616374, 287015146, 622855402, 480116389, 789392644, 853432555, 844152021, 961051662, 993704523, 596982934, 570748593, 954680453, 960122641, 453513668, 110166774, 117263709, 335549904, 198048428, 161774384, 892799603, 227905896, 411244879, 645838945, 859620719, 105345166, 128052587, 825207417 };
            var years = new[] { 2024, 2025 };
            var groupingIds = GenerateStaticRandomStrings(2800, 5, 7);

            var exampleDocs = new List<ExampleCollection>();
            foreach (var bucketId in bucketIds)
            {
                foreach (var year in years)
                {
                    for (int i = 1; i <= 12; i++)
                    {
                        var exampleDoc = GetRandomExampleDoc(bucketId, year, i, groupingIds);
                        exampleDocs.Add(exampleDoc);
                    }
                }
            }
            return exampleDocs;
        }

        private static ExampleCollection GetRandomExampleDoc(int bucketId, int year, int week, List<string> groupingIds)
        {
            return new ExampleCollection()
            {
                BucketId = bucketId,
                Year = year,
                Week = week,
                ExampleMetricsList = GetRandomMetricList(groupingIds),
                ExampleGuid = Guid.NewGuid(),
            };
        }

        private static IEnumerable<ExampleMetrics> GetRandomMetricList(List<string> groupingIds)
        {
            Random random = new Random();

            foreach (var group in groupingIds)
            {
                decimal min = 1.5m;
                decimal max = 5.5m;

                yield return new ExampleMetrics()
                {
                    CounterOne = random.NextInt64(1, 100000),
                    CounterTwo = random.NextInt64(1, 100000),
                    CounterThree = random.NextInt64(1, 100000),
                    DeviceId = random.NextInt64(10000, 100000),
                    LocationCode = "E1",
                    MeasurementOne = (decimal)random.NextDouble() * (max - min) + min,
                    MeasurementThree = (decimal)random.NextDouble() * (max - min) + min,
                    MeasurementTwo = (decimal)random.NextDouble() * (max - min) + min,
                    PrimaryGroupingId = group,
                    SecondaryGroupingId = "S" + group,
                    PrimaryGroupName = "Name " + group,
                    SecondaryGroupingName = "Name " + "S" + group,
                    CustomEvents = GetCustomEvents()

                };
            }

        }

        private static CustomEvent[] GetCustomEvents()
        {
            Random random = new Random();
            var size = random.Next(0, 100);
            var customEvents = new CustomEvent[size];

            for (int i = 0; i < size; i++)
            {
                var min = 1.5m;
                var max = 5.5m;

                customEvents[i] = new CustomEvent()
                {
                    CustomEventId = random.Next(1000, 10000),
                    CustomMesurmentOne = (decimal)random.NextDouble() * (max - min) + min,
                    CustomMesurmentTwo = (decimal)random.NextDouble() * (max - min) + min,
                };
            }

            return customEvents;
        }

        private static string GenerateRandomNumericString(Random random, int length)
        {
            var digits = new char[length];
            for (int i = 0; i < length; i++)
            {
                digits[i] = (char)('0' + random.Next(0, 10));
            }
            return new string(digits);
        }

        private static List<string> GenerateStaticRandomStrings(int count, int minLength, int maxLength)
        {
            Random random = new Random();
            var uniqueStrings = new List<string>();

            while (uniqueStrings.Count < count)
            {
                int length = random.Next(minLength, maxLength + 1);
                string randomString = GenerateRandomNumericString(random, length);
                uniqueStrings.Add(randomString);
            }

            return uniqueStrings;
        }
    }
}
