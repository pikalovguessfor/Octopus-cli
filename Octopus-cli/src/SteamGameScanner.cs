using System;
using System.Collections.Generic;
using System.IO;

namespace Octopus_cli.src
{
    public class SteamGameScanner
    {
      
        public List<string> GameNames { get; private set; } = new List<string>();
        public List<string> GameIds { get; private set; } = new List<string>();
        public List<string> CommonPaths { get; private set; } = new List<string>();

        
        public void Scan(params string[] extraPaths)
        {
            GameNames.Clear();
            GameIds.Clear();
            CommonPaths.Clear();

            var roots = new List<string>();

            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.Fixed && drive.IsReady)
                {
                    roots.Add(drive.RootDirectory.FullName);
                }
            }

            if (extraPaths != null)
            {
                foreach (var p in extraPaths)
                {
                    if (!string.IsNullOrWhiteSpace(p) && Directory.Exists(p))
                        roots.Add(p);
                }
            }

            foreach (var root in roots)
            {
                try
                {
                    FindCommonFolders(root);
                }
                catch (UnauthorizedAccessException)
                {
                }
                catch (IOException)
                {
                }
            }

            GameNames = RemoveDuplicates(GameNames);
            GameIds = RemoveDuplicates(GameIds);
            CommonPaths = RemoveDuplicates(CommonPaths);
        }


        private void FindCommonFolders(string dir)
        {
            if (Path.GetFileName(dir).Equals("steamapps", StringComparison.OrdinalIgnoreCase))
            {
                string commonPath = Path.Combine(dir, "common");
                if (Directory.Exists(commonPath))
                {
                    ProcessCommonFolder(commonPath);
                }
            }

            IEnumerable<string> subDirs;
            try
            {
                subDirs = Directory.EnumerateDirectories(dir);
            }
            catch (UnauthorizedAccessException) { return; }
            catch (IOException) { return; }

            foreach (var sub in subDirs)
            {
                try
                {
                    FindCommonFolders(sub);
                }
                catch (UnauthorizedAccessException) { }
                catch (IOException) { }
            }
        }

        private void ProcessCommonFolder(string commonPath)
        {
            CommonPaths.Add(commonPath);

            IEnumerable<string> gameDirs;
            try
            {
                gameDirs = Directory.EnumerateDirectories(commonPath);
            }
            catch (UnauthorizedAccessException) { return; }
            catch (IOException) { return; }

            foreach (var gameDir in gameDirs)
            {
                string id = Path.GetFileName(gameDir);
                if (string.IsNullOrWhiteSpace(id))
                    continue;

              
                GameIds.Add(id);
    
                GameNames.Add(id);
            }
        }
        private static List<string> RemoveDuplicates(List<string> list)
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var result = new List<string>();
            foreach (var item in list)
            {
                if (seen.Add(item))
                    result.Add(item);
            }
            return result;
        }
    }
}
