using System;
using UnityEngine;

// Token: 0x020001CE RID: 462
public class LaserBeam : MonoBehaviour
{
	// Token: 0x040011F0 RID: 4592
	public float scrollSpeed = 0.5f;

	// Token: 0x040011F1 RID: 4593
	public LineRenderer beamRenderer;

	// Token: 0x040011F2 RID: 4594
	public GameObject dotObject;

	// Token: 0x040011F3 RID: 4595
	public Renderer dotRenderer;

	// Token: 0x040011F4 RID: 4596
	public GameObject dotSpotlight;

	// Token: 0x040011F5 RID: 4597
	public Vector2 scrollDir;

	// Token: 0x040011F6 RID: 4598
	public float maxDistance = 100f;

	// Token: 0x040011F7 RID: 4599
	public float stillBlendFactor = 0.1f;

	// Token: 0x040011F8 RID: 4600
	public float movementBlendFactor = 0.5f;

	// Token: 0x040011F9 RID: 4601
	public float movementThreshhold = 0.15f;

	// Token: 0x040011FA RID: 4602
	public bool isFirstPerson;

	// Token: 0x040011FB RID: 4603
	public Transform emissionOverride;
}
