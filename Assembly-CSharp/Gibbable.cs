using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200051E RID: 1310
public class Gibbable : PrefabAttribute, IClientComponent
{
	// Token: 0x060029E6 RID: 10726 RVA: 0x00100D0D File Offset: 0x000FEF0D
	protected override Type GetIndexedType()
	{
		return typeof(Gibbable);
	}

	// Token: 0x040021F2 RID: 8690
	public GameObject gibSource;

	// Token: 0x040021F3 RID: 8691
	public Material[] customMaterials;

	// Token: 0x040021F4 RID: 8692
	public GameObject materialSource;

	// Token: 0x040021F5 RID: 8693
	public bool copyMaterialBlock = true;

	// Token: 0x040021F6 RID: 8694
	public bool applyDamageTexture;

	// Token: 0x040021F7 RID: 8695
	public PhysicMaterial physicsMaterial;

	// Token: 0x040021F8 RID: 8696
	public GameObjectRef fxPrefab;

	// Token: 0x040021F9 RID: 8697
	public bool spawnFxPrefab = true;

	// Token: 0x040021FA RID: 8698
	[Tooltip("If enabled, gibs will spawn even though we've hit a gib limit")]
	public bool important;

	// Token: 0x040021FB RID: 8699
	public bool useContinuousCollision;

	// Token: 0x040021FC RID: 8700
	public float explodeScale;

	// Token: 0x040021FD RID: 8701
	public float scaleOverride = 1f;

	// Token: 0x040021FE RID: 8702
	[ReadOnly]
	public int uniqueId;

	// Token: 0x040021FF RID: 8703
	public Gibbable.BoundsEffectType boundsEffectType;

	// Token: 0x04002200 RID: 8704
	public bool isConditional;

	// Token: 0x04002201 RID: 8705
	[ReadOnly]
	public Bounds effectBounds;

	// Token: 0x04002202 RID: 8706
	public List<Gibbable.OverrideMesh> MeshOverrides = new List<Gibbable.OverrideMesh>();

	// Token: 0x04002203 RID: 8707
	public bool UsePerGibWaterCheck;

	// Token: 0x02000D53 RID: 3411
	[Serializable]
	public struct OverrideMesh
	{
		// Token: 0x0400477B RID: 18299
		public bool enabled;

		// Token: 0x0400477C RID: 18300
		public Gibbable.ColliderType ColliderType;

		// Token: 0x0400477D RID: 18301
		public Vector3 BoxSize;

		// Token: 0x0400477E RID: 18302
		public Vector3 ColliderCentre;

		// Token: 0x0400477F RID: 18303
		public float ColliderRadius;

		// Token: 0x04004780 RID: 18304
		public float CapsuleHeight;

		// Token: 0x04004781 RID: 18305
		public int CapsuleDirection;

		// Token: 0x04004782 RID: 18306
		public bool BlockMaterialCopy;
	}

	// Token: 0x02000D54 RID: 3412
	public enum ColliderType
	{
		// Token: 0x04004784 RID: 18308
		Box,
		// Token: 0x04004785 RID: 18309
		Sphere,
		// Token: 0x04004786 RID: 18310
		Capsule
	}

	// Token: 0x02000D55 RID: 3413
	public enum ParentingType
	{
		// Token: 0x04004788 RID: 18312
		None,
		// Token: 0x04004789 RID: 18313
		GibsOnly,
		// Token: 0x0400478A RID: 18314
		FXOnly,
		// Token: 0x0400478B RID: 18315
		All
	}

	// Token: 0x02000D56 RID: 3414
	public enum BoundsEffectType
	{
		// Token: 0x0400478D RID: 18317
		None,
		// Token: 0x0400478E RID: 18318
		Electrical,
		// Token: 0x0400478F RID: 18319
		Glass,
		// Token: 0x04004790 RID: 18320
		Scrap,
		// Token: 0x04004791 RID: 18321
		Stone,
		// Token: 0x04004792 RID: 18322
		Wood
	}
}
