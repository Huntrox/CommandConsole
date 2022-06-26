using UnityEngine;

namespace HuntroxGames.Utils
{
    public static class VectorExtensionMethods 
    {

        public static Vector3Int RoundToVector3Int(this Vector3 vector3) 
            => new Vector3Int
            {
                x = Mathf.RoundToInt(vector3.x),
                y = Mathf.RoundToInt(vector3.y),
                z = Mathf.RoundToInt(vector3.z)
            };

        public static Vector3 SwapYZ(this Vector3 vector3)
        {
            var oldY = vector3.y;
            var oldZ = vector3.z;
            vector3.z = oldY;
            vector3.y = oldZ;
            return vector3;
        }
        public static Vector2 XY(this Vector3 v) => new Vector2(v.x, v.y);
        public static Vector3 WithX(this Vector3 v, float x) => new Vector3(x, v.y, v.z);
        public static Vector3 WithY(this Vector3 v, float y) => new Vector3(v.x, y, v.z);
        public static Vector3 WithZ(this Vector3 v, float z) => new Vector3(v.x, v.y, z);
        public static Vector2 WithX(this Vector2 v, float x) => new Vector2(x, v.y);
        public static Vector2 WithY(this Vector2 v, float y) => new Vector2(v.x, y);
        public static Vector3 WithZ(this Vector2 v, float z) => new Vector3(v.x, v.y, z);

        // axisDirection - unit vector in direction of an axis (eg, defines a line that passes through zero)
        // point - the point to find nearest on line for
        public static Vector3 NearestPointOnAxis(this Vector3 axisDirection, Vector3 point, bool isNormalized = false)
        {
            if (!isNormalized) axisDirection.Normalize();
            var d = Vector3.Dot(point, axisDirection);
            return axisDirection * d;
        }

        // lineDirection - unit vector in direction of line
        // pointOnLine - a point on the line (allowing us to define an actual line in space)
        // point - the point to find nearest on line for
        public static Vector3 NearestPointOnLine(
            this Vector3 lineDirection, Vector3 point, Vector3 pointOnLine, bool isNormalized = false)
        {
            if (!isNormalized) lineDirection.Normalize();
            var d = Vector3.Dot(point - pointOnLine, lineDirection);
            return pointOnLine + (lineDirection * d);
        }
        public static Vector3 ZtoY(this Vector3 position, float OffsetY)
            => new Vector3(position.x, position.y, position.y + OffsetY);

        public static Vector3 Round(this Vector3 vector3, int decimalPlaces = 2)
        {
            float multiplier = 1;
            for (int i = 0; i < decimalPlaces; i++)
            {
                multiplier *= 10f;
            }

            return new Vector3(
                Mathf.Round(vector3.x * multiplier) / multiplier,
                Mathf.Round(vector3.y * multiplier) / multiplier,
                Mathf.Round(vector3.z * multiplier) / multiplier);
        }

    }
}