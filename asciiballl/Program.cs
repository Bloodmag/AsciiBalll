using System;
using System.Threading;
using System.Timers;
using ConsoleWindowRenderer;

namespace asciiballl
{
    class Program
    {
        static string[] map = new string[13] { "#*#*#*#*",
                                        "*000000#",
                                        "#000000*",
                                        "*000000#",
                                        "#000000*",
                                        "*000000#",
                                        "#000000*",
                                        "*000000#",
                                        "#000000*",
                                        "*000000#",
                                        "#000000*",
                                        "*000000#",
                                        "#*#*#*#*"};

        static int mapHeight = 13;
        static int mapWidth = 8;

        static char[] brush = new char[] { '\u2588' ,'\u2593', '\u2592', '\u2591' };
        static char[] brush2 = " .:-=+*#%@\u2588".ToCharArray();

        static int viewHeight = 40;
        static int viewWidth = 120;

        static char[] frame = new string(' ', viewWidth * viewHeight).ToCharArray();

        static void Main(string[] args)
        {
            W.SetupBuffer(viewWidth, viewHeight);
            Console.CursorVisible = false;

            Vector2d pos = new Vector2d(2, 2);
            Vector2d dir = new Vector2d(1, 0);
            while (true)
            {
                //Console.Clear();
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
                for(int i = 0; i < viewWidth; i++)
                {
                    var h = GetHeights(pos, dir, Math.PI * (i - viewWidth / 2)/viewWidth / 4);
                    for(int j = 0; j<viewHeight; j++)
                    {
                        if(j>=h.Item1&&j<h.Item2)
                        //frame[j * viewWidth + i] = brush[h.Item3];
                        {
                            //Console.ForegroundColor = (h.Item4) ? ConsoleColor.Red : ConsoleColor.White;
                            //Console.SetCursorPosition(i, j);
                            //Console.Write(brush[h.Item3]);
                            frame[j * viewWidth + i] = h.Item4? brush[0]:brush[1];
                        }
                        else
                        {
                            //Console.Write(' ');
                            frame[j * viewWidth + i] = ' ';
                        }
                            
                    }
                }
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.LeftArrow) dir.Rotate(-0.1);
                    if (key.Key == ConsoleKey.RightArrow) dir.Rotate(0.1);
                    if (key.Key == ConsoleKey.UpArrow) pos+=dir/10;
                }
                for(int i = 0; i< viewHeight * viewWidth;i++)
                {
                    W.Buf[i].Char.AsciiChar = (byte)frame[i];
                    W.Buf[i].Attributes = 3;
                }
                W.Draw();
                //Console.SetCursorPosition(0, 0);
                //Console.Write(frame);
                //Console.SetCursorPosition(0, 0);
                //Console.Write("dir.X = {0}  dir.y = {1}   {2}",dir.X.ToString(),dir.Y.ToString(),brush[0]);
                Thread.Sleep(50);
            }

        }

        static (int,int,int, bool) GetHeights(Vector2d pos, Vector2d dir, double angle)
        {
            Vector2d newdir = new Vector2d(dir.X, dir.Y);
            newdir.Rotate(angle);
            double distanceToWall = 0;
            double maxDistance = 16;
            bool hit = false;
            char e = ' ';
            while (!hit && distanceToWall < maxDistance)
            {
                distanceToWall += 0.1;
                e = map[(int)Math.Round(pos.Y + newdir.Y * distanceToWall)][(int)Math.Round(pos.X + newdir.X * distanceToWall)];
                if ( e == '#'|| e=='*') hit = true;
            }
            int bot = viewHeight - (int)Math.Round(viewHeight / 2 - (viewHeight / distanceToWall));
            int top = viewHeight / 2 ;
            return (top, bot,(int)Math.Round(10 * (distanceToWall / maxDistance)),(e=='#'));
        }
    }

    public static class Renderer
    {
        static int viewHeight = 40;
        static int viewWidth = 120;
    }
    public class Vector2d
    {
        private double x;
        private double y;

        public Vector2d(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        public double Length { get => Math.Sqrt(x * x + y * y); }

        public void Normalize()
        {
            double l = Length;
            x /= l;
            y /= l;
        }

        public void Rotate(double angle)
        {
            double tmp = x * Math.Cos(angle) - y * Math.Sin(angle);
            y = x * Math.Sin(angle) + y * Math.Cos(angle);
            x = tmp;
        }

        public static Vector2d operator - (Vector2d v)
        {
            return new Vector2d(-v.x, -v.y);
        }

        public static Vector2d operator - (Vector2d l, Vector2d r)
        {
            return new Vector2d(l.x-r.x,l.y-r.y);
        }

        public static Vector2d operator +(Vector2d l, Vector2d r)
        {
            return new Vector2d(l.x + r.x, l.y + r.y);
        }
        public static Vector2d operator /(Vector2d l, double r)
        {
            return new Vector2d(l.X/r, l.Y / r);
        }

    }
}
