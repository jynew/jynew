using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class RaycastTest : MonoBehaviour
{
    public MeshRenderer mr;
    new public Collider collider;
    public LayerMask layerMask;
    public bool createTriangle;
    public int triangleIndex;
    
    RaycastHit hitInfo;

    void Update()
    {
        if (createTriangle)
        {
            createTriangle = false;
            CreateTriangle();
        }
    }

    void CreateTriangle()
    {
        Mesh mesh = new Mesh();

        Vector3 p1 = new Vector3(0, 0, 0);
        Vector3 p2 = new Vector3(0, 0, 1);
        Vector3 p3 = new Vector3(0, 1, 0);

        float offset = 0.01f;
        
        Vector3 p4 = new Vector3(offset, 0, 0);
        Vector3 p5 = new Vector3(offset, 0, 1);
        Vector3 p6 = new Vector3(offset, 1, 0);

        var verts = new Vector3[] { p1, p2, p3,
                                    p6, p5, p4,
        };

        

        var triangles = new int[] { 0, 1, 2,
                                    3, 4, 5,
        };

        mesh.name = "Triangle";

        mesh.vertices = verts;
        mesh.triangles = triangles;

        var mf = GetComponent<MeshFilter>();
        mf.sharedMesh = mesh;

        var mc = GetComponent<MeshCollider>();
        mc.sharedMesh = mesh;

        //float tStamp = Time.realtimeSinceStartup;

        //for (int i = 0; i < 100000; i++)
        //{
        //    verts[0].x += 0.1f;

        //    mesh.vertices = verts;

        //    mc.sharedMesh = mesh;
        //}

        //float time = Time.realtimeSinceStartup - tStamp;
        //Debug.Log("Time " + time);
    }
    
    void Swap<T>(ref T v1, ref T v2)
    {
        T temp = v1;
        v1 = v2;
        v2 = temp;
    }

    // Rigidbody rb;

    void OnDrawGizmos()
    {
        if (!mr) return;

        // Collider collider = mr.GetComponent<Collider>();
        // rb = GetComponent<Rigidbody>();

        Vector3 pos = transform.position;

        Vector3 colliderPos = mr.bounds.min;

        // var dir = colliderPos - pos;
        var dir = Vector3.left;

        Physics.queriesHitBackfaces = true;
        // Collider mc = GetComponent<Collider>();

        float tStamp = Time.realtimeSinceStartup;
        Ray ray = new Ray();

        // for (int i = 0; i < 1000000; i++)
        {
            ray.origin = pos;
            ray.direction = dir;
            if (Physics.Raycast(ray, out hitInfo, 10000))
            {
                if (Vector3.Dot(dir, hitInfo.normal) >= 0) Gizmos.color = Color.green; else Gizmos.color = Color.red;

                Gizmos.DrawLine(hitInfo.point, hitInfo.point + hitInfo.normal);

                Gizmos.color = Color.white;
                Gizmos.DrawLine(pos, hitInfo.point);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(pos, pos + (dir.normalized * 1000));
            }
        }

        // float time = Time.realtimeSinceStartup - tStamp;
        // Debug.Log("Raycast Time " + time);

        // if (Physics.CheckBox(transform.position, transform.lossyScale * 0.5f)) Gizmos.color = Color.red; else Gizmos.color = Color.green;
        // Gizmos.DrawCube(transform.position, transform.lossyScale);
        // return;
        var mf = GetComponent<MeshFilter>();
        var mesh = mf.sharedMesh;

        var sourceMf = mr.GetComponent<MeshFilter>();

        Mesh sourceMesh = sourceMf.sharedMesh;

        Vector3[] sourceVerts = sourceMesh.vertices;
        int[] sourceTris = sourceMesh.triangles;
        // return;
        // int triIndex = triangleIndex * 3;

        for (int triIndex = 0; triIndex < sourceTris.Length; triIndex += 3)
        {
            Vector3 vertPos1 = mr.transform.TransformPoint(sourceVerts[sourceTris[triIndex + 2]]);
            Vector3 vertPos2 = mr.transform.TransformPoint(sourceVerts[sourceTris[triIndex + 0]]);
            Vector3 vertPos3 = mr.transform.TransformPoint(sourceVerts[sourceTris[triIndex + 1]]);

            var tri = new TriangleTest();

            tri.a = vertPos1;
            tri.b = vertPos2;
            tri.c = vertPos3;

            tri.Calc();

            Vector3 origin = tri.a + (tri.dirAb / 2) + ((tri.c - tri.h1) / 2);

            if (Physics.CheckBox(origin, new Vector3(0.05f, tri.h, tri.ab) / 2, Quaternion.LookRotation(tri.dirAb, tri.dirAc)))
            {
                Gizmos.color = Color.red;

                Gizmos.DrawLine(tri.a, tri.b);
                Gizmos.DrawLine(tri.b, tri.c);
                Gizmos.DrawLine(tri.c, tri.a);
                Gizmos.DrawLine(tri.c, tri.h1);
            }
            else Gizmos.color = Color.green;

            //transform.position = tri.h1;

            //float distance = 0;

            //transform.localScale = new Vector3(1, tri.h, tri.hb);
            //transform.rotation = Quaternion.LookRotation(tri.dirAb, tri.dirAc);

            //Physics.SyncTransforms();

            //if (Physics.ComputePenetration(mc, transform.position, transform.rotation, collider, collider.transform.position, collider.transform.rotation, out dir, out distance))
            //{
            //    // Gizmos.DrawSphere(tri.h1, 0.01f);

            //    Gizmos.color = Color.blue;
            //    if (drawTriangle)
            //    {
            //        Gizmos.DrawLine(tri.a, tri.b);
            //        Gizmos.DrawLine(tri.b, tri.c);
            //        Gizmos.DrawLine(tri.c, tri.a);
            //        Gizmos.DrawLine(tri.c, tri.h1);
            //    }
            //}

            //transform.localScale = new Vector3(1, tri.h, tri.ah);
            //transform.rotation = Quaternion.LookRotation(-tri.dirAb, tri.dirAc);
            //Physics.SyncTransforms();

            //if (Physics.ComputePenetration(mc, transform.position, transform.rotation, collider, collider.transform.position, collider.transform.rotation, out dir, out distance))
            //{
            //    // Gizmos.DrawSphere(tri.h1, 0.01f);

            //    Gizmos.color = Color.blue;
            //    if (drawTriangle)
            //    {
            //        Gizmos.DrawLine(tri.a, tri.b);
            //        Gizmos.DrawLine(tri.b, tri.c);
            //        Gizmos.DrawLine(tri.c, tri.a);
            //        Gizmos.DrawLine(tri.c, tri.h1);
            //    }
            //}
        }

        Physics.queriesHitBackfaces = false; 
    }

    public bool step2;
    public bool drawTriangle;
}

public struct TriangleTest
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

    void Swap<T>(ref T v1, ref T v2)
    {
        T temp = v1;
        v1 = v2;
        v2 = temp;
    }
}
