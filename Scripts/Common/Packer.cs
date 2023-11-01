using UnityEngine;

public static class Packer
{
    /// <summary>
    /// Pack 2 low-precision [0-1] floats values to a float.
    /// Each value [0-1] has 4096 steps(12 bits).
    /// </summary>
    public static float Pack(float x, float y)
    {
        x = x < 0 ? 0 : 1 < x ? 1 : x;
        y = y < 0 ? 0 : 1 < y ? 1 : y;
        const int PRECISION = (1 << 12) - 1;
        return (Mathf.FloorToInt(y * PRECISION) << 12)
               + Mathf.FloorToInt(x * PRECISION);
    }
}
