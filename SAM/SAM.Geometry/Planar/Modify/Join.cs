﻿using System.Collections.Generic;
using System.Linq;

namespace SAM.Geometry.Planar
{
    public static partial class Modify
    {
        public static Polyline2D Join(this Polyline2D polyline2D_1, Polyline2D polyline2D_2, double tolerance = Core.Tolerance.Distance)
        {
            if (polyline2D_1 == null || polyline2D_2 == null)
                return null;

            List<Segment2D> segment2Ds_1 = polyline2D_1.GetSegments();
            List<Segment2D> segment2Ds_2 = polyline2D_2.GetSegments();

            if (segment2Ds_1 == null || segment2Ds_2 == null || segment2Ds_1.Count == 0 || segment2Ds_2.Count == 0)
                return null;

            if (segment2Ds_1[0].Start.Distance(segment2Ds_2[0].Start) < tolerance)
            {
                segment2Ds_2[0] = new Segment2D(segment2Ds_1[0].Start, segment2Ds_2[0].End);
                segment2Ds_2.Reverse();
                segment2Ds_2.ForEach(x => x.Reverse());

                segment2Ds_2.AddRange(segment2Ds_1);
                return new Polyline2D(segment2Ds_2);
            }

            int count_1 = segment2Ds_1.Count;

            if (segment2Ds_1[count_1 - 1].End.Distance(segment2Ds_2[0].Start) < tolerance)
            {
                segment2Ds_2[0] = new Segment2D(segment2Ds_1[count_1 - 1].End, segment2Ds_2[0].End);

                segment2Ds_1.AddRange(segment2Ds_2);
                return new Polyline2D(segment2Ds_1);
            }

            int count_2 = segment2Ds_2.Count;

            if (segment2Ds_1[count_1 - 1].End.Distance(segment2Ds_2[count_2 - 1].End) < tolerance)
            {
                segment2Ds_2.Reverse();
                segment2Ds_2.ForEach(x => x.Reverse());
                segment2Ds_2[0] = new Segment2D(segment2Ds_1[count_1 - 1].End, segment2Ds_2[0].End);

                segment2Ds_1.AddRange(segment2Ds_2);
                return new Polyline2D(segment2Ds_1);
            }

            if (segment2Ds_1[0].Start.Distance(segment2Ds_2[count_2 - 1].End) < tolerance)
            {
                segment2Ds_2[count_2 - 1] = new Segment2D(segment2Ds_2[count_2 - 1].Start, segment2Ds_1[0].Start);

                segment2Ds_2.AddRange(segment2Ds_1);
                return new Polyline2D(segment2Ds_2);
            }

            return null;
        }
    }
}