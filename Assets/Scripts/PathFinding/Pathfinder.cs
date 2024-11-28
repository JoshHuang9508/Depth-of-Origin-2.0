using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public class Node
    {
        public Vector2 Position; // 節點位置
        public Node Parent; // 父節點，用於回溯路徑
        public float G; // 起點到當前節點的實際代價
        public float H; // 當前節點到終點的啟發式估計值
        public float F => G + H; // 總代價

        public Node(Vector2 position, Node parent = null)
        {
            Position = position;
            Parent = parent;
            G = 0;
            H = 0;
        }
    }

    // 地圖大小和障礙物
    static float gridSize = 0.25f;
    static private string obstacleTag = "Wall";
    static private List<Vector2> directions = new()
    {
        new Vector2(0, gridSize),    // 上
        new Vector2(gridSize, 0),    // 右
        new Vector2(0, -gridSize),   // 下
        new Vector2(-gridSize, 0),   // 左
        new Vector2(-gridSize, gridSize),   // 左上
        new Vector2(gridSize, gridSize),    // 右上
        new Vector2(-gridSize, -gridSize),  // 左下
        new Vector2(gridSize, -gridSize)    // 右下
    };

    // void OnDrawGizmos()
    // {
    //     // 繪製搜尋範圍
    //     Gizmos.color = Color.blue; // 開放列表
    //     foreach (var pos in openListVisual)
    //     {
    //         Gizmos.DrawSphere(pos, 0.2f);
    //     }

    //     Gizmos.color = Color.red; // 封閉列表
    //     foreach (var pos in closedListVisual)
    //     {
    //         Gizmos.DrawCube(pos, Vector3.one * 0.2f);
    //     }
    // }

    static public List<Vector2> FindPath(Vector2 start, Vector2 end, float maxDistance = 10, float hitBoxRadius = 0.5f)
    {
        List<Node> openList = new(); // 開放列表
        HashSet<Vector2> closedList = new(); // 封閉列表

        Debug.Log("Start: " + start);
        Debug.Log("End: " + end);

        // 確保起點和終點是gridSize的倍數
        start = new Vector2(Mathf.Round(start.x / gridSize) * gridSize, Mathf.Round(start.y / gridSize) * gridSize);
        end = new Vector2(Mathf.Round(end.x / gridSize) * gridSize, Mathf.Round(end.y / gridSize) * gridSize);

        Node startNode = new(start);
        Node endNode = new(end);
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            // 找到 F 值最小的節點
            Node currentNode = openList[0];
            foreach (var node in openList)
            {
                if (node.F < currentNode.F)
                {
                    currentNode = node;
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode.Position);

            // 如果到達終點(範圍內)，回溯路徑
            if (currentNode.Position == endNode.Position || Vector2.Distance(currentNode.Position, endNode.Position) < 0.5f)
            {
                List<Vector2> path = new();
                while (currentNode != null)
                {
                    path.Add(currentNode.Position);
                    currentNode = currentNode.Parent;
                }
                path.Reverse();

                // 繪製最終路徑
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Debug.DrawLine(path[i], path[i + 1], Color.green, 1);
                }

                return path;
            }

            // 超出最大距離則停止搜尋
            if (Vector2.Distance(currentNode.Position, start) > maxDistance)
            {
                Debug.LogWarning("Search exceeded maximum distance.");
                return null;
            }

            // 遍歷鄰居節點
            foreach (var direction in directions)
            {
                Vector2 neighborPos = currentNode.Position + direction;

                Collider2D collider = Physics2D.OverlapCircle(neighborPos, hitBoxRadius);

                // 如果該節點是障礙物，跳過
                if (collider != null && collider.CompareTag(obstacleTag)) continue;

                // 如果該節點在封閉列表中，跳過
                if (closedList.Contains(neighborPos)) continue;

                // 計算鄰居的 G, H 和 F 值
                Node neighborNode = new(neighborPos, currentNode)
                {
                    G = currentNode.G + 1,
                    H = Mathf.Abs(neighborPos.x - end.x) + Mathf.Abs(neighborPos.y - end.y)
                };

                // 如果鄰居在開放列表中且新的 G 值更大，跳過
                Node existingNode = openList.Find(node => node.Position == neighborPos);

                if (existingNode != null && neighborNode.G >= existingNode.G) continue;

                // 加入開放列表
                if (existingNode == null)
                {
                    openList.Add(neighborNode);
                }
                else
                {
                    existingNode.Parent = currentNode;
                    existingNode.G = neighborNode.G;
                }

                // 繪製鄰居節點
                Debug.DrawLine(currentNode.Position, neighborPos, Color.yellow, 1);
            }
        }
        // 無法找到路徑
        return null;
    }
}
