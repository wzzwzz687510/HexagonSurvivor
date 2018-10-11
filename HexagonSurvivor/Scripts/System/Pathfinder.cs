namespace HexagonSurvivor
{
    using System;
    using System.Collections.Generic;
    using Priority_Queue;

    public interface WeightedGraph<L>
    {
        float Cost(Location a, Location b);
        IEnumerable<Location> Neighbors(Location id);
    }

    public struct Location
    {
        // Implementation notes: I am using the default Equals but it can
        // be slow. You'll probably want to override both Equals and
        // GetHashCode in a real project.

        public readonly int x, y;
        public Location(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Location(float x,float y)
        {
            this.x = (int)x;
            this.y = (int)y;
        }

        public Location(HexCoordinate hex)
        {
            this.x = hex.col;
            this.y = hex.row;
        }

        public override bool Equals(object obj)
        {
            if (this.GetHashCode() != obj.GetHashCode())
                return false;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return ("(" + x + "," + y + ")").GetStableHashCode();
        }
    }

    public class HexGrid : WeightedGraph<Location>
    {
        public HashSet<Location> walls = new HashSet<Location>();

        public HexGrid()
        {
            foreach (var item in SystemManager._instance.mapGenerator.dirGridEntity.Values)
            {
                if (item.isBlocked)
                    walls.Add(new Location(item.hex));
            }

            foreach (var item in SystemManager._instance.mapGenerator.emptyGrid)
            {
                walls.Add(new Location(item));
            }
        }

        public bool Passable(Location id)
        {
            return !walls.Contains(id);
        }

        public float Cost(Location a, Location b)
        {
            GridEntity gridEntity;
            SystemManager._instance.mapGenerator.dirGridEntity.TryGetValue(new HexCoordinate(b.x,b.y),out gridEntity);
            return gridEntity.gridElement.cost;
        }

        public IEnumerable<Location> Neighbors(Location id)
        {
            GridEntity gridEntity;
            SystemManager._instance.mapGenerator.dirGridEntity.TryGetValue(new HexCoordinate(id.x, id.y), out gridEntity);
            for (int i = 0; i < 6; i++)
            {
                Location next = new Location(GridUtils.HexNeighbor(gridEntity.hex, i));
                if (Passable(next))
                {
                    yield return next;
                }
            }
        }
    }

    public class AStarSearch
    {
        public Dictionary<Location, Location> cameFrom
            = new Dictionary<Location, Location>();
        public Dictionary<Location, float> costSoFar
            = new Dictionary<Location, float>();

        private Location start, goal;

        // Note: a generic version of A* would abstract over Location and
        // also Heuristic
        static public float Heuristic(Location a, Location b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
        }

        public AStarSearch(WeightedGraph<Location> graph, Location start, Location goal)
        {
            this.start = start;
            this.goal = goal;
            var frontier = new SimplePriorityQueue<Location>();
            frontier.Enqueue(start, 0);

            cameFrom[start] = start;
            costSoFar[start] = 0;

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();

                if (current.Equals(goal))
                {
                    break;
                }

                foreach (var next in graph.Neighbors(current))
                {
                    float newCost = costSoFar[current]
                        + graph.Cost(current, next);
                    if (!costSoFar.ContainsKey(next)
                        || newCost < costSoFar[next])
                    {
                        costSoFar[next] = newCost;
                        float priority = newCost + Heuristic(next, goal);
                        frontier.Enqueue(next, priority);
                        cameFrom[next] = current;
                    }
                }
            }
        }

        public Stack<HexCoordinate> path
        {
            get
            {
                Stack<HexCoordinate> hex = new Stack<HexCoordinate>();
                Location temp = goal;
                while (true)
                {
                    hex.Push(new HexCoordinate(temp.x, temp.y));
                    temp = cameFrom[temp];
                    if (temp.Equals(start))
                        break;
                }
                return hex;
            }
        }
    }
}