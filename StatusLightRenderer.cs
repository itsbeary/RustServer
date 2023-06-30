using System;
using UnityEngine;

// Token: 0x020002EC RID: 748
public class StatusLightRenderer : MonoBehaviour, IClientComponent
{
	// Token: 0x06001E3B RID: 7739 RVA: 0x000CE0E8 File Offset: 0x000CC2E8
	protected void Awake()
	{
		this.propertyBlock = new MaterialPropertyBlock();
		this.targetRenderer = base.GetComponent<Renderer>();
		this.targetLight = base.GetComponent<Light>();
		this.colorID = Shader.PropertyToID("_Color");
		this.emissionID = Shader.PropertyToID("_EmissionColor");
	}

	// Token: 0x06001E3C RID: 7740 RVA: 0x000CE138 File Offset: 0x000CC338
	public void SetOff()
	{
		if (this.targetRenderer)
		{
			this.targetRenderer.sharedMaterial = this.offMaterial;
			this.targetRenderer.SetPropertyBlock(null);
		}
		if (this.targetLight)
		{
			this.targetLight.color = Color.clear;
		}
	}

	// Token: 0x06001E3D RID: 7741 RVA: 0x000CE18C File Offset: 0x000CC38C
	public void SetOn()
	{
		if (this.targetRenderer)
		{
			this.targetRenderer.sharedMaterial = this.onMaterial;
			this.targetRenderer.SetPropertyBlock(this.propertyBlock);
		}
		if (this.targetLight)
		{
			this.targetLight.color = this.lightColor;
		}
	}

	// Token: 0x06001E3E RID: 7742 RVA: 0x000CE1E8 File Offset: 0x000CC3E8
	public void SetRed()
	{
		this.propertyBlock.Clear();
		this.propertyBlock.SetColor(this.colorID, this.GetColor(197, 46, 0, byte.MaxValue));
		this.propertyBlock.SetColor(this.emissionID, this.GetColor(191, 0, 2, byte.MaxValue, 2.916925f));
		this.lightColor = this.GetColor(byte.MaxValue, 111, 102, byte.MaxValue);
		this.SetOn();
	}

	// Token: 0x06001E3F RID: 7743 RVA: 0x000CE26C File Offset: 0x000CC46C
	public void SetGreen()
	{
		this.propertyBlock.Clear();
		this.propertyBlock.SetColor(this.colorID, this.GetColor(19, 191, 13, byte.MaxValue));
		this.propertyBlock.SetColor(this.emissionID, this.GetColor(19, 191, 13, byte.MaxValue, 2.5f));
		this.lightColor = this.GetColor(156, byte.MaxValue, 102, byte.MaxValue);
		this.SetOn();
	}

	// Token: 0x06001E40 RID: 7744 RVA: 0x000CE2F6 File Offset: 0x000CC4F6
	private Color GetColor(byte r, byte g, byte b, byte a)
	{
		return new Color32(r, g, b, a);
	}

	// Token: 0x06001E41 RID: 7745 RVA: 0x000CE307 File Offset: 0x000CC507
	private Color GetColor(byte r, byte g, byte b, byte a, float intensity)
	{
		return new Color32(r, g, b, a) * intensity;
	}

	// Token: 0x04001777 RID: 6007
	public Material offMaterial;

	// Token: 0x04001778 RID: 6008
	public Material onMaterial;

	// Token: 0x04001779 RID: 6009
	private MaterialPropertyBlock propertyBlock;

	// Token: 0x0400177A RID: 6010
	private Renderer targetRenderer;

	// Token: 0x0400177B RID: 6011
	private Color lightColor;

	// Token: 0x0400177C RID: 6012
	private Light targetLight;

	// Token: 0x0400177D RID: 6013
	private int colorID;

	// Token: 0x0400177E RID: 6014
	private int emissionID;
}
