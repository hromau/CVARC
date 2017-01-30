using AIRLab.Mathematics;
using UnityEngine;

namespace UnityCommons
{
    public static class Metrics
    {        
        public static readonly float Centimeter = 0.1f;
        public static readonly float Decimeter = 10 * Centimeter;
        public static readonly float Meter = 100 * Centimeter;
        public static readonly float Pudge = 20 * Centimeter;

        static Metrics()
        {
            Physics.gravity = new Vector3(0, -9.81f / Centimeter, 0);
        }

        public static Frame3D ToCvarcBasis(this Frame3D unityCoordinates)
        {
            var u = unityCoordinates;
            return new Frame3D(u.Z / Centimeter, -u.X / Centimeter, u.Y / Centimeter, 
                -u.Pitch, -u.Yaw, -u.Roll);
        }

        public static Frame3D ToUnityBasis(this Frame3D cvarcCoordinates)
        {
            var c = cvarcCoordinates;
            return new Frame3D(-c.Y * Centimeter, c.Z * Centimeter, c.X * Centimeter,
                -c.Pitch, -c.Yaw, -c.Roll);
        }

        public static Vector3 ToCvarcBasis(this Vector3 unityCoordinates)
        {
            return new Vector3(unityCoordinates.z, -unityCoordinates.x, unityCoordinates.y) / Centimeter;
        }
        
        public static Vector3 ToUnityBasis(this Vector3 cvarcCoordinates)
        {
            return new Vector3(-cvarcCoordinates.y, cvarcCoordinates.z, cvarcCoordinates.x) * Centimeter;
        }

        public static Quaternion ToCvarcBasis(this Quaternion cvarcQuaternion)
        {
            return Quaternion.Euler(-cvarcQuaternion.eulerAngles.x,
                                    -cvarcQuaternion.eulerAngles.y,
                                    -cvarcQuaternion.eulerAngles.z);
        }
        
        public static Quaternion ToUnityBasis(this Quaternion cvarcQuaternion)
        {
            // this transformation is inverse to itself:
            return cvarcQuaternion.ToCvarcBasis();
        }
    }
}
