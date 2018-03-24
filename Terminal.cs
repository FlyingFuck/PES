using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PES
{
    class Terminal
    {
        private Dictionary<string, int> PesProcesses;
        int ProcOutputLimit = 10;
        
        public void Init(int limit = 10)
        {
            PesProcesses = new Dictionary<string, int>();
            ProcOutputLimit = limit;
        }

        public string GetName()
        {
            StackTrace stackTrace = new StackTrace();
            return stackTrace.GetFrame(1).GetMethod().Name;
        }
        
        public void Write(string text)
        {
            StackTrace stackTrace = new StackTrace();
            string sender = stackTrace.GetFrame(1).GetMethod().Name;

            if (PesProcesses.ContainsKey(sender))
                PesProcesses[sender] = PesProcesses[sender] + 1;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(sender + "> ");
            Console.Write(text);
            Console.ResetColor();
        }

        public void Write(string text, bool newline)
        {
            StackTrace stackTrace = new StackTrace();
            string sender = stackTrace.GetFrame(1).GetMethod().Name;

            if (!PesProcesses.ContainsKey(sender))
                PesProcesses.Add(sender, 0);

            if (PesProcesses.ContainsKey(sender))
                PesProcesses[sender] = PesProcesses[sender] + 1;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(sender + "> ");
            Console.Write(text);
            if (newline)
                Console.WriteLine();
            Console.ResetColor();
        }
    }
}
