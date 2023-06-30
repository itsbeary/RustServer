using System;
using UnityEngine;

// Token: 0x02000476 RID: 1142
public class BuoyancyPoint : MonoBehaviour
{
	// Token: 0x060025D3 RID: 9683 RVA: 0x000EEC14 File Offset: 0x000ECE14
	public void Start()
	{
		this.randomOffset = UnityEngine.Random.Range(0f, 20f);
	}

	// Token: 0x060025D4 RID: 9684 RVA: 0x000EEC2B File Offset: 0x000ECE2B
	public void OnDrawGizmos()
	{
		Gizmos.color = BuoyancyPoint.gizmoColour;
		Gizmos.DrawSphere(base.transform.position, this.size * 0.5f);
	}

	// Token: 0x04001DF8 RID: 7672
	public float buoyancyForce = 10f;

	// Token: 0x04001DF9 RID: 7673
	public float size = 0.1f;

	// Token: 0x04001DFA RID: 7674
	public float waveScale = 0.2f;

	// Token: 0x04001DFB RID: 7675
	public float waveFrequency = 1f;

	// Token: 0x04001DFC RID: 7676
	public bool doSplashEffects = true;

	// Token: 0x04001DFD RID: 7677
	[NonSerialized]
	public float randomOffset;

	// Token: 0x04001DFE RID: 7678
	[NonSerialized]
	public bool wasSubmergedLastFrame;

	// Token: 0x04001DFF RID: 7679
	[NonSerialized]
	public float nexSplashTime;

	// Token: 0x04001E00 RID: 7680
	private static readonly Color gizmoColour = new Color(1f, 0f, 0f, 0.25f);
}
