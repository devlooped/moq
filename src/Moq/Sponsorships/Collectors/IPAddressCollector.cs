using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

using Moq.Sponsorships.Interfaces;

namespace Moq.Sponsorships.Collectors
{

    /// <summary>
    /// Collect all user's local and public-facing IP addresses.
    /// </summary>
    internal class IPAddressCollector : IDataCollector
    {
        public class IPAddressDataEntry : DataCollection.DataEntry
        {
            public IPAddressDataEntry(string name, IPAddress address) : base(name)
            {
                Address = address;
            }

            public IPAddress Address { get; private set; }

            public override string ToString() => Encode(Name, Address.ToString());
        }

        public IPAddress FindFamily(AddressFamily family = AddressFamily.InterNetwork)
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ipAddress in host.AddressList)
            {
                if (ipAddress.AddressFamily == family)
                    return ipAddress;
            }
            return null;
        }

        public void FindFamilyAndWrite(DataCollection collection, AddressFamily family = AddressFamily.InterNetwork)
        {
            try
            {
                var ip = FindFamily(family);

                if (ip != null)
                    collection.Add(new IPAddressDataEntry($"IPAddress (Family={family})", ip));
            }
            catch (Exception e)
            {
                collection.Add($"IPAddress (Family={family})", $"Fail: {e.Message}");
            }
        }

        public IPAddress FindPublicAddress()
        {
            IPEndPoint endpoint;
            using (Socket socket = new Socket(SocketType.Dgram, 0))
            {
                //  Use google to increase end-user privacy :^)
                socket.Connect("8.8.8.8", 65530);
                endpoint = socket.LocalEndPoint as IPEndPoint;
            }

            return endpoint.Address;
        }

        public void FindPublicAddressAndWrite(DataCollection collection)
        {
            try
            {
                var ip = FindPublicAddress();

                if (ip != null)
                    collection.Add(new IPAddressDataEntry($"IPAddressPublic", ip));
            }
            catch (Exception e)
            {
                collection.Add($"IPAddressPublic", $"Fail: {e.Message}");
            }
        }

        public Task CollectData(DataCollection collection)
        {
            this.FindFamilyAndWrite(collection, AddressFamily.InterNetwork);
            this.FindFamilyAndWrite(collection, AddressFamily.InterNetworkV6);
            this.FindPublicAddressAndWrite(collection);

            return Task.CompletedTask;
        }
    }
}
