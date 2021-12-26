// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Alpha of color - intensity of mask
Shader "Custom/NeonHeightInstancing"
{

    Properties
    {
        _MainTex ("Main texture", 2D) = "white" {}
        _SkinTex ("Mask texture", 2D) = "white" {}

        [Space]

        _EmissionMap("Emission map", 2D) = "black"{}

        [Space]

        _MainColor ("Main color (red channel)", Color) = (1,0,0,1)
        _AddColor ("Additional color (green channel)", Color) = (0,1,0,1)
        _ExtColor ("Extra color (blue channel)", Color) = (0,0,1,1)

        [Space]

        [Toggle]
        _Emission("Emission", Float) = 0
        _EmissionPower("Emission Power", Float) = 1

        [Space]
        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull", Float) = 2 //"Back"
    }

    CGINCLUDE
    float inverse(float value)
    {
        return (value - 1) * -1;
    }
    ENDCG

    SubShader
    {
        Cull [_Cull]
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma fragment frag
            #pragma vertex vert
            #pragma multi_compile_instancing
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            sampler2D _MainTex;
            sampler2D _SkinTex;
            float4 _MainTex_ST;
            float4 _MainColor;
            float4 _AddColor;
            float4 _ExtColor;
            float _Emission;
            float _EmissionPower;
            sampler2D _EmissionMap;

            UNITY_INSTANCING_BUFFER_START(InstanceProps)
            UNITY_DEFINE_INSTANCED_PROP(float4, _LowestColor)
            UNITY_DEFINE_INSTANCED_PROP(float4, _MiddleColor)
            UNITY_DEFINE_INSTANCED_PROP(float4, _MaxColor)
            UNITY_DEFINE_INSTANCED_PROP(float4, _InitialPosition)
            UNITY_DEFINE_INSTANCED_PROP(float, _LowestDelta)
            UNITY_DEFINE_INSTANCED_PROP(float, _MiddleDelta)
            UNITY_INSTANCING_BUFFER_END(InstanceProps)

            struct Interpolators
            {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uvSplat : TEXCOORD1;
                float3 normal : NORMAL;
                fixed4 diff : COLOR0; // diffuse lighting color
                fixed4 world : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };

            struct VertexData
            {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            Interpolators vert(VertexData v)
            {
                Interpolators i;
                UNITY_INITIALIZE_OUTPUT(Interpolators, i);
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, i);
                i.position = UnityObjectToClipPos(v.position);
                i.uv = TRANSFORM_TEX(v.uv, _MainTex);
                i.uvSplat = v.uv;
                i.normal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(i.normal, _WorldSpaceLightPos0.xyz));
                nl = clamp(nl, 0, 1);
                i.diff = lerp(unity_AmbientSky, _LightColor0 * nl, nl);
                //i.world = mul(unity_ObjectToWorld, v.position);
                i.world = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0));
                UNITY_TRANSFER_FOG(i, i.position);
                return i;
            }

            float4 frag(Interpolators i) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(i);
                const float4 lowest_color = UNITY_ACCESS_INSTANCED_PROP(InstanceProps, _LowestColor);
                const float4 middle_color = UNITY_ACCESS_INSTANCED_PROP(InstanceProps, _MiddleColor);
                const float4 max_color = UNITY_ACCESS_INSTANCED_PROP(InstanceProps, _MaxColor);
                const float4 initial_position = UNITY_ACCESS_INSTANCED_PROP(InstanceProps, _InitialPosition);
                const float lowest_delta = UNITY_ACCESS_INSTANCED_PROP(InstanceProps, _LowestDelta);
                const float middle_delta = UNITY_ACCESS_INSTANCED_PROP(InstanceProps, _MiddleDelta);
                
                const float4 splat = tex2D(_SkinTex, i.uvSplat);
                const float3 tex = tex2D(_MainTex, i.uv);

                const float inverse_emission = inverse(_Emission);
                float4 color = float4(tex.rgb * (1 - splat.r - splat.g - splat.b) +
                                      lerp(tex.rgb, _MainColor.rgb, _MainColor.a) * splat.r * inverse_emission +
                                      lerp(tex.rgb, _AddColor.rgb, _AddColor.a) * splat.g * inverse_emission +
                                      lerp(tex.rgb, _ExtColor.rgb, _ExtColor.a) * splat.b,
                                      splat.a);
                color.rgb *= i.diff.rgb;
                float3 emission_color;
                float2 emission_red = tex2D(_EmissionMap, i.uv).rg;

                const float delta = i.world.y - initial_position.y;

                if (delta <= lowest_delta)
                {
                    emission_red.r += emission_red.g;
                    emission_color = lowest_color;
                }
                else if (delta <= middle_delta)
                {
                    emission_color = middle_color;
                }
                else if (delta > middle_delta)
                {
                    emission_color = max_color;
                }

                color.rgb += (emission_red.r * emission_color * _EmissionPower * _Emission);

                UNITY_APPLY_FOG(i.fogCoord, color);
                return color;
            }
            ENDCG
        }

    }
}