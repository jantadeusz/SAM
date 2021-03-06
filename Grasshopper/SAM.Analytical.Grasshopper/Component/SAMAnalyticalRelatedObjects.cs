﻿using Grasshopper.Kernel;
using SAM.Analytical.Grasshopper.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using SAM.Core;
using SAM.Core.Grasshopper;

namespace SAM.Analytical.Grasshopper
{
    public class SAMCoreRelatedObjects : GH_SAMComponent
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("4d3acd0a-63a9-47eb-ae7d-a4961c9d620b");

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.SAM_Small;

        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public SAMCoreRelatedObjects()
          : base("AdjacencyCluster.RelatedObjects", "AdjacencyCluster.RelatedObjects",
              "Related Objects in AdjacencyCluster",
              "SAM", "Analytical")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager inputParamManager)
        {
            inputParamManager.AddParameter(new GooAdjacencyClusterParam(), "_adjacencyCluster", "_adjacencyCluster", "SAM AdjacencyCluster", GH_ParamAccess.item);
            inputParamManager.AddParameter(new GooSAMObjectParam<SAMObject>(), "_object", "_object", "SAM Object", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddGenericParameter("RelatedObjects", "RelatedObjects", "Related Objects", GH_ParamAccess.list);
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

            SAMObject sAMObject = null;
            if (!dataAccess.GetData(1, ref sAMObject))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            IEnumerable<object> result = adjacencyCluster.GetRelatedObjects(sAMObject);

            if (result == null)
            {
                dataAccess.SetDataList(0, null);
                return;
            }

            if (result.Count() == 0)
            {
                dataAccess.SetDataList(0, result);
                return;
            }

            dataAccess.SetDataList(0, result);
        }
    }
}