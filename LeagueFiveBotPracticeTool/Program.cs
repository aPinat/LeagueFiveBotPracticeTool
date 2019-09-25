using System;
using System.Diagnostics;

namespace LeagueFiveBotPracticeTool
{
    class Program
    {
        static void Main(string[] _)
        {
            foreach (Process process in LeagueClient.GetLeagueClientProcesses())
            {
                var apiAuth = LeagueClient.GetAPIPortAndToken(process);
                String body = "{  \"customGameLobby\": {\"configuration\": {\"gameMode\": \"PRACTICETOOL\",\"gameTypeConfig\": {\"id\": 1},\"mapId\": 11,\"spectatorPolicy\": \"AllAllowed\",\"teamSize\": 5},\"lobbyName\": \"aPinat\",\"lobbyPassword\": \"aPinat\"},\"isCustom\": true}";
                string result = LeagueClient.SendAPIRequest(apiAuth, "POST", "/lol-lobby/v2/lobby", body);
                Console.WriteLine(result);
            }
        }
    }
}