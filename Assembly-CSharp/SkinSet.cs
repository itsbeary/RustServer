using System;
using UnityEngine;

// Token: 0x02000763 RID: 1891
[CreateAssetMenu(menuName = "Rust/Skin Set")]
public class SkinSet : ScriptableObject
{
	// Token: 0x06003497 RID: 13463 RVA: 0x00144D54 File Offset: 0x00142F54
	internal Color GetSkinColor(float skinNumber)
	{
		return this.SkinColour.Evaluate(skinNumber);
	}

	// Token: 0x04002B1D RID: 11037
	public string Label;

	// Token: 0x04002B1E RID: 11038
	public Gradient SkinColour;

	// Token: 0x04002B1F RID: 11039
	public HairSetCollection HairCollection;

	// Token: 0x04002B20 RID: 11040
	[Header("Models")]
	public GameObjectRef Head;

	// Token: 0x04002B21 RID: 11041
	public GameObjectRef Torso;

	// Token: 0x04002B22 RID: 11042
	public GameObjectRef Legs;

	// Token: 0x04002B23 RID: 11043
	public GameObjectRef Feet;

	// Token: 0x04002B24 RID: 11044
	public GameObjectRef Hands;

	// Token: 0x04002B25 RID: 11045
	[Header("Censored Variants")]
	public GameObjectRef CensoredTorso;

	// Token: 0x04002B26 RID: 11046
	public GameObjectRef CensoredLegs;

	// Token: 0x04002B27 RID: 11047
	[Header("Materials")]
	public Material HeadMaterial;

	// Token: 0x04002B28 RID: 11048
	public Material BodyMaterial;

	// Token: 0x04002B29 RID: 11049
	public Material EyeMaterial;
}
