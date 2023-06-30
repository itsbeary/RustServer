using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000A0 RID: 160
public class MobilePhone : global::HeldEntity
{
	// Token: 0x06000E77 RID: 3703 RVA: 0x00079FBC File Offset: 0x000781BC
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MobilePhone.OnRpcMessage", 0))
		{
			if (rpc == 1529322558U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - AnswerPhone ");
				}
				using (TimeWarning.New("AnswerPhone", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(1529322558U, "AnswerPhone", this, player))
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
							this.AnswerPhone(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in AnswerPhone");
					}
				}
				return true;
			}
			if (rpc == 2754362156U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ClearCurrentUser ");
				}
				using (TimeWarning.New("ClearCurrentUser", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(2754362156U, "ClearCurrentUser", this, player))
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
							this.ClearCurrentUser(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in ClearCurrentUser");
					}
				}
				return true;
			}
			if (rpc == 1095090232U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - InitiateCall ");
				}
				using (TimeWarning.New("InitiateCall", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(1095090232U, "InitiateCall", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.InitiateCall(rpcmessage3);
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in InitiateCall");
					}
				}
				return true;
			}
			if (rpc == 2606442785U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_AddSavedNumber ");
				}
				using (TimeWarning.New("Server_AddSavedNumber", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2606442785U, "Server_AddSavedNumber", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(2606442785U, "Server_AddSavedNumber", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_AddSavedNumber(rpcmessage4);
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in Server_AddSavedNumber");
					}
				}
				return true;
			}
			if (rpc == 1402406333U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_RemoveSavedNumber ");
				}
				using (TimeWarning.New("Server_RemoveSavedNumber", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1402406333U, "Server_RemoveSavedNumber", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(1402406333U, "Server_RemoveSavedNumber", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage5 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_RemoveSavedNumber(rpcmessage5);
						}
					}
					catch (Exception ex5)
					{
						Debug.LogException(ex5);
						player.Kick("RPC Error in Server_RemoveSavedNumber");
					}
				}
				return true;
			}
			if (rpc == 2704491961U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_RequestCurrentState ");
				}
				using (TimeWarning.New("Server_RequestCurrentState", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(2704491961U, "Server_RequestCurrentState", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage6 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_RequestCurrentState(rpcmessage6);
						}
					}
					catch (Exception ex6)
					{
						Debug.LogException(ex6);
						player.Kick("RPC Error in Server_RequestCurrentState");
					}
				}
				return true;
			}
			if (rpc == 942544266U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_RequestPhoneDirectory ");
				}
				using (TimeWarning.New("Server_RequestPhoneDirectory", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(942544266U, "Server_RequestPhoneDirectory", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(942544266U, "Server_RequestPhoneDirectory", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage7 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_RequestPhoneDirectory(rpcmessage7);
						}
					}
					catch (Exception ex7)
					{
						Debug.LogException(ex7);
						player.Kick("RPC Error in Server_RequestPhoneDirectory");
					}
				}
				return true;
			}
			if (rpc == 1240133378U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerDeleteVoicemail ");
				}
				using (TimeWarning.New("ServerDeleteVoicemail", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1240133378U, "ServerDeleteVoicemail", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(1240133378U, "ServerDeleteVoicemail", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage8 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ServerDeleteVoicemail(rpcmessage8);
						}
					}
					catch (Exception ex8)
					{
						Debug.LogException(ex8);
						player.Kick("RPC Error in ServerDeleteVoicemail");
					}
				}
				return true;
			}
			if (rpc == 1221129498U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerHangUp ");
				}
				using (TimeWarning.New("ServerHangUp", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(1221129498U, "ServerHangUp", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage9 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ServerHangUp(rpcmessage9);
						}
					}
					catch (Exception ex9)
					{
						Debug.LogException(ex9);
						player.Kick("RPC Error in ServerHangUp");
					}
				}
				return true;
			}
			if (rpc == 239260010U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerPlayVoicemail ");
				}
				using (TimeWarning.New("ServerPlayVoicemail", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(239260010U, "ServerPlayVoicemail", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(239260010U, "ServerPlayVoicemail", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage10 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ServerPlayVoicemail(rpcmessage10);
						}
					}
					catch (Exception ex10)
					{
						Debug.LogException(ex10);
						player.Kick("RPC Error in ServerPlayVoicemail");
					}
				}
				return true;
			}
			if (rpc == 189198880U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerSendVoicemail ");
				}
				using (TimeWarning.New("ServerSendVoicemail", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(189198880U, "ServerSendVoicemail", this, player, 5UL))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage11 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ServerSendVoicemail(rpcmessage11);
						}
					}
					catch (Exception ex11)
					{
						Debug.LogException(ex11);
						player.Kick("RPC Error in ServerSendVoicemail");
					}
				}
				return true;
			}
			if (rpc == 2760189029U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerStopVoicemail ");
				}
				using (TimeWarning.New("ServerStopVoicemail", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2760189029U, "ServerStopVoicemail", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(2760189029U, "ServerStopVoicemail", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage12 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ServerStopVoicemail(rpcmessage12);
						}
					}
					catch (Exception ex12)
					{
						Debug.LogException(ex12);
						player.Kick("RPC Error in ServerStopVoicemail");
					}
				}
				return true;
			}
			if (rpc == 3900772076U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetCurrentUser ");
				}
				using (TimeWarning.New("SetCurrentUser", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(3900772076U, "SetCurrentUser", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage13 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetCurrentUser(rpcmessage13);
						}
					}
					catch (Exception ex13)
					{
						Debug.LogException(ex13);
						player.Kick("RPC Error in SetCurrentUser");
					}
				}
				return true;
			}
			if (rpc == 2760249627U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - UpdatePhoneName ");
				}
				using (TimeWarning.New("UpdatePhoneName", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2760249627U, "UpdatePhoneName", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(2760249627U, "UpdatePhoneName", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage14 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.UpdatePhoneName(rpcmessage14);
						}
					}
					catch (Exception ex14)
					{
						Debug.LogException(ex14);
						player.Kick("RPC Error in UpdatePhoneName");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000E78 RID: 3704 RVA: 0x0007B380 File Offset: 0x00079580
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.telephone == null)
		{
			info.msg.telephone = Facepunch.Pool.Get<ProtoBuf.Telephone>();
		}
		info.msg.telephone.phoneNumber = this.Controller.PhoneNumber;
		info.msg.telephone.phoneName = this.Controller.PhoneName;
		info.msg.telephone.lastNumber = this.Controller.lastDialedNumber;
		info.msg.telephone.savedNumbers = this.Controller.savedNumbers;
		if (!info.forDisk)
		{
			info.msg.telephone.usingPlayer = this.Controller.currentPlayerRef.uid;
		}
	}

	// Token: 0x06000E79 RID: 3705 RVA: 0x0007B445 File Offset: 0x00079645
	public override void ServerInit()
	{
		base.ServerInit();
		this.Controller.ServerInit();
	}

	// Token: 0x06000E7A RID: 3706 RVA: 0x0007B458 File Offset: 0x00079658
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.Controller.PostServerLoad();
	}

	// Token: 0x06000E7B RID: 3707 RVA: 0x0007B46B File Offset: 0x0007966B
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		this.Controller.DoServerDestroy();
	}

	// Token: 0x06000E7C RID: 3708 RVA: 0x0007B47E File Offset: 0x0007967E
	public override void OnParentChanging(global::BaseEntity oldParent, global::BaseEntity newParent)
	{
		base.OnParentChanging(oldParent, newParent);
		this.Controller.OnParentChanged(newParent);
	}

	// Token: 0x06000E7D RID: 3709 RVA: 0x0007B494 File Offset: 0x00079694
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void ClearCurrentUser(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ClearCurrentUser(msg);
	}

	// Token: 0x06000E7E RID: 3710 RVA: 0x0007B4A2 File Offset: 0x000796A2
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void SetCurrentUser(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.SetCurrentUser(msg);
	}

	// Token: 0x06000E7F RID: 3711 RVA: 0x0007B4B0 File Offset: 0x000796B0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void InitiateCall(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.InitiateCall(msg);
	}

	// Token: 0x06000E80 RID: 3712 RVA: 0x0007B4BE File Offset: 0x000796BE
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void AnswerPhone(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.AnswerPhone(msg);
	}

	// Token: 0x06000E81 RID: 3713 RVA: 0x0007B4CC File Offset: 0x000796CC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	private void ServerHangUp(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ServerHangUp(msg);
	}

	// Token: 0x06000E82 RID: 3714 RVA: 0x0007B4DA File Offset: 0x000796DA
	public override void DestroyShared()
	{
		base.DestroyShared();
		this.Controller.DestroyShared();
	}

	// Token: 0x06000E83 RID: 3715 RVA: 0x0007B4ED File Offset: 0x000796ED
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void UpdatePhoneName(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.UpdatePhoneName(msg);
	}

	// Token: 0x06000E84 RID: 3716 RVA: 0x0007B4FB File Offset: 0x000796FB
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void Server_RequestPhoneDirectory(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.Server_RequestPhoneDirectory(msg);
	}

	// Token: 0x06000E85 RID: 3717 RVA: 0x0007B509 File Offset: 0x00079709
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void Server_AddSavedNumber(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.Server_AddSavedNumber(msg);
	}

	// Token: 0x06000E86 RID: 3718 RVA: 0x0007B517 File Offset: 0x00079717
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void Server_RemoveSavedNumber(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.Server_RemoveSavedNumber(msg);
	}

	// Token: 0x06000E87 RID: 3719 RVA: 0x0007B525 File Offset: 0x00079725
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void Server_RequestCurrentState(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.SetPhoneStateWithPlayer(this.Controller.serverState);
	}

	// Token: 0x06000E88 RID: 3720 RVA: 0x0007B53D File Offset: 0x0007973D
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void ServerSendVoicemail(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ServerSendVoicemail(msg);
	}

	// Token: 0x06000E89 RID: 3721 RVA: 0x0007B54B File Offset: 0x0007974B
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void ServerPlayVoicemail(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ServerPlayVoicemail(msg);
	}

	// Token: 0x06000E8A RID: 3722 RVA: 0x0007B559 File Offset: 0x00079759
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void ServerStopVoicemail(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ServerStopVoicemail(msg);
	}

	// Token: 0x06000E8B RID: 3723 RVA: 0x0007B567 File Offset: 0x00079767
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void ServerDeleteVoicemail(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ServerDeleteVoicemail(msg);
	}

	// Token: 0x06000E8C RID: 3724 RVA: 0x0007B578 File Offset: 0x00079778
	public void ToggleRinging(bool state)
	{
		MobileInventoryEntity associatedEntity = ItemModAssociatedEntity<MobileInventoryEntity>.GetAssociatedEntity(this.GetItem(), true);
		if (associatedEntity != null)
		{
			associatedEntity.ToggleRinging(state);
		}
	}

	// Token: 0x06000E8D RID: 3725 RVA: 0x0007B5A4 File Offset: 0x000797A4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		ProtoBuf.Entity msg = info.msg;
		if (((msg != null) ? msg.telephone : null) != null)
		{
			this.Controller.PhoneNumber = info.msg.telephone.phoneNumber;
			this.Controller.PhoneName = info.msg.telephone.phoneName;
			this.Controller.lastDialedNumber = info.msg.telephone.lastNumber;
			this.Controller.currentPlayerRef.uid = info.msg.telephone.usingPlayer;
			PhoneDirectory savedNumbers = this.Controller.savedNumbers;
			if (savedNumbers != null)
			{
				savedNumbers.ResetToPool();
			}
			this.Controller.savedNumbers = info.msg.telephone.savedNumbers;
			if (this.Controller.savedNumbers != null)
			{
				this.Controller.savedNumbers.ShouldPool = false;
			}
		}
	}

	// Token: 0x06000E8E RID: 3726 RVA: 0x0007B690 File Offset: 0x00079890
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer && old.HasFlag(global::BaseEntity.Flags.Busy) != next.HasFlag(global::BaseEntity.Flags.Busy))
		{
			if (next.HasFlag(global::BaseEntity.Flags.Busy))
			{
				if (!base.IsInvoking(new Action(this.Controller.WatchForDisconnects)))
				{
					base.InvokeRepeating(new Action(this.Controller.WatchForDisconnects), 0f, 0.1f);
				}
			}
			else
			{
				base.CancelInvoke(new Action(this.Controller.WatchForDisconnects));
			}
		}
		this.Controller.OnFlagsChanged(old, next);
	}

	// Token: 0x0400097B RID: 2427
	public PhoneController Controller;
}
