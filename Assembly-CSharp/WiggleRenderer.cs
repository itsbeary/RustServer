using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000651 RID: 1617
public class WiggleRenderer : PostProcessEffectRenderer<Wiggle>
{
	// Token: 0x06002EDD RID: 11997 RVA: 0x00119DDD File Offset: 0x00117FDD
	public override void Init()
	{
		base.Init();
		this.wiggleShader = Shader.Find("Hidden/PostProcessing/Wiggle");
	}

	// Token: 0x06002EDE RID: 11998 RVA: 0x00119DF8 File Offset: 0x00117FF8
	public override void Render(PostProcessRenderContext context)
	{
		CommandBuffer command = context.command;
		command.BeginSample("Wiggle");
		this.timer += base.settings.speed.value * Time.deltaTime;
		PropertySheet propertySheet = context.propertySheets.Get(this.wiggleShader);
		propertySheet.properties.Clear();
		propertySheet.properties.SetFloat(this.timerProperty, this.timer);
		propertySheet.properties.SetFloat(this.scaleProperty, base.settings.scale.value);
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
		command.EndSample("Wiggle");
	}

	// Token: 0x040026BF RID: 9919
	private int timerProperty = Shader.PropertyToID("_timer");

	// Token: 0x040026C0 RID: 9920
	private int scaleProperty = Shader.PropertyToID("_scale");

	// Token: 0x040026C1 RID: 9921
	private Shader wiggleShader;

	// Token: 0x040026C2 RID: 9922
	private float timer;
}
