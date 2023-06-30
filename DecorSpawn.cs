using System;
using UnityEngine;

// Token: 0x0200066F RID: 1647
public class DecorSpawn : MonoBehaviour, IClientComponent
{
	// Token: 0x0400273D RID: 10045
	public SpawnFilter Filter;

	// Token: 0x0400273E RID: 10046
	public string ResourceFolder = string.Empty;

	// Token: 0x0400273F RID: 10047
	public uint Seed;

	// Token: 0x04002740 RID: 10048
	public float ObjectCutoff = 0.2f;

	// Token: 0x04002741 RID: 10049
	public float ObjectTapering = 0.2f;

	// Token: 0x04002742 RID: 10050
	public int ObjectsPerPatch = 10;

	// Token: 0x04002743 RID: 10051
	public float ClusterRadius = 2f;

	// Token: 0x04002744 RID: 10052
	public int ClusterSizeMin = 1;

	// Token: 0x04002745 RID: 10053
	public int ClusterSizeMax = 10;

	// Token: 0x04002746 RID: 10054
	public int PatchCount = 8;

	// Token: 0x04002747 RID: 10055
	public int PatchSize = 100;

	// Token: 0x04002748 RID: 10056
	public bool LOD = true;
}
