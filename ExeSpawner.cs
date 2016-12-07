using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Carto
{

    public class ExeSpawner
    {
        public ProcessStartInfo Info;
        public Process Proc;

        public ExeSpawner(string ProgName, string Arguments, string EnvirVar)
        {
            Info = new ProcessStartInfo();
            string EnvPath = Environment.GetEnvironmentVariable(EnvirVar);

            Info.WorkingDirectory = EnvPath;
            Info.CreateNoWindow = true;
            Info.FileName = EnvPath + "\\" + ProgName;
            Info.UseShellExecute = false;
            Info.Arguments = Arguments;
        }

        public int Run()
        {
            Proc = new Process();
            Proc.StartInfo = Info;
            bool Started = Proc.Start();
            if (!Started) return Int32.MaxValue;
            Proc.WaitForExit();
            return Proc.ExitCode;
        }
    }
}
