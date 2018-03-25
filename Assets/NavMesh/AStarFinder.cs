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

        sourceNode.position = sourcePos;
        targetNode.position = targetPos;

        openSet.Add(sourceNode);
        sourceNode.opened = true;

        while (openSet.Count != 0)
        {
            TNavNode current = this.openSet.Pop();
            if (current == targetNode)
            {
                path.Add(current);
                while (current.parent != null)
                {
                    current = current.parent;
                    path.Add(current);
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
                    this.openSet.Add(neighbor);
                    neighbor.opened = true;
                }

                float gScore = current.gScore + this.HeuristicCostEstimate(current.position, neighbor.position);
                if (gScore >= neighbor.gScore)
                    continue;

                neighbor.parent = current;
                neighbor.gScore = gScore;
                neighbor.fScore = neighbor.gScore + this.HeuristicCostEstimate(neighbor.position, targetPos);
            }
        }

        return false;
    }
}
