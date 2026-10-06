using System;
using System.IO;

namespace Octopus_cli.src
{
    public class _Logger
    {

        private string _CurrentTime;
        private string _PathToLog = null;

        private void CreateLogFile()
        {
            File.WriteAllText(this._PathToLog, " ");
        }

        private void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public void Write(string message) 
        {
            StreamWriter sw = new StreamWriter(_PathToLog);
            sw.WriteLine(message);
            sw.Close();
        }

        public _Logger()
        {
            CreateDirectory("./Logs/");
            this._CurrentTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            this._PathToLog = "./Logs/" + _CurrentTime;

            StreamWriter sw = new StreamWriter(_PathToLog);
            sw.WriteLine(this._CurrentTime + Service._CurrentDirectory);
            sw.WriteLine(Service._OperatingSystem);
            sw.WriteLine("Curent version = " + Service._Version);
            sw.Close();
        }
    }
}
