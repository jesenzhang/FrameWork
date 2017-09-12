﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2017 /////
////////////////////////////////////////////

Shader "CameraFilterPack/EyesVision_1" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_MainTex2 ("Base (RGB)", 2D) = "white" {}
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
uniform sampler2D _MainTex2;
uniform float _TimeX;
uniform float _Speed;
uniform float _Value;
uniform float _Value2;
uniform float _Value3;
uniform float _Value4;
uniform float4 _ScreenResolution;
uniform float2 _MainTex_TexelSize;

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

float nrand(float2 n) {

return frac(sin(dot(n.xy, float2(12.9898, 78.233)))* 43758.5453);
}
float3 linearDodge( float3 s, float3 d )
{
return s + d;
}
inline float2 getOffset(float time, float2 uv)
{ 
float a = 1.0 + 0.5 * sin(time + uv.x * _Value);
float b = 1.0 + 0.5 * cos(time + uv.y * _Value);
return 0.02 * float2(a + sin(b), b + cos(a));
}
half4 _MainTex_ST;
float4 frag(v2f i) : COLOR
{
float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);
float2 uv = uvst.xy;
float t = float(int(_TimeX * sin(_TimeX)/8));
t+=t = float(int(_TimeX * _Value3));
float2 suv = uv + 0.004* float2( nrand(t)*-6, nrand(t + 23.0)*4);
suv*=0.8;
suv+=float2(0.075,.05);
#if UNITY_UV_STARTS_AT_TOP
if (_MainTex_TexelSize.y < 0)
uv = 1-uv;
#endif
suv += getOffset(_Value2* _TimeX, uv);
float3 oldfilm = tex2D(_MainTex2,suv).rgb;
uv = uvst.xy;
uv +=float2(oldfilm.r,oldfilm.g)/8;
float3 col = tex2D(_MainTex,uv).rgb;
col.r+=0.08;
col.g+=0.08;
col.b-=0.03;
col=linearDodge(oldfilm,col);
float3 black=float3(0.0,0.0,0.0);
float Dist= t-(sin(_TimeX/2)*sin(_TimeX/2)*t);
float dist2 = 1.0 - smoothstep(Dist,Dist-0.6, length(float2(0.5,0.5) - uv.y));
col=lerp(col,black,dist2*_Value4);
return float4(col, 1.0);
}

ENDCG
}

}
}