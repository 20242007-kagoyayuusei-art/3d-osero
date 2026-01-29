using UnityEngine;

public class Cell : MonoBehaviour
{
    public int x;
    public int y;
    public int z;

    public bool hasStone = false;

    public bool IsEmpty()
    {
        return !hasStone;
    }
}