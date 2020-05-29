﻿using NetTopologySuite.Geometries;
using NetTopologySuite.Noding.Snapround;
using NetTopologySuite.Operation.Polygonize;
using SAM.Geometry.Planar;
using System.Collections.Generic;
using System.Linq;

namespace SAM.Geometry
{
    public static partial class Convert
    {
        public static List<Polygon> ToNTS_Polygons(this IEnumerable<Segment2D> segment2Ds, double tolerance = Core.Tolerance.MicroDistance)
        {
            if (segment2Ds == null)
                return null;

            if (segment2Ds.Count() == 0)
                return new List<Polygon>();

            return ToNTS_Polygons(segment2Ds.ToList().ConvertAll(x => x.ToNTS(tolerance)), tolerance);
        }

        public static List<Polygon> ToNTS_Polygons(this IEnumerable<NetTopologySuite.Geometries.Geometry> geometries, double tolerance = Core.Tolerance.MicroDistance)
        {
            if (geometries == null)
                return null;

            List<Polygon> result = new List<Polygon>();

            if (geometries.Count() == 0)
                return result;

            Polygonizer polygonizer = new Polygonizer(false);
            GeometryNoder geometryNoder = new GeometryNoder(new PrecisionModel(1 / tolerance));

            List<LineString> lineStrings = geometryNoder.Node(geometries).ToList();
            if (lineStrings == null || lineStrings.Count == 0)
                return result;

            Modify.RemoveAlmostSimilar_NTS(lineStrings, tolerance);
            if (lineStrings == null || lineStrings.Count == 0)
                return result;

            List<LineString> lineStrings_Short = lineStrings.FindAll(x => x.Length < tolerance && x.Coordinates.Length > 1);
            if(lineStrings_Short != null && lineStrings_Short.Count > 0)
            {
                lineStrings.RemoveAll(x => lineStrings_Short.Contains(x));
                foreach (LineString lineString_Short in lineStrings_Short)
                {
                    Coordinate coordinate_Main = lineString_Short[0];

                    for(int i = 1; i < lineStrings_Short.Count; i++)
                    {
                        Coordinate coordinate_ToReplace = lineString_Short[i];
                        foreach (LineString lineString in lineStrings)
                        {
                            for(int j = 0; j < lineString.Count; j++)
                            {
                                if (lineString[j].Distance(coordinate_ToReplace) <= tolerance)
                                    lineString[j] = coordinate_Main;
                            }
                        }
                    }
                }
            }

            polygonizer.Add(lineStrings.ToArray());

            IEnumerable<NetTopologySuite.Geometries.Geometry> geometries_Result = polygonizer.GetPolygons();
            if (geometries_Result == null)
                return null;

            if (geometries_Result.Count() == 0)
            {
                result.AddRange(geometries.ToList().FindAll(x => x is Polygon).Cast<Polygon>());
                result.AddRange(geometries.ToList().FindAll(x => x is LinearRing).ConvertAll(x => new Polygon((LinearRing)x)));
                return result;
            }

            foreach (NetTopologySuite.Geometries.Geometry geometry in geometries_Result)
            {
                Polygon polygon = geometry as Polygon;
                if (polygon == null)
                    continue;

                polygon = Planar.Query.SimplifyByNTS_Snapper(polygon, tolerance);
                if (polygon.Area < tolerance)
                    continue;

                result.Add(polygon);
            }

            return result;
        }
    }
}