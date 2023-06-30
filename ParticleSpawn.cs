using System;
using UnityEngine;

// Token: 0x02000687 RID: 1671
public class ParticleSpawn : SingletonComponent<ParticleSpawn>, IClientComponent
{
	// Token: 0x170003EA RID: 1002
	// (get) Token: 0x06003006 RID: 12294 RVA: 0x00120D51 File Offset: 0x0011EF51
	// (set) Token: 0x06003007 RID: 12295 RVA: 0x00120D59 File Offset: 0x0011EF59
	public Vector3 Origin { get; private set; }

	// Token: 0x0400279D RID: 10141
	public GameObjectRef[] Prefabs;

	// Token: 0x0400279E RID: 10142
	public int PatchCount = 8;

	// Token: 0x0400279F RID: 10143
	public int PatchSize = 100;
}
