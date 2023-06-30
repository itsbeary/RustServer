using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000E0 RID: 224
public class TimerSwitch : IOEntity
{
	// Token: 0x060013A4 RID: 5028 RVA: 0x0009DA6C File Offset: 0x0009BC6C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("TimerSwitch.OnRpcMessage", 0))
		{
			if (rpc == 4167839872U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SVSwitch ");
				}
				using (TimeWarning.New("SVSwitch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(4167839872U, "SVSwitch", this, player, 3f))
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
							this.SVSwitch(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SVSwitch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060013A5 RID: 5029 RVA: 0x0009DBD4 File Offset: 0x0009BDD4
	public override void ResetIOState()
	{
		base.ResetIOState();
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		if (base.IsInvoking(new Action(this.AdvanceTime)))
		{
			this.EndTimer();
		}
	}

	// Token: 0x060013A6 RID: 5030 RVA: 0x0009DC00 File Offset: 0x0009BE00
	public override bool WantsPassthroughPower()
	{
		return this.IsPowered() && base.IsOn();
	}

	// Token: 0x060013A7 RID: 5031 RVA: 0x0009DC12 File Offset: 0x0009BE12
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!this.IsPowered() || !base.IsOn())
		{
			return 0;
		}
		return base.GetPassthroughAmount(0);
	}

	// Token: 0x060013A8 RID: 5032 RVA: 0x0009DC30 File Offset: 0x0009BE30
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.SetFlag(BaseEntity.Flags.Reserved8, inputAmount > 0, false, false);
		}
	}

	// Token: 0x060013A9 RID: 5033 RVA: 0x0009DC48 File Offset: 0x0009BE48
	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.UpdateFromInput(inputAmount, inputSlot);
			if (!this.IsPowered() && base.IsInvoking(new Action(this.AdvanceTime)))
			{
				this.EndTimer();
				return;
			}
			if (this.timePassed != -1f)
			{
				base.SetFlag(BaseEntity.Flags.On, false, false, false);
				this.SwitchPressed();
				return;
			}
		}
		else if (inputSlot == 1 && inputAmount > 0)
		{
			this.SwitchPressed();
		}
	}

	// Token: 0x060013AA RID: 5034 RVA: 0x0009DCAF File Offset: 0x0009BEAF
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SVSwitch(BaseEntity.RPCMessage msg)
	{
		this.SwitchPressed();
	}

	// Token: 0x060013AB RID: 5035 RVA: 0x0009DCB8 File Offset: 0x0009BEB8
	public void SwitchPressed()
	{
		if (base.IsOn())
		{
			return;
		}
		if (!this.IsPowered())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		this.MarkDirty();
		base.InvokeRepeating(new Action(this.AdvanceTime), 0f, 0.1f);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x060013AC RID: 5036 RVA: 0x0009DD0C File Offset: 0x0009BF0C
	public void AdvanceTime()
	{
		if (this.timePassed < 0f)
		{
			this.timePassed = 0f;
		}
		this.timePassed += 0.1f;
		if (this.timePassed >= this.timerLength)
		{
			this.EndTimer();
			return;
		}
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060013AD RID: 5037 RVA: 0x0009DD5F File Offset: 0x0009BF5F
	public void EndTimer()
	{
		base.CancelInvoke(new Action(this.AdvanceTime));
		this.timePassed = -1f;
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		base.SendNetworkUpdateImmediate(false);
		this.MarkDirty();
	}

	// Token: 0x060013AE RID: 5038 RVA: 0x0009DD95 File Offset: 0x0009BF95
	public float GetPassedTime()
	{
		return this.timePassed;
	}

	// Token: 0x060013AF RID: 5039 RVA: 0x0009DD9D File Offset: 0x0009BF9D
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.timePassed == -1f)
		{
			if (base.IsOn())
			{
				base.SetFlag(BaseEntity.Flags.On, false, false, true);
				return;
			}
		}
		else
		{
			this.SwitchPressed();
		}
	}

	// Token: 0x060013B0 RID: 5040 RVA: 0x0009DDCB File Offset: 0x0009BFCB
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericFloat1 = this.GetPassedTime();
		info.msg.ioEntity.genericFloat2 = this.timerLength;
	}

	// Token: 0x060013B1 RID: 5041 RVA: 0x0009DE00 File Offset: 0x0009C000
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.timerLength = info.msg.ioEntity.genericFloat2;
			this.timePassed = info.msg.ioEntity.genericFloat1;
		}
	}

	// Token: 0x04000C38 RID: 3128
	public float timerLength = 10f;

	// Token: 0x04000C39 RID: 3129
	public Transform timerDrum;

	// Token: 0x04000C3A RID: 3130
	private float timePassed = -1f;
}
