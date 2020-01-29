﻿using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAM.Geometry.Grasshopper
{
    public static partial class Convert
    {
        public static Spatial.IClosed3D ToSAM(this BrepLoop brepLoop, bool simplify = true)
        {

            if (brepLoop.Face.IsPlanar(Tolerance.MicroDistance))
            {
                return new Spatial.Face(brepLoop.To3dCurve().ToSAM(simplify) as Spatial.IClosedPlanar3D);
            }
            else
            {
                Spatial.IGeometry3D geometry3D = brepLoop.To3dCurve().ToSAM(simplify);
                if (geometry3D is Spatial.Polyline3D)
                {
                    Spatial.PolycurveLoop3D polycurveLoop3D = new Spatial.PolycurveLoop3D(((Spatial.Polyline3D)geometry3D).GetSegments());
                    return new Spatial.Surface(polycurveLoop3D);
                }
            }



            return null;
        }
    }
}