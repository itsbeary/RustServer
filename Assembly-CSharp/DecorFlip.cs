using System;
using UnityEngine;

// Token: 0x02000666 RID: 1638
public class DecorFlip : DecorComponent
{
	// Token: 0x06002FC1 RID: 12225 RVA: 0x0011FD7C File Offset: 0x0011DF7C
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		uint num = pos.Seed(World.Seed) + 4U;
		if (SeedRandom.Value(ref num) > 0.5f)
		{
			return;
		}
		switch (this.FlipAxis)
		{
		case DecorFlip.AxisType.X:
		case DecorFlip.AxisType.Z:
			rot = Quaternion.AngleAxis(180f, rot * Vector3.up) * rot;
			return;
		case DecorFlip.AxisType.Y:
			rot = Quaternion.AngleAxis(180f, rot * Vector3.forward) * rot;
			return;
		default:
			return;
		}
	}

	// Token: 0x04002733 RID: 10035
	public DecorFlip.AxisType FlipAxis = DecorFlip.AxisType.Y;

	// Token: 0x02000DD4 RID: 3540
	public enum AxisType
	{
		// Token: 0x040049B0 RID: 18864
		X,
		// Token: 0x040049B1 RID: 18865
		Y,
		// Token: 0x040049B2 RID: 18866
		Z
	}
}
