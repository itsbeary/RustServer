using System;
using UnityEngine;

// Token: 0x02000282 RID: 642
public class SocketMod_WaterDepth : SocketMod
{
	// Token: 0x06001D1C RID: 7452 RVA: 0x000C9620 File Offset: 0x000C7820
	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 vector = place.position + place.rotation * this.worldPosition;
		vector.y = WaterSystem.GetHeight(vector) - 0.1f;
		float overallWaterDepth = WaterLevel.GetOverallWaterDepth(vector, false, this.AllowWaterVolumes, null, true);
		if (overallWaterDepth > this.MinimumWaterDepth && overallWaterDepth < this.MaximumWaterDepth)
		{
			return true;
		}
		if (overallWaterDepth <= this.MinimumWaterDepth)
		{
			Construction.lastPlacementError = SocketMod_WaterDepth.TooShallowPhrase.translated;
		}
		else
		{
			Construction.lastPlacementError = SocketMod_WaterDepth.TooDeepPhrase.translated;
		}
		return false;
	}

	// Token: 0x040015A7 RID: 5543
	public float MinimumWaterDepth = 2f;

	// Token: 0x040015A8 RID: 5544
	public float MaximumWaterDepth = 4f;

	// Token: 0x040015A9 RID: 5545
	public bool AllowWaterVolumes;

	// Token: 0x040015AA RID: 5546
	public static Translate.Phrase TooDeepPhrase = new Translate.Phrase("error_toodeep", "Water is too deep");

	// Token: 0x040015AB RID: 5547
	public static Translate.Phrase TooShallowPhrase = new Translate.Phrase("error_shallow", "Water is too shallow");
}
