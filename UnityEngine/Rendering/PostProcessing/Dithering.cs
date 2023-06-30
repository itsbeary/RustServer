using System;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A67 RID: 2663
	[Preserve]
	[Serializable]
	internal sealed class Dithering
	{
		// Token: 0x06003F9B RID: 16283 RVA: 0x00176574 File Offset: 0x00174774
		internal void Render(PostProcessRenderContext context)
		{
			Texture2D[] blueNoise = context.resources.blueNoise64;
			Assert.IsTrue(blueNoise != null && blueNoise.Length != 0);
			int num = this.m_NoiseTextureIndex + 1;
			this.m_NoiseTextureIndex = num;
			if (num >= blueNoise.Length)
			{
				this.m_NoiseTextureIndex = 0;
			}
			float value = Random.value;
			float value2 = Random.value;
			Texture2D texture2D = blueNoise[this.m_NoiseTextureIndex];
			PropertySheet uberSheet = context.uberSheet;
			uberSheet.properties.SetTexture(ShaderIDs.DitheringTex, texture2D);
			uberSheet.properties.SetVector(ShaderIDs.Dithering_Coords, new Vector4((float)context.screenWidth / (float)texture2D.width, (float)context.screenHeight / (float)texture2D.height, value, value2));
		}

		// Token: 0x0400393E RID: 14654
		private int m_NoiseTextureIndex;
	}
}
