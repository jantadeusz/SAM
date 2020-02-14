﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GH_IO.Types;
using Grasshopper.Kernel.Types;


namespace SAM.Geometry.Grasshopper
{
    public static partial class Convert
    {
        public static Spatial.Point3D ToSAM(this Rhino.Geometry.Point3d point3d)
        {
            return new Spatial.Point3D(point3d.X, point3d.Y, point3d.Z);
        }

        public static Spatial.Point3D ToSAM(this GH_Point point)
        {
            return ToSAM(point.Value);
        }

        public static Spatial.Point3D ToSAM(this GH_Point3D point3D)
        {
            return new Spatial.Point3D(point3D.x, point3D.y, point3D.z);
        }
    }
}