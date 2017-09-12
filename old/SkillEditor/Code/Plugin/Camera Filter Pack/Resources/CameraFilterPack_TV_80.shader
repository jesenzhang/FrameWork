﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/TV_80" {
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
ZTest Always
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
half2 texcoord  : TEXCOORD0;
float4 vertex   : SV_POSITION;
fixed4 color    : COLOR;
};   

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;

return OUT;
}
//            
//inline float mod(float x,float modu) {
//  return x - floor(x * (1.0 / modu)) * modu;
//}   


inline float2 curve(float2 uv)
{
uv 		= (uv - 0.5) * 2.0;
uv     *= 1.1;	
uv.x   *= 1.0 + pow((abs(uv.y) * 0.2), 2.0);
uv.y   *= 1.0 + pow((abs(uv.x) * 0.25), 2.0);
uv 	 	= (uv / 2.0) + 0.5;
uv 		=  uv *0.92 + 0.04;
return uv;
}
float rand(float2 co){
return frac(sin(dot(co.xy ,float2(12.9898,78.233))) * 43758.5453);
}

fixed4 frag (v2f i) : COLOR
{
float2 q = i.texcoord.xy;
float2 uv = q;
//  uv = curve( uv );
float3 col;
float x =  sin(0.3*_TimeX+uv.y*21.0)*sin(0.7*_TimeX+uv.y*29.0)*sin(0.3+0.33*_TimeX+uv.y*31.0)*0.0017;
float4 text = tex2D(_MainTex,float2(x+uv.x+0.001,uv.y+0.001));
col.rgb = text.xyz + 0.05;
text = tex2D(_MainTex,0.75*float2(x+0.025, -0.02)+float2(uv.x+0.001,uv.y+0.001));
col.r += 0.08*text.x;
col.g += 0.05*text.y;
col.b += 0.08*text.z;   
col = clamp(col*0.6+0.4*col*col,0.0,1.0);
float vig = (0.0 + 16.0*uv.x*uv.y*(1.0-uv.x)*(1.0-uv.y));
col *= pow(vig,0.3);
col *= float3(3.66,2.94,2.66);
//col *= col.brg;//float3(0.66,0.94,0.66);
float scans = clamp( 0.35+0.35*sin(3.5*_TimeX+uv.y*_ScreenResolution.y*1.5), 0.0, 1.0);
float s = pow(scans,1.7);
col = col*(0.4+0.7*s) ;

//col.rgb = dot(col.rgb, float3(.222, .707, .071));

float noiseIntensity = .75;
float pixelDensity = 250.;
float cx=clamp(rand(float2(floor(uv.x * pixelDensity * 1.0), floor(uv.y * pixelDensity)) *_TimeX / 1000.) + 1. - noiseIntensity, 0., 1.);
float3 colorx = float3(cx,cx,cx);	

float barHeight = 6.;
float barSpeed = 2.6;
float barOverflow = 1.2;
float bar = clamp(floor(sin(uv.y * 6. + _TimeX * 2.6) + 1.95), 0., 1.1);

col *= 1.0+0.01*sin(110.0*_TimeX)+bar/4;
if (uv.x < 0.0 || uv.x > 1.0)
col = 0.0;
if (uv.y < 0.0 || uv.y > 1.0)
col = 0.0;


return fixed4(col,1.0);
}

ENDCG
}

}
}