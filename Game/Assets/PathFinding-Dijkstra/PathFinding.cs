using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Diagnostics;

public class PathFindingDijkstra : MonoBehaviour
{
    public Transform seeker, target;

    Grid grid;
    public List<Node> path = new List<Node>();

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FindPath(seeker.position, target.position);

            if (path != null && path.Count > 0)
            {
                UnityEngine.Debug.Log("Path finded.");
                StopAllCoroutines();
                StartCoroutine(MoveSeekerAlongPath());
            }
        }
    }

    IEnumerator MoveSeekerAlongPath()
    {
        float moveSpeed = 5f;
        float heightOffset = 1.0f;

        foreach (Node node in path)
        {
            Vector3 targetPosition = node.worldPosition + Vector3.up * heightOffset;

            while (Vector3.Distance(seeker.position, targetPosition) > 0.1f)
            {
                seeker.position = Vector3.MoveTowards(seeker.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }

        Debug.Log("Seeker arrived to the target!");
        path.Clear(); 
    }


    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = previousNode[currentNode];
        }
        path.Reverse();

        grid.path = path;
        this.path = path;
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

    void FindPath(Vector3 startPos, Vector3 endPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node endNode = grid.NodeFromWorldPoint(endPos);

        Dictionary<Node, int> distances = new Dictionary<Node, int>();
        Dictionary<Node, Node> previousNodes = new Dictionary<Node, Node>();

        Heap<Node> free = new Heap<Node>(grid.MaxSize);
        HashSet<Node> visited = new HashSet<Node>();

        foreach (Node n in grid.GetAllNodes())
        {
            distances[node] = int.MaxValue;
        }

        distances[startNode] = 0;
        free.Add(startNode);

        int iterationCount = 0;

        while (free.Count > 0)
        {
            iterationCount++;

            Node currentNode = free.RemoveFirst();

            if (currentNode == targetNode)
            {
                sw.Stop();
                Debug.Log("Path found: " + sw.ElapsedMilliseconds + "ms");
                Debug.Log("Number of cycles: " + iterationCount);
                RetracePath(startNode, targetNode);
                return;
            }

            visited.Add(currentNode);

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {

                if (!neighbour.walkable || visited.Contains(neighbour))
                {
                    continue;
                }

                int newDistance = distances[currentNode] + GetDistance(currentNode, neighbour);

                if (newDistance < distances[neighbor])
                {
                    distances[neighbour] = newDistance;
                    previousNodes[neighbour] = currentNode;

                    if (!free.Contains(neighbour))
                    {
                        free.Add(neighbour);
                    }
                }
            }
        }

        Debug.Log("Path not found. Number of cycles: " + iterationCount); 
    }
}