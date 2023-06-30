using System;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000055 RID: 85
public class CardReader : IOEntity
{
	// Token: 0x0600093A RID: 2362 RVA: 0x00058120 File Offset: 0x00056320
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CardReader.OnRpcMessage", 0))
		{
			if (rpc == 979061374U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerCardSwiped ");
				}
				using (TimeWarning.New("ServerCardSwiped", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(979061374U, "ServerCardSwiped", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ServerCardSwiped(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ServerCardSwiped");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600093B RID: 2363 RVA: 0x00058288 File Offset: 0x00056488
	public override void ResetIOState()
	{
		base.ResetIOState();
		base.CancelInvoke(new Action(this.GrantCard));
		base.CancelInvoke(new Action(this.CancelAccess));
		this.CancelAccess();
	}

	// Token: 0x0600093C RID: 2364 RVA: 0x000582BA File Offset: 0x000564BA
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return base.GetPassthroughAmount(outputSlot);
	}

	// Token: 0x0600093D RID: 2365 RVA: 0x000582CD File Offset: 0x000564CD
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
	}

	// Token: 0x0600093E RID: 2366 RVA: 0x000582D7 File Offset: 0x000564D7
	public void CancelAccess()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		this.MarkDirty();
	}

	// Token: 0x0600093F RID: 2367 RVA: 0x000582E9 File Offset: 0x000564E9
	public void FailCard()
	{
		Effect.server.Run(this.accessDeniedEffect.resourcePath, this.audioPosition.position, Vector3.up, null, false);
	}

	// Token: 0x06000940 RID: 2368 RVA: 0x00058310 File Offset: 0x00056510
	public override void ServerInit()
	{
		base.ServerInit();
		base.SetFlag(this.AccessLevel1, this.accessLevel == 1, false, true);
		base.SetFlag(this.AccessLevel2, this.accessLevel == 2, false, true);
		base.SetFlag(this.AccessLevel3, this.accessLevel == 3, false, true);
	}

	// Token: 0x06000941 RID: 2369 RVA: 0x00058368 File Offset: 0x00056568
	public void GrantCard()
	{
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		this.MarkDirty();
		Effect.server.Run(this.accessGrantedEffect.resourcePath, this.audioPosition.position, Vector3.up, null, false);
	}

	// Token: 0x06000942 RID: 2370 RVA: 0x0005839C File Offset: 0x0005659C
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void ServerCardSwiped(BaseEntity.RPCMessage msg)
	{
		if (!this.IsPowered())
		{
			return;
		}
		if (Vector3Ex.Distance2D(msg.player.transform.position, base.transform.position) > 1f)
		{
			return;
		}
		if (base.IsInvoking(new Action(this.GrantCard)) || base.IsInvoking(new Action(this.FailCard)))
		{
			return;
		}
		if (base.HasFlag(BaseEntity.Flags.On))
		{
			return;
		}
		NetworkableId networkableId = msg.read.EntityID();
		Keycard keycard = BaseNetworkable.serverEntities.Find(networkableId) as Keycard;
		Effect.server.Run(this.swipeEffect.resourcePath, this.audioPosition.position, Vector3.up, msg.player.net.connection, false);
		if (keycard != null)
		{
			Item item = keycard.GetItem();
			if (item != null && keycard.accessLevel == this.accessLevel && item.conditionNormalized > 0f)
			{
				Analytics.Azure.OnKeycardSwiped(msg.player, this);
				base.Invoke(new Action(this.GrantCard), 0.5f);
				item.LoseCondition(1f);
				return;
			}
			base.Invoke(new Action(this.FailCard), 0.5f);
		}
	}

	// Token: 0x06000943 RID: 2371 RVA: 0x000584CF File Offset: 0x000566CF
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericInt1 = this.accessLevel;
		info.msg.ioEntity.genericFloat1 = this.accessDuration;
	}

	// Token: 0x06000944 RID: 2372 RVA: 0x00058504 File Offset: 0x00056704
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.accessLevel = info.msg.ioEntity.genericInt1;
			this.accessDuration = info.msg.ioEntity.genericFloat1;
		}
	}

	// Token: 0x0400061E RID: 1566
	public float accessDuration = 10f;

	// Token: 0x0400061F RID: 1567
	public int accessLevel;

	// Token: 0x04000620 RID: 1568
	public GameObjectRef accessGrantedEffect;

	// Token: 0x04000621 RID: 1569
	public GameObjectRef accessDeniedEffect;

	// Token: 0x04000622 RID: 1570
	public GameObjectRef swipeEffect;

	// Token: 0x04000623 RID: 1571
	public Transform audioPosition;

	// Token: 0x04000624 RID: 1572
	public BaseEntity.Flags AccessLevel1 = BaseEntity.Flags.Reserved1;

	// Token: 0x04000625 RID: 1573
	public BaseEntity.Flags AccessLevel2 = BaseEntity.Flags.Reserved2;

	// Token: 0x04000626 RID: 1574
	public BaseEntity.Flags AccessLevel3 = BaseEntity.Flags.Reserved3;
}
