using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarFinder
{
    //todo, openset use min heap
    private PriorityQueue openSet = new PriorityQueue();


    private float HeuristicCostEstimate(Vector3 v0, Vector3 v1)
    {
        return Vector3.Distance(v0, v1);
    }

    public bool FindPath(TNavNode sourceNode, TNavNode targetNode, Vector3 sourcePos, Vector3 targetPos, List<TNavNode> path)
    {
        openSet.Clear();

        sourceNode.gScore = 0;
        sourceNode.fScore = Vector3.Distance(sourcePos, targetPos);

        openSet.Add(sourceNode);
        sourceNode.opened = true;
        sourceNode.position = sourcePos;

        while (openSet.Count != 0)
        {
            TNavNode current = this.openSet.Pop();
            if (current == targetNode)
            {
                while (current.parent != null)
                {
                    path.Add(current);
                    current = current.parent;
                }
                path.Reverse();

                return true;
            }

            current.opened = false;
            current.closed = true;

            for(int i = 0; i < current.GetNeighborCount(); i++)
            {
                TNavNode neighbor = current.GetNeighbor(i);

                if (neighbor.closed == true)
                    continue;

                if (neighbor.opened == false)
                {
                    if (neighbor == targetNode)
                        neighbor.position = targetPos;
                    else
                        neighbor.position = neighbor.centroid;
                    this.openSet.Add(neighbor);
                    neighbor.opened = true;
                }

                float gScore = current.gScore + this.HeuristicCostEstimate(current.position, neighbor.position);
                if (gScore >= neighbor.gScore)
                    continue;

                neighbor.parent = current;
                neighbor.gScore = gScore;
                neighbor.fScore = neighbor.gScore + this.HeuristicCostEstimate(neighbor.position, targetPos);

                current.CalculatePortal(i);
            }
        }

        return false;
    }
}
