using System;
using UnityEngine;

// Token: 0x02000198 RID: 408
public class ProceduralDungeonCell : BaseMonoBehaviour
{
	// Token: 0x06001844 RID: 6212 RVA: 0x000B5E74 File Offset: 0x000B4074
	public void Awake()
	{
		this.spawnGroups = base.GetComponentsInChildren<SpawnGroup>();
	}

	// Token: 0x04001110 RID: 4368
	public bool north;

	// Token: 0x04001111 RID: 4369
	public bool east;

	// Token: 0x04001112 RID: 4370
	public bool south;

	// Token: 0x04001113 RID: 4371
	public bool west;

	// Token: 0x04001114 RID: 4372
	public bool entrance;

	// Token: 0x04001115 RID: 4373
	public bool hasSpawn;

	// Token: 0x04001116 RID: 4374
	public Transform exitPointHack;

	// Token: 0x04001117 RID: 4375
	public SpawnGroup[] spawnGroups;

	// Token: 0x04001118 RID: 4376
	public MeshRenderer[] mapRenderers;
}
