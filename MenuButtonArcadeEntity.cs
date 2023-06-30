using System;

// Token: 0x02000153 RID: 339
public class MenuButtonArcadeEntity : TextArcadeEntity
{
	// Token: 0x06001743 RID: 5955 RVA: 0x000B11BD File Offset: 0x000AF3BD
	public bool IsHighlighted()
	{
		return this.alpha == 1f;
	}

	// Token: 0x04000FD2 RID: 4050
	public string titleText = "";

	// Token: 0x04000FD3 RID: 4051
	public string selectionSuffix = " - ";

	// Token: 0x04000FD4 RID: 4052
	public string clickMessage = "";
}
