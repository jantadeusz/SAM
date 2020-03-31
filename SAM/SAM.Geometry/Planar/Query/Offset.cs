﻿using System.Collections.Generic;
using System.Linq;

namespace SAM.Geometry.Planar
{
    public static partial class Query
    {
        public static List<Polygon2D> Offset(this Polygon2D polygon2D, double offset, bool inside, bool simplify = true, bool orient = true, double tolerance = Core.Tolerance.Distance)
        {
            return Offset(polygon2D, new double[] { offset }, inside, simplify, orient, tolerance);
        }

        public static List<Polygon2D> Offset(this Polygon2D polygon2D, IEnumerable<double> offsets, bool inside, bool simplify = true, bool orient = true, double tolerance = Core.Tolerance.Distance)
        {
            if (polygon2D == null || offsets == null)
                return null;

            if (offsets.Count() < 1)
                return null;

            int count = polygon2D.Count;

            List<double> offsets_Temp = new List<double>(offsets);
            double offset_Temp= offsets.Last();
            while (offsets_Temp.Count < count)
                offsets_Temp.Add(offset_Temp);

            if (inside)
                return Offset_Inside(polygon2D, offsets_Temp, simplify, orient, tolerance);
            else
                return Offset_Outside(polygon2D, offsets_Temp, simplify, orient, tolerance);
        }

        private static List<Polygon2D> Offset_Inside(this Polygon2D polygon2D, List<double> offsets, bool simplify = true, bool orient = true, double tolerance = Core.Tolerance.Distance)
        {
            Orientation orientation = polygon2D.GetOrientation();

            List<Segment2D> segment2Ds = polygon2D?.GetSegments();
            if (segment2Ds == null || segment2Ds.Count == 0)
                return null;

            List<Segment2D> segment2Ds_Offset = new List<Segment2D>();
            for(int i =0; i < segment2Ds.Count; i++)
            {
                Segment2D segment2D = segment2Ds[i];

                Segment2D segment2D_Offset = segment2D.Offset(offsets[i], orientation);
                segment2D_Offset = segment2D_Offset.ExtendOrTrim(polygon2D, tolerance);

                if (segment2D_Offset != null && segment2D_Offset.GetLength() >= tolerance)
                    segment2Ds_Offset.Add(segment2D_Offset);
            }

            segment2Ds_Offset = Modify.Split(segment2Ds_Offset, tolerance);

            for (int i = 0; i < segment2Ds.Count; i++)
            {
                Segment2D segment2D = segment2Ds[i];
                segment2Ds_Offset.RemoveAll(x => segment2D.Distance(x) < offsets[i] - Core.Tolerance.MacroDistance);
            }

            List<Polygon2D> polygon2Ds_Temp = new PointGraph2D(segment2Ds_Offset, false, tolerance).GetPolygon2Ds();
            if (polygon2Ds_Temp == null || polygon2Ds_Temp.Count == 0)
                return null;

            List<Polygon2D> polygon2Ds = new List<Polygon2D>();
            for (int i = 0; i < polygon2Ds_Temp.Count; i++)
            {
                Polygon2D polygon2D_Temp = polygon2Ds_Temp[i];

                List<Segment2D> segment2Ds_Temp = Query.IntersectionSegment2Ds(polygon2D, polygon2Ds_Temp, false, tolerance);
                if (segment2Ds_Temp != null && segment2Ds_Temp.Count > 0)
                    continue;

                polygon2Ds.Add(polygon2D_Temp);
            }

            polygon2Ds = new PointGraph2D(polygon2Ds, false, tolerance).GetPolygon2Ds_External();
            if (polygon2Ds == null || polygon2Ds.Count == 0)
                return null;

            for (int i = 0; i < polygon2Ds.Count; i++)
            {
                if (simplify)
                {
                    List<Point2D> point2Ds_Simplify = polygon2Ds[i].GetPoints();
                    Modify.SimplifyByAngle(point2Ds_Simplify, true, Core.Tolerance.Angle);
                    polygon2Ds[i] = new Polygon2D(point2Ds_Simplify);
                }

                if (orient)
                    polygon2Ds[i].SetOrientation(orientation);
            }

            return polygon2Ds;
        }

        private static List<Polygon2D> Offset_Outside(this Polygon2D polygon2D, List<double> offsets, bool simplify = true, bool orient = true, double tolerance = Core.Tolerance.Distance)
        {
            //TODO: Implement Outside Offset
            throw new System.Exception();
        }
    }
}