using System;
using UnityEngine;

// Token: 0x02000683 RID: 1667
public class RandomDynamicPrefab : MonoBehaviour, IClientComponent, ILOD
{
	// Token: 0x04002793 RID: 10131
	public uint Seed;

	// Token: 0x04002794 RID: 10132
	public float Distance = 100f;

	// Token: 0x04002795 RID: 10133
	public float Probability = 0.5f;

	// Token: 0x04002796 RID: 10134
	public string ResourceFolder = string.Empty;
}
