using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Supershapes : MonoBehaviour {

    public int total;
    public float radius = 1;

    private Mesh mesh;
    private Vector3[,] points;
    private Vector3[] vertices;
    private Vector3[] normals;
    private Color32[] cubeUV;

    public bool guizmos = false;
    public bool onlyOne= false;

    public int gizmoX = 0;
    public int gizmoY = 0;

    float HALF_PI = Mathf.PI / 2;
    float TWO_PI = Mathf.PI * 2;

    public float m = 0;
    float mChange = 0;
    public float a = 1;
    public float b = 1;
    public bool active = false;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "SuperShape";

        Generate();
    }


    private void Update()
    {
        if (!active)
            return;

        m = Map(Mathf.Sin(mChange), -1, 1, 0, 7);
        mChange += Time.deltaTime;
        Deformer();
    }


    private void Deformer()
    {
        for (int i = 0; i < total + 1; i++)
        {
            float lat = Map(i, 0, total, -HALF_PI, HALF_PI);
            float r2 = supershape(lat, m, 0.2f, 1.7f, 1.7f);

            for (int j = 0; j < total + 1; j++)
            {
                float lon = Map(j, 0, total, -Mathf.PI, Mathf.PI);
                float r1 = supershape(lon, m, 0.2f, 1.7f, 1.7f);

                float x = radius * r1 * Mathf.Cos(lon) * r2 * Mathf.Cos(lat);
                float y = radius * r1 * Mathf.Sin(lon) * r2 * Mathf.Cos(lat);
                float z = radius * r2 * Mathf.Sin(lat);
                points[i, j] = new Vector3(x, y, z);
                vertices[total * i + j] = points[i, j];
                //SetVertex(total * i + j, x, y, z);
            }
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }



    void Generate()
    {
        CreateVertices();
        CreateTriangles();
    }


    private void CreateVertices()
    {
        points = new Vector3[total + 1, total + 1];
        vertices = new Vector3[(total + 1) * (total + 1)];
        normals = new Vector3[vertices.Length];
        cubeUV = new Color32[vertices.Length];

        /*for (int i = 0; i < total + 1; i++)
        {
            float lat = Map(i, 0, total, HALF_PI, -HALF_PI);
            for (int j = 0; j < total + 1; j++)
            {
                float lon = Map(j, 0, total, Mathf.PI, Mathf.PI);
                float x = radius * Mathf.Sin(lat) * Mathf.Cos(lon);
                float y = radius * Mathf.Sin(lat) * Mathf.Sin(lon);
                float z = radius * Mathf.Cos(lat);
                points[i,j] = new Vector3(x, y, z);
                SetVertex(total * i + j, x, y, z);
            }
        }*/

        for (int i = 0; i < total + 1; i++)
        {
            float lat = Map(i, 0, total, -HALF_PI, HALF_PI);
            float r2 = supershape(lat, m, 0.2f, 1.7f, 1.7f);

            for (int j = 0; j < total + 1; j++)
            {
                float lon = Map(j, 0, total, -Mathf.PI, Mathf.PI);
                float r1 = supershape(lon, m, 0.2f, 1.7f, 1.7f);

                float x = radius * r1 * Mathf.Cos(lon) * r2 * Mathf.Cos(lat);
                float y = radius * r1 * Mathf.Sin(lon) * r2 * Mathf.Cos(lat);
                float z = radius * r2 * Mathf.Sin(lat);
                points[i, j] = new Vector3(x, y, z);
                SetVertex(total * i + j, x, y, z);
            }
        }

        //Deformer();
        mesh.vertices= vertices;
        mesh.normals = normals;
        //mesh.colors32 = cubeUV;
    }


    private void SetVertex(int i, float x, float y, float z)
    {
        Vector3 v = new Vector3(x, y, z);

        float x2 = v.x * v.x;
        float y2 = v.y * v.y;
        float z2 = v.z * v.z;
        Vector3 s;
        s.x = v.x * Mathf.Sqrt(1f - y2 / 2f - z2 / 2f + y2 * z2 / 3f);
        s.y = v.y * Mathf.Sqrt(1f - x2 / 2f - z2 / 2f + x2 * z2 / 3f);
        s.z = v.z * Mathf.Sqrt(1f - x2 / 2f - y2 / 2f + x2 * y2 / 3f);

        normals[i] = s;
        vertices[i] = v;
        //cubeUV[i] = new Color32((byte)x, (byte)y, (byte)z, 0);
    }

    private void CreateTriangles()
    {
        int[] triangles = new int[(total + 1) * (total + 1) * 6];
        
        int ti = 0;
        for (int i = 0; i < total; i++)
        {
            for (int j = 0; j < total; j++)
            {
                triangles[ti] = total * i + j;
                triangles[ti + 2] = triangles[ti + 3] = total * (i+1) + j;
                triangles[ti + 1] = triangles[ti + 4] = total * i + j + 1;
                triangles[ti + 5] = total * (i + 1) + j + 1;
                ti += 6;
            }
        }
        
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }


    float Map(float value, float from, float to, float from2, float to2)
    {
        return from2 + ((value - from) * (to2 - from2) / (to - from));
    }

    float supershape(float theta, float m, float n1, float n2, float n3)
    {

        //return 1;

        float t1 = Mathf.Abs((1 / a) * Mathf.Cos(m * theta / 4));
        t1 = Mathf.Pow(t1, n2);
        float t2 = Mathf.Abs((1 / b) * Mathf.Sin(m * theta / 4));
        t2 = Mathf.Pow(t2, n3);
        float t3 = t1 + t2;
        float r = Mathf.Pow(t3, -1 / n1);
        return r;
    }




    private void OnDrawGizmos()
    {
        if (!guizmos)
            return;

        if (points == null)
        {
            return;
        }
        Gizmos.color = Color.black;

        if (onlyOne)
            Gizmos.DrawSphere(points[gizmoX, gizmoY], 0.05f);
        else
        {
            for (int i = 0; i < total + 1; i++)
            {
                for (int j = 0; j < total + 1; j++)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawSphere(points[i, j], 0.05f);
                }
            }
        }
    }
}
