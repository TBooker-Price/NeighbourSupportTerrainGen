using UnityEngine;

// We are using cubic representation of a hex grid as in http://www.redblobgames.com/grids/hexagons/
// Convention: Coords are axial with 1st entry x (in cubic, the "column") 2nd entry z (in cubic, the "row")
// (So "Coords.y" would actually give the z component of the cubic representation, not the y component)

/// <summary>
/// Hex objects form part of the terrain. Stores coordinates and the landscape type.
/// </summary>
public class Hex
{
    public Vector2Int Coords { get { return new Vector2Int(X, Z); } } // Axial coords ((x,z) of the cubic coords)
    public int X { get; private set; }
    public int Y { get { return -(X + Z); } }
    public int Z { get; private set; }
    public Vector3Int CubicCoords { get { return new Vector3Int(X, Y, Z); } }

    public LandscapeData LandscapeType { get; private set; }


    public Hex(Vector2Int coords, LandscapeData landscapeType)
    {
        X = coords[0]; // "column"
        Z = coords[1]; // "row"

        LandscapeType = landscapeType;
    }

    public void SetLandscapeData(LandscapeData landscapeType)
    {
        LandscapeType = landscapeType;
    }

    public int DistanceFromOrigin()
    {
        return Mathf.Max(Mathf.Abs(X), Mathf.Abs(Y), Mathf.Abs(Z));
    }
}
