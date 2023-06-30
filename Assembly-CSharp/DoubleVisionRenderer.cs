using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000637 RID: 1591
public class DoubleVisionRenderer : PostProcessEffectRenderer<DoubleVision>
{
	// Token: 0x06002EAE RID: 11950 RVA: 0x00118526 File Offset: 0x00116726
	public override void Init()
	{
		base.Init();
		this.doubleVisionShader = Shader.Find("Hidden/PostProcessing/DoubleVision");
	}

	// Token: 0x06002EAF RID: 11951 RVA: 0x00118540 File Offset: 0x00116740
	public override void Render(PostProcessRenderContext context)
	{
		CommandBuffer command = context.command;
		command.BeginSample("DoubleVision");
		PropertySheet propertySheet = context.propertySheets.Get(this.doubleVisionShader);
		propertySheet.properties.Clear();
		propertySheet.properties.SetVector(this.displaceProperty, base.settings.displace.value);
		propertySheet.properties.SetFloat(this.amountProperty, base.settings.amount.value);
		command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
		command.EndSample("DoubleVision");
	}

	// Token: 0x04002667 RID: 9831
	private int displaceProperty = Shader.PropertyToID("_displace");

	// Token: 0x04002668 RID: 9832
	private int amountProperty = Shader.PropertyToID("_amount");

	// Token: 0x04002669 RID: 9833
	private Shader doubleVisionShader;
}
