using System;

namespace Octopus_cli.src
{

    public class Program
    {

        public static void Main(string[] args)
        {
            _Logger Logger = new _Logger();
            DlcIdsFromStore Steam = new DlcIdsFromStore();
            SteamGameScanner GameScanner = new SteamGameScanner();

            //Trying to find games in the default steam path and in the extra paths provided by the user
            Console.WriteLine("Welcome to Octopus! Program to unlock steam dlc");
            Console.WriteLine("Scanning drives for steam games...");
            GameScanner.Scan();

            if(GameScanner.GameNames.Count == 0)
            {
                Console.WriteLine("No steam games found. Please make sure steam is installed and you have games installed.");
                Logger.Write("No steam games found. Please make sure steam is installed and you have games installed.");
                return;
            }

            Console.WriteLine("Found" + GameScanner.GameNames.Count + " steam games.");
            for (int i = 0; i < GameScanner.GameNames.Count; i++)
            {
                Console.WriteLine(i + ": " + GameScanner.GameNames[i] + " (AppID: " + GameScanner.GameIds[i] + ")");
            }

            for(int i = 0; i < GameScanner.GameNames.Count; i++)
            {
                if (!int.TryParse(GameScanner.GameIds[i], out int gameAppId))
                {
                    Console.WriteLine("Skip " + GameScanner.GameNames[i] + ": AppID is not numeric ('" + GameScanner.GameIds[i] + "')");
                    Logger.Write("Skip " + GameScanner.GameNames[i] + ": AppID is not numeric ('" + GameScanner.GameIds[i] + "')");
                    continue;
                }
                int[] dlcIds = Steam.GetDlcIds(gameAppId, Logger);
                if (dlcIds.Length > 0)
                {
                    Console.WriteLine("Found " + dlcIds.Length + " DLCs for " + GameScanner.GameNames[i] + ":");
                    Logger.Write("Found " + dlcIds.Length + " DLCs for " + GameScanner.GameNames[i] + ":");
                    foreach (int dlcId in dlcIds)
                    {
                        Console.WriteLine("- DLC AppID: " + dlcId);
                        Logger.Write("- DLC AppID: " + dlcId);
                    }
                }
                else
                {
                    Console.WriteLine("No DLCs found for " + GameScanner.GameNames[i]);
                    Logger.Write("No DLCs found for " + GameScanner.GameNames[i]);
                }
            }

            Console.ReadLine();

        }
    }
}