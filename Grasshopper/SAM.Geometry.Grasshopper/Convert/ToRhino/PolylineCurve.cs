﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel.Types;

namespace SAM.Geometry.Grasshopper
{
    public static partial class Convert
    {
        public static Rhino.Geometry.PolylineCurve ToRhino_PolylineCurve(this IEnumerable<Spatial.ICurve3D> curve3Ds)
        {
            List<Rhino.Geometry.Point3d> points = curve3Ds.ToList().ConvertAll(x => x.GetEnd().ToRhino());
            points.Add(curve3Ds.First().GetEnd().ToRhino());

            return new Rhino.Geometry.PolylineCurve(points);
        }


        public static Rhino.Geometry.PolylineCurve ToRhino_PolylineCurve(this Spatial.Polygon3D polygon3D)
        {
            List<Rhino.Geometry.Point3d> points = polygon3D.GetPoints().ConvertAll(x => x.ToRhino());
            points.Add(polygon3D.GetPoints().First().ToRhino());

            return new Rhino.Geometry.PolylineCurve(points);
        }

        public static Rhino.Geometry.PolylineCurve ToRhino_PolylineCurve(this Spatial.Polyline3D polyline3D)
        {
            List<Rhino.Geometry.Point3d> points = polyline3D.Points.ConvertAll(x => x.ToRhino());

            return new Rhino.Geometry.PolylineCurve(points);
        }

        public static Rhino.Geometry.PolylineCurve ToRhino_PolylineCurve(this Planar.Polygon2D polygon2D)
        {
            List<Rhino.Geometry.Point3d> points = polygon2D.Points.ConvertAll(x => x.ToRhino());
            points.Add(polygon2D.Points.First().ToRhino());

            return new Rhino.Geometry.PolylineCurve(points);
        }

        public static Rhino.Geometry.PolylineCurve ToRhino_PolylineCurve(this Rhino.Geometry.Curve curve)
        {
            Rhino.Geometry.PolylineCurve polylineCurve = curve.ToPolyline(0, 0, 0.2, 0);
            return polylineCurve;
        }
    }
}