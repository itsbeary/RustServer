using System;
using UnityEngine;

// Token: 0x02000685 RID: 1669
public class RandomStaticPrefab : MonoBehaviour
{
	// Token: 0x06003003 RID: 12291 RVA: 0x00120CBC File Offset: 0x0011EEBC
	protected void Start()
	{
		uint num = base.transform.position.Seed(World.Seed + this.Seed);
		if (SeedRandom.Value(ref num) > this.Probability)
		{
			GameManager.Destroy(this, 0f);
			return;
		}
		Prefab.LoadRandom("assets/bundled/prefabs/autospawn/" + this.ResourceFolder, ref num, null, null, true).Spawn(base.transform, true);
		GameManager.Destroy(this, 0f);
	}

	// Token: 0x0400279A RID: 10138
	public uint Seed;

	// Token: 0x0400279B RID: 10139
	public float Probability = 0.5f;

	// Token: 0x0400279C RID: 10140
	public string ResourceFolder = string.Empty;
}
