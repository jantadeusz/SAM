﻿using System.Linq;
using System.Collections.Generic;


namespace SAM.Geometry.Planar
{
    public static partial class Create
    {
        public static Polyline2D Polyline2D(this IEnumerable<Segment2D> segment2Ds, bool split = true, double tolerance = Tolerance.MicroDistance)
        {
            if (segment2Ds == null || segment2Ds.Count() == 0)
                return null;

            if (segment2Ds.Count() == 1)
                return new Polyline2D(new List<Point2D>() { segment2Ds.ElementAt(0).Start, segment2Ds.ElementAt(0).End});

            return new PointGraph2D(segment2Ds, split).GetPolyline2D();
        }

    }
}