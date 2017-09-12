﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2017 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Color_Chromatic_Plus" 
{
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(-0.02, 0.02)) = 0.02
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
uniform float _Value;
uniform float _Value2;
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
float2 p = uvst.xy;
float4 col = tex2D(_MainTex, p);
float4 col2 = col;
float2 offset = float2(_Distortion,.0);
col2.r = tex2D(_MainTex, p+offset.xy).r;
col2.b = tex2D(_MainTex, p+offset.yx).b;
float dist = 1.0 - smoothstep(_Value, _Value - _Value2, length(float2(0.5, 0.5) - p));

col = lerp(col, col2, dist);
return col;
}

ENDCG
}

}
}