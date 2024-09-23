using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public static class Convert
    {
        private static string[] kmgUnit =
        {
            "K", "M", "G", "T", "P", "E"
        };

        public static string NumberToCommaString(in long value)
        {
            return $"{value:N0}";
        }

        public static string NumberToUnitString(in long value)
        {
            string[] split = $"{value:N0}".Split(',');

            if (split.Length > 0)
            {
                if(split.Length == 1) return split[0];
                else
                {
                    int decimalIndex = split.Length - 2;
                    float mix = float.Parse($"{split[0]}.{split[1]}");

                    //== 반올림 제거 : [ System.Globalization.CultureInfo.InvariantCulture ]
                    return mix.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture) + kmgUnit[decimalIndex];
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public static Color ArgbToColor(uint argb)
        {
            float a = ((argb >> 24) & 0xFF) / 255f;
            float r = ((argb >> 16) & 0xFF) / 255f;
            float g = ((argb >> 8) & 0xFF) / 255f;
            float b = (argb & 0xFF) / 255f;

            return new Color(r, g, b, a);
        }
    }

}
