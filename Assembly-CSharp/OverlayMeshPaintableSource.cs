using System;
using UnityEngine;

// Token: 0x020002D5 RID: 725
public class OverlayMeshPaintableSource : MeshPaintableSource
{
	// Token: 0x06001DE3 RID: 7651 RVA: 0x000CCBB0 File Offset: 0x000CADB0
	public override void UpdateMaterials(MaterialPropertyBlock block, Texture2D textureOverride = null, bool forEditing = false, bool isSelected = false)
	{
		base.UpdateMaterials(block, textureOverride, forEditing, isSelected);
		if (this.baseTexture != null)
		{
			float num = (float)this.baseTexture.width / (float)this.baseTexture.height;
			float num2 = (float)(this.texWidth / this.texHeight);
			float num3 = 1f;
			float num4 = 0f;
			float num5 = 1f;
			float num6 = 0f;
			if (num2 <= num)
			{
				float num7 = (float)this.texHeight * num;
				num3 = (float)this.texWidth / num7;
				num4 = (1f - num3) / 2f;
			}
			else
			{
				float num8 = (float)this.texWidth / num;
				num5 = (float)this.texHeight / num8;
				num6 = (1f - num5) / 2f;
			}
			block.SetTexture(this.baseTextureName, this.baseTexture);
			block.SetVector(OverlayMeshPaintableSource.STPrefixed.Get(this.baseTextureName), new Vector4(num3, num5, num4, num6));
			return;
		}
		block.SetTexture(this.baseTextureName, Texture2D.blackTexture);
	}

	// Token: 0x040016E1 RID: 5857
	private static readonly Memoized<string, string> STPrefixed = new Memoized<string, string>((string s) => s + "_ST");

	// Token: 0x040016E2 RID: 5858
	public string baseTextureName = "_Decal1Texture";

	// Token: 0x040016E3 RID: 5859
	[NonSerialized]
	public Texture2D baseTexture;
}
