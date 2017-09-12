﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2017 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Light_Water" {
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
#pragma glsl
#include "UnityCG.cginc"


uniform sampler2D _MainTex;
uniform float _TimeX;
uniform float _Distortion;
uniform float4 _ScreenResolution;
uniform float _Alpha;
uniform float _Distance;
uniform float _Size;
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
float2 uv = uvst.xy;
float2 sp = uv - 1.2;//vec2(.25, .35);

float2 p = sp*_Distance - float2(10.0,10.0);
float2 w = p;
float c = 0.2;
float inten = 0.01;
float t = _TimeX* (1.0 - (3.0 / float(0+1)));
w = p + float2(cos(t - w.x) + sin(t + w.y), sin(t - w.y) + cos(t + w.x));
c += 1.2/length(float2(p.x / (sin(w.x+t)/inten),p.y / (cos(w.y+t)/inten)));
c /= 1.5;
c = 1.5-sqrt(c*_Size);
float aaa=c*c;
float4 ax = float4(aaa,aaa,aaa, 999.0) + float4(0.0, 0.3, 0.5, 1.0);

float3 lrp=ax.rgb*_Alpha;

float4 sum = tex2D( _MainTex, uv+float2(lrp.r/3.5,0));

sum.rgb=sum.rgb+lrp;

return sum;
}

ENDCG
}

}
}