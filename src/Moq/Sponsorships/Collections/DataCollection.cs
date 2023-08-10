using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moq.Sponsorships
{

    /// <summary>
    /// A collection of user's personally identifiable information (PII)
    /// which can be easily serialized and transmitted to other entities.
    /// </summary>
    public class DataCollection
    {
        /// <summary>
        /// An arbitrary key-value pair of user personal information
        /// </summary>
        public abstract class DataEntry
        {
            /// <summary>
            /// The key used to identify this data entry.
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            ///
            /// </summary>
            /// <param name="name"></param>
            public DataEntry(string name)
            {
                Name = name;
            }

            /// <summary>
            /// Sanitize key and append to value
            /// Note that "value" is not itself sanitized, and can escape serialization.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            protected string Encode(string key, string value)
                => $"{Sanitize(key)}: {value}";

            /// <summary>
            /// Encode a string in base64 to keep it from accidentally
            /// making distinguishing different types of PII difficult.
            /// </summary>
            /// <param name="input">The string to encode</param>
            /// <param name="originalEncoding">The encoding that the string is using; used to convert the input into a byte array.</param>
            /// <returns></returns>
            protected string Sanitize(string input, Encoding originalEncoding)
                => Convert.ToBase64String(  originalEncoding.GetBytes(input), Base64FormattingOptions.None);

            /// <summary>
            /// See <see cref="Sanitize(string,System.Text.Encoding)"/>.
            /// </summary>
            /// <param name="input">The string to encode</param>
            /// <returns></returns>
            protected string Sanitize(string input)
                => Sanitize(input, Encoding.Default);

            /// <summary>
            /// Serialize this data-entry.
            /// Restrictions:
            ///  -  All string-y data should be encoded as a byte array
            ///     to ensure that PII cannot impersonate other PII.
            ///  -  This includes the key! The provided instance method <see cref="Sanitize(string,System.Text.Encoding)"/>
            ///     or the instance method <see cref="Encode"/> can help achieve this.
            ///  -  Format should be "[safe_key]: [safe_data]"
            /// </summary>
            /// <returns></returns>
            public abstract override string ToString();
        }

        internal class StringyDataEntry : DataEntry
        {
            public StringyDataEntry(string name, string value) : base(name)
            {
                Value = value;
            }

            public string Value { get; private set; }

            public override string ToString() => Encode(Name, Sanitize(Value));
        }

        internal class NumericDataEntry : DataEntry
        {
            public NumericDataEntry(string name, long value) : base(name)
            {
            }

            public long Value { get; private set; }

            public override string ToString() => Encode(Name, Value.ToString("x8"));
        }

        /// <summary>
        /// The user's personally identifiable information, in list format.
        /// We don't need to know the underlying types, all we need is the properly exposed
        /// and implemented <see cref="DataEntry.ToString"/>.
        /// </summary>
        private List<DataEntry> Entries { get; } = new List<DataEntry>();

        /// <summary>
        /// Creates a new empty data collection
        /// </summary>
        public DataCollection()
        {

        }

        /// <summary>
        /// Add more deeply private information to the list of
        /// data to be collected.
        /// </summary>
        /// <param name="entry"></param>
        public void Add(DataEntry entry)
            => Entries.Add(entry);

        /// <summary>
        /// Determine if we have personal information with the provided name
        /// in the collection
        /// </summary>
        /// <param name="name"></param>
        public bool Has(string name)
            => Entries.Any(pair => pair.Name.CompareTo(name) == 0);

        /// <summary>
        /// Get all data with the matching name from this collection
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<DataEntry> Get(string name)
            => Entries.Where(pair => pair.Name.CompareTo(name) == 0);

        /// <summary>
        /// Turn this collection of personally identifiable information
        /// into an easy-to-transmit format.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            List<string> serializedEntries = Entries
                .Select(entry => entry.ToString())
                .ToList();

            return String.Join("\n", serializedEntries);
        }
    }
}
