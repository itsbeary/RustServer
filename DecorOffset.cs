using System;
using UnityEngine;

// Token: 0x02000667 RID: 1639
public class DecorOffset : DecorComponent
{
	// Token: 0x06002FC3 RID: 12227 RVA: 0x0011FE2C File Offset: 0x0011E02C
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		uint num = pos.Seed(World.Seed) + 1U;
		pos.x += scale.x * SeedRandom.Range(ref num, this.MinOffset.x, this.MaxOffset.x);
		pos.y += scale.y * SeedRandom.Range(ref num, this.MinOffset.y, this.MaxOffset.y);
		pos.z += scale.z * SeedRandom.Range(ref num, this.MinOffset.z, this.MaxOffset.z);
	}

	// Token: 0x04002734 RID: 10036
	public Vector3 MinOffset = new Vector3(0f, 0f, 0f);

	// Token: 0x04002735 RID: 10037
	public Vector3 MaxOffset = new Vector3(0f, 0f, 0f);
}
