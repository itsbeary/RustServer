using System;
using UnityEngine;

// Token: 0x0200025A RID: 602
public class ConstructionSkin_CustomDetail : ConstructionSkin
{
	// Token: 0x06001C90 RID: 7312 RVA: 0x000C6C54 File Offset: 0x000C4E54
	public override uint GetStartingDetailColour(uint playerColourIndex)
	{
		if (playerColourIndex > 0U)
		{
			return (uint)Mathf.Clamp(playerColourIndex, 1f, (float)(this.ColourLookup.AllColours.Length + 1));
		}
		return (uint)UnityEngine.Random.Range(1, this.ColourLookup.AllColours.Length + 1);
	}

	// Token: 0x0400152B RID: 5419
	public ConstructionSkin_ColourLookup ColourLookup;
}
