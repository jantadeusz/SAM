﻿using System;
using System.Collections.Generic;
using System.Linq;

using SAM.Core;
using SAM.Geometry.Spatial;


namespace SAM.Analytical
{
    public class PlanarBoundary3D : SAMObject
    {
        private Plane plane;
        private Edge2DLoop edge2DLoop;
        private List<Edge2DLoop> internalEdge2DLoops;

        public PlanarBoundary3D(Face face)
            : base()
        {
            plane = face.GetPlane();
            edge2DLoop = new Edge2DLoop(face.Boundary);
        }

        public PlanarBoundary3D(IClosedPlanar3D closedPlanar3D)
        {
            plane = closedPlanar3D.GetPlane();
            edge2DLoop = new Edge2DLoop(closedPlanar3D);
        }

        public PlanarBoundary3D(PlanarBoundary3D planarBoundary3D)
            : base(planarBoundary3D)
        {
            plane = (Geometry.Spatial.Plane)planarBoundary3D.plane.Clone();
            edge2DLoop = new Edge2DLoop(planarBoundary3D.edge2DLoop);

            if (planarBoundary3D.internalEdge2DLoops != null)
                internalEdge2DLoops = planarBoundary3D.internalEdge2DLoops.ConvertAll(x => new Edge2DLoop(x));
        }

        public PlanarBoundary3D(Guid guid, string name, IEnumerable<ParameterSet> parameterSets, Plane plane, Edge2DLoop edge2DLoop, IEnumerable<Edge2DLoop> internalEdge2DLoop)
            : base(guid, name, parameterSets)
        {
            this.plane = new Plane(plane);
            this.edge2DLoop = new Edge2DLoop(edge2DLoop);
            if (internalEdge2DLoop != null)
                this.internalEdge2DLoops = internalEdge2DLoop.ToList().ConvertAll(x => new Edge2DLoop(x));
        }

        public Plane Plane
        {
            get
            {
                return new Geometry.Spatial.Plane(plane);
            }
        }

        public Edge2DLoop Edge2DLoop
        {
            get
            {
                return new Edge2DLoop(edge2DLoop);
            }
        }

        public List<Edge2DLoop> InternalEdge2DLoops
        {
            get
            {
                if (internalEdge2DLoops == null)
                    return null;

                return internalEdge2DLoops.ConvertAll(x => new Edge2DLoop(x));
            }
        }

        public Edge3DLoop GetEdge3DLoop()
        {
            return new Edge3DLoop(plane, edge2DLoop);
        }

        public List<Edge3DLoop> GetInternalEdge3DLoops()
        {
            if (internalEdge2DLoops == null)
                return null;

            return internalEdge2DLoops.ConvertAll(x => new Edge3DLoop(plane, x));
        }

        public List<IClosedPlanar3D> GetInternalClosedPlanar3Ds()
        {
            if (internalEdge2DLoops == null)
                return null;

            List<IClosedPlanar3D> result = new List<IClosedPlanar3D>();
            foreach (Edge2DLoop edge2DLoop in internalEdge2DLoops)
            {
                Polygon3D polygon3D = new Polygon3D(edge2DLoop.Edge2Ds.ConvertAll(x => ((ICurve3D)plane.Convert(x.Curve2D)).GetStart()));
                result.Add(polygon3D);
            }

            return result;
        }

        public bool Coplanar(PlanarBoundary3D planarBoundary3D, double tolerance = Geometry.Tolerance.Angle)
        {
            return plane.Coplanar(planarBoundary3D.plane, tolerance);
        }

        public Face GetFace()
        {
            return new Face(plane, edge2DLoop.GetClosed2D());
        }

        public void Snap(IEnumerable<Point3D> point3Ds, double maxDistance = double.NaN)
        {
            Edge3DLoop edge3DLoop = GetEdge3DLoop();
            edge3DLoop.Snap(point3Ds, maxDistance);
        }

        public BoundingBox3D GetBoundingBox(double offset = 0)
        {
            return GetFace().GetBoundingBox(offset);
        }



        public static bool TryGetPlanarBoundary3D(List<Face> faces, out PlanarBoundary3D planarBoundary3D)
        {
            planarBoundary3D = null;

            if (faces == null || faces.Count() == 0)
                return false;

            Face face_Max = null;
            double area_Max = double.MinValue;
            foreach (Face face in faces)
            {
                double area = face.GetArea();
                if (area > area_Max)
                {
                    area_Max = area;
                    face_Max = face;
                }

            }

            if (face_Max == null)
                return false;

            planarBoundary3D = new PlanarBoundary3D(face_Max);
            foreach (Face face in faces)
            {
                if (face == face_Max)
                    continue;

                if (face_Max.Inside(face))
                {
                    if (planarBoundary3D.internalEdge2DLoops == null)
                        planarBoundary3D.internalEdge2DLoops = new List<Edge2DLoop>();

                    planarBoundary3D.internalEdge2DLoops.Add(new Edge2DLoop(face));
                }
            }

            return true;
        }

        public static bool TryGetPlanarBoundary3Ds(List<Face> faces, out List<PlanarBoundary3D> planarBoundary3Ds)
        {
            planarBoundary3Ds = null;

            if (faces == null || faces.Count() == 0)
                return false;

            planarBoundary3Ds = new List<PlanarBoundary3D>();


            if (faces.Count() == 1)
            {
                planarBoundary3Ds.Add(new PlanarBoundary3D(faces.First()));
                return true;
            }
            
            List<Face> faceList = faces.ToList();
            faceList.Sort((x, y) => y.GetArea().CompareTo(x.GetArea()));

            while(faceList.Count > 0)
            {
                List<Face> faceList_ToRemove = new List<Face>();

                Face face = faceList.First();
                PlanarBoundary3D planarBoundary3D = new PlanarBoundary3D(face);
                planarBoundary3Ds.Add(planarBoundary3D);
                faceList_ToRemove.Add(face);

                Geometry.Orientation orientation = Geometry.Orientation.Undefined;
                if(face.Boundary is Geometry.Planar.Polygon2D)
                    orientation = ((Geometry.Planar.Polygon2D)face.Boundary).GetOrientation();

                foreach (Face face_Internal in faces)
                {
                    if (face_Internal == face)
                        continue;

                    if (!face.Inside(face_Internal))
                        continue;

                    if (planarBoundary3D.internalEdge2DLoops == null)
                        planarBoundary3D.internalEdge2DLoops = new List<Edge2DLoop>();

                    Geometry.Planar.IClosed2D closed2D = planarBoundary3D.plane.Convert(face_Internal.ToClosedPlanar3D());
                    if(orientation != Geometry.Orientation.Undefined)
                    {
                        if (closed2D is Geometry.Planar.Polygon2D)
                        {
                            Geometry.Planar.Polygon2D polygon2D = (Geometry.Planar.Polygon2D)closed2D;
                            Geometry.Orientation orientation_Internal = polygon2D.GetOrientation();
                            if (orientation == orientation_Internal)
                                polygon2D.Reverse();
                        }
                    }

                    planarBoundary3D.internalEdge2DLoops.Add(new Edge2DLoop(new Face(planarBoundary3D.plane, closed2D)));
                    faceList_ToRemove.Add(face_Internal);
                }

                faceList_ToRemove.ForEach(x => faceList.Remove(x));
            }

            return true;
        }
    }
}