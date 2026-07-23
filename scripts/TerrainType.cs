using Godot;
using System;

[GlobalClass]
public partial class TerrainType : Resource
{
    [Export]
    public StringName Id { get; set; } = "";

    [Export]
    public int MoveCost { get; set; } = 0;

    [Export]
    public bool IsAccessible { get; set; } = true;
}
