using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200001F RID: 31
public class StrobeLight : IOEntity
{
	// Token: 0x0600008C RID: 140 RVA: 0x000048A8 File Offset: 0x00002AA8
	public float GetFrequency()
	{
		if (base.HasFlag(BaseEntity.Flags.Reserved6))
		{
			return this.speedSlow;
		}
		if (base.HasFlag(BaseEntity.Flags.Reserved7))
		{
			return this.speedMed;
		}
		if (base.HasFlag(BaseEntity.Flags.Reserved8))
		{
			return this.speedFast;
		}
		return this.speedSlow;
	}

	// Token: 0x0600008D RID: 141 RVA: 0x000048F8 File Offset: 0x00002AF8
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetStrobe(BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		this.SetStrobe(flag);
	}

	// Token: 0x0600008E RID: 142 RVA: 0x00004918 File Offset: 0x00002B18
	private void SetStrobe(bool wantsOn)
	{
		this.ServerEnableStrobing(wantsOn);
		if (wantsOn)
		{
			this.UpdateSpeedFlags();
		}
	}

	// Token: 0x0600008F RID: 143 RVA: 0x0000492C File Offset: 0x00002B2C
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetStrobeSpeed(BaseEntity.RPCMessage msg)
	{
		int num = msg.read.Int32();
		this.currentSpeed = num;
		this.UpdateSpeedFlags();
	}

	// Token: 0x06000090 RID: 144 RVA: 0x00004954 File Offset: 0x00002B54
	public void UpdateSpeedFlags()
	{
		base.SetFlag(BaseEntity.Flags.Reserved6, this.currentSpeed == 1, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved7, this.currentSpeed == 2, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved8, this.currentSpeed == 3, false, true);
	}

	// Token: 0x06000091 RID: 145 RVA: 0x000049A4 File Offset: 0x00002BA4
	public void ServerEnableStrobing(bool wantsOn)
	{
		base.SetFlag(BaseEntity.Flags.Reserved6, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved7, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved8, false, false, true);
		base.SetFlag(BaseEntity.Flags.On, wantsOn, false, true);
		base.SendNetworkUpdateImmediate(false);
		this.UpdateSpeedFlags();
		if (wantsOn)
		{
			base.InvokeRandomized(new Action(this.SelfDamage), 0f, 10f, 0.1f);
			return;
		}
		base.CancelInvoke(new Action(this.SelfDamage));
	}

	// Token: 0x06000092 RID: 146 RVA: 0x00004A2C File Offset: 0x00002C2C
	public void SelfDamage()
	{
		float num = this.burnRate / this.lifeTimeSeconds;
		base.Hurt(num * this.MaxHealth(), DamageType.Decay, this, false);
	}

	// Token: 0x06000093 RID: 147 RVA: 0x00004A5C File Offset: 0x00002C5C
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		bool flag = false;
		if (inputSlot == 0)
		{
			flag = inputAmount > 0;
		}
		else if (inputSlot == 1)
		{
			if (inputAmount == 0)
			{
				return;
			}
			flag = true;
		}
		else if (inputSlot == 2)
		{
			if (inputAmount == 0)
			{
				return;
			}
			flag = false;
		}
		this.SetStrobe(flag);
	}

	// Token: 0x06000094 RID: 148 RVA: 0x00004A9C File Offset: 0x00002C9C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("StrobeLight.OnRpcMessage", 0))
		{
			if (rpc == 1433326740U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetStrobe ");
				}
				using (TimeWarning.New("SetStrobe", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1433326740U, "SetStrobe", this, player, 3f))
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
							this.SetStrobe(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SetStrobe");
					}
				}
				return true;
			}
			if (rpc == 1814332702U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetStrobeSpeed ");
				}
				using (TimeWarning.New("SetStrobeSpeed", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1814332702U, "SetStrobeSpeed", this, player, 3f))
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
							this.SetStrobeSpeed(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in SetStrobeSpeed");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x04000075 RID: 117
	public float frequency;

	// Token: 0x04000076 RID: 118
	public MeshRenderer lightMesh;

	// Token: 0x04000077 RID: 119
	public Light strobeLight;

	// Token: 0x04000078 RID: 120
	private float speedSlow = 10f;

	// Token: 0x04000079 RID: 121
	private float speedMed = 20f;

	// Token: 0x0400007A RID: 122
	private float speedFast = 40f;

	// Token: 0x0400007B RID: 123
	public float burnRate = 10f;

	// Token: 0x0400007C RID: 124
	public float lifeTimeSeconds = 21600f;

	// Token: 0x0400007D RID: 125
	public const BaseEntity.Flags Flag_Slow = BaseEntity.Flags.Reserved6;

	// Token: 0x0400007E RID: 126
	public const BaseEntity.Flags Flag_Med = BaseEntity.Flags.Reserved7;

	// Token: 0x0400007F RID: 127
	public const BaseEntity.Flags Flag_Fast = BaseEntity.Flags.Reserved8;

	// Token: 0x04000080 RID: 128
	private int currentSpeed = 1;
}
