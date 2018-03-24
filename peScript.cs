using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Threading;

namespace PES
{
    class peScript
    {
        private List<string[]> MODULES;
        private List<string> Sarg;
        private List<float> Farg;
        Terminal term;

        public void InitPes(string path = "", Terminal t = null)
        {
            term = t;
            if (string.IsNullOrWhiteSpace(path))
                path = AppDomain.CurrentDomain.BaseDirectory;

            MODULES = new List<string[]>();
            Console.WriteLine("Searching for modules..");
            foreach (var file in Directory.GetFiles(path))
            {
                string fff = file.Replace(path, "");
                if (fff.Substring(fff.Length - 4) == ".pem" && fff.Substring(0, 7) == "module_")
                {
                    string module_name = fff.Split('_')[1];
                    string module_version = fff.Split('_')[2];
                    Console.WriteLine(module_name + "v" + module_version + " loaded.");

                    MODULES.Add(new string[] { path + fff, module_name, module_version });
                }
            }
            Console.WriteLine(MODULES.Count + " modules found.");
            if (Directory.Exists("tmp"))
                Directory.Delete("tmp", true);
            Directory.CreateDirectory("tmp");
        }

        public string Run(string command)
        {
            command = command.ToLower();
            if (!command.Contains('+'))
                return "ERR:0143:Argument not found.";

            string[] module_array = new string[] { };
            string module = command.Split('+')[0];

            string[] args = command.Substring(module.Length).Split('+');

            Sarg = new List<string>();
            Farg = new List<float>();

            foreach (var arg in args)
            {
                if (float.TryParse(arg, out float test))
                {
                    Farg.Add(test);
                }
                else
                {
                    Sarg.Add(arg);
                }
            }
            
            Sarg.RemoveAt(0);

            bool found = false;
            for (int i = 0; i < MODULES.Count; i++)
            {
                if (MODULES[i][1] == module)
                {
                    module_array = MODULES[i];
                    found = true;
                    break;
                }
            }
            if (!found)
                return "ERR:0162:Module not found.";

            if (Directory.Exists(@"tmp\" + module_array[1]))
                Directory.Delete(@"tmp\" + module_array[1], true);
            Directory.CreateDirectory(@"tmp\" + module_array[1]);
            ZipFile.ExtractToDirectory(module_array[0], @"tmp\" + module_array[1]);

            string[] Lines = File.ReadAllLines(@"tmp\" + module_array[1] + @"\code.pes");

            bool Skip = false;
            bool SkipW = false;
            bool Loop = false;
            int LoopInt = 0;

            for (int i = 0; i < Lines.Count(); i++)
            {
                Thread.Sleep(100);
                var lineRaw = Lines[i];
                if (string.IsNullOrWhiteSpace(lineRaw) || string.IsNullOrEmpty(lineRaw))
                    continue;
                string line = lineRaw;
                while (true)
                {
                    if (line[0] == ' ' || line[0] == '	')
                    {
                        line = line.Remove(0, 1);
                    }
                    else
                    {
                        break;
                    }
                }
                
                if (line.Length > 4 && line.Substring(0, 5) == "IFEND")
                {
                    Skip = false;
                    continue;
                }
                if (Skip)
                {
                    continue;
                }




                if (SkipW && line.Length > 4 && line.Substring(0, 5) == "WHEND")
                {
                    SkipW = false;
                    continue;
                }
                if (SkipW)
                {
                    continue;
                }

                Skip = !Process(line);


                if (line.Length > 4 && line.Substring(0, 5) == "WHILE")
                {
                    Loop = Process(line.Substring(6));
                    SkipW = !Loop;
                    LoopInt = i;
                }
                if(Loop && line.Length > 4 && line.Substring(0, 5) == "WHEND")
                {
                    i = LoopInt;
                }
            }

            return "";
        }

        private bool Process(string line)
        {
            string command = line.Split(' ')[0];
            switch (command.ToLower())
            {
                case "+":
                case "ifend":
                case "whend":
                    return true;
                case "print":
                    term.Write(line.Split('"')[1], true);
                    return true;
                case "ifs":
                    return IFS(line);
                case "iff":
                    return IFF(line);
                case "while":
                    return true;
            }
            return false;
        }

        private bool IFS(string line)
        {
            string a1 = line.Split(' ')[1];
            string sep = line.Split(' ')[2];
            string a2 = line.Split(' ')[3];

            if (a1[0] == '"')
                a1 = a1.Split('"')[1];
            if (a2[0] == '"')
                a2 = a2.Split('"')[1];


            if (a1.Substring(0, 4) == "sarg")
            {
                int index1 = a1.Length - 1;
                int index2 = Convert.ToInt32(a1.Substring(index1));
                a1 = Sarg[index2];
            }
            if (a2.Substring(0, 4) == "sarg")
                a2 = Sarg[Convert.ToInt32(a2[a1.Length - 1])];

            bool result = a1 == a2;
            if(sep == "=")
                return result;
            if (sep == "!=" || sep == "=!")
                return !result;
            return false;
        }

        private bool IFF(string line)
        {
            string tmpa1 = line.Split(' ')[1];
            string sep = line.Split(' ')[2];
            string tmpa2 = line.Split(' ')[3];

            float a1;
            float a2;

            float.TryParse(tmpa1, out a1);
            float.TryParse(tmpa2, out a2);

            if (tmpa1.Substring(0, 4) == "farg")
            {
                int index1 = tmpa1.Length - 1;
                int index2 = Convert.ToInt32(tmpa1.Substring(index1));
                a1 = Farg[index2];
            }
            if (tmpa2.Substring(0, 4) == "farg")
            {
                int index1 = tmpa2.Length - 1;
                int index2 = Convert.ToInt32(tmpa2.Substring(index1));
                a2 = Farg[index2];
            }
            
            switch (sep)
            {
                case "<":
                    return a1 < a2;
                case ">":
                    return a1 > a2;
                case "=":
                    return a1 == a2;
                case "<=":
                    return a1 <= a2;
                case ">=":
                    return a1 >= a2;
                case "!=":
                    return a1 != a2;
            }


            return true;
        }
    }
}
