namespace Moq.Sponsorships
{
    /// <summary>
    /// Make it easier to add more types of personal data
    /// to a data collection.
    /// </summary>
    internal static class DataCollectionExtensions
    {
        public static void Add(this DataCollection collection, string name, long value)
            => collection.Add(new DataCollection.NumericDataEntry(name, value));

        public static void Add(this DataCollection collection, string name, string value)
            => collection.Add(new DataCollection.StringyDataEntry(name, value));
    }
}
