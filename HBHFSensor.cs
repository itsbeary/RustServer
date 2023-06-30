using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000080 RID: 128
public class HBHFSensor : BaseDetector
{
	// Token: 0x06000C0C RID: 3084 RVA: 0x00069944 File Offset: 0x00067B44
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("HBHFSensor.OnRpcMessage", 0))
		{
			if (rpc == 3206885720U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetIncludeAuth ");
				}
				using (TimeWarning.New("SetIncludeAuth", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(3206885720U, "SetIncludeAuth", this, player, 3f))
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
							this.SetIncludeAuth(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SetIncludeAuth");
					}
				}
				return true;
			}
			if (rpc == 2223203375U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetIncludeOthers ");
				}
				using (TimeWarning.New("SetIncludeOthers", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(2223203375U, "SetIncludeOthers", this, player, 3f))
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
							this.SetIncludeOthers(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in SetIncludeOthers");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000C0D RID: 3085 RVA: 0x00069C44 File Offset: 0x00067E44
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return Mathf.Min(this.detectedPlayers, this.GetCurrentEnergy());
	}

	// Token: 0x06000C0E RID: 3086 RVA: 0x00069C57 File Offset: 0x00067E57
	public override void OnObjects()
	{
		base.OnObjects();
		this.UpdatePassthroughAmount();
		base.InvokeRandomized(new Action(this.UpdatePassthroughAmount), 0f, 1f, 0.1f);
	}

	// Token: 0x06000C0F RID: 3087 RVA: 0x00069C86 File Offset: 0x00067E86
	public override void OnEmpty()
	{
		base.OnEmpty();
		this.UpdatePassthroughAmount();
		base.CancelInvoke(new Action(this.UpdatePassthroughAmount));
	}

	// Token: 0x06000C10 RID: 3088 RVA: 0x00069CA8 File Offset: 0x00067EA8
	public void UpdatePassthroughAmount()
	{
		if (base.isClient)
		{
			return;
		}
		int num = this.detectedPlayers;
		this.detectedPlayers = 0;
		if (this.myTrigger.entityContents != null)
		{
			foreach (BaseEntity baseEntity in this.myTrigger.entityContents)
			{
				if (!(baseEntity == null) && baseEntity.IsVisible(base.transform.position + base.transform.forward * 0.1f, 10f))
				{
					BasePlayer component = baseEntity.GetComponent<BasePlayer>();
					bool flag = component.CanBuild();
					if ((!flag || this.ShouldIncludeAuthorized()) && (flag || this.ShouldIncludeOthers()) && component != null && component.IsAlive() && !component.IsSleeping() && component.isServer)
					{
						this.detectedPlayers++;
					}
				}
			}
		}
		if (num != this.detectedPlayers && this.IsPowered())
		{
			this.MarkDirty();
			if (this.detectedPlayers > num)
			{
				Effect.server.Run(this.detectUp.resourcePath, base.transform.position, Vector3.up, null, false);
				return;
			}
			if (this.detectedPlayers < num)
			{
				Effect.server.Run(this.detectDown.resourcePath, base.transform.position, Vector3.up, null, false);
			}
		}
	}

	// Token: 0x06000C11 RID: 3089 RVA: 0x00069E28 File Offset: 0x00068028
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetIncludeAuth(BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		if (msg.player.CanBuild() && this.IsPowered())
		{
			base.SetFlag(BaseEntity.Flags.Reserved3, flag, false, true);
		}
	}

	// Token: 0x06000C12 RID: 3090 RVA: 0x00069E64 File Offset: 0x00068064
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetIncludeOthers(BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		if (msg.player.CanBuild() && this.IsPowered())
		{
			base.SetFlag(BaseEntity.Flags.Reserved2, flag, false, true);
		}
	}

	// Token: 0x06000C13 RID: 3091 RVA: 0x0003018A File Offset: 0x0002E38A
	public bool ShouldIncludeAuthorized()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved3);
	}

	// Token: 0x06000C14 RID: 3092 RVA: 0x0000564C File Offset: 0x0000384C
	public bool ShouldIncludeOthers()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved2);
	}

	// Token: 0x040007C2 RID: 1986
	public GameObjectRef detectUp;

	// Token: 0x040007C3 RID: 1987
	public GameObjectRef detectDown;

	// Token: 0x040007C4 RID: 1988
	public const BaseEntity.Flags Flag_IncludeOthers = BaseEntity.Flags.Reserved2;

	// Token: 0x040007C5 RID: 1989
	public const BaseEntity.Flags Flag_IncludeAuthed = BaseEntity.Flags.Reserved3;

	// Token: 0x040007C6 RID: 1990
	private int detectedPlayers;
}
