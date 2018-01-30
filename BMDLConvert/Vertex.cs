using System;

namespace BMDLConvert
{
    public class Vertex : IEquatable<Vertex>
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float U { get; set; }
        public float V { get; set; }
        public float NormalX { get; set; }
        public float NormalY { get; set; }
        public float NormalZ { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Vertex other)
                return Equals(other);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return unchecked(X.GetHashCode() ^
                             Y.GetHashCode() ^
                             Z.GetHashCode() ^
                             U.GetHashCode() ^
                             V.GetHashCode() ^
                             NormalX.GetHashCode() ^
                             NormalY.GetHashCode() ^
                             NormalZ.GetHashCode());
        }

        public bool Equals(Vertex other)
        {
            return X == other.X &&
                Y == other.Y &&
                Z == other.Z &&
                U == other.U &&
                V == other.V &&
                NormalX == other.NormalX &&
                NormalY == other.NormalY &&
                NormalZ == other.NormalZ;
        }
    }
}