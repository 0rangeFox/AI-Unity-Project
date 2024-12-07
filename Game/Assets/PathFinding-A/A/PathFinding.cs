using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PathFinding : MonoBehaviour
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

    void FindPath(Vector3 startPos, Vector3 endPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(endPos);

        Heap<Node> free = new Heap<Node>(grid.MaxSize);
        HashSet<Node> visited = new HashSet<Node>();

        free.Add(startNode);

        int iterationCount = 0;

        while (free.Count > 0)
        {
            iterationCount++;

            Node currentNode = free.RemoveFirst();
            //for (int i = 1; i < free.Count; i++)
            //{
            //    if (free[i].fCost < currentNode.fCost || free[i].fCost == currentNode.fCost && free[i].hCost < currentNode.hCost)
            //    {
            //        currentNode = free[i];
            //    }
            //}

            //free.Remove(currentNode);
            visited.Add(currentNode);

            if (currentNode == targetNode)
            {
                sw.Stop();
                Debug.Log($"[A*] Total time: {sw.ElapsedMilliseconds}ms | Total Cycles: {iterationCount}");
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || visited.Contains(neighbour))
                {
                    continue;
                }

                int costToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (costToNeighbour < neighbour.gCost || !free.Contains(neighbour))
                {
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

        print("Path not found. Number of cycles: " + iterationCount); // Log caso o caminho n�o seja encontrado
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
            currentNode = currentNode.parent;
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
}
