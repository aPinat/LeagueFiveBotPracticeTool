﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace LeagueFiveBotPracticeTool
{
    internal static class LeagueClient
    {
        private static readonly Regex TokenRegex = new Regex("--remoting-auth-token=(.+?)\\s");
        private static readonly Regex PortRegex = new Regex("--app-port=(\\d+?)\\s");
        public class ApiAuth
        {
            public ApiAuth(string port, string token)
            {
                Port = port;
                Token = token;
            }

            public string Port { get; }

            public string Token { get; }
        }

        public static IEnumerable<Process> GetLeagueClientProcesses()
        {
            return Process.GetProcessesByName("LeagueClientUx");
        }

        public static ApiAuth GetApiPortAndToken(Process process)
        {
            using (var searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id))
            using (var objects = searcher.Get())
            {
                var commandLine = (string)objects.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"];
                if (commandLine == null)
                {
                    var fileName = Assembly.GetEntryAssembly()?.Location;
                    if (fileName != null)
                    {
                        var currentProcessInfo = new ProcessStartInfo
                        {
                            UseShellExecute = true,
                            WorkingDirectory = Environment.CurrentDirectory,
                            FileName = fileName,
                            Verb = "runas"
                        };

                        Process.Start(currentProcessInfo);
                    }
                    Environment.Exit(0);
                }

                try
                {
                    var port = PortRegex.Match(commandLine).Groups[1].Value;
                    var token = TokenRegex.Match(commandLine).Groups[1].Value;
                    return new ApiAuth(port, token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return null;
        }
        public static string SendApiRequest(ApiAuth apiAuth, string method, string endpointUrl, string body)
        {
            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes("riot:" + apiAuth.Token));
            ServicePointManager.ServerCertificateValidationCallback = (send, certificate, chain, sslPolicyErrors) => true;
            using (var client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.Authorization, "Basic " + auth);
                try
                {
                    return body == null ? client.DownloadString("https://127.0.0.1:" + apiAuth.Port + endpointUrl) : client.UploadString("https://127.0.0.1:" + apiAuth.Port + endpointUrl, method, body);
                }
                catch (WebException ex)
                {
                    using (var streamReader = new StreamReader(ex.Response.GetResponseStream() ?? throw new ArgumentNullException())) return streamReader.ReadToEnd();
                }
            }
        }
    }
}