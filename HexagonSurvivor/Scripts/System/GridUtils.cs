using System.Collections.Generic;
using UnityEngine;

namespace HexagonSurvivor
{
    public struct CubeCoordinate
    {
        public int x, y, z;
        public CubeCoordinate(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public struct HexCoordinate
    {
        public int col, row;
        public HexCoordinate(int col, int row)
        {
            this.col = col;
            this.row = row;
        }

        public HexCoordinate(Vector2 v2)
        {
            this.col = (int)v2.x;
            this.row = (int)v2.y;
        }
    }

    public class GridUtils
    {
        public static Vector2[][] hexDirections = { new Vector2[] { new Vector2( 1, 0), new Vector2( 0, -1), new Vector2(-1, -1),
                                                                    new Vector2(-1, 0), new Vector2(-1,  1), new Vector2( 0,  1)},
                                                    new Vector2[] { new Vector2( 1, 0), new Vector2( 1, -1), new Vector2( 0, -1),
                                                                    new Vector2(-1, 0), new Vector2( 0,  1), new Vector2( 1,  1)}};

        public enum HexDirection
        {
            Right = 0,
            BottomRight = 1,
            BottomLeft = 2,
            Left = 3,
            TopLeft = 4,
            TopRight = 5
        }

        static HexCoordinate CubeCoordinate2HexCoordinate(CubeCoordinate cube)
        {
            var col = cube.x + (cube.z - (cube.z & 1)) / 2;
            var row = cube.z;
            return new HexCoordinate(col, row);
        }

        static CubeCoordinate HexCoordinate2CubeCoordinate(HexCoordinate hex)
        {
            var x = hex.col - (hex.row - (hex.row & 1)) / 2;
            var z = hex.row;
            var y = -x - z;
            return new CubeCoordinate(x, y, z);
        }

        public static HexCoordinate HexNeighbor(HexCoordinate hex,int direction)
        {
            var parity = hex.row & 1;
            var dir = hexDirections[parity][direction];
            return new HexCoordinate(hex.col + (int)dir[0], hex.row + (int)dir[1]);
        }

        static HexCoordinate HexAdd(HexCoordinate hex, int direction,int radius)
        {
            HexCoordinate result = hex;
            for (int i = 0; i < radius; i++)
            {
                result = HexNeighbor(result, direction);
            }
            return result;
        }

        static int CubeDistance(CubeCoordinate a, CubeCoordinate b)
        {
            return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2;
        }

        public static int HexDistance(HexCoordinate a, HexCoordinate b)
        {
            var ac = HexCoordinate2CubeCoordinate(a);
            var bc = HexCoordinate2CubeCoordinate(b);
            return CubeDistance(ac, bc);
        }

        static float Lerp(int a,int b,float t)
        {
            return a + (b - a) * t;
        }

        static CubeCoordinate CubeRound(float x, float y, float z)
        {
            var rx = Mathf.RoundToInt(x);
            var ry = Mathf.RoundToInt(y);
            var rz = Mathf.RoundToInt(z);

            var xDif = Mathf.Abs(rx - x);
            var yDif = Mathf.Abs(ry - y);
            var zDif = Mathf.Abs(rz - z);

            if (xDif > yDif && xDif > zDif)
                rx = -ry - rz;
            else if (yDif > zDif)
                ry = -rx - rz;
            else
                rz = -rx - ry;

            return new CubeCoordinate(rx, ry, rz);
        }

        static CubeCoordinate CubeLerp(CubeCoordinate a, CubeCoordinate b,float t)
        {
            return CubeRound(Lerp(a.x, b.x, t), Lerp(a.y, b.y, t), Lerp(a.z, b.z, t));
        }

        public static List<HexCoordinate> HexLineDraw(HexCoordinate a, HexCoordinate b)
        {
            List<HexCoordinate> result = new List<HexCoordinate>();
            var ac = HexCoordinate2CubeCoordinate(a);
            var bc = HexCoordinate2CubeCoordinate(b);
            var num = CubeDistance(ac, bc);
            for (int i = 0; i < num; i++)
            {
                result.Add(CubeCoordinate2HexCoordinate(CubeLerp(ac, bc, 1.0f / num * i)));
            }

            return result;
        }

        public static List<HexCoordinate> HexReachable(HexCoordinate start, int movement)
        {
            List<HexCoordinate> visited = new List<HexCoordinate>();
            visited.Add(start);
            List<List<HexCoordinate>> fringes = new List<List<HexCoordinate>>();
            fringes.Add(visited);

            for (int i = 1; i < movement; i++)
            {
                fringes.Add(new List<HexCoordinate>());
                foreach (var hex in fringes[i-1])
                {
                    for (int dir = 0; dir < 6; dir++)
                    {
                        var neighbor = HexNeighbor(hex, dir);
                        GridEntity gridEntity;
                        if (!visited.Contains(neighbor) && SystemManager._instance.mapGenerator.dirGridEntity.TryGetValue(new HexCoordinate(neighbor.col, neighbor.row), out gridEntity))
                        {
                            if (gridEntity.isBlocked)
                                continue;
                            visited.Add(neighbor);
                            fringes[i].Add(neighbor);
                        }
                    }
                }
            }

            return visited;
        }

        public static List<HexCoordinate> HexRing(HexCoordinate center,int radius)
        {
            List<HexCoordinate> result = new List<HexCoordinate>();

            HexCoordinate cube = HexAdd(center, 4, radius);

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < radius; j++)
                {
                    result.Add(cube);
                    cube = HexNeighbor(cube, i);
                }
            }

            return result;
        }

        public static List<HexCoordinate> HexSpiralRings(HexCoordinate center,int radius)
        {
            List<HexCoordinate> result = new List<HexCoordinate>();
            List<HexCoordinate> temp;
            for (int i = 0; i < radius; i++)
            {
                temp = HexRing(center, i);
                foreach (var item in temp)
                {
                    result.Add(item);
                }
            }

            return result;
        }

        public static Queue<HexCoordinate> PathFinding(HexCoordinate start,HexCoordinate end)
        {
            Queue<HexCoordinate> visited = new Queue<HexCoordinate>();

            return visited;
        }
    }
}