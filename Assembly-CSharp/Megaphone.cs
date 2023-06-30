using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200009C RID: 156
public class Megaphone : HeldEntity
{
	// Token: 0x06000E1D RID: 3613 RVA: 0x00077898 File Offset: 0x00075A98
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Megaphone.OnRpcMessage", 0))
		{
			if (rpc == 4196056309U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_ToggleBroadcasting ");
				}
				using (TimeWarning.New("Server_ToggleBroadcasting", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(4196056309U, "Server_ToggleBroadcasting", this, player))
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
							this.Server_ToggleBroadcasting(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_ToggleBroadcasting");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000149 RID: 329
	// (get) Token: 0x06000E1E RID: 3614 RVA: 0x000779FC File Offset: 0x00075BFC
	// (set) Token: 0x06000E1F RID: 3615 RVA: 0x00077A03 File Offset: 0x00075C03
	[ReplicatedVar(Default = "100")]
	public static float MegaphoneVoiceRange { get; set; } = 100f;

	// Token: 0x06000E20 RID: 3616 RVA: 0x00077A0C File Offset: 0x00075C0C
	private void UpdateItemCondition()
	{
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null || !ownerItem.hasCondition)
		{
			return;
		}
		ownerItem.LoseCondition(this.VoiceDamageAmount);
	}

	// Token: 0x06000E21 RID: 3617 RVA: 0x00077A38 File Offset: 0x00075C38
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.FromOwner]
	private void Server_ToggleBroadcasting(BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Int8() == 1;
		base.SetFlag(BaseEntity.Flags.On, flag, false, true);
		if (flag)
		{
			if (!base.IsInvoking(new Action(this.UpdateItemCondition)))
			{
				base.InvokeRepeating(new Action(this.UpdateItemCondition), 0f, this.VoiceDamageMinFrequency);
				return;
			}
		}
		else if (base.IsInvoking(new Action(this.UpdateItemCondition)))
		{
			base.CancelInvoke(new Action(this.UpdateItemCondition));
		}
	}

	// Token: 0x04000926 RID: 2342
	[Header("Megaphone")]
	public VoiceProcessor voiceProcessor;

	// Token: 0x04000927 RID: 2343
	public float VoiceDamageMinFrequency = 2f;

	// Token: 0x04000928 RID: 2344
	public float VoiceDamageAmount = 1f;

	// Token: 0x04000929 RID: 2345
	public AudioSource VoiceSource;

	// Token: 0x0400092A RID: 2346
	public SoundDefinition StartBroadcastingSfx;

	// Token: 0x0400092B RID: 2347
	public SoundDefinition StopBroadcastingSfx;
}
