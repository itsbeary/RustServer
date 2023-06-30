using System;
using UnityEngine;

// Token: 0x020004E2 RID: 1250
public class NoPlayersIOReset : FacepunchBehaviour
{
	// Token: 0x06002898 RID: 10392 RVA: 0x000FAFDB File Offset: 0x000F91DB
	protected void OnEnable()
	{
		base.InvokeRandomized(new Action(this.Check), this.timeBetweenChecks, this.timeBetweenChecks, this.timeBetweenChecks * 0.1f);
	}

	// Token: 0x06002899 RID: 10393 RVA: 0x000FB007 File Offset: 0x000F9207
	protected void OnDisable()
	{
		base.CancelInvoke(new Action(this.Check));
	}

	// Token: 0x0600289A RID: 10394 RVA: 0x000FB01B File Offset: 0x000F921B
	private void Check()
	{
		if (!PuzzleReset.AnyPlayersWithinDistance(base.transform, this.radius))
		{
			this.Reset();
		}
	}

	// Token: 0x0600289B RID: 10395 RVA: 0x000FB038 File Offset: 0x000F9238
	private void Reset()
	{
		foreach (IOEntity ioentity in this.entitiesToReset)
		{
			if (ioentity.IsValid() && ioentity.isServer)
			{
				ioentity.ResetIOState();
				ioentity.MarkDirty();
			}
		}
	}

	// Token: 0x040020E5 RID: 8421
	[SerializeField]
	private IOEntity[] entitiesToReset;

	// Token: 0x040020E6 RID: 8422
	[SerializeField]
	private float radius;

	// Token: 0x040020E7 RID: 8423
	[SerializeField]
	private float timeBetweenChecks;
}
