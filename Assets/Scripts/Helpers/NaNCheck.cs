using System.Runtime.InteropServices;
using Unity.Mathematics;

namespace Helpers {
    static class NaNCheck {
        [StructLayout(LayoutKind.Explicit)]
        struct FloatUnion {
            [FieldOffset(0)]
            public float value;

            [FieldOffset(0)]
            public int binary;
        }

        public static bool IsNaN(float3 v) {
            return IsNaN(v.x) || IsNaN(v.y) || IsNaN(v.z);
        }

        public static bool IsNaN(float f) {
            FloatUnion union = new FloatUnion {
                value = f
            };
            return ((union.binary & 0x7F800000) == 0x7F800000) && ((union.binary & 0x007FFFFF) != 0);
        }

    }
}
