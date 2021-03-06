﻿using Grasshopper.Kernel;
using SAM.Analytical.Grasshopper.Properties;
using SAM.Core;
using SAM.Core.Grasshopper;
using SAM.Geometry.Grasshopper;
using SAM.Geometry.Planar;
using SAM.Geometry.Spatial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace SAM.Analytical.Grasshopper
{
    public class SAMAnalyticalNormals : GH_SAMComponent
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("7d48087c-a4d6-4f0a-b887-eb9b0a13fc73");

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.SAM_Small;

        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public SAMAnalyticalNormals()
          : base("SAMAnalytical.Normals", "SAMAnalytical.Normals",
              "Gets Internal Point and Normal Vector for SAM Analytical Object please connect to GH 'Vector Display' component",
              "SAM", "Analytical")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager inputParamManager)
        {
            inputParamManager.AddParameter(new GooSAMObjectParam<SAMObject>(), "_analytical", "_analytical", "SAM Analytical Object", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddPointParameter("InternalPoint", "InternalPoint", "Internal Point", GH_ParamAccess.list);
            outputParamManager.AddVectorParameter("Normal", "Normal", "Normal Vector", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            dataAccess.SetDataList(0, null);
            dataAccess.SetDataList(1, null);

            SAMObject sAMObject = null;
            if (!dataAccess.GetData(0, ref sAMObject))
                return;

            List<PlanarBoundary3D> planarBoundary3Ds = null;

            if (sAMObject is AnalyticalModel)
            {
                planarBoundary3Ds = ((AnalyticalModel)sAMObject).AdjacencyCluster?.GetPanels()?.ConvertAll(x => x.PlanarBoundary3D);
            } 
            else if(sAMObject is AdjacencyCluster)
            {
                planarBoundary3Ds = ((AdjacencyCluster)sAMObject).GetPanels()?.ConvertAll(x => x.PlanarBoundary3D);
            }
            else if(sAMObject is Panel)
            {
                planarBoundary3Ds = new List<PlanarBoundary3D>() { ((Panel)sAMObject).PlanarBoundary3D };
            }
            else if(sAMObject is Aperture)
            {
                planarBoundary3Ds = new List<PlanarBoundary3D>() { ((Aperture)sAMObject).PlanarBoundary3D };
            }

            if (planarBoundary3Ds == null || planarBoundary3Ds.Count == 0)
                return;

            List<Rhino.Geometry.Point3d> internalPoints = new List<Rhino.Geometry.Point3d>();
            List<Rhino.Geometry.Vector3d> normals = new List<Rhino.Geometry.Vector3d>();

            foreach(PlanarBoundary3D planarBoundary3D in planarBoundary3Ds)
            {
                Rhino.Geometry.Point3d internalPoint = Rhino.Geometry.Point3d.Unset;
                Rhino.Geometry.Vector3d normal = Rhino.Geometry.Vector3d.Unset;

                Face3D face3D = planarBoundary3D.GetFace3D();
                if(face3D != null)
                {
                    Point3D point3D = face3D.InternalPoint3D();
                    if (point3D != null)
                        internalPoint = point3D.ToRhino();

                    Vector3D vector3D = face3D.GetPlane().Normal;
                    if (vector3D != null)
                        normal = vector3D.ToRhino();
                }

                internalPoints.Add(internalPoint);
                normals.Add(normal);
            }


            dataAccess.SetDataList(0, internalPoints);
            dataAccess.SetDataList(1, normals);
        }
    }
}