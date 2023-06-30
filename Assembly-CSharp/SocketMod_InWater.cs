using System;
using UnityEngine;

// Token: 0x0200027D RID: 637
public class SocketMod_InWater : SocketMod
{
	// Token: 0x06001D0B RID: 7435 RVA: 0x000C8FFC File Offset: 0x000C71FC
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.cyan;
		Gizmos.DrawSphere(Vector3.zero, 0.1f);
	}

	// Token: 0x06001D0C RID: 7436 RVA: 0x000C9028 File Offset: 0x000C7228
	public override bool DoCheck(Construction.Placement place)
	{
		if (WaterLevel.Test(place.position + place.rotation * this.worldPosition - new Vector3(0f, 0.1f, 0f), true, true, null) == this.wantsInWater)
		{
			return true;
		}
		if (this.wantsInWater)
		{
			Construction.lastPlacementError = SocketMod_InWater.WantsWaterPhrase.translated;
		}
		else
		{
			Construction.lastPlacementError = SocketMod_InWater.NoWaterPhrase.translated;
		}
		return false;
	}

	// Token: 0x04001598 RID: 5528
	public bool wantsInWater = true;

	// Token: 0x04001599 RID: 5529
	public static Translate.Phrase WantsWaterPhrase = new Translate.Phrase("error_inwater_wants", "Must be placed in water");

	// Token: 0x0400159A RID: 5530
	public static Translate.Phrase NoWaterPhrase = new Translate.Phrase("error_inwater", "Can't be placed in water");
}
