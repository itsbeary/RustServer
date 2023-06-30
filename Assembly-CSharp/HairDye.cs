using System;
using UnityEngine;

// Token: 0x02000752 RID: 1874
[Serializable]
public class HairDye
{
	// Token: 0x0600345A RID: 13402 RVA: 0x00143774 File Offset: 0x00141974
	public void Apply(HairDyeCollection collection, MaterialPropertyBlock block)
	{
		if (this.sourceMaterial != null)
		{
			for (int i = 0; i < 8; i++)
			{
				if ((this.copyProperties & (HairDye.CopyPropertyMask)(1 << i)) != (HairDye.CopyPropertyMask)0)
				{
					MaterialPropertyDesc materialPropertyDesc = HairDye.transferableProps[i];
					if (this.sourceMaterial.HasProperty(materialPropertyDesc.nameID))
					{
						if (materialPropertyDesc.type == typeof(Color))
						{
							block.SetColor(materialPropertyDesc.nameID, this.sourceMaterial.GetColor(materialPropertyDesc.nameID));
						}
						else if (materialPropertyDesc.type == typeof(float))
						{
							block.SetFloat(materialPropertyDesc.nameID, this.sourceMaterial.GetFloat(materialPropertyDesc.nameID));
						}
					}
				}
			}
		}
	}

	// Token: 0x0600345B RID: 13403 RVA: 0x00143840 File Offset: 0x00141A40
	public void ApplyCap(HairDyeCollection collection, HairType type, MaterialPropertyBlock block)
	{
		if (collection.applyCap)
		{
			if (type == HairType.Head || type == HairType.Armpit || type == HairType.Pubic)
			{
				block.SetColor(HairDye._HairBaseColorUV1, this.capBaseColor.gamma);
				block.SetTexture(HairDye._HairPackedMapUV1, (collection.capMask != null) ? collection.capMask : Texture2D.blackTexture);
				return;
			}
			if (type == HairType.Facial)
			{
				block.SetColor(HairDye._HairBaseColorUV2, this.capBaseColor.gamma);
				block.SetTexture(HairDye._HairPackedMapUV2, (collection.capMask != null) ? collection.capMask : Texture2D.blackTexture);
			}
		}
	}

	// Token: 0x04002AC0 RID: 10944
	[ColorUsage(false, true)]
	public Color capBaseColor;

	// Token: 0x04002AC1 RID: 10945
	public Material sourceMaterial;

	// Token: 0x04002AC2 RID: 10946
	[InspectorFlags]
	public HairDye.CopyPropertyMask copyProperties;

	// Token: 0x04002AC3 RID: 10947
	private static MaterialPropertyDesc[] transferableProps = new MaterialPropertyDesc[]
	{
		new MaterialPropertyDesc("_DyeColor", typeof(Color)),
		new MaterialPropertyDesc("_RootColor", typeof(Color)),
		new MaterialPropertyDesc("_TipColor", typeof(Color)),
		new MaterialPropertyDesc("_Brightness", typeof(float)),
		new MaterialPropertyDesc("_DyeRoughness", typeof(float)),
		new MaterialPropertyDesc("_DyeScatter", typeof(float)),
		new MaterialPropertyDesc("_HairSpecular", typeof(float)),
		new MaterialPropertyDesc("_HairRoughness", typeof(float))
	};

	// Token: 0x04002AC4 RID: 10948
	private static int _HairBaseColorUV1 = Shader.PropertyToID("_HairBaseColorUV1");

	// Token: 0x04002AC5 RID: 10949
	private static int _HairBaseColorUV2 = Shader.PropertyToID("_HairBaseColorUV2");

	// Token: 0x04002AC6 RID: 10950
	private static int _HairPackedMapUV1 = Shader.PropertyToID("_HairPackedMapUV1");

	// Token: 0x04002AC7 RID: 10951
	private static int _HairPackedMapUV2 = Shader.PropertyToID("_HairPackedMapUV2");

	// Token: 0x02000E71 RID: 3697
	public enum CopyProperty
	{
		// Token: 0x04004BDC RID: 19420
		DyeColor,
		// Token: 0x04004BDD RID: 19421
		RootColor,
		// Token: 0x04004BDE RID: 19422
		TipColor,
		// Token: 0x04004BDF RID: 19423
		Brightness,
		// Token: 0x04004BE0 RID: 19424
		DyeRoughness,
		// Token: 0x04004BE1 RID: 19425
		DyeScatter,
		// Token: 0x04004BE2 RID: 19426
		Specular,
		// Token: 0x04004BE3 RID: 19427
		Roughness,
		// Token: 0x04004BE4 RID: 19428
		Count
	}

	// Token: 0x02000E72 RID: 3698
	[Flags]
	public enum CopyPropertyMask
	{
		// Token: 0x04004BE6 RID: 19430
		DyeColor = 1,
		// Token: 0x04004BE7 RID: 19431
		RootColor = 2,
		// Token: 0x04004BE8 RID: 19432
		TipColor = 4,
		// Token: 0x04004BE9 RID: 19433
		Brightness = 8,
		// Token: 0x04004BEA RID: 19434
		DyeRoughness = 16,
		// Token: 0x04004BEB RID: 19435
		DyeScatter = 32,
		// Token: 0x04004BEC RID: 19436
		Specular = 64,
		// Token: 0x04004BED RID: 19437
		Roughness = 128
	}
}
