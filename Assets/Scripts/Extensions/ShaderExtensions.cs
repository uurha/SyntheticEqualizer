using UnityEngine;

namespace Extensions
{
    public static class ShaderExtensions
    {
        public static readonly int InitialPosition = Shader.PropertyToID("_InitialPosition");
        public static readonly int LowestColor = Shader.PropertyToID("_LowestColor");
        public static readonly int MiddleColor = Shader.PropertyToID("_MiddleColor");
        public static readonly int MaxColor = Shader.PropertyToID("_MaxColor");
        public static readonly int LowestDelta = Shader.PropertyToID("_LowestDelta");
        public static readonly int MiddleDelta = Shader.PropertyToID("_MiddleDelta");
    }
}
