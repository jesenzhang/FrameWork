﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2017 /////
////////////////////////////////////////////

Shader "CameraFilterPack/BlurHole" {
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
uniform float _Radius;
uniform float _SpotSize;
uniform float _CenterX;
uniform float _CenterY;
uniform float _Alpha;
uniform float _Alpha2;
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

float kernel[6];
float3 final_colour = 0.0;

kernel[0]=0;
kernel[1]=0;
kernel[2]=0;
kernel[3]=0;
kernel[4]=0;
kernel[5]=0;

float Z = 0.0;
for (int j = 0; j <= 2; ++j)
{
kernel[2+j] = kernel[2-j] = 0.4;
}

for (int j = 0; j < 6; ++j)
{
Z += kernel[j];
}

for (int u=-2; u <= 2; ++u)
{
for (int j=-2; j <= 2; ++j)
{
float kernelmult = kernel[2+j] * kernel[2+u];
float4 tex = tex2D(_MainTex, (uvst.xy * _ScreenResolution.xy + float2(float(u*_Distortion),float(j*_Distortion))) / _ScreenResolution.xy);
final_colour +=  kernelmult * (tex).rgb;
}
}
float2 center=float2(_CenterX,_CenterY);
float dist2 = 1.0 - smoothstep( _Radius,_Radius+0.15*_SpotSize, length(center - uvst.xy));
float3 colorx=final_colour/(Z*Z);
float3 cm=tex2D(_MainTex,uvst.xy);
float3 crgb2=lerp(cm,colorx,_Alpha2);		
float3 crgb=lerp(crgb2,colorx,(1-dist2)*_Alpha);		

return float4(crgb, 1.0);

}

ENDCG
}

}
}