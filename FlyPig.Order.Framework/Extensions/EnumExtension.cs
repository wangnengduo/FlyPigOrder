using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class EnumExtension
    {

        public static string GetDescription(this Enum source)
        {
            try
            {
                var type = source.GetType();
                FieldInfo field = type.GetField(Enum.GetName(type, source));
                var descAttr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (descAttr == null)
                {
                    return string.Empty;
                }
                return descAttr.Description;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static string GetDisplayName(this Enum source)
        {
            try
            {
                var type = source.GetType();
                FieldInfo field = type.GetField(Enum.GetName(type, source));
                var descAttr = Attribute.GetCustomAttribute(field, typeof(DisplayNameAttribute)) as DisplayNameAttribute;
                if (descAttr == null)
                {
                    return string.Empty;
                }
                return descAttr.DisplayName;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
