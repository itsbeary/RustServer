using System;
using UnityEngine;

// Token: 0x020002F9 RID: 761
[RequireComponent(typeof(Wearable))]
public class WearableHairCap : MonoBehaviour
{
	// Token: 0x06001E62 RID: 7778 RVA: 0x000CEDEC File Offset: 0x000CCFEC
	public void ApplyHairCap(MaterialPropertyBlock block)
	{
		if (this.Type == HairType.Head || this.Type == HairType.Armpit || this.Type == HairType.Pubic)
		{
			Texture texture = block.GetTexture(WearableHairCap._HairPackedMapUV1);
			block.SetColor(WearableHairCap._HairBaseColorUV1, this.BaseColor.gamma);
			block.SetTexture(WearableHairCap._HairPackedMapUV1, (this.Mask != null) ? this.Mask : texture);
			return;
		}
		if (this.Type == HairType.Facial)
		{
			Texture texture2 = block.GetTexture(WearableHairCap._HairPackedMapUV2);
			block.SetColor(WearableHairCap._HairBaseColorUV2, this.BaseColor.gamma);
			block.SetTexture(WearableHairCap._HairPackedMapUV2, (this.Mask != null) ? this.Mask : texture2);
		}
	}

	// Token: 0x04001794 RID: 6036
	public HairType Type;

	// Token: 0x04001795 RID: 6037
	[ColorUsage(false, true)]
	public Color BaseColor = Color.black;

	// Token: 0x04001796 RID: 6038
	public Texture Mask;

	// Token: 0x04001797 RID: 6039
	private static MaterialPropertyBlock block;

	// Token: 0x04001798 RID: 6040
	private static int _HairBaseColorUV1 = Shader.PropertyToID("_HairBaseColorUV1");

	// Token: 0x04001799 RID: 6041
	private static int _HairBaseColorUV2 = Shader.PropertyToID("_HairBaseColorUV2");

	// Token: 0x0400179A RID: 6042
	private static int _HairPackedMapUV1 = Shader.PropertyToID("_HairPackedMapUV1");

	// Token: 0x0400179B RID: 6043
	private static int _HairPackedMapUV2 = Shader.PropertyToID("_HairPackedMapUV2");
}
