using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

// Token: 0x020000B3 RID: 179
public class PowerCounter : global::IOEntity
{
	// Token: 0x0600103E RID: 4158 RVA: 0x00087928 File Offset: 0x00085B28
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PowerCounter.OnRpcMessage", 0))
		{
			if (rpc == 3554226761U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SERVER_SetTarget ");
				}
				using (TimeWarning.New("SERVER_SetTarget", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3554226761U, "SERVER_SetTarget", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SERVER_SetTarget(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SERVER_SetTarget");
					}
				}
				return true;
			}
			if (rpc == 3222475159U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ToggleDisplayMode ");
				}
				using (TimeWarning.New("ToggleDisplayMode", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3222475159U, "ToggleDisplayMode", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ToggleDisplayMode(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in ToggleDisplayMode");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600103F RID: 4159 RVA: 0x0000564C File Offset: 0x0000384C
	public bool DisplayPassthrough()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved2);
	}

	// Token: 0x06001040 RID: 4160 RVA: 0x00087C28 File Offset: 0x00085E28
	public bool DisplayCounter()
	{
		return !this.DisplayPassthrough();
	}

	// Token: 0x06001041 RID: 4161 RVA: 0x00087C33 File Offset: 0x00085E33
	public bool CanPlayerAdmin(global::BasePlayer player)
	{
		return player != null && player.CanBuild();
	}

	// Token: 0x06001042 RID: 4162 RVA: 0x00087C46 File Offset: 0x00085E46
	public int GetTarget()
	{
		return this.targetCounterNumber;
	}

	// Token: 0x06001043 RID: 4163 RVA: 0x00025634 File Offset: 0x00023834
	public override void ResetState()
	{
		base.ResetState();
	}

	// Token: 0x06001044 RID: 4164 RVA: 0x00087C4E File Offset: 0x00085E4E
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void SERVER_SetTarget(global::BaseEntity.RPCMessage msg)
	{
		if (!this.CanPlayerAdmin(msg.player))
		{
			return;
		}
		this.targetCounterNumber = msg.read.Int32();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001045 RID: 4165 RVA: 0x00087C77 File Offset: 0x00085E77
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void ToggleDisplayMode(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanBuild())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved2, msg.read.Bit(), false, false);
		this.MarkDirty();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001046 RID: 4166 RVA: 0x00087CAC File Offset: 0x00085EAC
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (this.DisplayPassthrough() || this.counterNumber >= this.targetCounterNumber)
		{
			return base.GetPassthroughAmount(outputSlot);
		}
		return 0;
	}

	// Token: 0x06001047 RID: 4167 RVA: 0x00087CD0 File Offset: 0x00085ED0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.ioEntity == null)
		{
			info.msg.ioEntity = Facepunch.Pool.Get<ProtoBuf.IOEntity>();
		}
		info.msg.ioEntity.genericInt1 = this.counterNumber;
		info.msg.ioEntity.genericInt2 = this.GetPassthroughAmount(0);
		info.msg.ioEntity.genericInt3 = this.GetTarget();
	}

	// Token: 0x06001048 RID: 4168 RVA: 0x00087D44 File Offset: 0x00085F44
	public void SetCounterNumber(int newNumber)
	{
		this.counterNumber = newNumber;
	}

	// Token: 0x06001049 RID: 4169 RVA: 0x00007D2F File Offset: 0x00005F2F
	public override void SendIONetworkUpdate()
	{
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600104A RID: 4170 RVA: 0x00087D4D File Offset: 0x00085F4D
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.UpdateHasPower(inputAmount, inputSlot);
		}
	}

	// Token: 0x0600104B RID: 4171 RVA: 0x00087D5C File Offset: 0x00085F5C
	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (this.DisplayCounter() && inputAmount > 0 && inputSlot != 0)
		{
			int num = this.counterNumber;
			if (inputSlot == 1)
			{
				this.counterNumber++;
			}
			else if (inputSlot == 2)
			{
				this.counterNumber--;
				if (this.counterNumber < 0)
				{
					this.counterNumber = 0;
				}
			}
			else if (inputSlot == 3)
			{
				this.counterNumber = 0;
			}
			this.counterNumber = Mathf.Clamp(this.counterNumber, 0, 100);
			if (num != this.counterNumber)
			{
				this.MarkDirty();
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}
		if (inputSlot == 0)
		{
			base.UpdateFromInput(inputAmount, inputSlot);
		}
	}

	// Token: 0x0600104C RID: 4172 RVA: 0x00087DF8 File Offset: 0x00085FF8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			if (base.isServer)
			{
				this.counterNumber = info.msg.ioEntity.genericInt1;
			}
			this.targetCounterNumber = info.msg.ioEntity.genericInt3;
		}
	}

	// Token: 0x04000A6A RID: 2666
	private int counterNumber;

	// Token: 0x04000A6B RID: 2667
	private int targetCounterNumber = 10;

	// Token: 0x04000A6C RID: 2668
	public Canvas canvas;

	// Token: 0x04000A6D RID: 2669
	public CanvasGroup screenAlpha;

	// Token: 0x04000A6E RID: 2670
	public Text screenText;

	// Token: 0x04000A6F RID: 2671
	public const global::BaseEntity.Flags Flag_ShowPassthrough = global::BaseEntity.Flags.Reserved2;

	// Token: 0x04000A70 RID: 2672
	public GameObjectRef counterConfigPanel;

	// Token: 0x04000A71 RID: 2673
	public Color passthroughColor;

	// Token: 0x04000A72 RID: 2674
	public Color counterColor;
}
