﻿using SAM.Core;

namespace SAM.Analytical
{
    public static partial class Query
    {
        public static string ParameterName_Height(this Setting setting)
        {
            if (setting == null)
                return null;

            string result;
            if (setting.TryGetValue(ActiveSetting.Name.ParameterName_Height, out result))
                return result;

            return null;
        }

        public static string ParameterName_Height()
        {
            return ParameterName_Height(ActiveSetting.Setting);
        }

        public static string ParameterName_Width(this Setting setting)
        {
            if (setting == null)
                return null;

            string result;
            if (setting.TryGetValue(ActiveSetting.Name.ParameterName_Width, out result))
                return result;

            return null;
        }

        public static string ParameterName_Width()
        {
            return ParameterName_Width(ActiveSetting.Setting);
        }

        public static string ParameterName_Thickness(this Setting setting)
        {
            if (setting == null)
                return null;

            string result;
            if (setting.TryGetValue(ActiveSetting.Name.ParameterName_Thickness, out result))
                return result;

            return null;
        }

        public static string ParameterName_Thickness()
        {
            return ParameterName_Thickness(ActiveSetting.Setting);
        }

        public static string ParameterName_PanelType(this Setting setting)
        {
            if (setting == null)
                return null;

            string result;
            if (setting.TryGetValue(ActiveSetting.Name.ParameterName_PanelType, out result))
                return result;

            return null;
        }

        public static string ParameterName_PanelType()
        {
            return ParameterName_PanelType(ActiveSetting.Setting);
        }

        public static string ParameterName_Color(this Setting setting)
        {
            if (setting == null)
                return null;

            string result;
            if (setting.TryGetValue(ActiveSetting.Name.ParameterName_Color, out result))
                return result;

            return null;
        }

        public static string ParameterName_Color()
        {
            return ParameterName_Color(ActiveSetting.Setting);
        }

        public static string ParameterName_Transparent(this Setting setting)
        {
            if (setting == null)
                return null;

            string result;
            if (setting.TryGetValue(ActiveSetting.Name.ParameterName_Transparent, out result))
                return result;

            return null;
        }

        public static string ParameterName_Transparent()
        {
            return ParameterName_Transparent(ActiveSetting.Setting);
        }

        public static string ParameterName_InternalShadows(this Setting setting)
        {
            if (setting == null)
                return null;

            string result;
            if (setting.TryGetValue(ActiveSetting.Name.ParameterName_InternalShadows, out result))
                return result;

            return null;
        }

        public static string ParameterName_InternalShadows()
        {
            return ParameterName_InternalShadows(ActiveSetting.Setting);
        }

        public static string ParameterName_Ground(this Setting setting)
        {
            if (setting == null)
                return null;

            string result;
            if (setting.TryGetValue(ActiveSetting.Name.ParameterName_Ground, out result))
                return result;

            return null;
        }

        public static string ParameterName_Ground()
        {
            return ParameterName_Ground(ActiveSetting.Setting);
        }

        public static string ParameterName_Air(this Setting setting)
        {
            if (setting == null)
                return null;

            string result;
            if (setting.TryGetValue(ActiveSetting.Name.ParameterName_Air, out result))
                return result;

            return null;
        }

        public static string ParameterName_Air()
        {
            return ParameterName_Air(ActiveSetting.Setting);
        }

        public static string ParameterName_FrameWidth(this Setting setting)
        {
            if (setting == null)
                return null;

            string result;
            if (setting.TryGetValue(ActiveSetting.Name.ParameterName_FrameWidth, out result))
                return result;

            return null;
        }

        public static string ParameterName_FrameWidth()
        {
            return ParameterName_FrameWidth(ActiveSetting.Setting);
        }

        public static string ParameterName_NorthAngle(this Setting setting)
        {
            if (setting == null)
                return null;

            string result;
            if (setting.TryGetValue(ActiveSetting.Name.ParameterName_NorthAngle, out result))
                return result;

            return null;
        }

        public static string ParameterName_NorthAngle()
        {
            return ParameterName_NorthAngle(ActiveSetting.Setting);
        }

        public static string ParameterName_SpaceName(this Setting setting)
        {
            if (setting == null)
                return null;

            string result;
            if (setting.TryGetValue(ActiveSetting.Name.ParameterName_SpaceName, out result))
                return result;

            return null;
        }

        public static string ParameterName_SpaceName()
        {
            return ParameterName_SpaceName(ActiveSetting.Setting);
        }
        public static string ParameterName_Area(this Setting setting)
        {
            if (setting == null)
                return null;

            string result;
            if (setting.TryGetValue(ActiveSetting.Name.ParameterName_Area, out result))
                return result;

            return null;
        }

        public static string ParameterName_Area()
        {
            return ParameterName_Area(ActiveSetting.Setting);
        }

        public static string ParameterName_Volume(this Setting setting)
        {
            if (setting == null)
                return null;

            string result;
            if (setting.TryGetValue(ActiveSetting.Name.ParameterName_Volume, out result))
                return result;

            return null;
        }

        public static string ParameterName_Volume()
        {
            return ParameterName_Volume(ActiveSetting.Setting);
        }

        public static string ParameterName_FacingExternal(this Setting setting)
        {
            if (setting == null)
                return null;

            string result;
            if (setting.TryGetValue(ActiveSetting.Name.ParameterName_FacingExternal, out result))
                return result;

            return null;
        }

        public static string ParameterName_FacingExternal()
        {
            return ParameterName_FacingExternal(ActiveSetting.Setting);
        }

        public static string ParameterName_FacingExternalGlazing(this Setting setting)
        {
            if (setting == null)
                return null;

            string result;
            if (setting.TryGetValue(ActiveSetting.Name.ParameterName_FacingExternalGlazing, out result))
                return result;

            return null;
        }

        public static string ParameterName_FacingExternalGlazing()
        {
            return ParameterName_FacingExternalGlazing(ActiveSetting.Setting);
        }
    }
}