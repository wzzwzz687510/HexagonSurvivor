namespace HexagonSurvivor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MapGenerator : MonoBehaviour
    {

        public GameObject meshPrefab;

        public int width;
        public int height;
        public int wallThresholdSize = 50;
        public int blockThresholdSize = 50;

        public int biomeSize = 3;

        public string seed;
        public bool useRandomSeed;
        public int passageWidth = 4;

        public BiomeGrid[] biomeElements;

        [Range(40, 50)]
        public int randomFillPercent;

        int[,] map;
        System.Random pseudoRandom;
        private GameObject mapParent;

        void Start()
        {
            if (biomeElements.Length == 0)
            {
                Debug.Log("[Map Generator]Please set targets of biomeElements.");
                return;
            }

            GenerateMap();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                GenerateMap();
            }
        }

        void GenerateMap()
        {
            map = new int[width, height];
            RandomFillMap();

            for (int i = 0; i < 5; i++)
            {
                SmoothMap();
            }

            ProcessMap();
            ArrangeGridType();

            //MeshGenerator meshGen = GetComponent<MeshGenerator>();
            //meshGen.GenerateMesh(mapGrids);
            GenerateMesh();
        }

        void GenerateMesh()
        {
            Destroy(mapParent);
            mapParent = new GameObject("MapHolder");

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (map[x, y] > -1)
                    {
                        GameObject go = Instantiate(meshPrefab, new Vector3((x + y % 2 * 0.5f) * 1.25f, y * 1.0875f), Quaternion.identity, mapParent.transform);
                        go.GetComponent<GridEntity>().Init(new GridElement(biomeElements[map[x, y]]), new Vector2(x, y));
                        //go.GetComponent<SpriteRenderer>().sprite = mapGrid.gridElement.image;
                    }
                }
            }
        }

        void ArrangeGridType()
        {
            for (int i = 0; i < biomeElements.Length - 1; i++)
            {
                GenerateBiome(i, biomeElements[i].GridRarity);

                for (int j = 0; j < 4; j++)
                {
                    SmoothBiome(i);
                }
            }
        }

        void GenerateBiome(int mainType, int typeProbability)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (map[x, y] == mainType)
                    {
                        Debug.Log("main type:" + mainType);
                        map[x, y] = (pseudoRandom.Next(0, 100) < 40 + typeProbability) ? mainType + 1 : mainType;
                        Debug.Log(map[x, y]);
                    }
                }
            }
        }

        void SmoothBiome(int mainType)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (map[x, y] < mainType)
                    {
                        continue;
                    }

                    for (int i = mainType; i < biomeElements.Length; i++)
                    {
                        int neighbourTiles = 0;
                        for (int neighbourX = x - 1; neighbourX <= x + 1; neighbourX++)
                        {
                            for (int neighbourY = y - 1; neighbourY <= y + 1; neighbourY++)
                            {
                                if (IsInMapRange(neighbourX, neighbourY))
                                {
                                    if (neighbourX != x || neighbourY != y)
                                    {
                                        if (map[neighbourX, neighbourY] < mainType)
                                        {
                                            continue;
                                        }
                                        neighbourTiles += map[neighbourX, neighbourY] == mainType ? 1 : 0;
                                    }
                                }
                            }
                        }

                        if (neighbourTiles > 4)
                            map[x, y] = mainType;
                        else if (neighbourTiles < 4)
                            map[x, y] = mainType + 1;
                    }
                }
            }
        }

        void ProcessMap()
        {
            List<List<Coord>> airRegions = GetRegions(-1);

            foreach (List<Coord> landRegion in airRegions)
            {
                if (landRegion.Count < wallThresholdSize)
                {
                    foreach (Coord tile in landRegion)
                    {
                        map[tile.tileX, tile.tileY] = 0;
                    }
                }
            }

            List<List<Coord>> landRegions = GetRegions(0);
            List<LandBlock> survivingLandBlock = new List<LandBlock>();

            foreach (List<Coord> landRegion in landRegions)
            {
                if (landRegion.Count < blockThresholdSize)
                {
                    foreach (Coord tile in landRegion)
                    {
                        map[tile.tileX, tile.tileY] = -1;
                    }
                }
                else
                {
                    //int imageId = pseudoRandom.Next(0, gridElements.Count);
                    //foreach (var coord in landRegion)
                    //{
                    //    mapGrids.Add(new MapGrid(coord.tileX, coord.tileY, gridElements[imageId]));
                    //}
                    survivingLandBlock.Add(new LandBlock(landRegion, map));
                }
            }
            survivingLandBlock.Sort();
            survivingLandBlock[0].isMainBlock = true;
            survivingLandBlock[0].isAccessibleFromMainBlock = true;

            ConnectClosestBlocks(survivingLandBlock);
        }

        void ConnectClosestBlocks(List<LandBlock> allBlocks, bool forceAccessibilityFromMainBlock = false)
        {

            List<LandBlock> blockListA = new List<LandBlock>();
            List<LandBlock> blockListB = new List<LandBlock>();

            if (forceAccessibilityFromMainBlock)
            {
                foreach (LandBlock block in allBlocks)
                {
                    if (block.isAccessibleFromMainBlock)
                    {
                        blockListB.Add(block);
                    }
                    else
                    {
                        blockListA.Add(block);
                    }
                }
            }
            else
            {
                blockListA = allBlocks;
                blockListB = allBlocks;
            }

            int bestDistance = 0;
            Coord bestTileA = new Coord();
            Coord bestTileB = new Coord();
            LandBlock bestBlockA = new LandBlock();
            LandBlock bestBlockB = new LandBlock();
            bool possibleConnectionFound = false;

            foreach (LandBlock blockA in blockListA)
            {
                if (!forceAccessibilityFromMainBlock)
                {
                    possibleConnectionFound = false;
                    if (blockA.connectedBlocks.Count > 0)
                    {
                        continue;
                    }
                }

                foreach (LandBlock blockB in blockListB)
                {
                    if (blockA == blockB || blockA.IsConnected(blockB))
                    {
                        continue;
                    }

                    for (int tileIndexA = 0; tileIndexA < blockA.edgeTiles.Count; tileIndexA++)
                    {
                        for (int tileIndexB = 0; tileIndexB < blockB.edgeTiles.Count; tileIndexB++)
                        {
                            Coord tileA = blockA.edgeTiles[tileIndexA];
                            Coord tileB = blockB.edgeTiles[tileIndexB];
                            int distanceBetweenBlocks = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

                            if (distanceBetweenBlocks < bestDistance || !possibleConnectionFound)
                            {
                                bestDistance = distanceBetweenBlocks;
                                possibleConnectionFound = true;
                                bestTileA = tileA;
                                bestTileB = tileB;
                                bestBlockA = blockA;
                                bestBlockB = blockB;
                            }
                        }
                    }
                }
                if (possibleConnectionFound && !forceAccessibilityFromMainBlock)
                {
                    CreatePassage(bestBlockA, bestBlockB, bestTileA, bestTileB);
                }
            }

            if (possibleConnectionFound && forceAccessibilityFromMainBlock)
            {
                CreatePassage(bestBlockA, bestBlockB, bestTileA, bestTileB);
                ConnectClosestBlocks(allBlocks, true);
            }

            if (!forceAccessibilityFromMainBlock)
            {
                ConnectClosestBlocks(allBlocks, true);
            }
        }

        void CreatePassage(LandBlock blockA, LandBlock blockB, Coord tileA, Coord tileB)
        {
            LandBlock.ConnectBlocks(blockA, blockB);

            //Debug.DrawLine (CoordToWorldPoint (tileA), CoordToWorldPoint (tileB), Color.green, 3);

            List<Coord> line = GetLine(tileA, tileB);
            foreach (Coord c in line)
            {
                DrawCircle(c, passageWidth);
            }
        }

        void DrawCircle(Coord c, int r)
        {
            for (int x = -r; x <= r; x++)
            {
                for (int y = -r; y <= r; y++)
                {
                    if (x * x + y * y <= r * r)
                    {
                        int drawX = c.tileX + x;
                        int drawY = c.tileY + y;
                        if (IsInMapRange(drawX, drawY))
                        {
                            map[drawX, drawY] = 0;
                            //mapGrids.Add(new MapGrid(drawX, drawY, gridElements[pseudoRandom.Next(0, gridElements.Count)]));
                        }
                    }
                }
            }
        }

        List<Coord> GetLine(Coord from, Coord to)
        {
            List<Coord> line = new List<Coord>();

            int x = from.tileX;
            int y = from.tileY;

            int dx = to.tileX - from.tileX;
            int dy = to.tileY - from.tileY;

            bool inverted = false;
            int step = Math.Sign(dx);
            int gradientStep = Math.Sign(dy);

            int longest = Mathf.Abs(dx);
            int shortest = Mathf.Abs(dy);

            if (longest < shortest)
            {
                inverted = true;
                longest = Mathf.Abs(dy);
                shortest = Mathf.Abs(dx);

                step = Math.Sign(dy);
                gradientStep = Math.Sign(dx);
            }

            int gradientAccumulation = longest / 2;
            for (int i = 0; i < longest; i++)
            {
                line.Add(new Coord(x, y));

                if (inverted)
                {
                    y += step;
                }
                else
                {
                    x += step;
                }

                gradientAccumulation += shortest;
                if (gradientAccumulation >= longest)
                {
                    if (inverted)
                    {
                        x += gradientStep;
                    }
                    else
                    {
                        y += gradientStep;
                    }
                    gradientAccumulation -= longest;
                }
            }

            return line;
        }

        Vector3 CoordToWorldPoint(Coord tile)
        {
            return new Vector3((tile.tileX + tile.tileY % 2 * 0.5f) * 1.25f, tile.tileY * 1.0875f, 0);
        }

        List<List<Coord>> GetRegions(int tileType)
        {
            List<List<Coord>> regions = new List<List<Coord>>();
            int[,] mapFlags = new int[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                    {
                        List<Coord> newRegion = GetRegionTiles(x, y);
                        regions.Add(newRegion);

                        foreach (Coord tile in newRegion)
                        {
                            mapFlags[tile.tileX, tile.tileY] = 1;
                        }
                    }
                }
            }

            return regions;
        }

        List<Coord> GetRegionTiles(int startX, int startY)
        {
            List<Coord> tiles = new List<Coord>();
            int[,] mapFlags = new int[width, height];
            int tileType = map[startX, startY];

            Queue<Coord> queue = new Queue<Coord>();
            queue.Enqueue(new Coord(startX, startY));
            mapFlags[startX, startY] = 1;

            while (queue.Count > 0)
            {
                Coord tile = queue.Dequeue();
                tiles.Add(tile);

                for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                {
                    for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                    {
                        if (IsInMapRange(x, y) && (y == tile.tileY || x == tile.tileX))
                        {
                            if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                            {
                                mapFlags[x, y] = 1;
                                queue.Enqueue(new Coord(x, y));
                            }
                        }
                    }
                }
            }
            return tiles;
        }

        bool IsInMapRange(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        void RandomFillMap()
        {
            if (useRandomSeed)
            {
                seed = Time.time.ToString();
            }

            pseudoRandom = new System.Random(seed.GetHashCode());

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    {
                        map[x, y] = -1;
                    }
                    else
                    {
                        map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? -1 : 0;
                    }
                }
            }
        }

        void SmoothMap()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int neighbourWallTiles = GetSurroundingWallCount(x, y);

                    if (neighbourWallTiles > 4)
                        map[x, y] = -1;
                    else if (neighbourWallTiles < 4)
                        map[x, y] = 0;

                }
            }
        }

        int GetSurroundingWallCount(int gridX, int gridY)
        {
            int wallCount = 0;
            for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
            {
                for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
                {
                    if (IsInMapRange(neighbourX, neighbourY))
                    {
                        if (neighbourX != gridX || neighbourY != gridY)
                        {
                            wallCount -= map[neighbourX, neighbourY];
                        }
                    }
                    else
                    {
                        wallCount++;
                    }
                }
            }

            return wallCount;
        }

        struct Coord
        {
            public int tileX;
            public int tileY;

            public Coord(int x, int y)
            {
                tileX = x;
                tileY = y;
            }
        }

        class LandBlock : IComparable<LandBlock>
        {
            public List<Coord> tiles;
            public List<Coord> edgeTiles;
            public List<LandBlock> connectedBlocks;
            public int blockSize;
            public bool isAccessibleFromMainBlock;
            public bool isMainBlock;

            public LandBlock()
            {
            }

            public LandBlock(List<Coord> blockTiles, int[,] map)
            {
                tiles = blockTiles;
                blockSize = tiles.Count;
                connectedBlocks = new List<LandBlock>();

                edgeTiles = new List<Coord>();
                foreach (Coord tile in tiles)
                {
                    for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                    {
                        for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                        {
                            if (x == tile.tileX || y == tile.tileY)
                            {
                                if (map[x, y] == -1)
                                {
                                    edgeTiles.Add(tile);
                                }
                            }
                        }
                    }
                }
            }

            public void SetAccessibleFromMainBlock()
            {
                if (!isAccessibleFromMainBlock)
                {
                    isAccessibleFromMainBlock = true;
                    foreach (LandBlock connectedBlock in connectedBlocks)
                    {
                        connectedBlock.SetAccessibleFromMainBlock();
                    }
                }
            }

            public static void ConnectBlocks(LandBlock blockA, LandBlock blockB)
            {
                if (blockA.isAccessibleFromMainBlock)
                {
                    blockB.SetAccessibleFromMainBlock();
                }
                else if (blockB.isAccessibleFromMainBlock)
                {
                    blockA.SetAccessibleFromMainBlock();
                }
                blockA.connectedBlocks.Add(blockB);
                blockB.connectedBlocks.Add(blockA);
            }

            public bool IsConnected(LandBlock otherBlock)
            {
                return connectedBlocks.Contains(otherBlock);
            }

            public int CompareTo(LandBlock otherBlock)
            {
                return otherBlock.blockSize.CompareTo(blockSize);
            }
        }
    }
}