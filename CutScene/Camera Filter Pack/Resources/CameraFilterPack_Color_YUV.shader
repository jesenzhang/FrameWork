﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2017 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Color_YUV" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
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
uniform float _Y;
uniform float _U;
uniform float _V;

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

float3 YUV2RGB(float Y,float U, float V)
{
float R=Y+1.140*V;
float G=Y-0.395*U-0.581*V;
float B=Y+2.032*U;
return float3(R,G,B); 
} 

float3 RGB2YUV(float R,float G, float B)
{
float Y=0.299*R +0.587* G + 0.114 * B;
float U=-0.147*R - 0.289* G + 0.436* B;
float V= 0.615*R - 0.515* G - 0.100* B;
return float3(Y,U,V); 
} 

half4 _MainTex_ST;
float4 frag(v2f i) : COLOR
{
float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);
float2 p = uvst.xy;
float4 col = tex2D(_MainTex, p);
float3 colx = RGB2YUV(col.r,col.g,col.b);
col.rgb = YUV2RGB(colx.r+_Y,colx.g+_U,colx.b+_V);
return col;

}

ENDCG
}

}
}