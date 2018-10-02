using System.Collections.Generic;

public interface IGraph<T>
{
    IEnumerable<T> Neighbors(T node);

    ///// <summary>
    ///// Returns the cost of moving from node A to a neighbouring node B.
    ///// </summary>
    //float Cost(T nodeA, T nodeB);
}
