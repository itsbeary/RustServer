using System;
using UnityEngine;

// Token: 0x0200001B RID: 27
public class PatternFireworkShell : FireworkShell
{
	// Token: 0x04000060 RID: 96
	public GameObjectRef StarPrefab;

	// Token: 0x04000061 RID: 97
	public AnimationCurve StarCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000062 RID: 98
	public float Duration = 3f;

	// Token: 0x04000063 RID: 99
	public float Scale = 5f;

	// Token: 0x04000064 RID: 100
	[Header("Random Design")]
	[MinMax(0f, 1f)]
	public MinMax RandomSaturation = new MinMax(0f, 0.5f);

	// Token: 0x04000065 RID: 101
	[MinMax(0f, 1f)]
	public MinMax RandomValue = new MinMax(0.5f, 0.75f);
}
