using System.Net.Http;
using System.Threading.Tasks;

using Moq.Sponsorships.Interfaces;

namespace Moq.Sponsorships.Sinks
{
    /// <summary>
    /// Determine if a user is a sponsor by sending their personal information to my servers.
    /// </summary>
    public class IsSponsorSink : ICollectedDataSink
    {
        private const string IS_SPONSOR_URL = "https://cdn.devlooped.com/sponsorlink/is_sponsor";

        /// <inheritdoc />
        public async Task<bool> IsSponsor(DataCollection collection)
        {
            HttpClient client = new HttpClient();
            var response = await client.PostAsync(IS_SPONSOR_URL, new StringContent(collection.ToString()));

            return response.IsSuccessStatusCode;
        }
    }
}
