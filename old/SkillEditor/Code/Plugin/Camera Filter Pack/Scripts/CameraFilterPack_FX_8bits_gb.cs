﻿////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////
using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Pixel/8bits_gb")]
public class CameraFilterPack_FX_8bits_gb : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
private Material SCMaterial;
[Range(-1, 1)]
public float Brightness = 0;
#endregion
#region Properties
Material material
{
get
{
if(SCMaterial == null)
{
SCMaterial = new Material(SCShader);
SCMaterial.hideFlags = HideFlags.HideAndDontSave;	
}
return SCMaterial;
}
}
#endregion
void Start () 
{
SCShader = Shader.Find("CameraFilterPack/FX_8bits_gb");
if(!SystemInfo.supportsImageEffects)
{
enabled = false;
return;
}
}
void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture)
{
if(SCShader != null)
{
TimeX+=Time.deltaTime;
if (TimeX>100)  TimeX=0;
material.SetFloat("_TimeX", TimeX);
if (Brightness==0) Brightness=0.001f;
material.SetFloat("_Distortion", Brightness);
RenderTexture buffer = RenderTexture.GetTemporary(160, 144, 0);
Graphics.Blit(sourceTexture, buffer, material);
buffer.filterMode=FilterMode.Point;
Graphics.Blit(buffer, destTexture);
RenderTexture.ReleaseTemporary(buffer);
}
else
{
Graphics.Blit(sourceTexture, destTexture);	
}
}
void Update () 
{
#if UNITY_EDITOR
if (Application.isPlaying!=true)
{
SCShader = Shader.Find("CameraFilterPack/FX_8bits_gb");
}
#endif
}
void OnDisable ()
{
if(SCMaterial)
{
DestroyImmediate(SCMaterial);	
}
}
}