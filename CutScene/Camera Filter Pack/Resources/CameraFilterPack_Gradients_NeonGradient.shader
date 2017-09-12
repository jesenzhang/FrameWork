// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2017 /////
////////////////////////////////////////////


Shader "CameraFilterPack/Gradients_NeonGradient" { 
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

float square(float s) { return s * s; }
float3 square(float3 s) { return s * s; }

float3 neonGradient(float t) 
{
return clamp(float3(t * 1.3 + 0.1, square(abs(0.43 - t) * 1.7), (1.0 - t) * 1.7), 0.0, 1.0);
}


half4 _MainTex_ST;
float4 frag(v2f i) : COLOR
{
float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);
float t = uvst.x;
float j = t + (frac(sin(uvst.y * 7.5e2 + uvst.x * 6.4) * 1e2) - 0.5) * 0.005;
float2 uv = uvst.xy;
float4 tc = tex2D(_MainTex,uv);    
float b = (0.2126*tc.r + 0.7152*tc.g + 0.0722*tc.b);
b=lerp(b,1-b,_Value);
float3 map=lerp(tc,neonGradient(b),_Value2);
tc=float4(map,1.0);
return  tc;
}
ENDCG
}
}
}
