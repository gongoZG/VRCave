using System.Collections.Generic;
using UnityEngine;

public class WayPointPath : MonoBehaviour
{
    #region CREATE PATH

    [HideInInspector]
    public List<Transform> nodes = new List<Transform>();
    private int numPoints;
    private Vector3[] points;
    private float[] distances;
    public float Length { get; private set; }

    #endregion

    #region SMOOTH

    [SerializeField] private bool smoothRoute = true;
    [Range(40, 200)] public float editorVisualisationSubsteps = 100;

    private int p0n;
    private int p1n;
    private int p2n;
    private int p3n;

    private float i;
    private Vector3 P0;
    private Vector3 P1;
    private Vector3 P2;
    private Vector3 P3;

    #endregion

    private void Awake()
    {
        if (nodes.Count > 1)
        {
            CachePositionsAndDistances();
        }
        numPoints = nodes.Count;
    }

    #region GET ROUTE

    public RoutePoint GetRoutePoint(float dist)
    {
        Vector3 p1 = GetRoutePosition(dist);
        Vector3 p2 = GetRoutePosition(dist + 0.1f);
        Vector3 delta = p2 - p1;
        return new RoutePoint(p1, delta.normalized);
    }

    public Vector3 GetRoutePosition(float dist)
    {
        int point = 0;

        if (Length == 0)
        {
            Length = distances[distances.Length - 1];
        }

        dist = Mathf.Repeat(dist, Length);

        while (distances[point] < dist)
        {
            ++point;
        }

        p1n = ((point - 1) + numPoints) % numPoints;
        p2n = point;

        i = Mathf.InverseLerp(distances[p1n], distances[p2n], dist);

        if (smoothRoute)
        {
            p0n = ((point - 2) + numPoints) % numPoints;
            p2n = p2n % numPoints;
            p3n = (point + 1) % numPoints;            

            P0 = points[p0n];
            P1 = points[p1n];
            P2 = points[p2n];
            P3 = points[p3n];

            return CatmullRom(P0, P1, P2, P3, i);
        }
        else
        {
            return Vector3.Lerp(points[p1n], points[p2n], i);
        }
    }

    public struct RoutePoint
    {
        public Vector3 position;
        public Vector3 direction;


        public RoutePoint(Vector3 position, Vector3 direction)
        {
            this.position = position;
            this.direction = direction;
        }
    }

    #endregion

    #region DRAW PATH

    private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float i)
    {
        return 0.5f *
               ((2 * p1) + (-p0 + p2) * i + (2 * p0 - 5 * p1 + 4 * p2 - p3) * i * i +
                (-p0 + 3 * p1 - 3 * p2 + p3) * i * i * i);
    }

    private void CachePositionsAndDistances()
    {
        points = new Vector3[nodes.Count + 1];
        distances = new float[nodes.Count + 1];

        float accumulateDistance = 0;
        for (int i = 0; i < points.Length; ++i)
        {
            var t1 = nodes[(i) % nodes.Count];
            var t2 = nodes[(i + 1) % nodes.Count];
            if (t1 != null && t2 != null)
            {
                Vector3 p1 = t1.position;
                Vector3 p2 = t2.position;
                points[i] = nodes[i % nodes.Count].position;
                distances[i] = accumulateDistance;
                accumulateDistance += (p1 - p2).magnitude;
            }
        }
    }

    private void OnDrawGizmos()
    {
        DrawGizmos(false);
    }


    private void OnDrawGizmosSelected()
    {
        DrawGizmos(true);
    }

    private void DrawGizmos(bool selected)
    {
        Transform[] pathTransforms = GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }

        if (nodes.Count > 1)
        {
            numPoints = nodes.Count;

            CachePositionsAndDistances();
            Length = distances[distances.Length - 1];

            Gizmos.color = selected ? Color.yellow : new Color(1, 1, 0, 0.5f);
            Vector3 prev = nodes[0].position;
            if (smoothRoute)
            {
                for (float dist = 0; dist < Length; dist += Length / editorVisualisationSubsteps)
                {
                    Vector3 next = GetRoutePosition(dist + 1);
                    Gizmos.DrawLine(prev, next);
                    prev = next;
                }
                Gizmos.DrawLine(prev, nodes[0].position);
            }
            else
            {
                for (int n = 0; n < nodes.Count; ++n)
                {
                    Vector3 next = nodes[(n + 1) % nodes.Count].position;
                    Gizmos.DrawLine(prev, next);
                    prev = next;
                }
            }
        }
    }

    #endregion
}
