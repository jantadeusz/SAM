﻿using NetTopologySuite.Geometries;
using SAM.Geometry;
using SAM.Geometry.Planar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SAM.Analytical
{
    public static partial class Query
    {
        public static List<Panel> MergeCoplanarPanels(this IEnumerable<Panel> panels, double offset, double tolerance = Core.Tolerance.Distance)
        {
            if (panels == null)
                return null;

            Dictionary<PanelGroup, List<Panel>> dictionary = new Dictionary<PanelGroup, List<Panel>>();
            foreach (PanelGroup panelGroup in Enum.GetValues(typeof(PanelGroup)))
                dictionary[panelGroup] = new List<Panel>();

            foreach (Panel panel in panels)
            {
                if (panel == null)
                    continue;

                dictionary[Query.PanelGroup(panel.PanelType)].Add(panel);
            }

            List<Panel> result = new List<Panel>();

            List<Panel> panels_Temp = dictionary[Analytical.PanelGroup.Floor];
            panels_Temp.AddRange(dictionary[Analytical.PanelGroup.Roof]);

            panels_Temp = MergeCoplanarPanels_FloorAndRoof(panels_Temp, offset, tolerance);
            if (panels_Temp != null && panels_Temp.Count > 0)
                result.AddRange(panels_Temp);

            return result;
        }
    
        private static List<Panel> MergeCoplanarPanels_FloorAndRoof(this IEnumerable<Panel> panels, double offset, double tolerance = Core.Tolerance.Distance)
        {
            List<Tuple<double, Panel>> tuples = new List<Tuple<double, Panel>>();
            foreach (Panel panel in panels)
            {
                double elevation = panel.MaxElevation();
                tuples.Add(new Tuple<double, Panel>(elevation, panel));
            }

            List<Panel> result = new List<Panel>();
            HashSet<Guid> guids = new HashSet<Guid>();

            tuples.Sort((x, y) => x.Item1.CompareTo(y.Item1));
            while (tuples.Count > 0)
            {
                Tuple<double, Panel> tuple = tuples[0];
                tuples.RemoveAt(0);

                double elevation = tuple.Item1;
                double elevation_Offset = elevation + offset;
                List<Tuple<double, Panel>> tuples_Offset = new List<Tuple<double, Panel>>();
                foreach (Tuple<double, Panel> tuple_Temp in tuples)
                {
                    if (tuple_Temp.Item1 > elevation_Offset)
                        break;

                    if (!tuple_Temp.Item2.Construction.Name.Equals(tuple.Item2.Construction.Name))
                        continue;

                    tuples_Offset.Add(tuple_Temp);
                }

                foreach (Tuple<double, Panel> tuple_Temp in tuples_Offset)
                    tuples.Remove(tuple_Temp);

                tuples_Offset.Insert(0, tuple);

                Geometry.Spatial.Plane plane = tuple.Item2.GetFace3D().GetPlane();

                List<Tuple<Polygon, Panel>> tuples_Polygon = new List<Tuple<Polygon, Panel>>();
                foreach (Tuple<double, Panel> tuple_Temp in tuples_Offset)
                {
                    Panel panel = tuple_Temp.Item2;

                    Geometry.Spatial.Face3D face3D = plane.Project(panel.GetFace3D());

                    Face2D face2D = plane.Convert(face3D);

                    tuples_Polygon.Add(new Tuple<Polygon, Panel>(face2D.ToNTS(tolerance), panel));
                }

                tuples_Polygon.Sort((x, y) => y.Item1.Area.CompareTo(x.Item1.Area));

                List<Polygon> polygons_Temp = tuples_Polygon.ConvertAll(x => x.Item1);
                Modify.RemoveAlmostSimilar_NTS(polygons_Temp, tolerance);

                polygons_Temp = Geometry.Planar.Query.Union(polygons_Temp);
                foreach (Polygon polygon in polygons_Temp)
                {
                    Tuple<Polygon, Panel> tuple_Panel = tuples_Polygon.Find(x => polygon.Contains(x.Item1.InteriorPoint));
                    if (tuple_Panel == null)
                        continue;

                    Geometry.Spatial.Face3D face3D = new Geometry.Spatial.Face3D(plane, polygon.ToSAM());
                    Guid guid = tuple_Panel.Item2.Guid;
                    if (guids.Contains(guid))
                        guid = Guid.NewGuid();

                    Panel panel_New = new Panel(guid, tuple_Panel.Item2, face3D);

                    result.Add(panel_New);
                }
            }

            return result;
        }
    }
}