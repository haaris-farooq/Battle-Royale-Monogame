using System;
using System.Collections.Generic;
using System.Text;

namespace ComputingProjectHF
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }           

        //checks if two points are equals
        public static bool Equals(Point checkPoint1, Point checkPoint2)
        {
            return checkPoint1.X == checkPoint2.X && checkPoint1.Y == checkPoint2.Y;
        }

        //add two points together
        public static Point Plus(Point addPoint1, Point addPoint2)
        {
            return new Point(addPoint1.X + addPoint2.X, addPoint1.Y + addPoint2.Y);
        }

        //minus one point from another point
        public static Point Minus(Point subPoint1, Point subPoint2)
        {
            return new Point(subPoint1.X - subPoint2.X, subPoint1.Y - subPoint2.Y);
        }
    }
}
