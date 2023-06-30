using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000D4 RID: 212
public class SpookySpeaker : BaseCombatEntity
{
	// Token: 0x060012E6 RID: 4838 RVA: 0x000978D4 File Offset: 0x00095AD4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SpookySpeaker.OnRpcMessage", 0))
		{
			if (rpc == 2523893445U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetWantsOn ");
				}
				using (TimeWarning.New("SetWantsOn", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(2523893445U, "SetWantsOn", this, player, 3f))
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
							this.SetWantsOn(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SetWantsOn");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060012E7 RID: 4839 RVA: 0x00097A3C File Offset: 0x00095C3C
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.UpdateInvokes();
	}

	// Token: 0x060012E8 RID: 4840 RVA: 0x00097A4C File Offset: 0x00095C4C
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetWantsOn(BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		base.SetFlag(BaseEntity.Flags.On, flag, false, true);
		this.UpdateInvokes();
	}

	// Token: 0x060012E9 RID: 4841 RVA: 0x00097A78 File Offset: 0x00095C78
	public void UpdateInvokes()
	{
		if (base.IsOn())
		{
			base.InvokeRandomized(new Action(this.SendPlaySound), this.soundSpacing, this.soundSpacing, this.soundSpacingRand);
			base.Invoke(new Action(this.DelayedOff), 7200f);
			return;
		}
		base.CancelInvoke(new Action(this.SendPlaySound));
		base.CancelInvoke(new Action(this.DelayedOff));
	}

	// Token: 0x060012EA RID: 4842 RVA: 0x00097AED File Offset: 0x00095CED
	public void SendPlaySound()
	{
		base.ClientRPC(null, "PlaySpookySound");
	}

	// Token: 0x060012EB RID: 4843 RVA: 0x00062B09 File Offset: 0x00060D09
	public void DelayedOff()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x04000BD2 RID: 3026
	public SoundPlayer soundPlayer;

	// Token: 0x04000BD3 RID: 3027
	public float soundSpacing = 12f;

	// Token: 0x04000BD4 RID: 3028
	public float soundSpacingRand = 5f;
}
