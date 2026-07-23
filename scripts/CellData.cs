using System;

public class CellData 
{
    public TerrainType TerrainType { get; set; }

    public CellData(TerrainType terrainType)
    {
        TerrainType = terrainType;
    }
}
