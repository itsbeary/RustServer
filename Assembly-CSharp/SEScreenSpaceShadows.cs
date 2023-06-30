using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020009A6 RID: 2470
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Sonic Ether/SE Screen-Space Shadows")]
[ExecuteInEditMode]
public class SEScreenSpaceShadows : MonoBehaviour
{
	// Token: 0x0400358E RID: 13710
	private CommandBuffer blendShadowsCommandBuffer;

	// Token: 0x0400358F RID: 13711
	private CommandBuffer renderShadowsCommandBuffer;

	// Token: 0x04003590 RID: 13712
	private Camera attachedCamera;

	// Token: 0x04003591 RID: 13713
	public Light sun;

	// Token: 0x04003592 RID: 13714
	[Range(0f, 1f)]
	public float blendStrength = 1f;

	// Token: 0x04003593 RID: 13715
	[Range(0f, 1f)]
	public float accumulation = 0.9f;

	// Token: 0x04003594 RID: 13716
	[Range(0.1f, 5f)]
	public float lengthFade = 0.7f;

	// Token: 0x04003595 RID: 13717
	[Range(0.01f, 5f)]
	public float range = 0.7f;

	// Token: 0x04003596 RID: 13718
	[Range(0f, 1f)]
	public float zThickness = 0.1f;

	// Token: 0x04003597 RID: 13719
	[Range(2f, 92f)]
	public int samples = 32;

	// Token: 0x04003598 RID: 13720
	[Range(0.5f, 4f)]
	public float nearSampleQuality = 1.5f;

	// Token: 0x04003599 RID: 13721
	[Range(0f, 1f)]
	public float traceBias = 0.03f;

	// Token: 0x0400359A RID: 13722
	public bool stochasticSampling = true;

	// Token: 0x0400359B RID: 13723
	public bool leverageTemporalAA;

	// Token: 0x0400359C RID: 13724
	public bool bilateralBlur = true;

	// Token: 0x0400359D RID: 13725
	[Range(1f, 2f)]
	public int blurPasses = 1;

	// Token: 0x0400359E RID: 13726
	[Range(0.01f, 0.5f)]
	public float blurDepthTolerance = 0.1f;
}
