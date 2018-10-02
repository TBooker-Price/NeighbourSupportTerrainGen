using System.Collections.Generic;
using System.Linq;

public enum LandType
{
    Plains = 0,
    Woods,
    Cave,
    Hills,
    Mountains,
    Lake,
    Desert,
    Swamp,
}

/// <summary>
/// A LandscapeData just stores LandType enum along with an associated base probability.
/// This class also has a static list of landscapeDatas and supporMatrix hardcoded as an example.
/// </summary>
public class LandscapeData //: ScriptableObject
{
    public LandType typeLabel;
    public float baseProb; // base probability between 0.0 and 1.0 (can use for biasing the initial terrain seed)

    private static float[,] supportMatrix;
    private static readonly List<LandscapeData> landscapeDatas = new List<LandscapeData>()
    {
        new LandscapeData(LandType.Cave, 1f),
        new LandscapeData(LandType.Desert, 1f),
        new LandscapeData(LandType.Hills, 1f),
        new LandscapeData(LandType.Lake, 1f),
        new LandscapeData(LandType.Mountains, 1f),
        new LandscapeData(LandType.Plains, 1f),
        new LandscapeData(LandType.Swamp, 1f),
        new LandscapeData(LandType.Woods, 1f),
    };


    public LandscapeData(LandType typeLabel, float baseProb)
    {
        this.typeLabel = typeLabel;
        this.baseProb = baseProb;
    }


    public override string ToString()
    {
        return typeLabel.ToString();
    }

    /// <summary>
    /// Choose a landscape with chance proportional to its probability field.
    /// </summary>
    /// <returns></returns>
    public static LandscapeData ChooseRandomLandscape()
    {
        // pick a random point between 0 and the sum of probabilites over all landscape types
        float sumProbs = landscapeDatas.Select(x => x.baseProb).Sum();
        float randPoint = UnityEngine.Random.Range(0.0f, sumProbs);

        // choose the index corresponding to the random point above
        int chosenIndex = 0;
        float runningTotal = 0;
        for (int i = 0; i < landscapeDatas.Count; i++)
        {
            runningTotal += landscapeDatas[i].baseProb;
            if (runningTotal >= randPoint)
            {
                chosenIndex = i;
                break;
            }
        }

        return landscapeDatas[chosenIndex];
    }

    public static float SupportValue(LandType influencer, LandType influencee)
    {
        if (supportMatrix == null) InitialiseSupportMatrix();

        return supportMatrix[(int)influencer, (int)influencee];
    }

    private static void InitialiseSupportMatrix()
    {
        supportMatrix = new float[8, 8];

        // Plains influence on others
        AddToSprtMat(LandType.Plains, LandType.Plains, 10f);
        AddToSprtMat(LandType.Plains, LandType.Woods, 4f);
        AddToSprtMat(LandType.Plains, LandType.Cave, 1f);
        AddToSprtMat(LandType.Plains, LandType.Hills, 1f);
        AddToSprtMat(LandType.Plains, LandType.Mountains, 1f);
        AddToSprtMat(LandType.Plains, LandType.Lake, 1f);
        AddToSprtMat(LandType.Plains, LandType.Desert, 2f);
        AddToSprtMat(LandType.Plains, LandType.Swamp, 1f);

        // Woods influence on others
        AddToSprtMat(LandType.Woods, LandType.Plains, 4f);
        AddToSprtMat(LandType.Woods, LandType.Woods, 9f);
        AddToSprtMat(LandType.Woods, LandType.Cave, 1f);
        AddToSprtMat(LandType.Woods, LandType.Hills, 1f);
        AddToSprtMat(LandType.Woods, LandType.Mountains, 0f);
        AddToSprtMat(LandType.Woods, LandType.Lake, 1f);
        AddToSprtMat(LandType.Woods, LandType.Desert, -5f);
        AddToSprtMat(LandType.Woods, LandType.Swamp, 2f);

        // Cave influence on others
        AddToSprtMat(LandType.Cave, LandType.Plains, 1f);
        AddToSprtMat(LandType.Cave, LandType.Woods, 1f);
        AddToSprtMat(LandType.Cave, LandType.Cave, -10f);
        AddToSprtMat(LandType.Cave, LandType.Hills, 2f);
        AddToSprtMat(LandType.Cave, LandType.Mountains, 2f);
        AddToSprtMat(LandType.Cave, LandType.Lake, 0f);
        AddToSprtMat(LandType.Cave, LandType.Desert, -1f);
        AddToSprtMat(LandType.Cave, LandType.Swamp, -2f);

        // Hills influence on others
        AddToSprtMat(LandType.Hills, LandType.Plains, 3f);
        AddToSprtMat(LandType.Hills, LandType.Woods, 2f);
        AddToSprtMat(LandType.Hills, LandType.Cave, 3f);
        AddToSprtMat(LandType.Hills, LandType.Hills, 2f);
        AddToSprtMat(LandType.Hills, LandType.Mountains, 3f);
        AddToSprtMat(LandType.Hills, LandType.Lake, -1f);
        AddToSprtMat(LandType.Hills, LandType.Desert, 0f);
        AddToSprtMat(LandType.Hills, LandType.Swamp, -2f);

        // Mountains influence on others
        AddToSprtMat(LandType.Mountains, LandType.Plains, 1f);
        AddToSprtMat(LandType.Mountains, LandType.Woods, 1f);
        AddToSprtMat(LandType.Mountains, LandType.Cave, 4f);
        AddToSprtMat(LandType.Mountains, LandType.Hills, 5f);
        AddToSprtMat(LandType.Mountains, LandType.Mountains, 3f);
        AddToSprtMat(LandType.Mountains, LandType.Lake, -2f);
        AddToSprtMat(LandType.Mountains, LandType.Desert, -2f);
        AddToSprtMat(LandType.Mountains, LandType.Swamp, -2f);

        // Lake influence on others
        AddToSprtMat(LandType.Lake, LandType.Plains, 5f);
        AddToSprtMat(LandType.Lake, LandType.Woods, 3f);
        AddToSprtMat(LandType.Lake, LandType.Cave, 0f);
        AddToSprtMat(LandType.Lake, LandType.Hills, 0f);
        AddToSprtMat(LandType.Lake, LandType.Mountains, 0f);
        AddToSprtMat(LandType.Lake, LandType.Lake, 2f);
        AddToSprtMat(LandType.Lake, LandType.Desert, -6f);
        AddToSprtMat(LandType.Lake, LandType.Swamp, 2f);

        // Desert influence on others
        AddToSprtMat(LandType.Desert, LandType.Plains, 5f);
        AddToSprtMat(LandType.Desert, LandType.Woods, -5f);
        AddToSprtMat(LandType.Desert, LandType.Cave, -2f);
        AddToSprtMat(LandType.Desert, LandType.Hills, -1f);
        AddToSprtMat(LandType.Desert, LandType.Mountains, -1f);
        AddToSprtMat(LandType.Desert, LandType.Lake, -4f);
        AddToSprtMat(LandType.Desert, LandType.Desert, 10f);
        AddToSprtMat(LandType.Desert, LandType.Swamp, -5f);

        // Swamp influence on others
        AddToSprtMat(LandType.Swamp, LandType.Plains, 3f);
        AddToSprtMat(LandType.Swamp, LandType.Woods, 4f);
        AddToSprtMat(LandType.Swamp, LandType.Cave, 0f);
        AddToSprtMat(LandType.Swamp, LandType.Hills, 0f);
        AddToSprtMat(LandType.Swamp, LandType.Mountains, -1f);
        AddToSprtMat(LandType.Swamp, LandType.Lake, 3f);
        AddToSprtMat(LandType.Swamp, LandType.Desert, -5f);
        AddToSprtMat(LandType.Swamp, LandType.Swamp, 3f);
    }

    private static void AddToSprtMat(LandType influencer, LandType influencee, float value)
    {
        supportMatrix[(int)influencer, (int)influencee] = value;
    }
}
