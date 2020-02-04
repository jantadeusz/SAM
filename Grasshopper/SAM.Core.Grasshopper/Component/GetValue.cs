﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using SAM.Core.Grasshopper.Properties;

namespace SAM.Core.Grasshopper
{
    public class GetValue : GH_Component
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("f111f564-06a8-48bb-8db2-6cf50d07a3f7");

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.SAM_Explode;

        private GH_OutputParamManager outputParamManager;
        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public GetValue()
          : base("GetValue", "GetValue",
              "Get Value of object property",
              "SAM", "Core")
        {

        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager inputParamManager)
        {
            inputParamManager.AddGenericParameter("_object", "_object", "Object", GH_ParamAccess.item);
            inputParamManager.AddTextParameter("_name", "name", "Name", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            this.outputParamManager = outputParamManager;
            outputParamManager.AddGenericParameter("Value", "Value", "Property Value", GH_ParamAccess.item);
            //outputParamManager.AddGenericParameter("Points", "Pts", "Snap points", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="dataAccess">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            string name = null;
            if (!dataAccess.GetData(1, ref name) || string.IsNullOrWhiteSpace(name))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            GH_ObjectWrapper objectWrapper = null;
            if (!dataAccess.GetData(0, ref objectWrapper) || objectWrapper == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            object @object = objectWrapper.Value;

            if(@object is IGH_Goo)
            {
                try
                {
                    @object = (@object as dynamic).Value;
                }
                catch(Exception exception)
                {
                    @object = null;
                }
            }


            if(@object == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            Type type = @object.GetType();

            System.Reflection.PropertyInfo propertyInfo = null;

            System.Reflection.PropertyInfo[] propertyInfos = type.GetProperties();
            foreach(System.Reflection.PropertyInfo propertyInfo_Temp in propertyInfos)
            {
                if(propertyInfo_Temp.Name.Equals(name))
                {
                    propertyInfo = propertyInfo_Temp;
                    break;
                }
            }

            if(propertyInfo == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Missing property with provided name");
                return;
            }

            object value = propertyInfo.GetValue(@object);

            if(value is SAMObject)
            {
                value = new GooSAMObject<SAMObject>((SAMObject)value);
            }

            dataAccess.SetData(0, value);

        }

    }
}