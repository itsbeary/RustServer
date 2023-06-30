using System;
using System.Collections.Generic;
using Facepunch.BurstCloth;
using UnityEngine;

// Token: 0x020000FD RID: 253
public class BurstClothHitBoxCollision : BurstCloth, IClientComponent, IPrefabPreProcess
{
	// Token: 0x060015A5 RID: 5541 RVA: 0x000063A5 File Offset: 0x000045A5
	protected override void GatherColliders(List<CapsuleParams> colliders)
	{
	}

	// Token: 0x060015A6 RID: 5542 RVA: 0x000063A5 File Offset: 0x000045A5
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
	}

	// Token: 0x04000DD7 RID: 3543
	[Header("Rust Wearable BurstCloth")]
	public bool UseLocalGravity = true;

	// Token: 0x04000DD8 RID: 3544
	public float GravityStrength = 0.8f;

	// Token: 0x04000DD9 RID: 3545
	public float DefaultLength = 1f;

	// Token: 0x04000DDA RID: 3546
	public float MountedLengthMultiplier;

	// Token: 0x04000DDB RID: 3547
	public float DuckedLengthMultiplier = 0.5f;

	// Token: 0x04000DDC RID: 3548
	public float CorpseLengthMultiplier = 0.2f;

	// Token: 0x04000DDD RID: 3549
	public Transform UpAxis;

	// Token: 0x04000DDE RID: 3550
	[Header("Collision")]
	public Transform ColliderRoot;

	// Token: 0x04000DDF RID: 3551
	[Tooltip("Keywords in bone names which should be ignored for collision")]
	public string[] IgnoreKeywords;
}
