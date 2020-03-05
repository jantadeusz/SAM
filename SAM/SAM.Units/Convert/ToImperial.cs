﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAM.Units
{
    public static partial class Convert
    {
        public static double ToImperial(double value, UnitType from)
        {
            switch(from)
            {
                case UnitType.Meter:
                    return ByUnitType(value, from, UnitType.Feet);
            }

            return double.NaN;
        }
    }
}