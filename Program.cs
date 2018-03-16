using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PES
{
    class Program
    {
        bool CLOSE = false;
        /// <summary>
        /// 0: modules; 1: layout
        /// </summary>
        int InputMode = 0;
        ConsoleKey[] AvailableKeys =
        {
            ConsoleKey.A,
            ConsoleKey.B,
            ConsoleKey.C,
            ConsoleKey.D,
            ConsoleKey.E,
            ConsoleKey.F,
            ConsoleKey.G,
            ConsoleKey.H,
            ConsoleKey.I,
            ConsoleKey.J,
            ConsoleKey.K,
            ConsoleKey.L,
            ConsoleKey.M,
            ConsoleKey.N,
            ConsoleKey.O,
            ConsoleKey.P,
            ConsoleKey.Q,
            ConsoleKey.R,
            ConsoleKey.S,
            ConsoleKey.T,
            ConsoleKey.U,
            ConsoleKey.V,
            ConsoleKey.W,
            ConsoleKey.X,
            ConsoleKey.Y,
            ConsoleKey.Z,
            ConsoleKey.D0,
            ConsoleKey.D1,
            ConsoleKey.D2,
            ConsoleKey.D3,
            ConsoleKey.D4,
            ConsoleKey.D5,
            ConsoleKey.D6,
            ConsoleKey.D7,
            ConsoleKey.D8,
            ConsoleKey.D9,
            ConsoleKey.NumPad0,
            ConsoleKey.NumPad1,
            ConsoleKey.NumPad2,
            ConsoleKey.NumPad3,
            ConsoleKey.NumPad4,
            ConsoleKey.NumPad5,
            ConsoleKey.NumPad6,
            ConsoleKey.NumPad7,
            ConsoleKey.NumPad8,
            ConsoleKey.NumPad9,
            ConsoleKey.OemComma,
            ConsoleKey.OemPeriod,
            ConsoleKey.OemMinus,
            ConsoleKey.OemPlus,
            ConsoleKey.Add,
            ConsoleKey.Subtract,
            ConsoleKey.Spacebar
        };
        peScript script;
        Layout ship;

        static void Main(string[] args)
        => new Program().Game();
        
        private void Game()
        {
            GameInitializing();
            GameLoop();
        }

        private void GameInitializing()
        {
            Console.CursorVisible = false;
            script = new peScript();
            script.InitPes("");
            ship = new Layout();
            ship.LayoutInit();

            Task.Run(() =>
            {
                GameInput();
            });
        }

        private void GameLoop()
        {
            while(!CLOSE)
            {
                ship.Show();
                Thread.Sleep(1000);
            }
        }

        private void GameInput()
        {
            while (true)
            {
                string result = "";
                var buffer = new List<char>();
                int cursor_pos = 0;
                bool enter_pressed = false;
                do
                {
                    result = "";
                    foreach (var c in buffer)
                        result += c;

                    int x = Console.CursorLeft;
                    int y = Console.CursorTop;
                    Console.CursorTop = Console.WindowTop + Console.WindowHeight - 1;
                    Console.CursorLeft = 0;
                    for (int i = 0; i < Console.WindowWidth-1; i++)
                        Console.Write(' ');
                    Console.CursorLeft = 0;
                    Console.CursorTop = Console.WindowTop + Console.WindowHeight - 1;
                    Console.ResetColor();
                    Console.Write(result);
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.CursorLeft = cursor_pos;
                    Console.Write(" ");
                    Console.ResetColor();
                    // Restore previous position
                    Console.SetCursorPosition(x, y);

                    var k = Console.ReadKey(true);
                    if (AvailableKeys.Contains(k.Key))
                    {
                        buffer.Insert(cursor_pos, k.KeyChar);
                        cursor_pos++;
                    }

                    if (k.Key == ConsoleKey.Enter)
                    {
                        enter_pressed = true;
                    }

                    if (k.Key == ConsoleKey.Backspace && buffer.Count > 0)
                    {
                        buffer.RemoveAt(buffer.Count - 1);
                        cursor_pos--;
                    }

                    if (k.Key == ConsoleKey.LeftArrow && cursor_pos > 0)
                        cursor_pos--;

                    if (k.Key == ConsoleKey.RightArrow && cursor_pos < buffer.Count)
                        cursor_pos++;

                } while (!enter_pressed);
                cursor_pos = 0;
                switch (InputMode)
                {
                    case 0:
                        ProcessInput(result);
                        break;
                    case 1:
                        LayoutInput(result);
                        break;
                }
            }
        }

        private void ProcessInput(string buffer)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(">" + buffer);
            Console.ResetColor();
            buffer = buffer.ToLower();
            var lul = string.Empty;
            if (buffer.Length > 6 && buffer.Substring(0, 4) == "run.")
                lul  = script.Run(buffer.Substring(4));
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(lul);
            Console.ResetColor();
        }

        private void LayoutInput(string buffer)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(">" + buffer);
            Console.ResetColor();
            buffer = buffer.ToLower();
            ship.Breach();
            ship.Show();
        }
    }
}
