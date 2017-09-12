﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2017 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Distortion_Flag" {
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_TimeX ("Time", Range(0.0, 1.0)) = 1.0
		_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.3
		_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
	}
	SubShader 
	{
		Pass
		{
			Cull Off ZWrite Off ZTest Always
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0
			#include "UnityCG.cginc"
			
			
			uniform sampler2D _MainTex;
			uniform float _TimeX;
			uniform float _Distortion;
			uniform float4 _ScreenResolution;
			
		       struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };
 
            struct v2f
            {
                  float2 texcoord  : TEXCOORD0;
                  float4 vertex   : SV_POSITION;
                  float4 color    : COLOR;
           };   
             
  			v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color;
                
                return OUT;
            }
            

			half4 _MainTex_ST;
float4 frag(v2f i) : COLOR
{
float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);
					
				float2 uv = uvst;
				float y = 
						0.7*sin((uv.y + _TimeX) * 4.0) * 0.038 +
						0.3*sin((uv.y + _TimeX) * 8.0) * 0.010 +
						0.05*sin((uv.y + _TimeX) * 40.0) * 0.05;

				float x = 
						0.5*sin((uv.y + _TimeX) * 5.0) * 0.1 +
						0.2*sin((uv.x + _TimeX) * 10.0) * 0.05 +
						0.2*sin((uv.x + _TimeX) * 30.0) * 0.02;

				return tex2D(_MainTex, 0.79*(uv + float2(_Distortion*y+0.11, _Distortion*x+0.11)));

			}
			
			ENDCG
		}
		
	}
}