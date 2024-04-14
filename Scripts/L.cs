using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Coffee.UIEffects
{
    static class L
    {
        [Conditional("DEBUG")]
        public static void I(string message) => Debug.Log("[UIEffect] " + message);
        [Conditional("DEBUG")]
        public static void W(string message) => Debug.LogWarning("[UIEffect] " + message);
        public static void E(string message) => Debug.LogError("[UIEffect] " + message);
    }
}