using System;
using System.Linq;
using System.Collections.Generic;

public class EnemyPlayer : BasePlayer
{
    public override Stone.Color MyColor { get { return Stone.Color.White; } }

    private Random _random = new Random();


    public override bool TryGetSelected(out int x, out int y, out int z)
    {
        var availablePoints = GetAvailablePoints();
        var maxCount = availablePoints.Values.Max();
        var list = new List<Tuple<int, int, int>>();
        foreach (var availablePoint in availablePoints)
        {
            if (availablePoint.Value == maxCount)
            {
                list.Add(availablePoint.Key);
            }
        }
        if (list.Count > 0)
        {
            var point = list[_random.Next(list.Count)];
            x = point.Item1;
            y = point.Item2;
            z = point.Item3;
            return true;
        }
        return base.TryGetSelected(out x, out y, out z);
    }
}