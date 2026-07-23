using Godot;
using System;
using System.Collections.Generic;

public partial class TerrainDB : Node
{
    private static readonly Dictionary<string, TerrainType> _cache = [];

    static public TerrainType GetTerrainType(StringName id)
    {
        if (!_cache.ContainsKey(id))
        {
            switch (id)
            {
                case "grass": _cache[id] = GD.Load<TerrainType>("resources/terrain_types/grass.tres"); break;
                case "bush": _cache[id] = GD.Load<TerrainType>("resources/terrain_types/bush.tres"); break;
                case "stone": _cache[id] = GD.Load<TerrainType>("resources/terrain_types/stone.tres"); break;
                case "slab": _cache[id] = GD.Load<TerrainType>("resources/terrain_types/slab.tres"); break;
                default: GD.PushError($"Terrain Type ({id}) is unknow."); break;
            }
        }

        // if the type is 'unknow' the code will crash because of _cache[unknow] is not handle.
        return _cache[id];
    }
}
