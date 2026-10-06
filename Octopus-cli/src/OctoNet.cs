using System.Net.NetworkInformation;

namespace Octopus_cli.src
{
    public static class OctoNet
    {
        /*Public class to interact with internet*/

        public static bool Ping(string url)
        {
            try
            {
                string host = url;
                if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
                    host = uri.Host;

                using var ping = new Ping();
                PingReply reply = ping.Send(host, 3000);

                return reply.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }

        }
    }
}
