using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GraphFunctionName {
    Sine,
    Sine2D,
    MultiSine,
    MultiSine2D,
    Ripple,
    Cylinder,
    Sphere,
    Torus
}

public class Graph : MonoBehaviour {
    
    //Prefab
    public Transform pointPrefab;

    //Resolution
    [Range(10,100)]
    public int resolution = 10;

    //Functions
    public GraphFunctionName function;
    public delegate Vector3 GraphFunction(float u, float v, float t);
    static GraphFunction[] functions = {
        SinFunction, Sine2DFunction, MultiSineFunction, MultiSine2DFunction, Ripple, Cylinder, Sphere, Torus
    };

    //Ppints
    Transform[] points;

    //Control vars
    const float PI = Mathf.PI;

    private void Awake()
    {
        float step = 2f / resolution; //range = 2 -> [-1, 1]
        Vector3 scale = Vector3.one *step;
        Vector3 position = Vector3.zero;

        points = new Transform[resolution * resolution];

        /*
        for (int i = 0, z = 0; z < resolution; z++)
        {
            position.z = (z + 0.5f) * step - 1f;
            for (int x = 0; x < resolution; x++, i++)
            {
                Transform point = Instantiate(pointPrefab);
                position.x = (x + 0.5f) * step - 1f;
                point.localPosition = position;
                point.localScale = scale;
                point.SetParent(transform, false);
                points[i] = point;
            }
        }*/
        for (int i = 0; i < points.Length; i++)
        {
            Transform point = Instantiate(pointPrefab);
            point.localScale = scale;
            point.SetParent(transform, false);
            Destroy(point.GetComponent<Collider>());
            points[i] = point;
        }
    }


    private void Update()
    {
        float t = Time.time;
        GraphFunction f = functions[(int)function];
        /*
        for (int i = 0; i < points.Length; i++) {
            Vector3 position = points[i].localPosition;

            //position.y = SinFunction(position.x, t);
            position= f(position.x, position.z, t);

            points[i].localPosition = position;
        }
        */
        float step = 2f / resolution;
        for (int i = 0, z = 0; z < resolution; z++)
        {
            float v = (z + 0.5f) * step - 1f;
            for (int x = 0; x < resolution; x++, i++)
            {
                float u = (x + 0.5f) * step - 1f;
                points[i].localPosition = f(u, v, t);
            }
        }
    }


    #region Functions
    static Vector3 SinFunction(float x, float z, float t)
    {
        Vector3 p = new Vector3(x, 0, z);
        p.y = Mathf.Sin(PI * (x + t));
        return p;
    }

    static Vector3 MultiSineFunction(float x, float z, float t)
    {
        Vector3 p = new Vector3(x, 0, z);
        p.y = Mathf.Sin(PI * (x + t));
        p.y += Mathf.Sin(2f * PI * (x + 2f*t)) / 2f;
        p.y *= 2f / 3f; //[-1, 1] in y
        return p;
    }

    static Vector3 Sine2DFunction(float x, float z, float t)
    {
        Vector3 p = new Vector3(x, 0, z);
        p.y = Mathf.Sin(PI * (x + t));
        p.y += Mathf.Sin(PI * (z + t));
        p.y *= 0.5f;
        return p;
    }

    static Vector3 MultiSine2DFunction(float x, float z, float t)
    {
        Vector3 p = new Vector3(x, 0, z);
        p.y = 4f * Mathf.Sin(PI * (x + z + t * 0.5f));
        p.y += Mathf.Sin(PI * (x + t));
        p.y += Mathf.Sin(2f * PI * (z + 2f * t)) * 0.5f;
        p.y *= 1f / 5.5f;
        return p;
    }

    static Vector3 Ripple(float x, float z, float t)
    {
        Vector3 p = new Vector3(x, 0, z);
        float d = Mathf.Sqrt(x * x + z * z);
        p.y = Mathf.Sin(PI * (4f * d - t));
        p.y /= 1f + 10f * d;
        return p;
    }

    static Vector3 Cylinder(float u, float v, float t)
    {
        //float r = 1; //basic cylineder
        //float r = 1f + Mathf.Sin(6f * PI * u) * 0.2f; //Wobbly cylinder
        //float r = 1f + Mathf.Sin(2f * PI * v) * 0.2f; //v instead u
        float r = 0.8f + Mathf.Sin(PI * (6f * u + 2f * v + t)) * 0.2f;

        Vector3 p;
        p.x = r * Mathf.Sin(PI * u);
        p.y = v;
        p.z = r * Mathf.Cos(PI * u);
        return p;
    }

    static Vector3 Sphere(float u, float v, float t)
    {
        Vector3 p;
        float r = 0.8f + Mathf.Sin(PI * (6f * u + t)) * 0.1f;
        r += Mathf.Sin(PI * (4f * v + t)) * 0.1f;
        float s = r * Mathf.Cos(PI * 0.5f * v);
        p.x = s * Mathf.Sin(PI * u);
        p.y = r * Mathf.Sin(PI * 0.5f * v);
        p.z = s * Mathf.Cos(PI * u);
        return p;
    }

    static Vector3 Torus(float u, float v, float t)
    {
        Vector3 p;
        float r1 = 0.65f + Mathf.Sin(PI * (6f * u + t)) * 0.1f;
		float r2 = 0.2f + Mathf.Sin(PI * (4f * v + t)) * 0.05f;
        float s = r2 * Mathf.Cos(PI  * v) + r1;
        p.x = s * Mathf.Sin(PI * u);
        p.y = r2 * Mathf.Sin(PI *  v);
        p.z = s * Mathf.Cos(PI * u);
        return p;
    }
    #endregion
}
