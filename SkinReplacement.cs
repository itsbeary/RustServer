using System;

// Token: 0x02000762 RID: 1890
[Serializable]
public class SkinReplacement
{
	// Token: 0x04002B1B RID: 11035
	public SkinReplacement.SkinType skinReplacementType;

	// Token: 0x04002B1C RID: 11036
	public GameObjectRef targetReplacement;

	// Token: 0x02000E7F RID: 3711
	public enum SkinType
	{
		// Token: 0x04004C0E RID: 19470
		NONE,
		// Token: 0x04004C0F RID: 19471
		Hands,
		// Token: 0x04004C10 RID: 19472
		Head,
		// Token: 0x04004C11 RID: 19473
		Feet,
		// Token: 0x04004C12 RID: 19474
		Torso,
		// Token: 0x04004C13 RID: 19475
		Legs
	}
}
