using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarFinder
{
    private TNavNodeQueue openSet = new TNavNodeQueue();

    private float HeuristicCostEstimate(Vector3 v0, Vector3 v1)
    {
        return Vector3.Distance(v0, v1);
    }

    public bool FindPath(TNavNode sourceNode, TNavNode targetNode, List<TNavNode> path)
    {
        openSet.Clear();

        sourceNode.gScore = 0;
        sourceNode.fScore = Vector3.Distance(sourceNode.position, targetNode.position);
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


                Vector3 portalLeft, portalRight;
                current.GetProtal(i, out portalLeft, out portalRight);

                //http://digestingduck.blogspot.fi/2010/05/towards-better-navmesh-path-planning.html
                //http://digestingduck.blogspot.fi/2010/08/visibility-optimized-graph-experiment.html
                //if (TNavMeshUtility.isectSegSeg(current.position, targetNode.position, portalLeft, portalRight, ref neighbor.position) == false)
                //{
                //    Vector3 newLeft = portalLeft + (portalRight - portalLeft) * 0.1f;
                //    Vector3 newRight = portalRight + (portalLeft - portalRight) * 0.1f;
                //    float leftDistance = (targetNode.position - newLeft).sqrMagnitude;
                //    float rightDistance = (targetNode.position - newRight).sqrMagnitude;
                //    if (leftDistance < rightDistance)
                //        neighbor.position = newLeft;
                //    else
                //        neighbor.position = newRight;
                //}

                //neighbor.position = current.position在potal的投影
                //看起来和上面的方法好像差不多......
                Vector3 edge = portalRight - portalLeft;
                float edgeLengthSquare = edge.sqrMagnitude;
                if (edgeLengthSquare > 0.0001f)
                {
                    Vector3 v = current.position - portalLeft;
                    float pdot = Vector3.Dot(v, edge) / edgeLengthSquare;
                    pdot = Mathf.Clamp(pdot, 0.05f, 0.95f);
                    neighbor.position = portalLeft + pdot * edge;
                }
                else
                {
                    neighbor.position = portalLeft;
                }

                float gScore = current.gScore + this.HeuristicCostEstimate(current.position, neighbor.position);
                if (gScore >= neighbor.gScore)//neighbor.opened == true的话,neighbor.gCost=MaxValue，gCost一定小于neighbor.gCost
                    continue;

                neighbor.parent = current;
                neighbor.gScore = gScore;
                neighbor.fScore = neighbor.gScore + this.HeuristicCostEstimate(neighbor.position, targetNode.position);

                if (neighbor.opened == false)
                {
                    this.openSet.Add(neighbor);
                    neighbor.opened = true;
                }
                else
                {
                }
            }
        }

        return false;
    }
}
