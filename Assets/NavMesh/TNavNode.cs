using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNavNode
{
    private int[] vertexIndex = new int[3];

    public int index0
    {
        get { return this.vertexIndex[0]; }
    }
    public int index1
    {
        get { return this.vertexIndex[1]; }
    }
    public int index2
    {
        get { return this.vertexIndex[2]; }
    }

    public TNavNode(int index0, int index1, int index2)
    {
        vertexIndex[0] = index0;
        vertexIndex[1] = index1;
        vertexIndex[2] = index2;
    }

    public Vector3 centroid;

    private List<TNavNode> neighbors = new List<TNavNode>(3);
    private List<int> neighborEdges = new List<int>(3);

    public int GetNeighborCount()
    {
        return neighbors.Count;
    }

    public TNavNode GetNeighbor(int index)
    {
        return neighbors[index];
    }

    public bool opened = false;
    public bool closed = false;
    public float gScore = float.MaxValue;
    public float fScore = float.MaxValue;
    public TNavNode parent = null;

    //use for cost estimate
    public Vector3 position;

    public void Reset()
    {
        this.opened = false;
        this.closed = false;
        this.gScore = float.MaxValue;
        this.fScore = float.MaxValue;
        this.parent = null;

        this.position = centroid;
    }

    public void AddNeighbor(int edgeIndex, TNavNode neighbor)
    {
        this.neighbors.Add(neighbor);
        this.neighborEdges.Add(edgeIndex);
    }

    public void CalculatePortal(TNavNode node, out int left, out int right)
    {
        for(int i = 0; i < this.neighbors.Count; i++)
        {
            if(this.neighbors[i] == node)
            {
                int edgeIndex = neighborEdges[i];

                left = this.vertexIndex[edgeIndex];
                right = this.vertexIndex[(edgeIndex + 1) % 3];

                return;
            }
        }

        left = -1;
        right = -1;
    }
}