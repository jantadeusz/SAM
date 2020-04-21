﻿using System.Collections.Generic;
using System.Linq;

using NetTopologySuite.Geometries;

using SAM.Geometry.Planar;

namespace SAM.Geometry
{
    public static partial class Convert
    {
        public static List<Coordinate> ToNetTopologySuite(this IEnumerable<Point2D> point2Ds, double tolerance = Core.Tolerance.MicroDistance)
        {
            if (point2Ds == null)
                return null;

            return point2Ds.ToList().ConvertAll(x => x.ToNetTopologySuite(tolerance));
        }
    }
}