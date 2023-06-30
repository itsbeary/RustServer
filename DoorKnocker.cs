using System;
using Network;
using UnityEngine;

// Token: 0x0200006C RID: 108
public class DoorKnocker : BaseCombatEntity
{
	// Token: 0x06000AA6 RID: 2726 RVA: 0x00061630 File Offset: 0x0005F830
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("DoorKnocker.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000AA7 RID: 2727 RVA: 0x00061670 File Offset: 0x0005F870
	public void Knock(BasePlayer player)
	{
		base.ClientRPC<Vector3>(null, "ClientKnock", player.transform.position);
	}

	// Token: 0x040006EF RID: 1775
	public Animator knocker1;

	// Token: 0x040006F0 RID: 1776
	public Animator knocker2;
}
