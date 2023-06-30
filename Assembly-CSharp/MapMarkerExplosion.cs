using System;
using UnityEngine;

// Token: 0x020001BB RID: 443
public class MapMarkerExplosion : MapMarker
{
	// Token: 0x06001921 RID: 6433 RVA: 0x000B96C0 File Offset: 0x000B78C0
	public void SetDuration(float newDuration)
	{
		this.duration = newDuration;
		if (base.IsInvoking(new Action(this.DelayedDestroy)))
		{
			base.CancelInvoke(new Action(this.DelayedDestroy));
		}
		base.Invoke(new Action(this.DelayedDestroy), this.duration * 60f);
	}

	// Token: 0x06001922 RID: 6434 RVA: 0x000B9718 File Offset: 0x000B7918
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk)
		{
			Debug.LogWarning("Loaded explosion marker from disk, cleaning up");
			base.Invoke(new Action(this.DelayedDestroy), 3f);
		}
	}

	// Token: 0x06001923 RID: 6435 RVA: 0x00003384 File Offset: 0x00001584
	public void DelayedDestroy()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x040011B6 RID: 4534
	private float duration = 10f;
}
