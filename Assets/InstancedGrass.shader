﻿Shader "Instanced/InstancedGrass" {
    Properties {
		_Color ("Color", Color) = (0,1,0,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_MetallicTex ("Metallic (RGA, Metallic, Smoothness, Ambient Occlusion)", 2D) = "bump" {}
		_Metallic ("Metallic", Range(0, 1)) = 1
		_Smoothness ("Smoothness", Range(0, 1)) = 1
		_Occlusion ("Occlusion", Range(0, 1)) = 1

		[Header(Grass Settings)]
		_GrassSize ("Grass Size", Float) = 1
		_GrassCurve ("Grass Curve", Float) = 0.5

		[Header(Wind Settings)]
		_WindTex ("Wind Texture", 2D) = "white" {}
		_WindSize ("Wind Size", Float) = 1
		_WindSpeed ("Wind Speed", Float) = 1
		_WindStrength ("Wind Strength", Float) = 1

		[Header(Stamp)]
		_StampRadius ("Stamp Radius", Float) = 1
		_StampStrength ("Stamp Strength", Float) = 1
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
        Cull Off

        CGPROGRAM
        // Physically based Standard lighting model
        #pragma surface surf Standard addshadow keepalpha vertex:vert
        #pragma multi_compile_instancing
        #pragma instancing_options procedural:setup
		#include "UnityPBSLighting.cginc"

		half4 _Color;
        sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _MetallicTex;
		half _Metallic;
		half _Smoothness;
		half _Occlusion;
		half _GrassSize;
		half _GrassCurve;
		sampler2D _WindTex;
		half _WindSize;
		half _WindSpeed;
		half _WindStrength;
		half _StampRadius;
		half _StampStrength;

		float3 _CharacterPosition;

        struct Input
		{
            float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 worldPos;
        };

		#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
        StructuredBuffer<float4> positionBuffer;
		#endif

        void rotate2D(inout float2 v, float r)
        {
            float s, c;
            sincos(r, s, c);
            v = float2(v.x * c - v.y * s, v.x * s + v.y * c);
        }

        void setup()
        {
			#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
            float4 data = positionBuffer[unity_InstanceID];
			float scale = data.w * _GrassSize;

            unity_ObjectToWorld._11_21_31_41 = float4(scale, 0, 0, 0);
            unity_ObjectToWorld._12_22_32_42 = float4(0, scale, 0, 0);
            unity_ObjectToWorld._13_23_33_43 = float4(0, 0, scale, 0);
            unity_ObjectToWorld._14_24_34_44 = float4(data.xyz, 1);
            unity_WorldToObject = unity_ObjectToWorld;
            unity_WorldToObject._14_24_34 *= -1;
            unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;
			#endif
        }

		float GetWindStrength(float2 position, float height)
		{
            float windStrength = tex2Dlod(_WindTex, float4(position / _WindSize + float2(_Time.x * _WindSpeed + height * 0.01, 0), 0, 0)).r;
            return (windStrength - 0.5) * height * _WindStrength;
        }

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input,o);
            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
            float4 data = positionBuffer[unity_InstanceID];

			float3 worldPos = data.xyz;
			float dist = distance(_CharacterPosition, worldPos);
			float3 stampStrength = 1 - saturate(dist / _StampRadius);
			float3 stampDir = worldPos - _CharacterPosition;
			stampDir = normalize(stampDir) * stampStrength * _StampStrength * v.vertex.y;
			v.vertex.xz += stampDir.xz;

			float windStrength = GetWindStrength(data.xz, v.vertex.y);
            v.vertex.x += windStrength;   
            v.vertex.y += windStrength * _GrassCurve;

			#endif
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
		{
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            o.Albedo = c.rgb;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

			fixed4 metallicCol = tex2D(_MetallicTex, IN.uv_MainTex);
            o.Metallic = metallicCol.r * _Metallic;
            o.Smoothness = metallicCol.g * _Smoothness;
			o.Occlusion = metallicCol.a * _Occlusion;
            o.Alpha = c.a;

			clip(c.a - 0.1);
        }
        ENDCG
    }
}