using UnityEngine;

namespace GPUInstancer
{

    public abstract class GPUInstancerCell
    {
        // Primes for hashing
        internal static readonly int[] _primes = {31, 37, 41, 43};

        // Cell Info
        public Bounds cellBounds;
        public Bounds cellInnerBounds;

        public int coordX;
        public int coordY;
        public int coordZ;

        public bool isActive;

        public int CalculateHash()
        {
            return CalculateHash(coordX, coordY, coordZ);
        }

        public static int CalculateHash(int coordX, int coordY, int coordZ)
        {
            return (((((_primes[0] + coordX) * _primes[1]) + coordY) * _primes[2]) + coordZ) * _primes[3];
        }
    }

}
