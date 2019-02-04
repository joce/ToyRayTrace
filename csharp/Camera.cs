using System;

namespace ToyRayTrace
{
    public class Camera
    {
        Vec3 m_Origin;
        Vec3 m_LowerLeftCorner;
        Vec3 m_Horizontal;
        Vec3 m_Vertical;
        float m_LensRadius;
        Vec3 m_U, m_V, m_W; // Camera orientation

        public Camera(Vec3 lookFrom, Vec3 lookAt, Vec3 vUp, float vFov, float aspect, float aperture = 0, float focusDistance = 1)
        {
            m_LensRadius = aperture / 2f;
            var theta = vFov * MathF.PI / 180f;
            var halfHeight = MathF.Tan(theta / 2f);
            var halfWidth = aspect * halfHeight;

            m_W = Vec3.Normalize(lookFrom - lookAt);
            m_U = Vec3.Normalize(Vec3.Cross(vUp, m_W));
            m_V = Vec3.Cross(m_W, m_U);

            m_Origin = lookFrom;
            m_LowerLeftCorner = m_Origin -
                halfWidth * focusDistance * m_U -
                halfHeight * focusDistance * m_V -
                focusDistance * m_W;
            m_Horizontal = 2f * halfWidth * focusDistance * m_U;
            m_Vertical = 2f * halfHeight * focusDistance * m_V;
        }

        public Ray GetRay(float s, float t)
        {
            var random = m_LensRadius * Rng.NextInUnitDisc();
            var offset = m_U * random.X + m_V * random.Y;
            return new Ray(m_Origin + offset,
                m_LowerLeftCorner + s * m_Horizontal + t * m_Vertical - m_Origin - offset);
        }
    }
}
