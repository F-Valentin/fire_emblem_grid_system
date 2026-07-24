using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class GridBuilder
{
    static public Dictionary<Vector2I, CellData> BuildGridFromLayers(TileMapLayer[] layers)
    {
        Dictionary<Vector2I, CellData> grid = [];

        foreach (TileMapLayer layer in layers)
        {
            foreach (Vector2I coord in layer.GetUsedCells())
            {
                TileData tileData = layer.GetCellTileData(coord);

                // if the layer name doesn't exist godot will throw an error.
                string terrainID = (string)tileData.GetCustomData("terrain_id");

                if (terrainID == "")
                    continue;

                TerrainType terrainType = TerrainDB.GetTerrainType(terrainID);

                if (
                    grid.TryGetValue(coord, out CellData cellData)
                    && !cellData.TerrainType.IsAccessible
                )
                    continue;

                grid[coord] = new CellData(terrainType);
            }
        }

        return grid;
    }

    // Debug helper: prints the grid to the Godot output console as a simple
    // ASCII map, one character per cell (first letter of the terrain Id,
    // "." for an empty/unused cell).
    /*
    How it works:
    It finds the min/max X and Y among the grid's keys, then walks row by row printing one character per cell via GD.Print — the uppercase first letter of the terrain's Id (e.g. G for grass, S for stone), or . for a coordinate with no cell.
    Output shows up in Godot's Output panel when you run the scene.

    Exemple: 

    So concretely, if your grid has cells at (2,5), (4,1), (0,3):
    
    minX = grid.Keys.Min(c => c.X); // 0 smallest X
    maxX = grid.Keys.Max(c => c.X); // 4 largest X
    minY = grid.Keys.Min(c => c.Y); // 1 smallest Y
    maxY = grid.Keys.Max(c => c.Y); // 5 largest Y

    it allow us to find the start and end of each row and col.
    */
    static public void PrintGrid(Dictionary<Vector2I, CellData> grid)
    {
        if (grid.Count == 0)
        {
            GD.Print("Grid is empty.");
            return;
        }


        int minX = grid.Keys.Min(c => c.X);
        int maxX = grid.Keys.Max(c => c.X);
        int minY = grid.Keys.Min(c => c.Y);
        int maxY = grid.Keys.Max(c => c.Y);

        for (int y = minY; y <= maxY; y++)
        {
            string row = "";

            for (int x = minX; x <= maxX; x++)
            {
                if (grid.TryGetValue(new Vector2I(x, y), out CellData cell))
                {
                    string id = cell.TerrainType.Id.ToString();
                    row += id.Length > 0 ? char.ToUpper(id[0]) : '?';
                }
                else
                {
                    row += '.';
                }

                row += ' ';
            }

            GD.Print(row);
        }
    }
    
    static public Vector2I[] GetNeighbors(Vector2I position)
    {
        var left = new Vector2I(-1, 0) + position;
        var right = new Vector2I(1, 0) + position;
        var up = new Vector2I(0, 1) + position;
        var down = new Vector2I(0, -1) + position;

        return [
            left,
            right,
            up,
            down
        ];
    }

}
