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

        GridBuilder.PrintGrid(grid);
    }
}
