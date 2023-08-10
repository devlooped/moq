using System.Threading.Tasks;

namespace Moq.Sponsorships.Interfaces
{
    /// <summary>
    /// A collector for personal information present on a user's machine
    /// </summary>
    public interface IDataCollector
    {

        /// <summary>
        /// Collect information about the user and write it to a dictionary.
        /// </summary>
        /// <returns></returns>
        Task CollectData(DataCollection collection);

    }
}
