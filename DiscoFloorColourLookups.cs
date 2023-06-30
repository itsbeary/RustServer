using System;
using UnityEngine;

// Token: 0x0200039F RID: 927
public class DiscoFloorColourLookups : PrefabAttribute, IClientComponent
{
	// Token: 0x060020C1 RID: 8385 RVA: 0x000D88BE File Offset: 0x000D6ABE
	protected override Type GetIndexedType()
	{
		return typeof(DiscoFloorColourLookups);
	}

	// Token: 0x040019A2 RID: 6562
	public float[] InOutLookup;

	// Token: 0x040019A3 RID: 6563
	public float[] RadialLookup;

	// Token: 0x040019A4 RID: 6564
	public float[] RippleLookup;

	// Token: 0x040019A5 RID: 6565
	public float[] CheckerLookup;

	// Token: 0x040019A6 RID: 6566
	public float[] BlockLookup;

	// Token: 0x040019A7 RID: 6567
	public Gradient[] ColourGradients;
}
