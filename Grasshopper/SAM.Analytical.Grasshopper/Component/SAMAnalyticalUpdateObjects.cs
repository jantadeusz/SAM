﻿using Grasshopper.Kernel;
using SAM.Analytical.Grasshopper.Properties;
using SAM.Core;
using SAM.Core.Grasshopper;
using System;
using System.Collections.Generic;

namespace SAM.Analytical.Grasshopper
{
    public class SAMAnalyticalUpdateObjects : GH_SAMComponent
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("bd045519-57f7-4aaf-882a-5404ee4b714d");

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.SAM_Small;

        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public SAMAnalyticalUpdateObjects()
          : base("AdjacencyCluster.UpdateObjects", "AdjacencyCluster.UpdateObjects",
              "Update Objects in AdjacencyCluster such as Panel or Space",
              "SAM", "Analytical")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager inputParamManager)
        {
            inputParamManager.AddParameter(new GooAdjacencyClusterParam(), "_adjacencyCluster", "_adjacencyCluster", "SAM Analytical AdjacencyCluster", GH_ParamAccess.item);
            inputParamManager.AddParameter(new GooSAMObjectParam<SAMObject>(), "_objects", "_objects", "SAM Objects", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddParameter(new GooAdjacencyClusterParam(), "AdjacencyCluster", "AdjacencyCluster", "SAM Analytical AdjacencyCluster", GH_ParamAccess.item);
            outputParamManager.AddBooleanParameter("Successful", "Successful", "Successgully Added?", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="dataAccess">
        /// The DA object is used to retrieve from inputs and store in outputs.
        /// </param>
        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            AdjacencyCluster adjacencyCluster = null;

            if (!dataAccess.GetData(0, ref adjacencyCluster))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            List<SAMObject> sAMObjects = new List<SAMObject>();
            if (!dataAccess.GetDataList(1, sAMObjects))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            AdjacencyCluster adjacencyCluster_Result = adjacencyCluster.Clone() as AdjacencyCluster;

            List<bool> successfuls = new List<bool>();
            foreach(SAMObject sAMObject in sAMObjects)
            {
                if(!adjacencyCluster.Contains(sAMObject))
                {
                    successfuls.Add(false);
                    continue;
                }

                successfuls.Add(adjacencyCluster_Result.AddObject(sAMObject));
            }

            dataAccess.SetData(0, adjacencyCluster_Result);
            dataAccess.SetDataList(1, successfuls);
        }
    }
}