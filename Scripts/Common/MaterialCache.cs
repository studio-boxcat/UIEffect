using System.Collections.Generic;
using UnityEngine;

namespace Coffee.UIEffects
{
    public class MaterialCache
    {
        static Dictionary<ulong, MaterialEntry> materialMap = new();

        class MaterialEntry
        {
            public readonly Material material;
            public int referenceCount;

            public MaterialEntry(Material material)
            {
                this.material = material;
            }
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void ClearCache()
        {
            foreach (var entry in materialMap.Values)
                Object.DestroyImmediate(entry.material, false);
            materialMap.Clear();
        }
#endif

        public static ulong GetMaterialHash(Material baseMaterial, int variant)
        {
            return (ulong) baseMaterial.GetInstanceID() << 32 | (uint) variant;
        }

        public static bool TryRent(ulong hash, out Material material)
        {
            if (materialMap.TryGetValue(hash, out var materialEntry))
            {
                materialEntry.referenceCount++;
                material = materialEntry.material;
                return true;
            }
            else
            {
                material = null;
                return false;
            }
        }

        public static void RegisterNewlyRented(ulong hash, Material material)
        {
            materialMap.Add(hash, new MaterialEntry(material) {referenceCount = 1});
        }

        public static void Return(ulong hash)
        {
            var materialEntry = materialMap[hash];
            materialEntry.referenceCount--;

            if (materialEntry.referenceCount == 0)
            {
                Object.DestroyImmediate(materialEntry.material, false);
                materialMap.Remove(hash);
            }
        }
    }
}