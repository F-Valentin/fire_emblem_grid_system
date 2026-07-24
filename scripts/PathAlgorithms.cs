using System.Collections.Generic;
using Godot;
using System.Numerics;
using System;

public struct MovementRangeResult
{
    public Dictionary<Vector2I, int> Costs;
    public Dictionary<Vector2I, Vector2I> CameFrom;
}

public record PathStep(Vector2I Pos, int Cost);

public class PathAlgorithms
{
    static public MovementRangeResult DijkstraMovementRange(
        Dictionary<Vector2I, CellData> grid,
        Vector2I start,
        int moveBudget,
        Func<Dictionary<Vector2I, CellData>, Vector2I, bool> isAccessible = null
    )
    {
        Dictionary<Vector2I, int> costs = new() { [start] = 0 };
        Dictionary<Vector2I, Vector2I> cameFrom = new() { [start] = start };
        PriorityQueue<Vector2I, int> frontier = new();


        isAccessible ??= (g, pos) =>
            g.TryGetValue(pos, out var cell) && cell.TerrainType.IsAccessible;

        frontier.Enqueue(start, 0);

        while (frontier.Count > 0)
        {
            frontier.TryDequeue(out Vector2I currentPos, out int currentCost);


            foreach (Vector2I neighbor in GridBuilder.GetNeighbors(currentPos))
            {
                if (!isAccessible(grid, neighbor))
                    continue;

                int newCost = currentCost + grid[neighbor].TerrainType.MoveCost;

                if (newCost > moveBudget)
                    continue;
                    
                bool neverVisited = !costs.TryGetValue(neighbor, out int existingCost);
                bool foundCheaperPath = newCost < existingCost;

                if (neverVisited || foundCheaperPath)
                {
                    costs[neighbor] = newCost;
                    frontier.Enqueue(neighbor, newCost);
                }
            }
        }

        return new MovementRangeResult { Costs = costs, CameFrom = cameFrom };
    }

    static public List<List<PathStep>> ReconstructAllOptimalPaths(
        Vector2I start,
        Vector2I target,
        Dictionary<Vector2I, int> costs,
        Dictionary<Vector2I, CellData> grid
    )
    {
        var results = new List<List<PathStep>>();

        if (!costs.ContainsKey(target))
            return results;

        void Backtrack(Vector2I current, Stack<PathStep> path)
        {
            path.Push(new PathStep(current, costs[current]));

            if (current == start)
            {
                results.Add([.. path]);
            }
            else
            {
                int expectedPredCost = costs[current] - grid[current].TerrainType.MoveCost;

                foreach (Vector2I neighbor in GridBuilder.GetNeighbors(current))
                {
                    if (costs.TryGetValue(neighbor, out int neighborCost) && neighborCost == expectedPredCost)
                        Backtrack(neighbor, path);
                }
            }

            path.Pop();
        }

        Backtrack(target, new Stack<PathStep>());
        return results;
    }
}
