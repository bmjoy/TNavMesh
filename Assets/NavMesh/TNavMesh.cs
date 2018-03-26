using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text;


public class TNavMesh
{
    private static List<Vector3> vertices = new List<Vector3>();
    private static List<int> triangleIndices = new List<int>();
    private static AStarFinder finder = new AStarFinder();

    public static void Init(string navFile)
    {
        Serialize(navFile);

        BuildGraph();

        InitCollider();
    }

    private static void Serialize(string navFile)
    {
        //navmesh格式
        //文本格式分三行，第一行为顶点坐标，第二行为顶点索引，第三行为区域索引,行内数据以","为分隔符
        //v0.x,v0.y,v0.z,v1.x,v1.y,v1.z.....vn.x,vn.y,vn.z\n
        //index0,index1.............................indexn\n
        //area0,area1................................arean\n
        string[] data = File.ReadAllLines(navFile);

        string[] positions = data[0].Split(',');
        Vector3 v = Vector3.zero;
        for (int i = 0; i < positions.Length; i += 3)
        {
            v.x = float.Parse(positions[i]);
            v.y = float.Parse(positions[i + 1]);
            v.z = float.Parse(positions[i + 2]);

            vertices.Add(v);
        }

        string[] indices = data[1].Split(',');
        for (int i = 0; i < indices.Length; i++)
        {
            int index = int.Parse(indices[i]);
            triangleIndices.Add(index);
        }
    }

    private static List<TNavNode> navNodes = new List<TNavNode>();
    private static Dictionary<string, TNavNode> navNodeDict = new Dictionary<string, TNavNode>();
    private static void BuildGraph()
    {
        for (int i = 0; i < triangleIndices.Count; i += 3)
        {
            TNavNode node = new TNavNode(triangleIndices[i], triangleIndices[i + 1], triangleIndices[i + 2]);
            node.centroid = CalculateCentroid(node);
            navNodes.Add(node);

            //自己顺时针对邻接三角形来讲是逆时针........
            //navNodeDict.Add((node.index1 << 32) | node.index0, node);
            //navNodeDict.Add((node.index2 << 32) | node.index1, node);
            //navNodeDict.Add((node.index0 << 32) | node.index2, node);
            navNodeDict.Add(string.Format("{0},{1}", node.index1, node.index0), node);
            navNodeDict.Add(string.Format("{0},{1}", node.index2, node.index1), node);
            navNodeDict.Add(string.Format("{0},{1}", node.index0, node.index2), node);
        }

        foreach (TNavNode node in navNodes)
        {
            TNavNode neighbor = null;

            //index = (node.index0 << 32) | node.index1;
            if (navNodeDict.TryGetValue(string.Format("{0},{1}", node.index0, node.index1), out neighbor))
            {
                node.AddNeighbor(0, neighbor);
            }

            //index = (node.index1 << 32) | node.index2;
            if (navNodeDict.TryGetValue(string.Format("{0},{1}", node.index1, node.index2), out neighbor))
            {
                node.AddNeighbor(1, neighbor);
            }

            //index = (node.index2 << 32) | node.index0;
            if (navNodeDict.TryGetValue(string.Format("{0},{1}", node.index2, node.index0), out neighbor))
            {
                node.AddNeighbor(2, neighbor);
            }
        }
        navNodeDict.Clear();
    }

    private static Collider collider = null;
    private static Mesh colliderMesh = null;
    private static void InitCollider()
    {
        if (colliderMesh == null)
            colliderMesh = new Mesh();

        //colliderMesh.Clear();
        colliderMesh.vertices = vertices.ToArray();
        colliderMesh.triangles = triangleIndices.ToArray();

        if (collider == null)
        {
            GameObject gameObj = new GameObject("NavMeshCollider");
            MeshFilter meshFilter = gameObj.AddComponent<MeshFilter>();
            meshFilter.mesh = colliderMesh;

            collider = gameObj.AddComponent<MeshCollider>();
        }
    }

    public static void Clear()
    {
        vertices.Clear();
        triangleIndices.Clear();
        navNodes.Clear();
    }

    private static List<TNavNode> pathNodes = new List<TNavNode>();

    private static void Reset()
    {
        foreach (TNavNode node in navNodes)
        {
            node.Reset();
        }

        pathNodes.Clear();
    }

    public static bool CalculatePath(Vector3 sourcePosition, Vector3 targetPosition, List<Vector3> outPath)
    {
        outPath.Clear();

        TNavNode sourceNode = GetNode(sourcePosition);
        TNavNode targetNode = GetNode(targetPosition);
        if (sourceNode != null && targetNode != null)
        {
            if (sourceNode == targetNode)
            {
                outPath.Add(sourcePosition);
                outPath.Add(targetPosition);
            }
            else
            {
                TNavMesh.Reset();
                sourceNode.position = sourcePosition;
                targetNode.position = targetPosition;
                if (TNavMesh.finder.FindPath(sourceNode, targetNode, TNavMesh.pathNodes))
                {
                    var portals = CalculatePortals(targetPosition);
                    Funnel.StringPull(sourcePosition, targetPosition, portals, outPath);
                }
            }
        }

        return false;
    }

    private static List<Vector3> navPortals = new List<Vector3>();
    private static List<Vector3> CalculatePortals(Vector3 target)
    {
        navPortals.Clear();
        for (int i = 0; i < pathNodes.Count - 1; i++)
        {
            TNavNode node0 = pathNodes[i];
            TNavNode node1 = pathNodes[i+1];

            int left = -1;
            int right = -1;
            node0.CalculatePortal(node1, out left, out right);

            navPortals.Add(TNavMesh.vertices[left]);
            navPortals.Add(TNavMesh.vertices[right]);
        }

        navPortals.Add(target);
        navPortals.Add(target);

        return navPortals;
    }

    private static Vector3 CalculateCentroid(TNavNode node)
    {
        Vector3 v0 = vertices[node.index0];
        Vector3 v1 = vertices[node.index1];
        Vector3 v2 = vertices[node.index2];

        return (v0 + v1 + v2) / 3;
    }

    private static TNavNode GetNode(Vector3 pos)
    {
        Ray ray = new Ray(pos, new Vector3(0, -1, 0));
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            if (hit.triangleIndex < navNodes.Count)
                return navNodes[hit.triangleIndex];
        }

        return null;
    }

    //for debug
    public static List<Vector3> GetDebugPathNode()
    {
        List<Vector3> nodes = new List<Vector3>();
        foreach (var node in pathNodes)
        {
            nodes.Add(vertices[node.index0]);
            nodes.Add(vertices[node.index1]);

            nodes.Add(vertices[node.index1]);
            nodes.Add(vertices[node.index2]);

            nodes.Add(vertices[node.index2]);
            nodes.Add(vertices[node.index0]);
        }

        return nodes;
    }

    public static List<Vector3> GetDebugPortal()
    {
        return navPortals;
    }
}