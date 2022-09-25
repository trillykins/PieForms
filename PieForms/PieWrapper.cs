using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PieForms
{
    internal class PieWrapper
    {
        internal string[] PieList()
        {
            var result = new List<string>();
            var list = RunProcess("-l");
            foreach (var l in list)
            {
                if (result.Contains(".xml")) result.Add(l.Trim());
            }
            return result.ToArray();
        }

        internal void PieImport(IEnumerable<string> imports)
        {
            RunProcess($"-i {string.Join(" ", imports)}");
        }

        internal void PieExport(IEnumerable<string> exports)
        {
            RunProcess($"-x {string.Join(" ", exports)}");
        }

        internal void PieRemove(IEnumerable<string> removes)
        {
            RunProcess($"-r {string.Join(" ", removes)}");
        }

        internal void PieCreate(string filename)
        {
            RunProcess($"-c {filename}");
        }

        private static string[] RunProcess(string argument)
        {
            //if (!File.Exists("pie.exe"))
            //{
            //    throw new Exception();
            //}

            var processInfo = new ProcessStartInfo
            {
                FileName = "pie.exe",
                Arguments = argument,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using Process proc = Process.Start(processInfo) ?? throw new Exception();

            var lines = new List<string>();
            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine() ?? string.Empty;
                if (line.Trim().ToLower().EndsWith(".xml")) lines.Add(line.Trim());
            }
            return lines.Where(x => !string.IsNullOrEmpty(x)).ToArray();
        }
    }
}
