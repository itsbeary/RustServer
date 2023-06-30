using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000B6 RID: 182
public class PressButton : IOEntity
{
	// Token: 0x0600107B RID: 4219 RVA: 0x00088938 File Offset: 0x00086B38
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PressButton.OnRpcMessage", 0))
		{
			if (rpc == 3778543711U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Press ");
				}
				using (TimeWarning.New("Press", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(3778543711U, "Press", this, player, 3f))
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
							this.Press(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Press");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600107C RID: 4220 RVA: 0x00088AA0 File Offset: 0x00086CA0
	public override void ResetIOState()
	{
		base.ResetIOState();
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
		base.CancelInvoke(new Action(this.Unpress));
		base.CancelInvoke(new Action(this.UnpowerTime));
	}

	// Token: 0x0600107D RID: 4221 RVA: 0x00088AF0 File Offset: 0x00086CF0
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		if (!(this.sourceItem != null) && !this.smallBurst)
		{
			return base.GetPassthroughAmount(0);
		}
		if (base.HasFlag(BaseEntity.Flags.Reserved3))
		{
			return Mathf.Max(this.pressPowerAmount, base.GetPassthroughAmount(0));
		}
		return 0;
	}

	// Token: 0x0600107E RID: 4222 RVA: 0x00088B46 File Offset: 0x00086D46
	public void UnpowerTime()
	{
		base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
		this.MarkDirty();
	}

	// Token: 0x0600107F RID: 4223 RVA: 0x00088B5C File Offset: 0x00086D5C
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x06001080 RID: 4224 RVA: 0x00088B70 File Offset: 0x00086D70
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void Press(BaseEntity.RPCMessage msg)
	{
		if (base.IsOn())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		base.Invoke(new Action(this.UnpowerTime), this.pressPowerTime);
		base.SetFlag(BaseEntity.Flags.Reserved3, true, false, true);
		base.SendNetworkUpdateImmediate(false);
		this.MarkDirty();
		base.Invoke(new Action(this.Unpress), this.pressDuration);
	}

	// Token: 0x06001081 RID: 4225 RVA: 0x000582D7 File Offset: 0x000564D7
	public void Unpress()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		this.MarkDirty();
	}

	// Token: 0x06001082 RID: 4226 RVA: 0x00088BDB File Offset: 0x00086DDB
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericFloat1 = this.pressDuration;
	}

	// Token: 0x06001083 RID: 4227 RVA: 0x00088BFA File Offset: 0x00086DFA
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.pressDuration = info.msg.ioEntity.genericFloat1;
		}
	}

	// Token: 0x04000A81 RID: 2689
	public float pressDuration = 5f;

	// Token: 0x04000A82 RID: 2690
	public float pressPowerTime = 0.5f;

	// Token: 0x04000A83 RID: 2691
	public int pressPowerAmount = 2;

	// Token: 0x04000A84 RID: 2692
	public const BaseEntity.Flags Flag_EmittingPower = BaseEntity.Flags.Reserved3;

	// Token: 0x04000A85 RID: 2693
	public bool smallBurst;
}
