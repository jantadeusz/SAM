﻿using SAM.Geometry.Interfaces;

namespace SAM.Geometry.Spatial
{
    public interface IClosedPlanar3D : IClosed3D, IPlanar3D, IBoundable3D, IReversible
    {
        double GetArea();
    }
}