using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NotepadLogger.ClassLibrary;
using Serilog;

namespace NotepadLogger
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Sink<NotepadWindowSink>()
                .CreateLogger();

            if (args.Length == 1)
            {
                await GhostWriteFileAsync(args);
            }
            else
            {
                ConsoleReadWriter();
            }
        }

        private static void ConsoleReadWriter()
        {
            while (true)
            {
                var input = Console.ReadLine();
                Log.Information(string.IsNullOrEmpty(input)
                    ? Environment.NewLine
                    : input + Environment.NewLine);
            }
        }

        private static async Task GhostWriteFileAsync(IReadOnlyList<string> args)
        {
            var file = new FileInfo(args[0]);
            if (!file.Exists) return;

            var lines = await File.ReadAllLinesAsync(file.FullName);

            foreach (var line in lines)
            {
                foreach (var c in line)
                {
                    Log.Information($"{c}");
                    await Task.Delay(50);
                }
                Log.Information(Environment.NewLine);
            }
        }
    }
}