using System;
using UnityEngine;

// Token: 0x020002C7 RID: 711
public class Gib : ListComponent<Gib>
{
	// Token: 0x06001DB9 RID: 7609 RVA: 0x000CC454 File Offset: 0x000CA654
	public static string GetEffect(PhysicMaterial physicMaterial)
	{
		string nameLower = physicMaterial.GetNameLower();
		if (nameLower == "wood")
		{
			return "assets/bundled/prefabs/fx/building/wood_gib.prefab";
		}
		if (nameLower == "concrete")
		{
			return "assets/bundled/prefabs/fx/building/stone_gib.prefab";
		}
		if (nameLower == "metal")
		{
			return "assets/bundled/prefabs/fx/building/metal_sheet_gib.prefab";
		}
		if (nameLower == "rock")
		{
			return "assets/bundled/prefabs/fx/building/stone_gib.prefab";
		}
		if (!(nameLower == "flesh"))
		{
			return "assets/bundled/prefabs/fx/building/wood_gib.prefab";
		}
		return "assets/bundled/prefabs/fx/building/wood_gib.prefab";
	}

	// Token: 0x0400167A RID: 5754
	public static int gibCount;

	// Token: 0x0400167B RID: 5755
	public MeshFilter _meshFilter;

	// Token: 0x0400167C RID: 5756
	public MeshRenderer _meshRenderer;

	// Token: 0x0400167D RID: 5757
	public MeshCollider _meshCollider;

	// Token: 0x0400167E RID: 5758
	public BoxCollider _boxCollider;

	// Token: 0x0400167F RID: 5759
	public SphereCollider _sphereCollider;

	// Token: 0x04001680 RID: 5760
	public CapsuleCollider _capsuleCollider;

	// Token: 0x04001681 RID: 5761
	public Rigidbody _rigidbody;
}
