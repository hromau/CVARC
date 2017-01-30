using System;

namespace AIRLab.Mathematics
{
    /// <summary>
    ///   Пока нет своих кватернионов, есть такая структура.
    /// </summary>
    [Serializable]
    public struct Quat
    {
        public double W;
        public double X, Y, Z;

        public Quat(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
    }
}