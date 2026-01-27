using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePlayer : MonoBehaviour
{
    public abstract Stone.Color MyColor { get; }

    public virtual bool TryGetSelected(out int x, out int y, out int z)
    {
        x = 0;
        y = 0;
        z = 0;
        return false;
    }

    public Dictionary<Tuple<int, int, int>, int> GetAvailablePoints()
    {
        var game = Game.Instance;
        var stones = game.Stones;
        var availablePoints = new Dictionary<Tuple<int, int, int>, int>();
        for (var y = 0; y < Game.YNUM; y++)
        {
            for (var z = 0; z < Game.ZNUM; z++)
            {
                for (var x = 0; x < Game.XNUM; x++)
                {
                    if (stones[y][z][x].CurrentState == Stone.State.None)
                    {
                        var reverseCount = game.CalcTotalReverseCount(MyColor, x, y, z);
                        if (reverseCount > 0)
                        {
                            availablePoints[Tuple.Create(x, y, z)] = reverseCount;
                        }
                    }
                }
            }
        }
        return availablePoints;
    }

    public bool CanPut()
    {
        return GetAvailablePoints().Count > 0;
    }
}