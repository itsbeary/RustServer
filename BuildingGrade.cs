using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200074F RID: 1871
[CreateAssetMenu(menuName = "Rust/Building Grade")]
public class BuildingGrade : ScriptableObject
{
	// Token: 0x04002AAA RID: 10922
	public BuildingGrade.Enum type;

	// Token: 0x04002AAB RID: 10923
	public ulong skin;

	// Token: 0x04002AAC RID: 10924
	public bool enabledInStandalone;

	// Token: 0x04002AAD RID: 10925
	public float baseHealth;

	// Token: 0x04002AAE RID: 10926
	public List<ItemAmount> baseCost;

	// Token: 0x04002AAF RID: 10927
	public PhysicMaterial physicMaterial;

	// Token: 0x04002AB0 RID: 10928
	public ProtectionProperties damageProtecton;

	// Token: 0x04002AB1 RID: 10929
	public bool supportsColourChange;

	// Token: 0x04002AB2 RID: 10930
	public BaseEntity.Menu.Option upgradeMenu;

	// Token: 0x02000E68 RID: 3688
	public enum Enum
	{
		// Token: 0x04004BBE RID: 19390
		None = -1,
		// Token: 0x04004BBF RID: 19391
		Twigs,
		// Token: 0x04004BC0 RID: 19392
		Wood,
		// Token: 0x04004BC1 RID: 19393
		Stone,
		// Token: 0x04004BC2 RID: 19394
		Metal,
		// Token: 0x04004BC3 RID: 19395
		TopTier,
		// Token: 0x04004BC4 RID: 19396
		Count
	}
}
