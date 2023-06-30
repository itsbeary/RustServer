using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000081 RID: 129
public class HeldBoomBox : HeldEntity, ICassettePlayer
{
	// Token: 0x06000C16 RID: 3094 RVA: 0x00069EA8 File Offset: 0x000680A8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("HeldBoomBox.OnRpcMessage", 0))
		{
			if (rpc == 1918716764U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_UpdateRadioIP ");
				}
				using (TimeWarning.New("Server_UpdateRadioIP", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.CallsPerSecond.Test(1918716764U, "Server_UpdateRadioIP", this, player, 2UL))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(1918716764U, "Server_UpdateRadioIP", this, player))
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
							this.Server_UpdateRadioIP(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_UpdateRadioIP");
					}
				}
				return true;
			}
			if (rpc == 1785864031U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerTogglePlay ");
				}
				using (TimeWarning.New("ServerTogglePlay", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(1785864031U, "ServerTogglePlay", this, player))
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
							this.ServerTogglePlay(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in ServerTogglePlay");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000120 RID: 288
	// (get) Token: 0x06000C17 RID: 3095 RVA: 0x000037E7 File Offset: 0x000019E7
	public BaseEntity ToBaseEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06000C18 RID: 3096 RVA: 0x0006A1BC File Offset: 0x000683BC
	public override void ServerInit()
	{
		base.ServerInit();
		this.BoxController.HurtCallback = new Action<float>(this.HurtCallback);
	}

	// Token: 0x06000C19 RID: 3097 RVA: 0x0006A1DB File Offset: 0x000683DB
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	public void ServerTogglePlay(BaseEntity.RPCMessage msg)
	{
		this.BoxController.ServerTogglePlay(msg);
	}

	// Token: 0x06000C1A RID: 3098 RVA: 0x0006A1E9 File Offset: 0x000683E9
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.CallsPerSecond(2UL)]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void Server_UpdateRadioIP(BaseEntity.RPCMessage msg)
	{
		this.BoxController.Server_UpdateRadioIP(msg);
	}

	// Token: 0x06000C1B RID: 3099 RVA: 0x0006A1F7 File Offset: 0x000683F7
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		this.BoxController.Save(info);
	}

	// Token: 0x06000C1C RID: 3100 RVA: 0x0006A20C File Offset: 0x0006840C
	public void OnCassetteInserted(Cassette c)
	{
		this.BoxController.OnCassetteInserted(c);
	}

	// Token: 0x06000C1D RID: 3101 RVA: 0x0006A21A File Offset: 0x0006841A
	public void OnCassetteRemoved(Cassette c)
	{
		this.BoxController.OnCassetteRemoved(c);
	}

	// Token: 0x06000C1E RID: 3102 RVA: 0x0006A228 File Offset: 0x00068428
	public bool ClearRadioByUserId(ulong id)
	{
		return this.BoxController.ClearRadioByUserId(id);
	}

	// Token: 0x06000C1F RID: 3103 RVA: 0x0006A236 File Offset: 0x00068436
	public void HurtCallback(float amount)
	{
		if (base.GetOwnerPlayer() != null && base.GetOwnerPlayer().IsSleeping())
		{
			this.BoxController.ServerTogglePlay(false);
			return;
		}
		Item item = this.GetItem();
		if (item == null)
		{
			return;
		}
		item.LoseCondition(amount);
	}

	// Token: 0x06000C20 RID: 3104 RVA: 0x0006A271 File Offset: 0x00068471
	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		if (base.IsDisabled())
		{
			this.BoxController.ServerTogglePlay(false);
		}
	}

	// Token: 0x06000C21 RID: 3105 RVA: 0x0006A28D File Offset: 0x0006848D
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		this.BoxController.Load(info);
		base.Load(info);
	}

	// Token: 0x040007C7 RID: 1991
	public BoomBox BoxController;

	// Token: 0x040007C8 RID: 1992
	public SwapKeycard cassetteSwapper;
}
