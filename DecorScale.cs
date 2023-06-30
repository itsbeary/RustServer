using System;
using UnityEngine;

// Token: 0x02000669 RID: 1641
public class DecorScale : DecorComponent
{
	// Token: 0x06002FC7 RID: 12231 RVA: 0x0011FFE4 File Offset: 0x0011E1E4
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		uint num = pos.Seed(World.Seed) + 3U;
		float num2 = SeedRandom.Value(ref num);
		scale.x *= Mathf.Lerp(this.MinScale.x, this.MaxScale.x, num2);
		scale.y *= Mathf.Lerp(this.MinScale.y, this.MaxScale.y, num2);
		scale.z *= Mathf.Lerp(this.MinScale.z, this.MaxScale.z, num2);
	}

	// Token: 0x04002738 RID: 10040
	public Vector3 MinScale = new Vector3(1f, 1f, 1f);

	// Token: 0x04002739 RID: 10041
	public Vector3 MaxScale = new Vector3(2f, 2f, 2f);
}
