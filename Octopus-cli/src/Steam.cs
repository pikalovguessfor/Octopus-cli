using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;

namespace Octopus_cli.src
{
    public class Steam
    {
        private static readonly HttpClient _http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(15)
        };

        static Steam()
        {
            _http.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                "(KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            _http.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        }

        public int[] GetDlcIds(int gameAppId)
        {
            if (gameAppId <= 0)
                throw new ArgumentOutOfRangeException(nameof(gameAppId));

            string url = $"https://store.steampowered.com/api/appdetails?appids={gameAppId}&cc=us&l=english";

            string json;
            try
            {
                json = _http.GetStringAsync(url).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Steam API request failed for AppID {gameAppId}.", ex);
            }

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
            {
                Console.WriteLine($"[Steam] Unexpected root kind: {root.ValueKind}");
                return Array.Empty<int>();
            }

            if (!root.TryGetProperty(gameAppId.ToString(), out var appNode) ||
                appNode.ValueKind != JsonValueKind.Object)
            {
                Console.WriteLine($"[Steam] No data for AppID {gameAppId}");
                return Array.Empty<int>();
            }

            if (!appNode.TryGetProperty("success", out var successNode) ||
                successNode.ValueKind != JsonValueKind.True)
            {
                Console.WriteLine($"[Steam] success=false for AppID {gameAppId}");
                return Array.Empty<int>();
            }

            if (!appNode.TryGetProperty("data", out var dataNode) ||
                dataNode.ValueKind != JsonValueKind.Object)
            {
                return Array.Empty<int>();
            }

            if (!dataNode.TryGetProperty("dlc", out var dlcArray) ||
                dlcArray.ValueKind != JsonValueKind.Array)
            {
                Console.WriteLine($"[Steam] No DLC field for AppID {gameAppId}");
                return Array.Empty<int>();
            }

            var result = new List<int>(dlcArray.GetArrayLength());
            foreach (var item in dlcArray.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.Number && item.TryGetInt32(out int id))
                    result.Add(id);
            }

            return result.ToArray();
        }
    }
}