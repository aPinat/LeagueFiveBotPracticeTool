using System;

namespace LeagueFiveBotPracticeTool
{
    internal static class Program
    {
        private static void Main()
        {
            foreach (var process in LeagueClient.GetLeagueClientProcesses())
            {
                var apiAuth = LeagueClient.GetApiPortAndToken(process);
                const string body = "{\"customGameLobby\": {\"configuration\": {\"gameMode\": \"PRACTICETOOL\", \"gameTypeConfig\": {\"id\": 1}, \"mapId\": 11, \"teamSize\": 5}, \"lobbyName\": \"Pinat\", \"lobbyPassword\": \"aPinat\"}, \"isCustom\": true}";
                var result = LeagueClient.SendApiRequest(apiAuth, "POST", "/lol-lobby/v2/lobby", body);
                Console.WriteLine(result);
            }
        }
    }
}