using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Player : Sprite2D
{
    [Export]
    private Node2D _tileMapLayers = null;

    private TileMapLayer[] _layers;

    Dictionary<Vector2I, CellData> grid;

    private bool _isMoving = false;


    private int moveLeft = 6;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _layers = [.. _tileMapLayers.GetChildren().OfType<TileMapLayer>()];
        Position = _layers[0].MapToLocal(new Vector2I(2, 13));
        grid = GridBuilder.BuildGridFromLayers(_layers);

        // var s = new Vector2I(2, 13);
        // var res = PathAlgorithms.DijkstraMovementRange(grid, s, 10);

        // var layer = _layers[0];

        // foreach (var key in res.CameFrom.Keys)
        // {
        //     var rect = new ColorRect
        //     {
        //         Size = new Vector2(16, 16),
        //         Color = Colors.White
        //     };

        //     GetParent().CallDeferred("add_child", rect);

        //     rect.Position = key * layer.TileSet.TileSize;
        // }
    }

    async private void Move(Vector2I target)
    {
        if (_isMoving)
            return;
            
        moveLeft = 4;
        var pos = _layers[0].LocalToMap(Position);
        var res = PathAlgorithms.DijkstraMovementRange(grid, pos, moveLeft);
        var targetToMap = _layers[0].LocalToMap(target);
        GD.Print(pos);
        GD.Print(targetToMap);
        var paths = PathAlgorithms.ReconstructAllOptimalPaths(pos, targetToMap, res.Costs, grid);
        GD.Print(paths);
        GD.Print(paths.Count);

        if (paths.Count == 0)
            return;

        _isMoving = true;
            
        foreach (Vector2I step in paths[0])
        {
            Position = _layers[0].MapToLocal(step);
            await ToSignal(GetTree().CreateTimer(0.5), "timeout");
        }
        
        _isMoving = false;
    }


    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton p && p.Pressed)
        {
            var mb = GetViewport().GetMousePosition();

            Move(new Vector2I((int)mb.X, (int)mb.Y));
        }
    }
}
