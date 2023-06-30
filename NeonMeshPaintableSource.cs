using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200018F RID: 399
public class NeonMeshPaintableSource : MeshPaintableSource
{
	// Token: 0x060017FF RID: 6143 RVA: 0x000B46F4 File Offset: 0x000B28F4
	public override void UpdateMaterials(MaterialPropertyBlock block, Texture2D textureOverride = null, bool forEditing = false, bool isSelected = false)
	{
		base.UpdateMaterials(block, textureOverride, forEditing, false);
		if (forEditing)
		{
			block.SetFloat("_EmissionScale", this.editorEmissionScale);
			block.SetFloat("_Power", (float)(isSelected ? 1 : 0));
			if (!isSelected)
			{
				block.SetColor("_TubeInner", Color.clear);
				block.SetColor("_TubeOuter", Color.clear);
				return;
			}
		}
		else if (this.neonSign != null)
		{
			block.SetFloat("_Power", (float)((isSelected && this.neonSign.HasFlag(BaseEntity.Flags.Reserved8)) ? 1 : 0));
		}
	}

	// Token: 0x06001800 RID: 6144 RVA: 0x000B478C File Offset: 0x000B298C
	public override Color32[] UpdateFrom(Texture2D input)
	{
		NeonMeshPaintableSource.<>c__DisplayClass8_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		base.Init();
		CS$<>8__locals1.pixels = input.GetPixels32();
		this.texture.SetPixels32(CS$<>8__locals1.pixels);
		this.texture.Apply(true, false);
		CS$<>8__locals1.width = input.width;
		int height = input.height;
		int num = CS$<>8__locals1.width / 2;
		int num2 = height / 2;
		this.topLeft = this.<UpdateFrom>g__GetColorForRegion|8_0(0, num2, num, num2, ref CS$<>8__locals1);
		this.topRight = this.<UpdateFrom>g__GetColorForRegion|8_0(num, num2, num, num2, ref CS$<>8__locals1);
		this.bottomLeft = this.<UpdateFrom>g__GetColorForRegion|8_0(0, 0, num, num2, ref CS$<>8__locals1);
		this.bottomRight = this.<UpdateFrom>g__GetColorForRegion|8_0(num, 0, num, num2, ref CS$<>8__locals1);
		return CS$<>8__locals1.pixels;
	}

	// Token: 0x06001802 RID: 6146 RVA: 0x000B4854 File Offset: 0x000B2A54
	[CompilerGenerated]
	private Color <UpdateFrom>g__GetColorForRegion|8_0(int x, int y, int regionWidth, int regionHeight, ref NeonMeshPaintableSource.<>c__DisplayClass8_0 A_5)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		int num4 = y + regionHeight;
		for (int i = y; i < num4; i++)
		{
			int num5 = i * A_5.width + x;
			int num6 = num5 + regionWidth;
			for (int j = num5; j < num6; j++)
			{
				Color32 color = A_5.pixels[j];
				float num7 = (float)color.a / 255f;
				num += (float)color.r * num7;
				num2 += (float)color.g * num7;
				num3 += (float)color.b * num7;
			}
		}
		int num8 = regionWidth * regionHeight * 255;
		return new Color(this.lightingCurve.Evaluate(num / (float)num8), this.lightingCurve.Evaluate(num2 / (float)num8), this.lightingCurve.Evaluate(num3 / (float)num8), 1f);
	}

	// Token: 0x040010BD RID: 4285
	public NeonSign neonSign;

	// Token: 0x040010BE RID: 4286
	public float editorEmissionScale = 2f;

	// Token: 0x040010BF RID: 4287
	public AnimationCurve lightingCurve;

	// Token: 0x040010C0 RID: 4288
	[NonSerialized]
	public Color topLeft;

	// Token: 0x040010C1 RID: 4289
	[NonSerialized]
	public Color topRight;

	// Token: 0x040010C2 RID: 4290
	[NonSerialized]
	public Color bottomLeft;

	// Token: 0x040010C3 RID: 4291
	[NonSerialized]
	public Color bottomRight;
}
