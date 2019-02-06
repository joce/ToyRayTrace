using System;
using System.Numerics;

namespace ToyRayTrace
{
    public readonly struct Camera
    {
        readonly Vector3 m_Origin;
        readonly Vector3 m_LowerLeftCorner;
        readonly Vector3 m_Horizontal;
        readonly Vector3 m_Vertical;
        readonly float m_LensRadius;
        readonly Vector3 m_U, m_V, m_W; // Camera orientation

        public Camera(in Vector3 lookFrom, in Vector3 lookAt, in Vector3 vUp, float vFov, float aspect, float aperture = 0, float focusDistance = 1)
        {
            m_LensRadius = aperture / 2f;
            var theta = vFov * MathF.PI / 180f;
            var halfHeight = MathF.Tan(theta / 2f);
            var halfWidth = aspect * halfHeight;

            m_W = Vector3.Normalize(lookFrom - lookAt);
            m_U = Vector3.Normalize(Vector3.Cross(vUp, m_W));
            m_V = Vector3.Cross(m_W, m_U);

            m_Origin = lookFrom;
            m_LowerLeftCorner = m_Origin -
                m_U * halfWidth * focusDistance -
                m_V * halfHeight * focusDistance -
                m_W * focusDistance;
            m_Horizontal = m_U * 2f * halfWidth * focusDistance;
            m_Vertical = m_V * 2f * halfHeight * focusDistance;
        }

        public Ray GetRay(float s, float t, ref uint state)
        {
            var random = Rng.NextInUnitDisc(ref state) * m_LensRadius;
            var offset = m_U * random.X + m_V * random.Y;
            return new Ray(m_Origin + offset,
                m_LowerLeftCorner + m_Horizontal * s + m_Vertical * t - m_Origin - offset);
        }
    }
}
