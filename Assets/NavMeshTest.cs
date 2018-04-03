using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshTest : MonoBehaviour {
    public GameObject source;
    public GameObject target;

    List<Vector3> path = new List<Vector3>();

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();

    AStarFinder finder = new AStarFinder();

    List<Vector3> debugNavMeshVertices = new List<Vector3>();
    List<int>   debugNavMeshIndices = new List<int>();
    // Use this for initialization
    void Start () {
        unityPath = new NavMeshPath();

        MeshFilter meshFilter = this.gameObject.AddComponent<MeshFilter>();

        NavMeshTriangulation triangulatedNavMesh  = NavMesh.CalculateTriangulation();
        debugNavMeshVertices.AddRange(triangulatedNavMesh.vertices);
        debugNavMeshIndices.AddRange(triangulatedNavMesh.indices);

        //weld vertices
        for (int i = 0; i < triangulatedNavMesh.indices.Length; i+=3)
        {
            for(int j = 0; j < 3; j++)
            {
                int vertexIndex = triangulatedNavMesh.indices[i + j];
                Vector3 v = triangulatedNavMesh.vertices[vertexIndex];
                int index = FindIndex(v);
                if(index == -1)
                {
                    vertices.Add(v);
                    index = vertices.Count - 1;
                }
                triangles.Add(index);
            }
        }

        this.ExportNavMesh();

        TNavMesh.Init("E:/navmesh.txt");
    }

    int FindIndex(Vector3 v)
    {
        for(int i = 0; i < vertices.Count; i++)
        {
            //if (vertices[i] == v)
            if((vertices[i]- v).sqrMagnitude < 0.001f*0.001f)
                return i;
        }

        return -1;
    }

    void ExportNavMesh()
    {
        string position = "";
        for(int i = 0; i < vertices.Count; i++)
        {
            Vector3 v = vertices[i];
            position += v.x;
            position += ",";
            position += v.y;
            position += ",";
            position += v.z;

            if(i != vertices.Count-1)
                position += ",";
        }

        string indices = "";
        for (int i = 0; i < triangles.Count; i++)
        {
            indices += triangles[i];

            if (i != triangles.Count - 1)
                indices += ",";
        }

        File.WriteAllLines("E:/navmesh.txt", new string[]{ position, indices });
    }

    private void OnDrawGizmos()
    {
        //foreach (var triangle in this.pathTriangle)
        //{
        //    Gizmos.color = Color.red;
        //    Vector3 v0 = this.vertices[triangle.index0];
        //    Vector3 v1 = this.vertices[triangle.index1];
        //    Vector3 v2 = this.vertices[triangle.index2];
        //    Gizmos.DrawLine(v0, v1);
        //    Gizmos.DrawLine(v1, v2);
        //    Gizmos.DrawLine(v2, v0);
        //}

        //for (int i = 0; i < this.portals.Count; i += 2)
        //{
        //    Gizmos.color = Color.blue;
        //    Gizmos.DrawLine(this.portals[i], this.portals[i + 1]);
        //}

        for (int i = 0; i < TNavMesh.GetDebugPathNode().Count; i+=2)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawLine(TNavMesh.GetDebugPathNode()[i], TNavMesh.GetDebugPathNode()[i + 1]);
        }

        for (int i = 0; i < TNavMesh.GetDebugPathNodePosition().Count - 1; i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(TNavMesh.GetDebugPathNodePosition()[i], TNavMesh.GetDebugPathNodePosition()[i + 1]);
        }

        //List<Vector3> neighbor = TNavMesh.GetDebugNeighbor(this.target.transform.position);
        //for (int i = 0; i < neighbor.Count; i+=2)
        //{
        //    Gizmos.color = Color.yellow;
        //    Gizmos.DrawLine(neighbor[i], neighbor[i+1]);
        //}


        //for (int i = 0; i < TNavMesh.GetDebugPortal().Count; i += 2)
        //{
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawLine(TNavMesh.GetDebugPortal()[i], TNavMesh.GetDebugPortal()[i + 1]);
        //}


        for (int i = 0; i < path.Count - 1; i++)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(path[i], path[i + 1]);
        }

        if (unityPath != null)
        {
            for (int i = 0; i < unityPath.corners.Length - 1; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(unityPath.corners[i], unityPath.corners[i + 1]);
            }
        }

        //draw navmesh
        //for(int i = 0; i < debugNavMeshIndices.Count; i+=3)
        //{
        //    Vector3 v0 = debugNavMeshVertices[debugNavMeshIndices[i]];
        //    Vector3 v1 = debugNavMeshVertices[debugNavMeshIndices[i+1]];
        //    Vector3 v2 = debugNavMeshVertices[debugNavMeshIndices[i+2]];

        //    Gizmos.color = Color.red;
        //    Gizmos.DrawLine(v0, v1);
        //    Gizmos.DrawLine(v1, v2);
        //    Gizmos.DrawLine(v2, v0);
        //}
    }

    Vector3 lastStartPos = Vector3.positiveInfinity;
    Vector3 lastTargetPos = Vector3.positiveInfinity;

    NavMeshPath unityPath = null;
    private void Update()
    {
        Vector3 startPos = source.transform.position;
        Vector3 targetPos = target.transform.position;

        if (startPos == lastStartPos && targetPos == lastTargetPos)
            return;

        this.path.Clear();
        float time = Time.realtimeSinceStartup;
        if(TNavMesh.CalculatePath(startPos, targetPos, this.path))
        {

        }
        Debug.Log("TNavMesh: " + (Time.realtimeSinceStartup-time));

        time = Time.realtimeSinceStartup;
        if (NavMesh.CalculatePath(startPos, targetPos, NavMesh.AllAreas, unityPath))
        {

        }
        Debug.Log("UnityNavMesh: " + (Time.realtimeSinceStartup - time));

        lastStartPos = startPos;
        lastTargetPos = targetPos;
    }
}
