#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace FLS_Task1
{
    class Point2D
    {
        public Point2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point2D()
        {
            X = 0;
            Y = 0;
        }

        public int X { get; }

        public int Y { get; }

        public override string ToString()
        {
            return X + " " + Y;
        }

        public override bool Equals(object? obj)
        {
            return obj is Point2D other && Equals(other);
        }

        public bool Equals(Point2D other)
        {
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(Point2D left, Point2D right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Point2D left, Point2D right)
        {
            return !left.Equals(right);
        }
    }

    class Node
    {
        public Point2D Position { get; set; }
        public int PathLengthFromStart { get; set; }
        public Node PrevPosition { get; set; }
        public int HeuristicEstimatedPathLength { get; set; } //approx distance to finish
        public int EstimateFullPathLength => this.PathLengthFromStart + this.HeuristicEstimatedPathLength;
    }

    class TreasureMap
    {
        private const int Cost = 1; //cost of one point move

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

        static int GetHeuristicPathLength(Point2D from, Point2D to)
        {
            return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
        }


        static Collection<Node> GetNearNodes(Node node, Point2D finish, int[,] field, List<Point2D> lstRiv)
        {
            var result = new Collection<Node>();
            var nearestPoints = new Point2D[4];
            nearestPoints[0] = new Point2D(node.Position.X + 1, node.Position.Y);
            nearestPoints[1] = new Point2D(node.Position.X - 1, node.Position.Y);
            nearestPoints[2] = new Point2D(node.Position.X, node.Position.Y + 1);
            nearestPoints[3] = new Point2D(node.Position.X, node.Position.Y - 1);

            foreach (var point in nearestPoints)
            {
                if (point.X < 0 || point.X >= field.GetLength(0)) continue;
                if (point.Y < 0 || point.Y >= field.GetLength(1)) continue;
                if (lstRiv.Contains(point)) continue;
                var neighbour = new Node()
                {
                    Position = point,
                    PrevPosition = node,
                    PathLengthFromStart = node.PathLengthFromStart + Cost,
                    HeuristicEstimatedPathLength = GetHeuristicPathLength(point, finish)
                };
                result.Add(neighbour);
            }

            return result;
        }

        static List<Point2D> GetPathForNode(Node node)
        {
            var result = new List<Point2D>();
            var currentNode = node;
            while (currentNode != null)
            {
                result.Add(currentNode.Position);
                currentNode = currentNode.PrevPosition;
            }

            result.Reverse();
            return result;
        }

        static List<Point2D> FindPath(int[,] field, Point2D start, Point2D finish, List<Point2D> lstRiv)
        {
            var closedSet = new Collection<Node>();
            var openedSet = new Collection<Node>();
            var beginning = new Node()
            {
                Position = start,
                PrevPosition = null,
                PathLengthFromStart = 0,
                HeuristicEstimatedPathLength = GetHeuristicPathLength(start, finish)
            };
            openedSet.Add(beginning);
            while (openedSet.Count > 0)
            {
                var currentNode =
                    openedSet.OrderBy(node => node.EstimateFullPathLength)
                        .First(); //choose point with the lowest Full Length
                if (currentNode.Position.Equals(finish)) return GetPathForNode(currentNode);
                openedSet.Remove((currentNode));
                closedSet.Add(currentNode);
                foreach (var neighbour in GetNearNodes(currentNode, finish, field, lstRiv))
                {
                    if (closedSet.Count(node => node.Position.Equals(neighbour.Position)) > 0)
                        continue; //if point is already in closedSet - skip it
                    var openNode =
                        openedSet.FirstOrDefault(node =>
                            node.Position.Equals(neighbour
                                .Position)); //add point to openedSet, counting path from it to finish
                    if (openNode == null)
                        openedSet.Add(neighbour);
                    else if (openNode.PathLengthFromStart > neighbour.PathLengthFromStart)
                    {
                        openNode.PrevPosition = currentNode;
                        openNode.PathLengthFromStart = neighbour.PathLengthFromStart;
                    }
                }
            }

            return null;
        }

        static void Main(string[] args)
        {
            const string filePath = "Map1.txt"; //ENTER THE PATH TO THE MAP HERE
            var sr = new StreamReader(filePath);
            //not supported on Mac OS
            //Console.SetWindowSize(columns, rows);
            //Console.SetBufferSize(columns + 1, rows + 1);
            string line;
            var bridge = new Point2D();
            var treasure = new Point2D();
            var lstBase = new List<Point2D>();
            var lstRiver = new List<Point2D>();
            const int columns = 60;
            const int rows = 40;
            var field = new int [columns, rows]; //size of the map
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

            lstRiver.Remove(bridge);
            Console.SetCursorPosition(bridge.X, bridge.Y);
            Console.WriteLine('#');

            if (FindPath(field, lstBase[0], treasure, lstRiver).Count > 0)
            {
                foreach (var node in FindPath(field, lstBase[0], treasure, lstRiver))
                {
                    Console.SetCursorPosition(node.X, node.Y);
                    Console.Write('%');
                }
            }
            else
            {
                Console.WriteLine("There is no appropriate path");
            }

            Console.ReadLine();
        }
    }
}