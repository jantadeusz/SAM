﻿using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json.Linq;

using SAM.Core;
using SAM.Geometry.Spatial;


namespace SAM.Analytical
{
    public class Boundary2D : SAMObject
    {
        private BoundaryEdge2DLoop externalEdge2DLoop;
        private List<BoundaryEdge2DLoop> internalEdge2DLoops;

        public Boundary2D(Boundary2D boundary2D)
        {
            this.externalEdge2DLoop = new BoundaryEdge2DLoop(boundary2D.externalEdge2DLoop);
            if (boundary2D.internalEdge2DLoops != null)
                this.internalEdge2DLoops = boundary2D.internalEdge2DLoops.ConvertAll(x => new BoundaryEdge2DLoop(x));
        }
        
        public Boundary2D(BoundaryEdge2DLoop edge2DLoop)
            : base()
        {
            this.externalEdge2DLoop = new BoundaryEdge2DLoop(edge2DLoop);
        }

        public Boundary2D(IClosedPlanar3D closedPlanar3D)
        {
            externalEdge2DLoop = new BoundaryEdge2DLoop(closedPlanar3D);
        }

        public Boundary2D(Geometry.Planar.IClosed2D closed2D)
        {
            externalEdge2DLoop = new BoundaryEdge2DLoop(closed2D);
        }

        public Boundary2D(JObject jObject)
            : base(jObject)
        {

        }

        public BoundaryEdge2DLoop ExternalEdge2DLoop
        {
            get
            {
                return new BoundaryEdge2DLoop(externalEdge2DLoop);
            }
        }

        public List<BoundaryEdge2DLoop> InternalEdge2DLoops
        {
            get
            {
                if (internalEdge2DLoops == null)
                    return null;

                return internalEdge2DLoops.ConvertAll(x => new BoundaryEdge2DLoop(x));
            }
        }

        public BoundaryEdge3DLoop GetEdge3DLoop(Plane plane)
        {
            return new BoundaryEdge3DLoop(plane, externalEdge2DLoop);
        }

        public List<BoundaryEdge3DLoop> GetInternalEdge3DLoops(Plane plane)
        {
            if (internalEdge2DLoops == null)
                return null;

            return internalEdge2DLoops.ConvertAll(x => new BoundaryEdge3DLoop(plane, x));
        }

        public List<IClosedPlanar3D> GetInternalClosedPlanar3Ds(Plane plane)
        {
            if (internalEdge2DLoops == null)
                return null;

            List<IClosedPlanar3D> result = new List<IClosedPlanar3D>();
            foreach (BoundaryEdge2DLoop edge2DLoop in internalEdge2DLoops)
            {
                Polygon3D polygon3D = new Polygon3D(edge2DLoop.BoundaryEdge2Ds.ConvertAll(x => ((ICurve3D)plane.Convert(x.Curve2D)).GetStart()));
                result.Add(polygon3D);
            }

            return result;
        }

        public Face3D GetFace3D(Plane plane)
        {
            return new Face3D(plane, GetFace2D());
        }

        public Geometry.Planar.Face2D GetFace2D()
        {
            List<Geometry.Planar.IClosed2D> internalClosed2Ds = null;
            if (internalEdge2DLoops != null && internalEdge2DLoops.Count > 0)
                internalClosed2Ds = InternalEdge2DLoops.ConvertAll(x => x.GetClosed2D());

            return Geometry.Planar.Face2D.Create(externalEdge2DLoop.GetClosed2D(), internalClosed2Ds);
        }

        public override bool FromJObject(JObject jObject)
        {
            if (!base.FromJObject(jObject))
                return false;

            externalEdge2DLoop = new BoundaryEdge2DLoop(jObject.Value<JObject>("Edge2DLoop"));
            if (jObject.ContainsKey("InternalEdge2DLoops"))
                internalEdge2DLoops = Core.Create.IJSAMObjects<BoundaryEdge2DLoop>(jObject.Value<JArray>("InternalEdge2DLoops"));
            return true;
        }

        public override JObject ToJObject()
        {
            JObject jObject = base.ToJObject();
            jObject.Add("Edge2DLoop", externalEdge2DLoop.ToJObject());
            if (internalEdge2DLoops != null)
                jObject.Add("InternalEdge2DLoops", Core.Create.JArray(internalEdge2DLoops));
            
            return jObject;
        }

        public PlanarBoundary3D GetPlanarBoundary3D(Plane plane)
        {
            if (plane == null)
                return null;

            return new PlanarBoundary3D(plane, this);
        }


        public static Boundary2D Create(List<BoundaryEdge2DLoop> edge2DLoops, out List<BoundaryEdge2DLoop> edge2DLoops_Outside)
        {
            edge2DLoops_Outside = null;

            if (edge2DLoops == null || edge2DLoops.Count() == 0)
                return null;

            BoundaryEdge2DLoop edge2DLoop_Max = null;
            double area_Max = double.MinValue;

            Dictionary<BoundaryEdge2DLoop, Geometry.Planar.IClosed2D> dictionary = new Dictionary<BoundaryEdge2DLoop, Geometry.Planar.IClosed2D>();
            foreach (BoundaryEdge2DLoop edge2DLoop in edge2DLoops)
            {
                Geometry.Planar.IClosed2D closed2D = edge2DLoop.GetClosed2D();
                double area = edge2DLoop.GetArea();
                if (area > area_Max)
                {
                    area_Max = area;
                    edge2DLoop_Max = edge2DLoop;
                }
                dictionary[edge2DLoop] = closed2D;
            }

            if (edge2DLoop_Max == null)
                return null;

            Boundary2D boundary2D = new Boundary2D(edge2DLoop_Max);

            Geometry.Planar.IClosed2D closed2D_Max = dictionary[edge2DLoop_Max];
            dictionary.Remove(edge2DLoop_Max);

            foreach (KeyValuePair<BoundaryEdge2DLoop, Geometry.Planar.IClosed2D> keyValuePair in dictionary)
            {
                if (!closed2D_Max.Inside(keyValuePair.Value))
                {
                    if (edge2DLoops_Outside == null)
                        edge2DLoops_Outside = new List<BoundaryEdge2DLoop>();

                    edge2DLoops_Outside.Add(keyValuePair.Key);

                    continue;
                }

                if (boundary2D.internalEdge2DLoops == null)
                    boundary2D.internalEdge2DLoops = new List<BoundaryEdge2DLoop>();

                boundary2D.internalEdge2DLoops.Add(new BoundaryEdge2DLoop(keyValuePair.Key));
            }

            return boundary2D;
        }

        public static List<Boundary2D> Create(List<BoundaryEdge2DLoop> edge2DLoops)
        {
            if (edge2DLoops == null)
                return null;

            List<Boundary2D> result = new List<Boundary2D>();
            if (edge2DLoops.Count == 0)
                return result;

            List<BoundaryEdge2DLoop> edge2DLoops_All = new List<BoundaryEdge2DLoop>(edge2DLoops);
            while(edge2DLoops_All.Count > 0)
            {
                List<BoundaryEdge2DLoop> edge2DLoops_Outside = null;
                Boundary2D boundary2D = Create(edge2DLoops_All, out edge2DLoops_Outside);
                if (boundary2D != null)
                    result.Add(boundary2D);

                edge2DLoops_All = edge2DLoops_Outside;
            }

            return result;
        }
    }
}