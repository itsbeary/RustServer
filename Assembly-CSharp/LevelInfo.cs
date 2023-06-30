using System;
using UnityEngine;

// Token: 0x02000553 RID: 1363
public class LevelInfo : SingletonComponent<LevelInfo>
{
	// Token: 0x0400228C RID: 8844
	public string shortName;

	// Token: 0x0400228D RID: 8845
	public string displayName;

	// Token: 0x0400228E RID: 8846
	[TextArea]
	public string description;

	// Token: 0x0400228F RID: 8847
	[Tooltip("A background image to be shown when loading the map")]
	public Texture2D image;

	// Token: 0x04002290 RID: 8848
	[Space(10f)]
	[Tooltip("You should incrememnt this version when you make changes to the map that will invalidate old saves")]
	public int version = 1;
}
