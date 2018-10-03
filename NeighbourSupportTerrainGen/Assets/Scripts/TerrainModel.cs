using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Stores the landscape tiles in the terrain (as a graph). Contains methods for choosing terrain tiles based on neighbours, etc.
/// </summary>
public class TerrainModel : IGraph<Hex>
{
    private static List<Vector2Int> boardCoords; // a list of coordinates that correspond to all the tile positions on the board
    private static Dictionary<Vector2Int, Hex> coordsToHex;
    private static readonly Vector2Int[] neighbourDirs = new Vector2Int[]
    { // recall this is (x,z) from the cubic coords
        new Vector2Int(-1,1), // down left
        new Vector2Int(-1,0), // left
        new Vector2Int(0,-1), // up left
        new Vector2Int(1,-1), // up right
        new Vector2Int(1,0), // right
        new Vector2Int(0,1), // down right
    };

    public const int RADIUS = 10; // number of concentric rings around the central tile


    public TerrainModel()
    {
        GenerateBoardCoordinates();
        InitialiseTerrain();
    }

    public IEnumerable<Hex> Neighbors(Hex node)
    {
        foreach (var dir in neighbourDirs)
        {
            Vector2Int hexCoords = node.Coords;
            Vector2Int nextCoords = hexCoords + dir;

            // skip this hex tile if its coordinates would be out of bounds
            if (!CoordsAreInBounds(nextCoords)) continue;

            Hex nextHex = coordsToHex[nextCoords];
            yield return nextHex;
        }
    }

    public Hex this[Vector2Int coords] { get { return coordsToHex[coords]; } }

    private bool CoordsAreInBounds(Vector2Int coords)
    {
        return coords.HexDistFromOrigin() <= RADIUS;
    }

    /// <summary>
    /// Sets the boardCoords field to a list of relevant coordinates that can be iterated over.
    /// </summary>
    private void GenerateBoardCoordinates()
    {
        boardCoords = new List<Vector2Int>();
        for (int row = -RADIUS; row < (RADIUS + 1); row++)
        {
            for (int column = -RADIUS; column < (RADIUS + 1); column++)
            {
                if (Mathf.Abs(row + column) <= RADIUS) boardCoords.Add(new Vector2Int(row, column));
            }
        }
    }

    /// <summary>
    /// Sets the "seed" for the terrain by setting the hex tiles to have random landscape types.
    /// </summary>
    public void InitialiseTerrain()
    {
        coordsToHex = new Dictionary<Vector2Int, Hex>();
        foreach (Vector2Int coords in boardCoords)
        {
            LandscapeData landscape = LandscapeData.ChooseRandomLandscape();

            Hex newHex = new Hex(coords, landscape);
            coordsToHex[coords] = newHex;
        }

        SmoothTerrainUsingInfluence();
    }

    /// <summary>
    /// Goes over the board and reselects each landscape type using the influence of neighbouring hexes' landscape types.
    /// </summary>
    public void SmoothTerrainUsingInfluence()
    {
        foreach (Vector2Int coords in boardCoords)
        {
            IEnumerable<LandscapeData> neighbourScapes = Neighbors(coordsToHex[coords]).Select(x => x.LandscapeType);
            LandscapeData landscape = PickLandscapeUsingNeighbours(neighbourScapes);
            coordsToHex[coords].SetLandscapeData(landscape);
        }
    }

    /// <summary>
    /// Chooses a landscape given neighbouring landscapes.
    /// </summary>
    /// <param name="neighbourLandscapes"></param>
    /// <param name="maxAttempts"></param>
    /// <returns></returns>
    private static LandscapeData PickLandscapeUsingNeighbours(IEnumerable<LandscapeData> neighbourLandscapes, int maxAttempts=100)
    {
        LandscapeData choice = null;
        float support = 0;
        bool choiceAccepted = false;

        for (int i = 0; i < maxAttempts; i++)
        {
            choice = LandscapeData.ChooseRandomLandscape();
            support = CalculateSupportFromNeighbours(choice, neighbourLandscapes);
            choiceAccepted = support > UnityEngine.Random.Range(0, 61);
            if (choiceAccepted) break;
        }

        return choice;
    }

    /// <summary>
    /// Calculate the "support" for a landscape type based on its neighbours.
    /// </summary>
    private static float CalculateSupportFromNeighbours(LandscapeData landscape, IEnumerable<LandscapeData> neighbourLandscapes)
    {
        IEnumerable<float> influencesOnChoice = neighbourLandscapes
            .Select(x => LandscapeData.SupportValue(x.typeLabel, landscape.typeLabel));
        float totalInfluence = 0;
        foreach (var influence in influencesOnChoice) totalInfluence += influence;

        return totalInfluence;
    }

}
