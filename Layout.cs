using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PES
{
    class Layout
    {
        private List<List<string>> coords;

        public void LayoutInit()
        {
            coords = new List<List<string>>();
            for (int i = 0; i < 10; i++)
            {
                coords.Add(new List<string>());
                for (int o = 0; o < 10; o++)
                {
                    coords[i].Add(" ");
                }
            }
            coords[2][4] = "_";
            coords[3][3] = coords[3][5] = "|";
            coords[3][6] = coords[3][7] = coords[3][8] = "_";
            coords[4][2] = "_";
            coords[4][3] = coords[4][5] = coords[4][9] = "|";
            coords[5][1] = coords[5][9] = "|";
            coords[6][1] = coords[6][5] = coords[6][9] = "|";
            coords[6][2] = coords[6][3] = coords[6][4] = "_";
            coords[7][5] = coords[8][5] = coords[9][5] = "|";
            coords[7][9] = coords[8][9] = coords[9][9] = "|";
            coords[9][6] = coords[9][7] = coords[9][8] = "_";
        }

        public void Show()
        {
            int x = Console.CursorLeft;
            int y = Console.CursorTop;
            Console.CursorTop = Console.WindowTop + Console.WindowHeight - 1;
            Console.CursorLeft = 0;
            for (int i = 0; i < Console.WindowWidth - 1; i++)
                Console.Write(' ');
            Console.CursorLeft = 0;
            Console.CursorTop = Console.WindowTop + Console.WindowHeight - 1;
            foreach (var xcoord in coords)
            {
                foreach (var ycoord in xcoord)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    if (ycoord[0] == '!')
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(ycoord[1]);
                    }
                    else
                    {
                        Console.Write(ycoord);
                    }
                }
                Console.WriteLine();
            }
        }

        public void Breach(int X = -1, int Y = -1)
        {
            if (X < 0 && Y < 0)
            {
                while (true)
                {
                    var r = new Random();
                    X = r.Next(0, 9);
                    Y = r.Next(0, 9);
                    if (coords[X][Y] != " " && coords[X][Y][0] != '!')
                    {
                        coords[X][Y] = "!" + coords[X][Y];
                        break;
                    }
                }
            }
            else
            {
                if (coords[X][Y] != " " && coords[X][Y][0] != '!')
                {
                    coords[X][Y] = "!" + coords[X][Y];
                }
                else
                {
                    
                }
            }
        }
    }
}
