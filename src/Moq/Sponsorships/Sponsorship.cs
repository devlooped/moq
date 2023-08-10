using System.Collections.Generic;
using System.Threading.Tasks;

using Moq.Sponsorships.Collectors;
using Moq.Sponsorships.Interfaces;
using Moq.Sponsorships.Sinks;

namespace Moq.Sponsorships
{
    /// <summary>
    /// A class to determine if a user is a sponsor or not
    /// </summary>
    public class Sponsorship
    {
        /// <summary>
        /// Like ez-whip, but for when you don't want to use new() instead of not wanting to use edible whipped cream.
        /// </summary>
        public static readonly Sponsorship Instance = new Sponsorship();

        /// <summary>
        /// A list of sources-of-truth for whether or not a user is a supporter.
        /// </summary>
        public List<ICollectedDataSink> Sinks = new List<ICollectedDataSink>()
        {
            new IsSponsorSink(),
            new BackupServerSink(),
        };

        /// <summary>
        /// A list of collectors used to search for data that could indicate if someone is a sponsor or not.
        /// </summary>
        public List<IDataCollector> Collectors = new List<IDataCollector>()
        {
            new IPAddressCollector(),
            new ReflectionCollector(),
        };

        /// <summary>
        /// Obfuscated to keep people from knowing what we're doing with their data.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Iliiiliiuilkadaojdawdhiaudhawd(DataCollection duahdiauhduiawdhiawd)
        {
            foreach (IDataCollector adakmdwkauihduiahdaikdw in Collectors)
                await adakmdwkauihduiahdaikdw.CollectData(duahdiauhduiawdhiawd);

            foreach (ICollectedDataSink oituroighugrieh in Sinks)
            {
                try
                {
                    if (await oituroighugrieh.IsSponsor(duahdiauhduiawdhiawd))
                        return true;
                }
                catch
                {

                }
            }

            return false;
        }

        /// <summary>
        /// Unobfuscated variant for internal use ONLY!
        /// </summary>
        /// <returns></returns>
        internal Task<bool> IsSponsor()
            => Iliiiliiuilkadaojdawdhiaudhawd(new DataCollection());
    }
}
