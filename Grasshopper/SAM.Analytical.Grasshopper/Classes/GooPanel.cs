﻿using System;
using System.Collections.Generic;

using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using SAM.Core.Grasshopper;
using SAM.Analytical.Grasshopper.Properties;
using Rhino.Commands;
using Rhino.Input;
using System.Linq;

namespace SAM.Analytical.Grasshopper
{
    public class GooPanel : GooSAMObject<Panel>, IGH_PreviewData, IGH_BakeAwareData
    {
        public GooPanel()
            : base()
        {

        }

        public GooPanel(Panel panel)
            : base(panel)
        {

        }

        public BoundingBox ClippingBox
        {
            get
            {
                if (Value == null)
                    return BoundingBox.Empty;

                return Geometry.Grasshopper.Convert.ToRhino(Value.GetBoundingBox());
            }
        }

        public override IGH_Goo Duplicate()
        {
            return new GooPanel(Value);
        }

        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            GooPlanarBoundary3D gooPlanarBoundary3D = new GooPlanarBoundary3D(Value.PlanarBoundary3D);
            gooPlanarBoundary3D.DrawViewportWires(args);

            List<Aperture> apertures = Value.Apertures;
            if (apertures != null)
            {
                foreach (Aperture aperture in apertures)
                    foreach (Geometry.Spatial.IClosedPlanar3D closedPlanar3D in aperture.GetFace3D().GetEdges())
                    {
                        Geometry.Grasshopper.GooSAMGeometry gooSAMGeometry = new Geometry.Grasshopper.GooSAMGeometry(closedPlanar3D);
                        gooSAMGeometry.DrawViewportWires(args);
                    }
            }
        }

        public void DrawViewportMeshes(GH_PreviewMeshArgs args)
        {
            Geometry.Grasshopper.GooSAMGeometry gooSAMGeometry = new Geometry.Grasshopper.GooSAMGeometry(Value.GetFace3D());
            gooSAMGeometry.DrawViewportMeshes(args);

            List<Aperture> apertures = Value.Apertures;
            if (apertures != null)
            {
                foreach (Aperture aperture in apertures)
                    foreach (Geometry.Spatial.IClosedPlanar3D closedPlanar3D in aperture.GetFace3D().GetEdges())
                    {
                        Geometry.Grasshopper.GooSAMGeometry gooSAMGeometry_Aperture = new Geometry.Grasshopper.GooSAMGeometry(closedPlanar3D);
                        gooSAMGeometry_Aperture.DrawViewportMeshes(args);
                    }
            }
        }

        public bool BakeGeometry(RhinoDoc doc, ObjectAttributes att, out Guid obj_guid)
        {
            return Geometry.Grasshopper.Modify.BakeGeometry(Value.GetFace3D(), doc, att, out obj_guid);
        }
    }

    public class GooPanelParam : GH_PersistentParam<GooPanel>, IGH_PreviewObject, IGH_BakeAwareObject
    {
        public override Guid ComponentGuid => new Guid("278B438C-43EA-4423-999F-B6A906870939");

        protected override System.Drawing.Bitmap Icon => Resources.SAM_Small;

        bool IGH_PreviewObject.Hidden { get; set; }

        bool IGH_PreviewObject.IsPreviewCapable => !VolatileData.IsEmpty;

        BoundingBox IGH_PreviewObject.ClippingBox => Preview_ComputeClippingBox();

        public bool IsBakeCapable => true;

        void IGH_PreviewObject.DrawViewportMeshes(IGH_PreviewArgs args) => Preview_DrawMeshes(args);

        void IGH_PreviewObject.DrawViewportWires(IGH_PreviewArgs args) => Preview_DrawWires(args);

        public GooPanelParam()
            : base(typeof(Panel).Name, typeof(Panel).Name, typeof(Panel).FullName.Replace(".", " "), "Params", "SAM")
        { 
        }
        
        protected override GH_GetterResult Prompt_Plural(ref List<GooPanel> values)
        {
            throw new NotImplementedException();
        }

        protected override GH_GetterResult Prompt_Singular(ref GooPanel value)
        {
            //throw new NotImplementedException();

            Rhino.Input.Custom.GetObject getObject = new Rhino.Input.Custom.GetObject();
            getObject.SetCommandPrompt("Pick Surface to create panel");
            getObject.GeometryFilter = ObjectType.Surface;
            getObject.SubObjectSelect = true;
            getObject.DeselectAllBeforePostSelect = false;
            getObject.OneByOnePostSelect = true;

            if (getObject.CommandResult() != Result.Success)
                return GH_GetterResult.cancel;

            ObjRef objRef = getObject.Object(0);

            RhinoObject rhinoObject = objRef.Object();
            if (rhinoObject == null)
                return GH_GetterResult.cancel;

            Surface surface = objRef.Surface();
            if (surface == null)
                return GH_GetterResult.cancel;

            List<Panel> panels = Create.Panels(Geometry.Grasshopper.Convert.ToSAM(surface), PanelType.WallExternal, Query.Construction(PanelType.WallExternal), Core.Tolerance.MacroDistance);
            if (panels == null || panels.Count == 0)
                return GH_GetterResult.cancel;

            value = new GooPanel(panels.First());
            return GH_GetterResult.success;
        }

        public void BakeGeometry(RhinoDoc doc, List<Guid> obj_ids)
        {
             BakeGeometry(doc, doc.CreateDefaultAttributes(), obj_ids);
        }

        public void BakeGeometry(RhinoDoc doc, ObjectAttributes att, List<Guid> obj_ids)
        {
            foreach (var value in VolatileData.AllData(true))
            {
                Guid uuid = default;
                (value as IGH_BakeAwareData)?.BakeGeometry(doc, att, out uuid);
                obj_ids.Add(uuid);
            }
        }

    }
}
