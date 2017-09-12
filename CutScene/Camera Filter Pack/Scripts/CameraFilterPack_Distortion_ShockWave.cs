////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Distortion/ShockWave")]
public class CameraFilterPack_Distortion_ShockWave : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
private Vector4 ScreenResolution;
private Material SCMaterial;
[Range(-1.5f, 1.5f)]
public float PosX = 0.5f;
[Range(-1.5f, 1.5f)]
public float PosY = 0.5f;
[Range(0f, 10f)]
public float Speed = 1f;
[Range(0f, 10f)]
private float Size = 1f;

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

SCShader = Shader.Find("CameraFilterPack/Distortion_ShockWave");
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
material.SetFloat("_Value", PosX);
material.SetFloat("_Value2", PosY);
material.SetFloat("_Value3", Speed);
material.SetFloat("_Value4", Size);
material.SetVector("_ScreenResolution",new Vector4(sourceTexture.width,sourceTexture.height,0.0f,0.0f));
Graphics.Blit(sourceTexture, destTexture, material);
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
SCShader = Shader.Find("CameraFilterPack/Distortion_ShockWave");
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
