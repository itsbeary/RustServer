using System;
using UnityEngine;

// Token: 0x020005C6 RID: 1478
[ExecuteInEditMode]
public class MaterialOverlay : MonoBehaviour
{
	// Token: 0x06002C79 RID: 11385 RVA: 0x0010D8A0 File Offset: 0x0010BAA0
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.material)
		{
			Graphics.Blit(source, destination);
			return;
		}
		for (int i = 0; i < this.material.passCount; i++)
		{
			Graphics.Blit(source, destination, this.material, i);
		}
	}

	// Token: 0x0400244D RID: 9293
	public Material material;
}
