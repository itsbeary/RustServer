using System;
using UnityEngine;

// Token: 0x02000931 RID: 2353
public class ClimateBlendTexture : ProcessedTexture
{
	// Token: 0x06003863 RID: 14435 RVA: 0x0014F915 File Offset: 0x0014DB15
	public ClimateBlendTexture(int width, int height, bool linear = true)
	{
		this.material = base.CreateMaterial("Hidden/ClimateBlendLUTs");
		this.result = base.CreateRenderTexture("Climate Blend Texture", width, height, linear);
		this.result.wrapMode = TextureWrapMode.Clamp;
	}

	// Token: 0x06003864 RID: 14436 RVA: 0x0014F94E File Offset: 0x0014DB4E
	public bool CheckLostData()
	{
		if (!this.result.IsCreated())
		{
			this.result.Create();
			return true;
		}
		return false;
	}

	// Token: 0x06003865 RID: 14437 RVA: 0x0014F96C File Offset: 0x0014DB6C
	public void Blend(Texture srcLut1, Texture dstLut1, float lerpLut1, Texture srcLut2, Texture dstLut2, float lerpLut2, float lerp, ClimateBlendTexture prevLut, float time)
	{
		this.material.SetTexture("_srcLut1", srcLut1);
		this.material.SetTexture("_dstLut1", dstLut1);
		this.material.SetTexture("_srcLut2", srcLut2);
		this.material.SetTexture("_dstLut2", dstLut2);
		this.material.SetTexture("_prevLut", prevLut);
		this.material.SetFloat("_lerpLut1", lerpLut1);
		this.material.SetFloat("_lerpLut2", lerpLut2);
		this.material.SetFloat("_lerp", lerp);
		this.material.SetFloat("_time", time);
		Graphics.Blit(null, this.result, this.material);
	}

	// Token: 0x06003866 RID: 14438 RVA: 0x0014FA30 File Offset: 0x0014DC30
	public static void Swap(ref ClimateBlendTexture a, ref ClimateBlendTexture b)
	{
		ClimateBlendTexture climateBlendTexture = a;
		a = b;
		b = climateBlendTexture;
	}
}
