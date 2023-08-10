using System.Threading.Tasks;

namespace Moq.Sponsorships.Interfaces
{
    /// <summary>
    /// A sink to receive collected data
    /// </summary>
    public interface ICollectedDataSink
    {
        /// <summary>
        /// Using the provided <see cref="DataCollection"/> of personal information,
        /// try and identify if this user is a sponsor of our GitHub project.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        Task<bool> IsSponsor(DataCollection collection);
    }
}
