//AWAN SOFTWORKS LTD 2023

using UnityEngine;

namespace Armony.Utilities.Libraries
{
    public static class LibConversions
    {
        public static Vector3 ToVector3(this float f)
        {
            return new Vector3(f, f, f);
        }

        public static Vector2[] ToVector2Array(this Vector3[] v3)
        {
            return System.Array.ConvertAll<Vector3, Vector2>(v3, GetV3FromV2);
        }

        public static Vector2 GetV3FromV2(Vector3 v3)
        {
            return new Vector2(v3.x, v3.y);
        }

        public static Color ToColor(this float f, float a = 1)
        {
            return new Color(f, f, f, a);
        }

        public static int ToInt(this bool b)
        {
            return b ? 1 : 0;
        }

        public static int ToIntNegative(this bool b)
        {
            return b ? 1 : -1;
        }

        public static Color HexToColor(string hexCode, float alpha = 1)
        {
            if (ColorUtility.TryParseHtmlString("#" + hexCode, out Color color))
            {
                color.a = alpha;
                return color;
            }
            Debug.LogWarning("Invalid hexadecimal color value: " + hexCode);
            return Color.magenta;
        }

        public static string TimeToString(System.TimeSpan t)
        {
            string ret = "";

            int timeUnitsToDisplay = 0;
            if (t.TotalDays > 0)
            {
                timeUnitsToDisplay = 3;
            }
            else if (t.TotalHours > 0)
            {
                timeUnitsToDisplay = 2;
            }
            else if (t.TotalMinutes > 0)
            {
                timeUnitsToDisplay = 1;
            }

            switch (timeUnitsToDisplay)
            {
                case 3:
                    ret += t.Days + " days, ";
                    goto case 2;
                case 2:
                    ret += t.Hours + " hours, ";
                    goto case 1;
                case 1:
                    ret += t.Minutes + " minutes and ";
                    goto case 0;
                case 0:
                    ret += t.Seconds + " seconds";
                    break;
                default:
                    break;
            }

            return ret;
        }
    }
}
