using System;
using UnityEngine;

// Token: 0x02000753 RID: 1875
[CreateAssetMenu(menuName = "Rust/Hair Dye Collection")]
public class HairDyeCollection : ScriptableObject
{
	// Token: 0x0600345E RID: 13406 RVA: 0x00143A0C File Offset: 0x00141C0C
	public HairDye Get(float seed)
	{
		if (this.Variations.Length != 0)
		{
			return this.Variations[Mathf.Clamp(Mathf.FloorToInt(seed * (float)this.Variations.Length), 0, this.Variations.Length - 1)];
		}
		return null;
	}

	// Token: 0x04002AC8 RID: 10952
	public Texture capMask;

	// Token: 0x04002AC9 RID: 10953
	public bool applyCap;

	// Token: 0x04002ACA RID: 10954
	public HairDye[] Variations;
}
