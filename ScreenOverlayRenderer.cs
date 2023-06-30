using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200064B RID: 1611
public class ScreenOverlayRenderer : PostProcessEffectRenderer<ScreenOverlay>
{
	// Token: 0x06002ED2 RID: 11986 RVA: 0x001198A3 File Offset: 0x00117AA3
	public override void Init()
	{
		base.Init();
		this.overlayShader = Shader.Find("Hidden/PostProcessing/ScreenOverlay");
	}

	// Token: 0x06002ED3 RID: 11987 RVA: 0x001198BC File Offset: 0x00117ABC
	public override void Render(PostProcessRenderContext context)
	{
		CommandBuffer command = context.command;
		command.BeginSample("ScreenOverlay");
		PropertySheet propertySheet = context.propertySheets.Get(this.overlayShader);
		propertySheet.properties.Clear();
		Vector4 vector = new Vector4(1f, 0f, 0f, 1f);
		propertySheet.properties.SetVector("_UV_Transform", vector);
		propertySheet.properties.SetFloat("_Intensity", base.settings.intensity);
		if (TOD_Sky.Instance)
		{
			propertySheet.properties.SetVector("_LightDir", context.camera.transform.InverseTransformDirection(TOD_Sky.Instance.LightDirection));
			propertySheet.properties.SetColor("_LightCol", TOD_Sky.Instance.LightColor * TOD_Sky.Instance.LightIntensity);
		}
		if (base.settings.texture.value)
		{
			propertySheet.properties.SetTexture("_Overlay", base.settings.texture.value);
		}
		if (base.settings.normals.value)
		{
			propertySheet.properties.SetTexture("_Normals", base.settings.normals.value);
		}
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, (int)base.settings.blendMode.value, false, null);
		command.EndSample("ScreenOverlay");
	}

	// Token: 0x040026AB RID: 9899
	private Shader overlayShader;
}
