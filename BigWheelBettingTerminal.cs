using System;
using Network;
using UnityEngine;

// Token: 0x0200004E RID: 78
public class BigWheelBettingTerminal : StorageContainer
{
	// Token: 0x0600087A RID: 2170 RVA: 0x00051D70 File Offset: 0x0004FF70
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BigWheelBettingTerminal.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600087B RID: 2171 RVA: 0x00051DB0 File Offset: 0x0004FFB0
	public new void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(base.transform.TransformPoint(this.seatedPlayerOffset), this.offsetCheckRadius);
		base.OnDrawGizmos();
	}

	// Token: 0x0600087C RID: 2172 RVA: 0x00051DE0 File Offset: 0x0004FFE0
	public bool IsPlayerValid(BasePlayer player)
	{
		if (!player.isMounted || !(player.GetMounted() is BaseChair))
		{
			return false;
		}
		Vector3 vector = base.transform.TransformPoint(this.seatedPlayerOffset);
		return Vector3Ex.Distance2D(player.transform.position, vector) <= this.offsetCheckRadius;
	}

	// Token: 0x0600087D RID: 2173 RVA: 0x00051E32 File Offset: 0x00050032
	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		if (!this.IsPlayerValid(player))
		{
			return false;
		}
		bool flag = base.PlayerOpenLoot(player, panelToOpen, true);
		if (flag)
		{
			this.lastPlayer = player;
		}
		return flag;
	}

	// Token: 0x0600087E RID: 2174 RVA: 0x00051E54 File Offset: 0x00050054
	public bool TrySetBigWheel(BigWheelGame newWheel)
	{
		if (base.isClient)
		{
			return false;
		}
		if (this.bigWheel != null && this.bigWheel != newWheel)
		{
			float num = Vector3.SqrMagnitude(this.bigWheel.transform.position - base.transform.position);
			if (Vector3.SqrMagnitude(newWheel.transform.position - base.transform.position) >= num)
			{
				return false;
			}
			this.bigWheel.RemoveTerminal(this);
		}
		this.bigWheel = newWheel;
		return true;
	}

	// Token: 0x04000593 RID: 1427
	public BigWheelGame bigWheel;

	// Token: 0x04000594 RID: 1428
	public Vector3 seatedPlayerOffset = Vector3.forward;

	// Token: 0x04000595 RID: 1429
	public float offsetCheckRadius = 0.4f;

	// Token: 0x04000596 RID: 1430
	public SoundDefinition winSound;

	// Token: 0x04000597 RID: 1431
	public SoundDefinition loseSound;

	// Token: 0x04000598 RID: 1432
	[NonSerialized]
	public BasePlayer lastPlayer;
}
