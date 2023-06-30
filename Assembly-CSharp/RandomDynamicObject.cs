using System;
using UnityEngine;

// Token: 0x02000682 RID: 1666
public class RandomDynamicObject : MonoBehaviour, IClientComponent, ILOD
{
	// Token: 0x0400278F RID: 10127
	public uint Seed;

	// Token: 0x04002790 RID: 10128
	public float Distance = 100f;

	// Token: 0x04002791 RID: 10129
	public float Probability = 0.5f;

	// Token: 0x04002792 RID: 10130
	public GameObject[] Candidates;
}
