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

        public static int ToInt(this bool b, int mult = 1)
        {
            return (b ? 1 : 0) * mult;
        }

        public static int ToIntNegative(this bool b, int mult = 1)
        {
            return (b ? 1 : -1) * mult;
        }

        public static Color HexToColor(this string hex, float alpha = 1)
        {
            hex = hex.Trim(new char[] { ' ', '#' });
            if (hex.Length != 6)
            {
                return new Color(0, 0, 0, 1);
            }
            else
            {
                return new Color(
                    HexToFloat(hex.Substring(0, 2)),
                    HexToFloat(hex.Substring(2, 2)),
                    HexToFloat(hex.Substring(4, 2)),
                    Mathf.Clamp01(alpha)
                );
            }
        }

        public static float HexToFloat(string hex)
        {
            return ((float)int.Parse(hex, System.Globalization.NumberStyles.HexNumber)) / 256f;
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
