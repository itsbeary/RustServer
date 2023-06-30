using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200005C RID: 92
public class CollectableEasterEgg : BaseEntity
{
	// Token: 0x060009C7 RID: 2503 RVA: 0x0005BDA8 File Offset: 0x00059FA8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CollectableEasterEgg.OnRpcMessage", 0))
		{
			if (rpc == 2436818324U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_PickUp ");
				}
				using (TimeWarning.New("RPC_PickUp", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(2436818324U, "RPC_PickUp", this, player, 3f))
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
							this.RPC_PickUp(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_PickUp");
					}
				}
				return true;
			}
			if (rpc == 2243088389U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_StartPickUp ");
				}
				using (TimeWarning.New("RPC_StartPickUp", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(2243088389U, "RPC_StartPickUp", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_StartPickUp(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_StartPickUp");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060009C8 RID: 2504 RVA: 0x0005C0A8 File Offset: 0x0005A2A8
	public override void ServerInit()
	{
		int num = UnityEngine.Random.Range(0, 3);
		base.SetFlag(BaseEntity.Flags.Reserved1, num == 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved2, num == 1, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved3, num == 2, false, false);
		base.ServerInit();
	}

	// Token: 0x060009C9 RID: 2505 RVA: 0x0005C0F6 File Offset: 0x0005A2F6
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_StartPickUp(BaseEntity.RPCMessage msg)
	{
		if (msg.player == null)
		{
			return;
		}
		this.lastPickupStartTime = UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x060009CA RID: 2506 RVA: 0x0005C114 File Offset: 0x0005A314
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_PickUp(BaseEntity.RPCMessage msg)
	{
		if (msg.player == null)
		{
			return;
		}
		float num = UnityEngine.Time.realtimeSinceStartup - this.lastPickupStartTime;
		if (!(msg.player.GetHeldEntity() as EasterBasket))
		{
			if (num > 2f)
			{
				return;
			}
			if (num < 0.8f)
			{
				return;
			}
		}
		if (EggHuntEvent.serverEvent)
		{
			if (!EggHuntEvent.serverEvent.IsEventActive())
			{
				return;
			}
			EggHuntEvent.serverEvent.EggCollected(msg.player);
			int num2 = 1;
			msg.player.GiveItem(ItemManager.Create(this.itemToGive, num2, 0UL), BaseEntity.GiveItemReason.Generic);
		}
		Effect.server.Run(this.pickupEffect.resourcePath, base.transform.position + Vector3.up * 0.3f, Vector3.up, null, false);
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x0400067D RID: 1661
	public Transform artwork;

	// Token: 0x0400067E RID: 1662
	public float bounceRange = 0.2f;

	// Token: 0x0400067F RID: 1663
	public float bounceSpeed = 1f;

	// Token: 0x04000680 RID: 1664
	public GameObjectRef pickupEffect;

	// Token: 0x04000681 RID: 1665
	public ItemDefinition itemToGive;

	// Token: 0x04000682 RID: 1666
	private float lastPickupStartTime;
}
