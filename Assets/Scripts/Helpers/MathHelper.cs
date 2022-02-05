using Unity.Mathematics;

namespace Helpers {
    static class MathHelper {

        public static bool IsInsideRect(float2 point, float2 rectBottomLeft, float2 rectTopRight) {
            return (point.x >= rectBottomLeft.x && point.x <= rectTopRight.x) &&
                   (point.y >= rectBottomLeft.y && point.y <= rectTopRight.y);
        }

        public static bool IsInsideRectBounds(float2 point, float2 pos, float2 rectBounds) {
            float2 halfBounds = rectBounds * 0.5f;
            return IsInsideRect(point, new float2(pos.x - halfBounds.x, pos.y - halfBounds.y), new float2(pos.x + halfBounds.x, pos.y + halfBounds.y));
        }

    }
}
