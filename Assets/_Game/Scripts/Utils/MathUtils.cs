using UnityEngine;

namespace IronTide.Utils
{
    public static class MathUtils
    {
        /// <summary>
        /// Evaluate a parabolic arc position.
        /// t = 0..1, arcHeight = peak height above the lerp line.
        /// </summary>
        public static Vector3 ParabolicLerp(Vector3 start, Vector3 end, float arcHeight, float t)
        {
            var pos = Vector3.Lerp(start, end, t);
            pos.y  += arcHeight * Mathf.Sin(t * Mathf.PI);
            return pos;
        }

        /// <summary>
        /// Simple leading-shot prediction: where will a linearly-moving target be
        /// when a projectile travelling at projectileSpeed reaches it?
        /// Returns the predicted world position.
        /// </summary>
        public static Vector3 PredictLeadPosition(Vector3 shooterPos, Vector3 targetPos,
                                                   Vector3 targetVelocity, float projectileSpeed)
        {
            float dist     = Vector3.Distance(shooterPos, targetPos);
            float timeToHit = dist / Mathf.Max(projectileSpeed, 0.1f);
            return targetPos + targetVelocity * timeToHit;
        }

        /// <summary>Clamp an angle to [-180, 180] range.</summary>
        public static float NormalizeAngle(float angle)
        {
            while (angle >  180f) angle -= 360f;
            while (angle < -180f) angle += 360f;
            return angle;
        }
    }
}
