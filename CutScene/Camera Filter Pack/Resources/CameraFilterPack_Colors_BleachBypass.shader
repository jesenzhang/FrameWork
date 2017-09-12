// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2017 /////
////////////////////////////////////////////


Shader "CameraFilterPack/Colors_BleachBypass" { 
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
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
uniform float _Value;
uniform float _Value2;
uniform float _Value3;
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
float4 base = tex2D(_MainTex, uvst);
float3 lumCoeff = float3(0.25,0.65,0.1);
float lum = dot(lumCoeff,base.rgb);
float3 blend = lum.rrr;
float L = min(1,max(0,10*(lum- 0.45)));
float3 result1 = 2.0f * base.rgb * blend;
float3 result2 = 1.0f - 2.0f*(1.0f-blend)*(1.0f-base.rgb);
float3 newColor = lerp(result1,result2,L);
float A2 = _Value * base.a;
float3 lerpRGB = A2 * newColor.rgb;
lerpRGB += ((1.0f-A2) * base.rgb);
return float4(lerpRGB,base.a);
}
ENDCG
}
}
}
