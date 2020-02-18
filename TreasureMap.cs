using System;
using System.Collections.Generic;
using System.IO;

namespace FLS_Task1
{
    struct Point2D
    {
        public Point2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }

        public int Y { get; }

        public override string ToString()
        {
            return X + " " + Y;
        }
    }


    class TreasureMap
    {
        static void DrawLine(Point2D firstCrd, Point2D secondCrd, List<Point2D> lstRiv)
        {
            var dx = secondCrd.X - firstCrd.X;
            var dy = secondCrd.Y - firstCrd.Y;

            if (Math.Abs(dx) >= Math.Abs(dy))
            {
                var k = (double) dy / (double) dx;
                if (dx > 0)
                {
                    for (var i = 0; i <= dx; i++)
                    {
                        var x = i + firstCrd.X;
                        var y = (int) (i * k) + firstCrd.Y;
                        lstRiv.Add(new Point2D(x, y));
                        Console.SetCursorPosition(x, y);
                        Console.Write('~');
                    }
                }
                else if (dx < 0)
                {
                    for (var i = 0; i >= dx; i--)
                    {
                        var x = i + firstCrd.X;
                        var y = (int) (i * k) + firstCrd.Y;
                        lstRiv.Add(new Point2D(x, y));
                        Console.SetCursorPosition(x, y);
                        Console.Write('~');
                    }
                }
            }
            else if (Math.Abs(dx) <= Math.Abs(dy))
            {
                var k = (double) dx / (double) dy;
                if (dy > 0)
                {
                    for (var i = 0; i <= dy; i++)
                    {
                        var x = (int) (i * k) + firstCrd.X;
                        var y = i + firstCrd.Y;
                        lstRiv.Add(new Point2D(x, y));
                        Console.SetCursorPosition(x, y);
                        Console.Write('~');
                    }
                }
                else if (dy < 0)
                {
                    for (var i = 0; i >= dy; i--)
                    {
                        var x = (int) (i * k) + firstCrd.X;
                        var y = i + firstCrd.Y;
                        lstRiv.Add(new Point2D(x, y));
                        Console.SetCursorPosition(x, y);
                        Console.Write('~');
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            var filePath = "Map10.txt";
            var sr = new StreamReader(filePath);
            //not supported on Mac OS
            //Console.SetWindowSize(columns, rows);
            //Console.SetBufferSize(columns + 1, rows + 1);
            string line;
            var bridge = new Point2D();
            var treasure = new Point2D();
            var lstBase = new List<Point2D>();
            var lstRiver = new List<Point2D>();
            while ((line = sr.ReadLine()) != null)
            {
                line = line.Replace(" ", "");
                var openBrc = line.IndexOf('(');
                var closeBrc = line.IndexOf(')');
                if (line.Contains("BASE"))
                {
                    line = line.Substring(openBrc + 1, closeBrc - openBrc - 1);
                    var coordinates = line.Split(':');
                    var crd1 = coordinates[0].Split(',');
                    var crd2 = coordinates[1].Split(',');

                    var firstCrd = new Point2D(Convert.ToInt32(crd1[0]), Convert.ToInt32(crd1[1]));
                    var secondCrd = new Point2D(Convert.ToInt32(crd2[0]), Convert.ToInt32(crd2[1]));

                    for (var i = 0; i <= Math.Abs(firstCrd.X - secondCrd.X); i++)
                    {
                        Console.SetCursorPosition(firstCrd.X + i, firstCrd.Y);
                        lstBase.Add(new Point2D(firstCrd.X + i, firstCrd.Y));
                        Console.Write('@');
                    }

                    for (var i = 0; i <= Math.Abs(firstCrd.Y - secondCrd.Y); i++)
                    {
                        Console.SetCursorPosition(secondCrd.X, firstCrd.Y + i);
                        lstBase.Add(new Point2D(secondCrd.X, firstCrd.Y + i));
                        Console.Write('@');
                    }

                    for (var i = 0; i <= Math.Abs(firstCrd.X - secondCrd.X); i++)
                    {
                        Console.SetCursorPosition(secondCrd.X - i, secondCrd.Y);
                        lstBase.Add(new Point2D(secondCrd.X - i, secondCrd.Y));
                        Console.Write('@');
                    }

                    for (var i = 0; i <= Math.Abs(firstCrd.Y - secondCrd.Y); i++)
                    {
                        Console.SetCursorPosition(firstCrd.X, secondCrd.Y - i);
                        lstBase.Add(new Point2D(firstCrd.X, secondCrd.Y - i));
                        Console.Write('@');
                    }
                }
                else if (line.Contains("Treasure"))
                {
                    line = line.Substring(openBrc + 1, closeBrc - openBrc - 1);
                    var coordinates = line.Split(',');
                    var crd = new Point2D(Convert.ToInt32(coordinates[0]), Convert.ToInt32(coordinates[1]));
                    treasure = crd;
                    Console.SetCursorPosition(crd.X, crd.Y);
                    Console.WriteLine('+');
                }
                else if (line.Contains("bridge"))
                {
                    line = line.Substring(openBrc + 1, closeBrc - openBrc - 1);
                    var coordinates = line.Split(',');
                    var crd = new Point2D(Convert.ToInt32(coordinates[0]), Convert.ToInt32(coordinates[1]));
                    bridge = crd;
                }
                else if (line.Contains("WATER"))
                {
                    line = line.Substring(openBrc + 1, closeBrc - openBrc - 1);
                    line = line.Replace("a", "");
                    var coordinates = line.Split("->");
                    for (var i = 0; i < coordinates.Length - 1; i++)
                    {
                        var crd1 = coordinates[i].Split(',');
                        var crd2 = coordinates[i + 1].Split(',');
                        var firstCrd = new Point2D(Convert.ToInt32(crd1[0]), Convert.ToInt32(crd1[1]));
                        var secondCrd = new Point2D(Convert.ToInt32(crd2[0]), Convert.ToInt32(crd2[1]));

                        DrawLine(firstCrd, secondCrd, lstRiver);
                    }
                }
            }

            Console.SetCursorPosition(bridge.X, bridge.Y);
            Console.WriteLine('#');
            Console.ReadLine();
        }
    }
}