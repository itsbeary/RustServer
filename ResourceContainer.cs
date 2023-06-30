using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000C0 RID: 192
public class ResourceContainer : EntityComponent<BaseEntity>
{
	// Token: 0x0600114A RID: 4426 RVA: 0x0008DF34 File Offset: 0x0008C134
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ResourceContainer.OnRpcMessage", 0))
		{
			if (rpc == 548378753U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - StartLootingContainer ");
				}
				using (TimeWarning.New("StartLootingContainer", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(548378753U, "StartLootingContainer", this.GetBaseEntity(), player, 3f))
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
							this.StartLootingContainer(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in StartLootingContainer");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000199 RID: 409
	// (get) Token: 0x0600114B RID: 4427 RVA: 0x0008E0A4 File Offset: 0x0008C2A4
	public int accessedSecondsAgo
	{
		get
		{
			return (int)(UnityEngine.Time.realtimeSinceStartup - this.lastAccessTime);
		}
	}

	// Token: 0x0600114C RID: 4428 RVA: 0x0008E0B4 File Offset: 0x0008C2B4
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	private void StartLootingContainer(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!player || !player.CanInteract())
		{
			return;
		}
		if (!this.lootable)
		{
			return;
		}
		if (player.inventory.loot.StartLootingEntity(base.baseEntity, true))
		{
			this.lastAccessTime = UnityEngine.Time.realtimeSinceStartup;
			player.inventory.loot.AddContainer(this.container);
		}
	}

	// Token: 0x04000ACF RID: 2767
	public bool lootable = true;

	// Token: 0x04000AD0 RID: 2768
	[NonSerialized]
	public ItemContainer container;

	// Token: 0x04000AD1 RID: 2769
	[NonSerialized]
	public float lastAccessTime;
}
