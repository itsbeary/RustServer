using System;
using UnityEngine;

// Token: 0x0200010D RID: 269
public class DropBox : Mailbox
{
	// Token: 0x060015FF RID: 5631 RVA: 0x000ACA15 File Offset: 0x000AAC15
	public override bool PlayerIsOwner(BasePlayer player)
	{
		return this.PlayerBehind(player);
	}

	// Token: 0x06001600 RID: 5632 RVA: 0x000ACA20 File Offset: 0x000AAC20
	public bool PlayerBehind(BasePlayer player)
	{
		return Vector3.Dot(base.transform.forward, (player.transform.position - base.transform.position).normalized) <= -0.3f && GamePhysics.LineOfSight(player.eyes.position, this.EyePoint.position, 2162688, null);
	}

	// Token: 0x06001601 RID: 5633 RVA: 0x000ACA8C File Offset: 0x000AAC8C
	public bool PlayerInfront(BasePlayer player)
	{
		return Vector3.Dot(base.transform.forward, (player.transform.position - base.transform.position).normalized) >= 0.7f;
	}

	// Token: 0x06001602 RID: 5634 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool SupportsChildDeployables()
	{
		return true;
	}

	// Token: 0x04000E66 RID: 3686
	public Transform EyePoint;
}
