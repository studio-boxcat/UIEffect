using UnityEngine;

public static class Packer
{
    /// <summary>
    /// Pack 2 low-precision [0-1] floats values to a float.
    /// Each value [0-1] has 4096 steps(12 bits).
    /// </summary>
    public static float Pack(float x, float y)
    {
        // IEEE-754 single precision has a 24-bit significand (23 stored bits + 1 hidden).
        // Every integer ≤ 16 777 216 (2²⁴) is exact. Anything above that loses the least-significant bits.
        const uint MAX = (1u << 12) - 1u; // 4095
        var xi = (uint) Mathf.RoundToInt(Mathf.Clamp01(x) * MAX);
        var yi = (uint) Mathf.RoundToInt(Mathf.Clamp01(y) * MAX);
        return (yi << 12) | xi; // 0-24 bits only
    }
}