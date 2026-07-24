using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Level1 : Node2D
{
    [Export]
    private Node2D _tileMapLayers = null;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        TileMapLayer[] layers = [.. _tileMapLayers.GetChildren().OfType<TileMapLayer>()];

        Dictionary<Vector2I, CellData> grid = GridBuilder.BuildGridFromLayers(layers);

        // GridBuilder.PrintGrid(grid);
        var s = new Vector2I(2, 13);
        var res = PathAlgorithms.DijkstraMovementRange(grid, s, 10);

        var layer = layers[0];

        foreach (var key in res.CameFrom.Keys)
        {
            var rect = new ColorRect
            {
                Size = new Vector2(16, 16),
                Color = Colors.White
            };

            AddChild(rect);

            rect.Position = key * layer.TileSet.TileSize;
        }

        var t = new Vector2I(5, 10);
        var paths = PathAlgorithms.ReconstructAllOptimalPaths(s, t, res.Costs, grid);


        foreach (var path in paths)
        {
            foreach (PathStep step in path)
            {
                var rect = new ColorRect
                {
                    Size = new Vector2(16, 16),
                    Color = Colors.Red
                };

                AddChild(rect);

                rect.Position = step.Pos * layer.TileSet.TileSize;
            }
        }
    }
}
