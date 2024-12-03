using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Diagnostics;
public class PathFinding : MonoBehaviour
{
    public Transform seeker, target;

    Grid grid;

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    private void Update()
    {   if (Input.GetKeyDown(KeyCode.Space))
        {
            UnityEngine.Debug.Log("Tecla espaço pressionada!");
            FindPath(seeker.position, target.position);
        }
    }

    void FindPath (Vector3 startPos, Vector3 endPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(endPos);

        List<Node> free = new List<Node>();
        HashSet<Node> visited = new HashSet<Node>();

        free.Add(startNode);

        while (free.Count > 0) {
            Node currentNode = free[0];
            for (int i = 1; i < free.Count; i++)
            {
                if (free[i].fCost < currentNode.fCost || free[i].fCost == currentNode.fCost && free[i].hCost < currentNode.hCost)
                {
                    currentNode = free[i];
                }
            }

            free.Remove(currentNode);
            visited.Add(currentNode);

            if (currentNode == targetNode) {
                sw.Stop();
                print("Path found: " + sw.ElapsedMilliseconds + "ms");
                RetracePath(startNode, targetNode);
               return;
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
                if (!neighbour.walkable || visited.Contains(neighbour))
                {
                    continue;
                }

                int costToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (costToNeighbour < neighbour.gCost || !free.Contains(neighbour)) {
                    neighbour.gCost = costToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!free.Contains(neighbour))
                    {
                        free.Add(neighbour);
                    }
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode){
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        grid.path = path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }

        return 14 * dstX + 10 * (dstY - dstX);
    }
}
