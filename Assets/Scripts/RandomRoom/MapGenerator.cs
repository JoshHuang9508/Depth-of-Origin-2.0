using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapGenerator : Manager
{
    [Header("Attributes")]
    [SerializeField] private readonly int mapSize = 5;

    [Header("Rooms")]
    [SerializeField] private List<Room> rooms;

    [Header("Boss")]
    [SerializeField] private EnemySO boss;

    [Header("References")]
    [SerializeField] private GameObject playerSpawner;
    [SerializeField] private Transform MapTrans;

    // 方向向量
    static (int dx, int dy)[] directions = { (0, 1), (1, 0), (0, -1), (-1, 0) };
    // 對應門的二進制標記 (上, 右, 下, 左)
    static int[] doorFlags = { 0b1000, 0b0100, 0b0010, 0b0001 };
    // Offset
    static (int fx, int fy) roomOffset = (12, 12);
    // Huh?
    static System.Random random = new();

    private void Start()
    {
        isSetup = false;

        // 初始地圖陣列為 0b0000，表示所有方向都沒有門
        int[,] mapGrid = new int[mapSize, mapSize];

        // 隨機選擇起點和終點
        var start = (x: random.Next(mapSize), y: random.Next(mapSize));
        var end = start;
        while (end == start) end = (x: random.Next(mapSize), y: random.Next(mapSize));

        // 生成連通迷宮
        mapGrid = GenerateMapGrid(start, mapGrid);

        // 確保起點和終點之間的連通性
        while (!IsConnected(start, end, mapGrid))
        {
            mapGrid = AddExtraDoors(mapGrid);
        }

        // 生成房間
        GenerateRooms(mapGrid);

        // 設定出生點
        SetupStartRoom(start);

        // 設定Boss房
        SetupBossRoom(end);

        isSetup = true;
    }

    // 確保座標在地圖範圍內
    private bool InBounds(int x, int y) => x >= 0 && x < mapSize && y >= 0 && y < mapSize;

    // 迷宮生成主邏輯 (Prim 算法)
    private int[,] GenerateMapGrid((int x, int y) start, int[,] mapGrid)
    {
        bool[,] visited = new bool[mapSize, mapSize];
        List<(int x, int y, int direction)> queue = new();

        // 將起始房間的座標設為已訪問
        visited[start.x, start.y] = true;

        // 將起點房間四周的房間加入queue
        for (int i = 0; i < directions.Length; i++)
        {
            var (dx, dy) = directions[i];
            int nx = start.x + dx, ny = start.y + dy;
            if (InBounds(nx, ny))
            {
                queue.Add((start.x, start.y, i));
            }
        }

        // 持續處理queue
        while (queue.Count > 0)
        {
            // 隨機從queue選擇一個房間
            int randomIndex = random.Next(queue.Count);
            var (x, y, direction) = queue[randomIndex];
            queue.RemoveAt(randomIndex);

            var (dx, dy) = directions[direction];
            int nx = x + dx, ny = y + dy;

            // 如果該房間(nx, ny)未被訪問，連通兩個房間並標記為已訪問
            if (InBounds(nx, ny) && !visited[nx, ny])
            {
                visited[nx, ny] = true;
                mapGrid[x, y] |= doorFlags[direction];
                mapGrid[nx, ny] |= doorFlags[(direction + 2) % 4]; // 二進制高光時刻

                // 將新房間四周的房間加入queue
                for (int i = 0; i < directions.Length; i++)
                {
                    var (dx2, dy2) = directions[i];
                    int nnx = nx + dx2, nny = ny + dy2;
                    if (InBounds(nnx, nny) && !visited[nnx, nny])
                    {
                        queue.Add((nx, ny, i));
                    }
                }
            }
        }
        return mapGrid;
    }

    // 檢查起點和終點是否連通 (BFS)
    private bool IsConnected((int x, int y) start, (int x, int y) end, int[,] mapGrid)
    {
        bool[,] visited = new bool[mapSize, mapSize];
        Queue<(int x, int y)> queue = new Queue<(int x, int y)>();
        queue.Enqueue(start);
        visited[start.x, start.y] = true;

        while (queue.Count > 0)
        {
            var (x, y) = queue.Dequeue();

            if ((x, y) == end)
            {
                return true;
            }

            for (int i = 0; i < directions.Length; i++)
            {
                var (dx, dy) = directions[i];
                int nx = x + dx, ny = y + dy;

                if (InBounds(nx, ny) && !visited[nx, ny])
                {
                    if ((mapGrid[x, y] & doorFlags[i]) != 0) // 檢查是否有門
                    {
                        visited[nx, ny] = true;
                        queue.Enqueue((nx, ny));
                    }
                }
            }
        }
        return false;
    }

    // 隨機增加額外的門
    private int[,] AddExtraDoors(int[,] mapGrid)
    {
        for (int _ = 0; _ < mapSize; _++)
        {
            int x = random.Next(mapSize);
            int y = random.Next(mapSize);
            int direction = random.Next(4);

            var (dx, dy) = directions[direction];
            int nx = x + dx, ny = y + dy;

            if (InBounds(nx, ny))
            {
                mapGrid[x, y] |= doorFlags[direction];
                mapGrid[nx, ny] |= doorFlags[(direction + 2) % 4];
            }
        }
        return mapGrid;
    }

    // 生成房間
    private void GenerateRooms(int[,] mapGrid)
    {
        Dictionary<int, List<Room>> roomsWithIndex = new(16);

        for (int i = 0; i < 16; i++)
        {
            roomsWithIndex[i] = new List<Room>();
        }

        foreach (Room room in rooms)
        {
            roomsWithIndex[room.index].Add(room);
        }

        for (int y = mapSize - 1; y >= 0; y--)
        {
            for (int x = 0; x < mapSize; x++)
            {
                int roomIndex = mapGrid[x, y];
                Room room = roomsWithIndex[roomIndex][random.Next(roomsWithIndex[roomIndex].Count)];

                Instantiate(room.gameObject, new Vector3(x * roomOffset.fx, y * roomOffset.fy), Quaternion.identity, MapTrans);
            }
        }
    }

    private void SetupStartRoom((int x, int y) start)
    {
        Instantiate(playerSpawner, new Vector3(start.x * roomOffset.fx, start.y * roomOffset.fy), Quaternion.identity, transform.parent);
    }

    private void SetupBossRoom((int x, int y) end)
    {
        Spawner.SpawnMob(new Vector3(end.x * roomOffset.fx, end.y * roomOffset.fy), boss);
    }
}


