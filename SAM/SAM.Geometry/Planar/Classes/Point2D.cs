﻿using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace SAM.Geometry.Planar
{
    /// <summary>
    /// Planar Point
    /// </summary>
    public class Point2D : SAMGeometry, ISAMGeometry2D
    {
        private double[] coordinates = new double[2] { 0, 0 };

        public Point2D()
        {
            coordinates = new double[2] { 0, 0 };
        }

        public Point2D(JObject jObject)
        {
            FromJObject(jObject);
        }

        public Point2D(double x, double y)
        {
            coordinates = new double[2] { x, y };
        }

        public Point2D(double[] coordinates)
        {
            coordinates[0] = this.coordinates[0];
            coordinates[1] = this.coordinates[1];
        }

        public Point2D(Point2D point2D)
        {
            coordinates = new double[2] { point2D[0], point2D[1] };
        }

        public double this[int index]
        {
            get
            {
                return coordinates[index];
            }
            set
            {
                coordinates[index] = value;
            }
        }

        public double X
        {
            get
            {
                return coordinates[0];
            }
            set
            {
                if (coordinates == null)
                    coordinates = new double[2];

                coordinates[0] = value;
            }
        }

        public double Y
        {
            get
            {
                return coordinates[1];
            }
            set
            {
                if (coordinates == null)
                    coordinates = new double[2];

                coordinates[1] = value;
            }
        }

        public void Move(Vector2D vector2D)
        {
            coordinates[0] += vector2D[0];
            coordinates[1] += vector2D[1];
        }

        public Point2D GetMoved(Vector2D vector2D)
        {
            return new Point2D(vector2D[0] + coordinates[0], vector2D[1] + coordinates[1]);
        }

        public double Distance(Point2D point2D)
        {
            return Vector(point2D).Length;
        }

        public Vector2D Vector(Point2D point2D)
        {
            return new Vector2D(coordinates[0] - point2D[0], coordinates[1] - point2D[1]);
        }

        public Vector2D AsVector()
        {
            return new Vector2D(coordinates[0], coordinates[1]);
        }

        public override string ToString()
        {
            return string.Format("{0}(X={1},Y={2})", GetType().Name, coordinates[0], coordinates[1]);
        }

        public string ToString(double tolerance = Core.Tolerance.Distance)
        {
            return string.Format("Point2D(X={0},Y={1})", Core.Query.Round(coordinates[0], tolerance), Core.Query.Round(coordinates[1], tolerance));
        }

        public bool AlmostEquals(Point2D point2D, double tolerance = Core.Tolerance.Distance)
        {
            return ((System.Math.Abs(coordinates[0] - point2D.coordinates[0]) < tolerance) && (System.Math.Abs(coordinates[1] - point2D.coordinates[1]) < tolerance));
        }

        public bool AlmostOnSegment(Segment2D segment2D, double tolerance = Core.Tolerance.Distance)
        {
            Segment2D segment2D_temp = new Segment2D(new Point2D(0, 0), new Point2D(segment2D[1].X - segment2D[0].X, segment2D[1].Y - segment2D[0].Y));
            Point2D aPoint2D = new Point2D(coordinates[0] - segment2D[0].X, coordinates[1] - segment2D[0].Y);
            return System.Math.Abs(segment2D_temp[1].X * aPoint2D.Y - aPoint2D.X * segment2D_temp[1].Y) < tolerance;
        }

        public bool IsValid()
        {
            return !double.IsNaN(coordinates[0]) && !double.IsNaN(coordinates[1]);
        }

        public bool IsZero()
        {
            return coordinates[0] == 0 && coordinates[1] == 0;
        }

        public void Round(int decimals = Core.Rounding.Distance)
        {
            if (decimals == Core.Rounding.NoRounding)
                return;

            coordinates[0] = System.Math.Round(coordinates[0], decimals);
            coordinates[1] = System.Math.Round(coordinates[1], decimals);
        }

        public void Round(double tolerance = Core.Tolerance.Distance)
        {
            coordinates[0] = Core.Query.Round(coordinates[0], tolerance);
            coordinates[1] = Core.Query.Round(coordinates[1], tolerance);
        }

        public void Mirror(Point2D point2D)
        {
            this.Move(new Vector2D(point2D, this));
        }

        public void Mirror(Segment2D segment2D)
        {
            this.Move(new Vector2D(segment2D.Project(this), this));
        }

        public void Scale(Point2D point2D, double factor)
        {
            if (point2D == null)
                return;

            if (factor == 0)
                return;

            if (factor == 1)
            {
                coordinates[0] = point2D.coordinates[0];
                coordinates[1] = point2D.coordinates[1];
                return;
            }

            Vector2D vector = ToVector(point2D);
            vector.Length = vector.Length * factor;

            coordinates[0] = point2D.coordinates[0] + vector[0];
            coordinates[1] = point2D.coordinates[1] + vector[1];
        }

        public void Scale(double factor)
        {
            if (factor == 1)
                return;

            coordinates[0] = coordinates[0] * factor;
            coordinates[1] = coordinates[1] * factor;
        }

        public Point2D GetScaled(double factor)
        {
            Point2D point2D = new Point2D(this);

            point2D.Scale(factor);
            return point2D;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point2D))
                return false;

            Point2D point2D = (Point2D)obj;
            return point2D.coordinates[0].Equals(coordinates[0]) && point2D.coordinates[1].Equals(coordinates[1]);
        }

        public bool Equals(Point2D point2D, double tolerance)
        {
            if (tolerance == 0)
                return Equals(point2D);

            return Distance(point2D) <= tolerance;
        }

        public bool IsNaN()
        {
            return double.IsNaN(coordinates[0]) || double.IsNaN(coordinates[1]);
        }

        public Point2D Closest(IEnumerable<Point2D> point2Ds)
        {
            return Query.Closest(point2Ds, this);
        }

        public override ISAMGeometry Clone()
        {
            return new Point2D(this);
        }

        public Vector2D ToVector(Point2D point2D = null)
        {
            if (point2D == null)
                return new Vector2D(coordinates[0], coordinates[1]);
            else
                return new Vector2D(coordinates[0] - point2D[0], coordinates[1] - point2D[1]);
        }

        public override bool FromJObject(JObject jObject)
        {
            coordinates[0] = jObject.Value<double>("X");
            coordinates[1] = jObject.Value<double>("Y");

            return true;
        }

        public override JObject ToJObject()
        {
            JObject jObject = base.ToJObject();
            if (jObject == null)
                return null;

            jObject.Add("X", coordinates[0]);
            jObject.Add("Y", coordinates[1]);
            return jObject;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + coordinates[0].GetHashCode();
            hash = (hash * 7) + coordinates[1].GetHashCode();
            return hash;
        }

        public static Point2D Invalid
        {
            get
            {
                return new Point2D(double.NaN, double.NaN);
            }
        }

        public static Point2D Zero
        {
            get
            {
                return new Point2D(0, 0);
            }
        }

        public static bool operator ==(Point2D point2D_1, Point2D point2D_2)
        {
            if (ReferenceEquals(point2D_1, null) && ReferenceEquals(point2D_2, null))
                return true;

            if (ReferenceEquals(point2D_1, null) || ReferenceEquals(point2D_2, null))
                return false;

            return point2D_1.coordinates[0] == point2D_2.coordinates[0] && point2D_1.coordinates[1] == point2D_2.coordinates[1];
        }

        public static bool operator !=(Point2D point2D_1, Point2D point2D_2)
        {
            return point2D_1?.coordinates[0] != point2D_2?.coordinates[0] || point2D_1?.coordinates[1] != point2D_2?.coordinates[1];
        }

        public static Vector2D operator -(Point2D point2D_1, Point2D point2D_2)
        {
            return new Vector2D(point2D_1.coordinates[0] - point2D_2.coordinates[0], point2D_1.coordinates[1] - point2D_2.coordinates[1]);
        }
    }
}