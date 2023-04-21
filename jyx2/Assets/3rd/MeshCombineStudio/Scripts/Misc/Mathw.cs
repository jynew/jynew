using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace MeshCombineStudio
{
    public struct AABB3
    {
        public Vector3 min;
        public Vector3 max;

        public AABB3(Vector3 min, Vector3 max)
        {
            this.min = min;
            this.max = max;
        }
    }

    public struct Triangle3
    {
        public Vector3 a, b, c;
        public Vector3 dirAb, dirAc, dirBc;
        public Vector3 h1;

        public float ab, ac, bc;
        public float area, h, ah, hb;

        public void Calc()
        {
            var _a = a;
            var _b = b;
            var _c = c;

            var _dirAb = b - a;
            var _dirAc = c - a;
            var _dirBc = c - b;

            float _ab = _dirAb.magnitude;
            float _ac = _dirAc.magnitude;
            float _bc = _dirBc.magnitude;

            if (_ac > _ab && _ac > _bc)
            {
                a = _a;
                b = _c;
                c = _b;
            }
            else if (_bc > _ab)
            {
                a = _c;
                b = _b;
                c = _a;
            }

            dirAb = b - a;
            dirAc = c - a;
            dirBc = c - b;

            ab = dirAb.magnitude;
            ac = dirAc.magnitude;
            bc = dirBc.magnitude;

            float s = (ab + ac + bc) * 0.5f;

            area = Mathf.Sqrt(s * (s - ab) * (s - ac) * (s - bc));
            h = (2 * area) / ab;

            ah = Mathf.Sqrt((ac * ac) - (h * h));

            hb = ab - ah;

            // Debug.Log("ab " + ab + " ac " + ac + " bc " + bc + " area " + area + " h " + h + " ah " + ah);

            h1 = a + (dirAb * ((1 / ab) * ah));
        }
    }

    public struct Sphere3
    {
        public Vector3 center;
        public float radius;

        public Sphere3(Vector3 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }
    }

    public struct Int2
    {
        public int x, y;

        public Int2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public struct Int3
    {
        public int x, y, z;

        public Int3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Int3 operator +(Int3 a, Int3 b)
        {
            Int3 v;
            v.x = a.x + b.x;
            v.y = a.y + b.y;
            v.z = a.z + b.z;
            return v;
        }
    }

    public static class Mathw
    {
        public static readonly int[] bits = new int[] { 1 << 0 , 1 << 1, 1 << 2, 1 << 3, 1 << 4, 1 << 5, 1 << 6, 1 << 7, 1 << 8, 1 << 9, 1 << 10, 1 << 11, 1 << 12, 1 << 13, 1 << 14, 1 << 15, 1 << 16, 1 << 17,
                                                        1 << 18, 1 << 19, 1 << 20, 1 << 21, 1 << 22, 1 << 23, 1 << 24, 1 << 25, 1 << 26, 1 << 27, 1 << 28, 1 << 29, 1 << 30, 1 << 31};

        public static Vector3 Clamp(Vector3 v, float min, float max)
        {
            if (v.x < min) v.x = min;
            else if (v.x > max) v.x = max;
            if (v.y < min) v.y = min;
            else if (v.y > max) v.y = max;
            if (v.z < min) v.z = min;
            else if (v.z > max) v.z = max;
            return v;
        }

        public static Vector3 FloatToVector3(float v)
        {
            return new Vector3(v, v, v);
        }

        public static float SinDeg(float angle)
        {
            return Mathf.Sin(angle * Mathf.Deg2Rad);// * Mathf.Rad2Deg;
        }

        public static float GetMax(Vector3 v)
        {
            float max = v.x;
            if (v.y > max) max = v.y;
            if (v.z > max) max = v.z;
            return max;
        }

        public static Vector3 SetMin(Vector3 v, float min)
        {
            if (v.x < min) v.x = min;
            if (v.y < min) v.y = min;
            if (v.z < min) v.z = min;
            return v;
        }

        public static Vector3 Snap(Vector3 v, float snapSize)
        {
            v.x = Mathf.Floor(v.x / snapSize) * snapSize;
            v.y = Mathf.Floor(v.y / snapSize) * snapSize;
            v.z = Mathf.Floor(v.z / snapSize) * snapSize;

            return v;
        }

        public static Vector3 SnapRound(Vector3 v, float snapSize)
        {
            v.x = Mathf.Round(v.x / snapSize) * snapSize;
            v.y = Mathf.Round(v.y / snapSize) * snapSize;
            v.z = Mathf.Round(v.z / snapSize) * snapSize;

            return v;
        }

        public static Vector3 Divide(Vector3 a, Vector3 b)
        {
            a.x /= b.x;
            a.y /= b.y;
            a.z /= b.z;

            return a;
        }

        public static Vector3 Divide(float a, Vector3 b)
        {
            b.x = a / b.x;
            b.y = a / b.y;
            b.z = a / b.z;

            return b;
        }

        public static Vector3 Scale(Vector3 a, Int3 b)
        {
            a.x *= b.x;
            a.y *= b.y;
            a.z *= b.z;

            return a;
        }

        public static Vector3 Abs(Vector3 v)
        {
            return new Vector3(v.x < 0 ? -v.x : v.x, v.y < 0 ? -v.y : v.y, v.z < 0 ? -v.z : v.z);
        }

        public static bool IntersectAABB3Sphere3(AABB3 box, Sphere3 sphere)
        {
            Vector3 center = sphere.center;
            Vector3 min = box.min;
            Vector3 max = box.max;
            float totalDistance = 0f;
            float distance;
            if (center.x < min.x)
            {
                distance = center.x - min.x;
                totalDistance += distance * distance;
            }
            else if (center.x > max.x)
            {
                distance = center.x - max.x;
                totalDistance += distance * distance;
            }
            if (center.y < min.y)
            {
                distance = center.y - min.y;
                totalDistance += distance * distance;
            }
            else if (center.y > max.y)
            {
                distance = center.y - max.y;
                totalDistance += distance * distance;
            }
            if (center.z < min.z)
            {
                distance = center.z - min.z;
                totalDistance += distance * distance;
            }
            else if (center.z > max.z)
            {
                distance = center.z - max.z;
                totalDistance += distance * distance;
            }
            return totalDistance <= sphere.radius * sphere.radius;
        }

        public static bool IntersectAABB3Triangle3(Vector3 boxCenter, Vector3 boxHalfSize, Triangle3 triangle)
        {
            Vector3 v0, v1, v2;

            float min, max, fex, fey, fez;
            Vector3 normal, e0, e1, e2;

            v0 = triangle.a - boxCenter;
            v1 = triangle.b - boxCenter;
            v2 = triangle.c - boxCenter;

            e0 = v1 - v0;
            e1 = v2 - v1;
            e2 = v0 - v2;

            fex = Abs(e0[0]);
            fey = Abs(e0[1]);
            fez = Abs(e0[2]);

            if (!AxisTest_X01(v0, v2, boxHalfSize, e0[2], e0[1], fez, fey, out min, out max)) return false;
            if (!AxisTest_Y02(v0, v2, boxHalfSize, e0[2], e0[0], fez, fex, out min, out max)) return false;
            if (!AxisTest_Z12(v1, v2, boxHalfSize, e0[1], e0[0], fey, fex, out min, out max)) return false;

            fex = Abs(e1[0]);
            fey = Abs(e1[1]);
            fez = Abs(e1[2]);

            if (!AxisTest_X01(v0, v2, boxHalfSize, e1[2], e1[1], fez, fey, out min, out max)) return false;
            if (!AxisTest_Y02(v0, v2, boxHalfSize, e1[2], e1[0], fez, fex, out min, out max)) return false;
            if (!AxisTest_Z0(v0, v1, boxHalfSize, e1[1], e1[0], fey, fex, out min, out max)) return false;

            fex = Abs(e2[0]);
            fey = Abs(e2[1]);
            fez = Abs(e2[2]);

            if (!AxisTest_X2(v0, v1, boxHalfSize, e2[2], e2[1], fez, fey, out min, out max)) return false;
            if (!AxisTest_Y1(v0, v1, boxHalfSize, e2[2], e2[0], fez, fex, out min, out max)) return false;
            if (!AxisTest_Z12(v1, v2, boxHalfSize, e2[1], e2[0], fey, fex, out min, out max)) return false;

            GetMinMax(v0[0], v1[0], v2[0], out min, out max);

            if (min > boxHalfSize[0] || max < -boxHalfSize[0]) return false;

            GetMinMax(v0[1], v1[1], v2[1], out min, out max);

            if (min > boxHalfSize[1] || max < -boxHalfSize[1]) return false;

            GetMinMax(v0[2], v1[2], v2[2], out min, out max);

            if (min > boxHalfSize[2] || max < -boxHalfSize[2]) return false;
             
            normal = Vector3.Cross(e0, e1);
            if (!PlaneBoxOverlap(normal, v0, boxHalfSize)) return false;

            return true;
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void GetMinMax(float x0, float x1, float x2, out float min, out float max)
        {
            min = max = x0;

            if (x1 < min) min = x1;
            else if (x1 > max) max = x1;

            if (x2 < min) min = x2;
            else if (x2 > max) max = x2;
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool PlaneBoxOverlap(Vector3 normal, Vector3 vert, Vector3 maxBox)
        {
            float v;
            Vector3 vmin = Vector3.zero, vmax = Vector3.zero;

            for (int i = 0; i <= 2; i++)
            {
                v = vert[i];

                if (normal[i] > 0.0f)
                {
                    vmin[i] = -maxBox[i] - v;
                    vmax[i] = maxBox[i] - v;
                }
                else
                {
                    vmin[i] = maxBox[i] - v;
                    vmax[i] = -maxBox[i] - v;
                }
            }

            if (Vector3.Dot(normal, vmin) > 0.0f) return false;
            if (Vector3.Dot(normal, vmax) >= 0.0f) return true;

            return false;
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float Abs(float v)
        {
            return v < 0 ? -v : v;
        }

        /*======================== X-tests ========================*/
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool AxisTest_X01(Vector3 v0, Vector3 v2, Vector3 boxHalfSize, float a, float b, float fa, float fb, out float min, out float max)
        {
            float p0 = a * v0[1] - b * v0[2];
            float p2 = a * v2[1] - b * v2[2];

            if (p0 < p2) { min = p0; max = p2; } else { min = p2; max = p0; }

            float rad = fa * boxHalfSize[1] + fb * boxHalfSize[2];

            if (min > rad || max < -rad) return false;
            return true;
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool AxisTest_X2(Vector3 v0, Vector3 v1, Vector3 boxHalfSize, float a, float b, float fa, float fb, out float min, out float max)
        {
            float p0 = a * v0[1] - b * v0[2];
            float p1 = a * v1[1] - b * v1[2];

            if (p0 < p1) { min = p0; max = p1; } else { min = p1; max = p0; }

            float rad = fa * boxHalfSize[1] + fb * boxHalfSize[2];

            if (min > rad || max < -rad) return false;
            return true;
        }

        /*======================== Y-tests ========================*/

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool AxisTest_Y02(Vector3 v0, Vector3 v2, Vector3 boxHalfSize, float a, float b, float fa, float fb, out float min, out float max)
        {
            float p0 = -a * v0[0] + b * v0[2];
            float p2 = -a * v2[0] + b * v2[2];

            if (p0 < p2) { min = p0; max = p2; } else { min = p2; max = p0; }

            float rad = fa * boxHalfSize[0] + fb * boxHalfSize[2];

            if (min > rad || max < -rad) return false;
            return true;
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool AxisTest_Y1(Vector3 v0, Vector3 v1, Vector3 boxHalfSize, float a, float b, float fa, float fb, out float min, out float max)
        {
            float p0 = -a * v0[0] + b * v0[2];
            float p1 = -a * v1[0] + b * v1[2];

            if (p0 < p1) { min = p0; max = p1; } else { min = p1; max = p0; }

            float rad = fa * boxHalfSize[0] + fb * boxHalfSize[2];

            if (min > rad || max < -rad) return false;
            return true;
        }

        /*======================== Z-tests ========================*/

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool AxisTest_Z12(Vector3 v1, Vector3 v2, Vector3 boxHalfSize, float a, float b, float fa, float fb, out float min, out float max)
        {
            float p1 = a * v1[0] - b * v1[1];
            float p2 = a * v2[0] - b * v2[1];

            if (p2 < p1) { min = p2; max = p1; } else { min = p1; max = p2; }

            float rad = fa * boxHalfSize[0] + fb * boxHalfSize[1];

            if (min > rad || max < -rad) return false;
            return true;
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool AxisTest_Z0(Vector3 v0, Vector3 v1, Vector3 boxHalfSize, float a, float b, float fa, float fb, out float min, out float max)
        {
            float p0 = a * v0[0] - b * v0[1];
            float p1 = a * v1[0] - b * v1[1];

            if (p0 < p1) { min = p0; max = p1; } else { min = p1; max = p0; }

            float rad = fa * boxHalfSize[0] + fb * boxHalfSize[1];

            if (min > rad || max < -rad) return false;
            return true;
        }
    }
}
