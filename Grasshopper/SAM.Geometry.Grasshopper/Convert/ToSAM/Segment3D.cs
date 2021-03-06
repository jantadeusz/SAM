﻿using Grasshopper.Kernel.Types;

namespace SAM.Geometry.Grasshopper
{
    public static partial class Convert
    {
        public static Spatial.Segment3D ToSAM(this Rhino.Geometry.Line line)
        {
            return new Spatial.Segment3D(line.From.ToSAM(), line.To.ToSAM());
        }

        public static Spatial.Segment3D ToSAM(this Rhino.Geometry.LineCurve lineCurve)
        {
            return new Spatial.Segment3D(lineCurve.PointAtStart.ToSAM(), lineCurve.PointAtEnd.ToSAM());
        }

        public static Spatial.Segment3D ToSAM(this GH_Line line)
        {
            return ToSAM(line.Value);
        }
    }
}