using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000643 RID: 1603
public class GreyScaleRenderer : PostProcessEffectRenderer<GreyScale>
{
	// Token: 0x06002EC5 RID: 11973 RVA: 0x001190D9 File Offset: 0x001172D9
	public override void Init()
	{
		base.Init();
		this.greyScaleShader = Shader.Find("Hidden/PostProcessing/GreyScale");
	}

	// Token: 0x06002EC6 RID: 11974 RVA: 0x001190F4 File Offset: 0x001172F4
	public override void Render(PostProcessRenderContext context)
	{
		CommandBuffer command = context.command;
		command.BeginSample("GreyScale");
		PropertySheet propertySheet = context.propertySheets.Get(this.greyScaleShader);
		propertySheet.properties.Clear();
		propertySheet.properties.SetVector(this.dataProperty, new Vector4(base.settings.redLuminance.value, base.settings.greenLuminance.value, base.settings.blueLuminance.value, base.settings.amount.value));
		propertySheet.properties.SetColor(this.colorProperty, base.settings.color.value);
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
		command.EndSample("GreyScale");
	}

	// Token: 0x0400268F RID: 9871
	private int dataProperty = Shader.PropertyToID("_data");

	// Token: 0x04002690 RID: 9872
	private int colorProperty = Shader.PropertyToID("_color");

	// Token: 0x04002691 RID: 9873
	private Shader greyScaleShader;
}
