
namespace Octopus_cli.src
{
    public readonly struct Service
    {
        public static string _Version = "0.0.1";
        public static string _CurrentDirectory = System.IO.Directory.GetCurrentDirectory();
        public static OperatingSystem _OperatingSystem = Environment.OSVersion;
    }
}
