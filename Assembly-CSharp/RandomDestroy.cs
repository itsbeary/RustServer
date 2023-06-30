using System;
using UnityEngine;

// Token: 0x02000681 RID: 1665
public class RandomDestroy : MonoBehaviour
{
	// Token: 0x06002FFD RID: 12285 RVA: 0x00120B38 File Offset: 0x0011ED38
	protected void Start()
	{
		uint num = base.transform.position.Seed(World.Seed + this.Seed);
		if (SeedRandom.Value(ref num) > this.Probability)
		{
			GameManager.Destroy(this, 0f);
			return;
		}
		GameManager.Destroy(base.gameObject, 0f);
	}

	// Token: 0x0400278D RID: 10125
	public uint Seed;

	// Token: 0x0400278E RID: 10126
	public float Probability = 0.5f;
}
