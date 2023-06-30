using System;
using System.Collections.Generic;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000A7 RID: 167
public class NPCTalking : NPCShopKeeper, IConversationProvider
{
	// Token: 0x06000F51 RID: 3921 RVA: 0x00080C98 File Offset: 0x0007EE98
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("NPCTalking.OnRpcMessage", 0))
		{
			if (rpc == 4224060672U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ConversationAction ");
				}
				using (TimeWarning.New("ConversationAction", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.CallsPerSecond.Test(4224060672U, "ConversationAction", this, player, 5UL))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.MaxDistance.Test(4224060672U, "ConversationAction", this, player, 3f))
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
							this.ConversationAction(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ConversationAction");
					}
				}
				return true;
			}
			if (rpc == 2112414875U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_BeginTalking ");
				}
				using (TimeWarning.New("Server_BeginTalking", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.CallsPerSecond.Test(2112414875U, "Server_BeginTalking", this, player, 1UL))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.MaxDistance.Test(2112414875U, "Server_BeginTalking", this, player, 3f))
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
							this.Server_BeginTalking(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in Server_BeginTalking");
					}
				}
				return true;
			}
			if (rpc == 1597539152U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_EndTalking ");
				}
				using (TimeWarning.New("Server_EndTalking", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.CallsPerSecond.Test(1597539152U, "Server_EndTalking", this, player, 1UL))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.MaxDistance.Test(1597539152U, "Server_EndTalking", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_EndTalking(rpcmessage3);
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in Server_EndTalking");
					}
				}
				return true;
			}
			if (rpc == 2713250658U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_ResponsePressed ");
				}
				using (TimeWarning.New("Server_ResponsePressed", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.CallsPerSecond.Test(2713250658U, "Server_ResponsePressed", this, player, 5UL))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.MaxDistance.Test(2713250658U, "Server_ResponsePressed", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage4 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_ResponsePressed(rpcmessage4);
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in Server_ResponsePressed");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000F52 RID: 3922 RVA: 0x000812C0 File Offset: 0x0007F4C0
	public int GetConversationIndex(string conversationName)
	{
		for (int i = 0; i < this.conversations.Length; i++)
		{
			if (this.conversations[i].shortname == conversationName)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000F53 RID: 3923 RVA: 0x000812F8 File Offset: 0x0007F4F8
	public virtual string GetConversationStartSpeech(BasePlayer player)
	{
		return "intro";
	}

	// Token: 0x06000F54 RID: 3924 RVA: 0x000812FF File Offset: 0x0007F4FF
	public ConversationData GetConversation(string conversationName)
	{
		return this.GetConversation(this.GetConversationIndex(conversationName));
	}

	// Token: 0x06000F55 RID: 3925 RVA: 0x0008130E File Offset: 0x0007F50E
	public ConversationData GetConversation(int index)
	{
		return this.conversations[index];
	}

	// Token: 0x06000F56 RID: 3926 RVA: 0x00081318 File Offset: 0x0007F518
	public virtual ConversationData GetConversationFor(BasePlayer player)
	{
		return this.conversations[0];
	}

	// Token: 0x06000F57 RID: 3927 RVA: 0x000233C8 File Offset: 0x000215C8
	public bool ProviderBusy()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06000F58 RID: 3928 RVA: 0x00081322 File Offset: 0x0007F522
	public void ForceEndConversation(BasePlayer player)
	{
		base.ClientRPCPlayer(null, player, "Client_EndConversation");
	}

	// Token: 0x06000F59 RID: 3929 RVA: 0x00081331 File Offset: 0x0007F531
	public void ForceSpeechNode(BasePlayer player, int speechNodeIndex)
	{
		if (player == null)
		{
			return;
		}
		base.ClientRPCPlayer<int>(null, player, "Client_ForceSpeechNode", speechNodeIndex);
	}

	// Token: 0x06000F5A RID: 3930 RVA: 0x0008134B File Offset: 0x0007F54B
	public virtual void OnConversationEnded(BasePlayer player)
	{
		if (this.conversingPlayers.Contains(player))
		{
			this.conversingPlayers.Remove(player);
		}
	}

	// Token: 0x06000F5B RID: 3931 RVA: 0x00081368 File Offset: 0x0007F568
	public void CleanupConversingPlayers()
	{
		for (int i = this.conversingPlayers.Count - 1; i >= 0; i--)
		{
			BasePlayer basePlayer = this.conversingPlayers[i];
			if (basePlayer == null || !basePlayer.IsAlive() || basePlayer.IsSleeping())
			{
				this.conversingPlayers.RemoveAt(i);
			}
		}
	}

	// Token: 0x06000F5C RID: 3932 RVA: 0x000813C0 File Offset: 0x0007F5C0
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	[BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void Server_BeginTalking(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		this.CleanupConversingPlayers();
		ConversationData conversationFor = this.GetConversationFor(player);
		if (conversationFor != null)
		{
			if (this.conversingPlayers.Contains(player))
			{
				this.OnConversationEnded(player);
			}
			this.conversingPlayers.Add(player);
			this.UpdateFlags();
			this.OnConversationStarted(player);
			base.ClientRPCPlayer<int, string>(null, player, "Client_StartConversation", this.GetConversationIndex(conversationFor.shortname), this.GetConversationStartSpeech(player));
		}
	}

	// Token: 0x06000F5D RID: 3933 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnConversationStarted(BasePlayer speakingTo)
	{
	}

	// Token: 0x06000F5E RID: 3934 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void UpdateFlags()
	{
	}

	// Token: 0x06000F5F RID: 3935 RVA: 0x00081439 File Offset: 0x0007F639
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	[BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void Server_EndTalking(BaseEntity.RPCMessage msg)
	{
		this.OnConversationEnded(msg.player);
	}

	// Token: 0x06000F60 RID: 3936 RVA: 0x00081448 File Offset: 0x0007F648
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	[BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void ConversationAction(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		string text = msg.read.String(256);
		this.OnConversationAction(player, text);
	}

	// Token: 0x06000F61 RID: 3937 RVA: 0x00081475 File Offset: 0x0007F675
	public bool ValidConversationPlayer(BasePlayer player)
	{
		return Vector3.Distance(player.transform.position, base.transform.position) <= this.maxConversationDistance && !this.conversingPlayers.Contains(player);
	}

	// Token: 0x06000F62 RID: 3938 RVA: 0x000814B0 File Offset: 0x0007F6B0
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	[BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void Server_ResponsePressed(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		int num = msg.read.Int32();
		int num2 = msg.read.Int32();
		ConversationData conversationFor = this.GetConversationFor(player);
		if (conversationFor == null)
		{
			return;
		}
		ConversationData.ResponseNode responseNode = conversationFor.speeches[num].responses[num2];
		if (responseNode != null)
		{
			if (responseNode.conditions.Length != 0)
			{
				this.UpdateFlags();
			}
			bool flag = responseNode.PassesConditions(player, this);
			if (flag && !string.IsNullOrEmpty(responseNode.actionString))
			{
				this.OnConversationAction(player, responseNode.actionString);
			}
			int speechNodeIndex = conversationFor.GetSpeechNodeIndex(flag ? responseNode.resultingSpeechNode : responseNode.GetFailedSpeechNode(player, this));
			if (speechNodeIndex == -1)
			{
				this.ForceEndConversation(player);
				return;
			}
			this.ForceSpeechNode(player, speechNodeIndex);
		}
	}

	// Token: 0x06000F63 RID: 3939 RVA: 0x00081572 File Offset: 0x0007F772
	public BasePlayer GetActionPlayer()
	{
		return this.lastActionPlayer;
	}

	// Token: 0x06000F64 RID: 3940 RVA: 0x0008157C File Offset: 0x0007F77C
	public virtual void OnConversationAction(BasePlayer player, string action)
	{
		if (action == "openvending")
		{
			InvisibleVendingMachine vendingMachine = base.GetVendingMachine();
			if (vendingMachine != null && Vector3.Distance(player.transform.position, base.transform.position) < 5f)
			{
				this.ForceEndConversation(player);
				vendingMachine.PlayerOpenLoot(player, "vendingmachine.customer", false);
				return;
			}
		}
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition("scrap");
		NPCTalking.NPCConversationResultAction[] array = this.conversationResultActions;
		int i = 0;
		while (i < array.Length)
		{
			NPCTalking.NPCConversationResultAction npcconversationResultAction = array[i];
			if (npcconversationResultAction.action == action)
			{
				this.CleanupConversingPlayers();
				foreach (BasePlayer basePlayer in this.conversingPlayers)
				{
					if (!(basePlayer == player) && !(basePlayer == null))
					{
						int speechNodeIndex = this.GetConversationFor(player).GetSpeechNodeIndex("startbusy");
						this.ForceSpeechNode(basePlayer, speechNodeIndex);
					}
				}
				int num = npcconversationResultAction.scrapCost;
				List<Item> list = player.inventory.FindItemIDs(itemDefinition.itemid);
				foreach (Item item in list)
				{
					num -= item.amount;
				}
				if (num > 0)
				{
					int speechNodeIndex2 = this.GetConversationFor(player).GetSpeechNodeIndex("toopoor");
					this.ForceSpeechNode(player, speechNodeIndex2);
					return;
				}
				Analytics.Azure.OnNPCVendor(player, this, npcconversationResultAction.scrapCost, npcconversationResultAction.action);
				num = npcconversationResultAction.scrapCost;
				foreach (Item item2 in list)
				{
					int num2 = Mathf.Min(num, item2.amount);
					item2.UseItem(num2);
					num -= num2;
					if (num <= 0)
					{
						break;
					}
				}
				this.lastActionPlayer = player;
				base.BroadcastEntityMessage(npcconversationResultAction.broadcastMessage, npcconversationResultAction.broadcastRange, 1218652417);
				this.lastActionPlayer = null;
				return;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x04000A1E RID: 2590
	public ConversationData[] conversations;

	// Token: 0x04000A1F RID: 2591
	public NPCTalking.NPCConversationResultAction[] conversationResultActions;

	// Token: 0x04000A20 RID: 2592
	[NonSerialized]
	private float maxConversationDistance = 5f;

	// Token: 0x04000A21 RID: 2593
	private List<BasePlayer> conversingPlayers = new List<BasePlayer>();

	// Token: 0x04000A22 RID: 2594
	private BasePlayer lastActionPlayer;

	// Token: 0x02000BF9 RID: 3065
	[Serializable]
	public class NPCConversationResultAction
	{
		// Token: 0x040041FF RID: 16895
		public string action;

		// Token: 0x04004200 RID: 16896
		public int scrapCost;

		// Token: 0x04004201 RID: 16897
		public string broadcastMessage;

		// Token: 0x04004202 RID: 16898
		public float broadcastRange;
	}
}
