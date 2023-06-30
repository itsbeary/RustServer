using System;
using UnityEngine;

// Token: 0x02000668 RID: 1640
public class DecorRotate : DecorComponent
{
	// Token: 0x06002FC5 RID: 12229 RVA: 0x0011FF14 File Offset: 0x0011E114
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		uint num = pos.Seed(World.Seed) + 2U;
		float num2 = SeedRandom.Range(ref num, this.MinRotation.x, this.MaxRotation.x);
		float num3 = SeedRandom.Range(ref num, this.MinRotation.y, this.MaxRotation.y);
		float num4 = SeedRandom.Range(ref num, this.MinRotation.z, this.MaxRotation.z);
		rot = Quaternion.Euler(num2, num3, num4) * rot;
	}

	// Token: 0x04002736 RID: 10038
	public Vector3 MinRotation = new Vector3(0f, -180f, 0f);

	// Token: 0x04002737 RID: 10039
	public Vector3 MaxRotation = new Vector3(0f, 180f, 0f);
}
