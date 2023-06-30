using System;
using UnityEngine;

// Token: 0x020006FA RID: 1786
public abstract class TerrainModifier : PrefabAttribute
{
	// Token: 0x06003276 RID: 12918 RVA: 0x00137338 File Offset: 0x00135538
	public void Apply(Vector3 pos, float scale)
	{
		float opacity = this.Opacity;
		float num = scale * this.Radius;
		float num2 = scale * this.Fade;
		this.Apply(pos, opacity, num, num2);
	}

	// Token: 0x06003277 RID: 12919
	protected abstract void Apply(Vector3 position, float opacity, float radius, float fade);

	// Token: 0x06003278 RID: 12920 RVA: 0x00137368 File Offset: 0x00135568
	protected override Type GetIndexedType()
	{
		return typeof(TerrainModifier);
	}

	// Token: 0x04002956 RID: 10582
	public float Opacity = 1f;

	// Token: 0x04002957 RID: 10583
	public float Radius;

	// Token: 0x04002958 RID: 10584
	public float Fade;
}
