using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000CC RID: 204
public class SleepingBag : global::DecayEntity
{
	// Token: 0x0600123B RID: 4667 RVA: 0x00093380 File Offset: 0x00091580
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SleepingBag.OnRpcMessage", 0))
		{
			if (rpc == 3057055788U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - AssignToFriend ");
				}
				using (TimeWarning.New("AssignToFriend", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3057055788U, "AssignToFriend", this, player, 3f))
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
							this.AssignToFriend(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in AssignToFriend");
					}
				}
				return true;
			}
			if (rpc == 1335950295U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Rename ");
				}
				using (TimeWarning.New("Rename", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1335950295U, "Rename", this, player, 3f))
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
							this.Rename(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in Rename");
					}
				}
				return true;
			}
			if (rpc == 42669546U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_MakeBed ");
				}
				using (TimeWarning.New("RPC_MakeBed", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(42669546U, "RPC_MakeBed", this, player, 3f))
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
							this.RPC_MakeBed(rpcmessage3);
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RPC_MakeBed");
					}
				}
				return true;
			}
			if (rpc == 393812086U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_MakePublic ");
				}
				using (TimeWarning.New("RPC_MakePublic", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(393812086U, "RPC_MakePublic", this, player, 3f))
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
							this.RPC_MakePublic(rpcmessage4);
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in RPC_MakePublic");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600123C RID: 4668 RVA: 0x0003018A File Offset: 0x0002E38A
	public bool IsPublic()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved3);
	}

	// Token: 0x170001A6 RID: 422
	// (get) Token: 0x0600123D RID: 4669 RVA: 0x00093938 File Offset: 0x00091B38
	public virtual float unlockSeconds
	{
		get
		{
			if (this.unlockTime < UnityEngine.Time.realtimeSinceStartup)
			{
				return 0f;
			}
			return this.unlockTime - UnityEngine.Time.realtimeSinceStartup;
		}
	}

	// Token: 0x0600123E RID: 4670 RVA: 0x00093959 File Offset: 0x00091B59
	public virtual float GetUnlockSeconds(ulong playerID)
	{
		return this.unlockSeconds;
	}

	// Token: 0x0600123F RID: 4671 RVA: 0x00093961 File Offset: 0x00091B61
	public virtual bool ValidForPlayer(ulong playerID, bool ignoreTimers)
	{
		return this.deployerUserID == playerID && (ignoreTimers || this.unlockTime < UnityEngine.Time.realtimeSinceStartup);
	}

	// Token: 0x06001240 RID: 4672 RVA: 0x00093980 File Offset: 0x00091B80
	public static global::SleepingBag.CanAssignBedResult? CanAssignBed(global::BasePlayer player, global::SleepingBag newBag, ulong targetPlayer, int countOffset = 1, int maxOffset = 0, global::SleepingBag ignore = null)
	{
		int num = ConVar.Server.max_sleeping_bags + maxOffset;
		if (num < 0)
		{
			return null;
		}
		int num2 = countOffset;
		global::BasePlayer basePlayer = global::BasePlayer.FindByID(targetPlayer);
		if (player != basePlayer && basePlayer != null)
		{
			global::SleepingBag.BagAssignMode bagAssignMode = (global::SleepingBag.BagAssignMode)Mathf.Clamp(basePlayer.GetInfoInt("client.bagassignmode", 0), 0, 2);
			if (bagAssignMode != global::SleepingBag.BagAssignMode.Allowed)
			{
				if (bagAssignMode == global::SleepingBag.BagAssignMode.None)
				{
					return new global::SleepingBag.CanAssignBedResult?(new global::SleepingBag.CanAssignBedResult
					{
						Result = global::SleepingBag.BagResultType.BagBlocked
					});
				}
				if (bagAssignMode == global::SleepingBag.BagAssignMode.TeamAndFriendlyContacts)
				{
					bool flag = false;
					if (basePlayer.Team != null && basePlayer.Team.members.Contains(player.userID))
					{
						flag = true;
					}
					else
					{
						global::RelationshipManager.PlayerRelationshipInfo relations = global::RelationshipManager.ServerInstance.GetRelationships(targetPlayer).GetRelations(player.userID);
						if (relations != null && relations.type == global::RelationshipManager.RelationshipType.Friend)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						return new global::SleepingBag.CanAssignBedResult?(new global::SleepingBag.CanAssignBedResult
						{
							Result = global::SleepingBag.BagResultType.BagBlocked
						});
					}
				}
			}
		}
		foreach (global::SleepingBag sleepingBag in global::SleepingBag.sleepingBags)
		{
			if (sleepingBag != ignore && sleepingBag.deployerUserID == targetPlayer)
			{
				num2++;
				if (num2 > num)
				{
					return new global::SleepingBag.CanAssignBedResult?(new global::SleepingBag.CanAssignBedResult
					{
						Count = num2,
						Max = num,
						Result = global::SleepingBag.BagResultType.TooManyBags
					});
				}
			}
		}
		return new global::SleepingBag.CanAssignBedResult?(new global::SleepingBag.CanAssignBedResult
		{
			Count = num2,
			Max = num,
			Result = global::SleepingBag.BagResultType.Ok
		});
	}

	// Token: 0x06001241 RID: 4673 RVA: 0x00093B20 File Offset: 0x00091D20
	public static global::SleepingBag.CanBuildResult? CanBuildBed(global::BasePlayer player, Construction construction)
	{
		GameObject gameObject = GameManager.server.FindPrefab(construction.prefabID);
		if (((gameObject != null) ? gameObject.GetComponent<global::BaseEntity>() : null) is global::SleepingBag)
		{
			global::SleepingBag.CanAssignBedResult? canAssignBedResult = global::SleepingBag.CanAssignBed(player, null, player.userID, 1, 0, null);
			if (canAssignBedResult != null)
			{
				if (canAssignBedResult.Value.Result == global::SleepingBag.BagResultType.Ok)
				{
					global::SleepingBag.CanBuildResult canBuildResult = default(global::SleepingBag.CanBuildResult);
					canBuildResult.Result = true;
					canBuildResult.Phrase = global::SleepingBag.bagLimitPhrase;
					string[] array = new string[2];
					int num = 0;
					global::SleepingBag.CanAssignBedResult canAssignBedResult2 = canAssignBedResult.Value;
					array[num] = canAssignBedResult2.Count.ToString();
					int num2 = 1;
					canAssignBedResult2 = canAssignBedResult.Value;
					array[num2] = canAssignBedResult2.Max.ToString();
					canBuildResult.Arguments = array;
					return new global::SleepingBag.CanBuildResult?(canBuildResult);
				}
				return new global::SleepingBag.CanBuildResult?(new global::SleepingBag.CanBuildResult
				{
					Result = false,
					Phrase = global::SleepingBag.bagLimitReachedPhrase
				});
			}
		}
		return null;
	}

	// Token: 0x06001242 RID: 4674 RVA: 0x00093C08 File Offset: 0x00091E08
	public static global::SleepingBag[] FindForPlayer(ulong playerID, bool ignoreTimers)
	{
		return global::SleepingBag.sleepingBags.Where((global::SleepingBag x) => x.ValidForPlayer(playerID, ignoreTimers)).ToArray<global::SleepingBag>();
	}

	// Token: 0x06001243 RID: 4675 RVA: 0x00093C44 File Offset: 0x00091E44
	public static bool SpawnPlayer(global::BasePlayer player, NetworkableId sleepingBag)
	{
		global::SleepingBag[] array = global::SleepingBag.FindForPlayer(player.userID, true);
		global::SleepingBag sleepingBag2 = array.FirstOrDefault((global::SleepingBag x) => x.ValidForPlayer(player.userID, false) && x.net.ID == sleepingBag && x.unlockTime < UnityEngine.Time.realtimeSinceStartup);
		if (sleepingBag2 == null)
		{
			return false;
		}
		if (sleepingBag2.IsOccupied())
		{
			return false;
		}
		Vector3 vector;
		Quaternion quaternion;
		sleepingBag2.GetSpawnPos(out vector, out quaternion);
		player.RespawnAt(vector, quaternion, sleepingBag2);
		sleepingBag2.PostPlayerSpawn(player);
		global::SleepingBag[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			global::SleepingBag.SetBagTimer(array2[i], vector, global::SleepingBag.SleepingBagResetReason.Respawned);
		}
		return true;
	}

	// Token: 0x06001244 RID: 4676 RVA: 0x00093CE8 File Offset: 0x00091EE8
	public static void AddBagForPlayer(global::SleepingBag bag, ulong user, bool networkUpdate = true)
	{
		if (user == 0UL)
		{
			return;
		}
		List<global::SleepingBag> list;
		if (!global::SleepingBag.bagsPerPlayer.TryGetValue(user, out list))
		{
			list = new List<global::SleepingBag>();
			global::SleepingBag.bagsPerPlayer[user] = list;
		}
		if (!list.Contains(bag))
		{
			list.Add(bag);
			if (networkUpdate)
			{
				global::BasePlayer basePlayer = global::RelationshipManager.FindByID(user);
				if (basePlayer == null)
				{
					return;
				}
				basePlayer.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}
	}

	// Token: 0x06001245 RID: 4677 RVA: 0x00093D40 File Offset: 0x00091F40
	public static void RemoveBagForPlayer(global::SleepingBag bag, ulong user)
	{
		if (user == 0UL)
		{
			return;
		}
		List<global::SleepingBag> list;
		if (!global::SleepingBag.bagsPerPlayer.TryGetValue(user, out list))
		{
			return;
		}
		if (list.Remove(bag))
		{
			global::BasePlayer basePlayer = global::RelationshipManager.FindByID(user);
			if (basePlayer != null)
			{
				basePlayer.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}
		if (list.Count == 0)
		{
			global::SleepingBag.bagsPerPlayer.Remove(user);
		}
	}

	// Token: 0x06001246 RID: 4678 RVA: 0x00093D90 File Offset: 0x00091F90
	public static void OnBagChangedOwnership(global::SleepingBag bag, ulong oldUser)
	{
		if (bag.deployerUserID != oldUser)
		{
			global::SleepingBag.RemoveBagForPlayer(bag, oldUser);
			global::SleepingBag.AddBagForPlayer(bag, bag.deployerUserID, true);
		}
	}

	// Token: 0x06001247 RID: 4679 RVA: 0x00093DB0 File Offset: 0x00091FB0
	public static int GetSleepingBagCount(ulong userId)
	{
		if (userId == 0UL)
		{
			return 0;
		}
		List<global::SleepingBag> list;
		if (!global::SleepingBag.bagsPerPlayer.TryGetValue(userId, out list))
		{
			return 0;
		}
		return list.Count;
	}

	// Token: 0x06001248 RID: 4680 RVA: 0x00093DD9 File Offset: 0x00091FD9
	public virtual void SetUnlockTime(float newTime)
	{
		this.unlockTime = newTime;
	}

	// Token: 0x06001249 RID: 4681 RVA: 0x00093DE4 File Offset: 0x00091FE4
	public static bool DestroyBag(global::BasePlayer player, NetworkableId sleepingBag)
	{
		global::SleepingBag sleepingBag2 = global::SleepingBag.FindForPlayer(player.userID, true).FirstOrDefault((global::SleepingBag x) => x.net.ID == sleepingBag);
		if (sleepingBag2 == null)
		{
			return false;
		}
		global::SleepingBag.RemoveBagForPlayer(sleepingBag2, sleepingBag2.deployerUserID);
		sleepingBag2.deployerUserID = 0UL;
		sleepingBag2.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		player.SendRespawnOptions();
		Analytics.Azure.OnBagUnclaimed(player, sleepingBag2);
		return true;
	}

	// Token: 0x0600124A RID: 4682 RVA: 0x00093E50 File Offset: 0x00092050
	public static void ResetTimersForPlayer(global::BasePlayer player)
	{
		global::SleepingBag[] array = global::SleepingBag.FindForPlayer(player.userID, true);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].unlockTime = 0f;
		}
	}

	// Token: 0x0600124B RID: 4683 RVA: 0x00093E88 File Offset: 0x00092088
	public virtual void GetSpawnPos(out Vector3 pos, out Quaternion rot)
	{
		pos = base.transform.position + this.spawnOffset;
		rot = Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y, 0f);
	}

	// Token: 0x0600124C RID: 4684 RVA: 0x00093EDE File Offset: 0x000920DE
	public void SetPublic(bool isPublic)
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved3, isPublic, false, true);
	}

	// Token: 0x0600124D RID: 4685 RVA: 0x00093EEE File Offset: 0x000920EE
	private void SetDeployedBy(global::BasePlayer player)
	{
		if (player == null)
		{
			return;
		}
		this.deployerUserID = player.userID;
		global::SleepingBag.SetBagTimer(this, base.transform.position, global::SleepingBag.SleepingBagResetReason.Placed);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		this.notifyPlayerOnServerInit = true;
	}

	// Token: 0x0600124E RID: 4686 RVA: 0x00093F28 File Offset: 0x00092128
	public static void OnPlayerDeath(global::BasePlayer player)
	{
		global::SleepingBag[] array = global::SleepingBag.FindForPlayer(player.userID, true);
		for (int i = 0; i < array.Length; i++)
		{
			global::SleepingBag.SetBagTimer(array[i], player.transform.position, global::SleepingBag.SleepingBagResetReason.Death);
		}
	}

	// Token: 0x0600124F RID: 4687 RVA: 0x00093F64 File Offset: 0x00092164
	public static void SetBagTimer(global::SleepingBag bag, Vector3 position, global::SleepingBag.SleepingBagResetReason reason)
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		float? num = null;
		if (activeGameMode != null)
		{
			num = activeGameMode.EvaluateSleepingBagReset(bag, position, reason);
		}
		if (num != null)
		{
			bag.SetUnlockTime(UnityEngine.Time.realtimeSinceStartup + num.Value);
			return;
		}
		if (reason == global::SleepingBag.SleepingBagResetReason.Respawned && Vector3.Distance(position, bag.transform.position) <= ConVar.Server.respawnresetrange)
		{
			bag.SetUnlockTime(UnityEngine.Time.realtimeSinceStartup + bag.secondsBetweenReuses);
			bag.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
		if (reason == global::SleepingBag.SleepingBagResetReason.Placed)
		{
			float realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
			foreach (global::SleepingBag sleepingBag in global::SleepingBag.sleepingBags.Where((global::SleepingBag x) => x.deployerUserID != 0UL && x.deployerUserID == bag.deployerUserID && x.unlockTime > UnityEngine.Time.realtimeSinceStartup).ToArray<global::SleepingBag>())
			{
				if (bag.unlockTime > realtimeSinceStartup && Vector3.Distance(sleepingBag.transform.position, position) <= ConVar.Server.respawnresetrange)
				{
					realtimeSinceStartup = bag.unlockTime;
				}
			}
			bag.SetUnlockTime(Mathf.Max(realtimeSinceStartup, UnityEngine.Time.realtimeSinceStartup + bag.secondsBetweenReuses));
			bag.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06001250 RID: 4688 RVA: 0x000940B4 File Offset: 0x000922B4
	public override void ServerInit()
	{
		base.ServerInit();
		if (!global::SleepingBag.sleepingBags.Contains(this))
		{
			global::SleepingBag.sleepingBags.Add(this);
			if (this.deployerUserID != 0UL)
			{
				global::SleepingBag.AddBagForPlayer(this, this.deployerUserID, !Rust.Application.isLoadingSave);
			}
		}
		if (this.notifyPlayerOnServerInit)
		{
			this.notifyPlayerOnServerInit = false;
			this.NotifyPlayer(this.deployerUserID);
		}
	}

	// Token: 0x06001251 RID: 4689 RVA: 0x00094116 File Offset: 0x00092316
	public override void OnParentChanging(global::BaseEntity oldParent, global::BaseEntity newParent)
	{
		base.OnParentChanging(oldParent, newParent);
		base.Invoke(new Action(this.DelayedPlayerNotify), 0.1f);
	}

	// Token: 0x06001252 RID: 4690 RVA: 0x00094137 File Offset: 0x00092337
	private void DelayedPlayerNotify()
	{
		this.NotifyPlayer(this.deployerUserID);
	}

	// Token: 0x06001253 RID: 4691 RVA: 0x00094145 File Offset: 0x00092345
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		global::SleepingBag.AddBagForPlayer(this, this.deployerUserID, !Rust.Application.isLoadingSave);
	}

	// Token: 0x06001254 RID: 4692 RVA: 0x00094164 File Offset: 0x00092364
	private void NotifyPlayer(ulong id)
	{
		global::BasePlayer basePlayer = global::BasePlayer.FindByID(id);
		if (basePlayer != null && basePlayer.IsConnected)
		{
			basePlayer.SendRespawnOptions();
		}
	}

	// Token: 0x06001255 RID: 4693 RVA: 0x0009418F File Offset: 0x0009238F
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		global::SleepingBag.sleepingBags.RemoveAll((global::SleepingBag x) => x == this);
		global::SleepingBag.RemoveBagForPlayer(this, this.deployerUserID);
		this.NotifyPlayer(this.deployerUserID);
	}

	// Token: 0x06001256 RID: 4694 RVA: 0x000941C8 File Offset: 0x000923C8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.sleepingBag = Facepunch.Pool.Get<ProtoBuf.SleepingBag>();
		info.msg.sleepingBag.name = this.niceName;
		info.msg.sleepingBag.deployerID = this.deployerUserID;
	}

	// Token: 0x06001257 RID: 4695 RVA: 0x00094218 File Offset: 0x00092418
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void Rename(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		string text = msg.read.String(256);
		text = WordFilter.Filter(text);
		if (string.IsNullOrEmpty(text))
		{
			text = "Unnamed Sleeping Bag";
		}
		if (text.Length > 24)
		{
			text = text.Substring(0, 22) + "..";
		}
		this.niceName = text;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		this.NotifyPlayer(this.deployerUserID);
	}

	// Token: 0x06001258 RID: 4696 RVA: 0x00094294 File Offset: 0x00092494
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void AssignToFriend(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (this.deployerUserID != msg.player.userID)
		{
			return;
		}
		ulong num = msg.read.UInt64();
		if (num == 0UL)
		{
			return;
		}
		if (ConVar.Server.max_sleeping_bags > 0)
		{
			global::SleepingBag.CanAssignBedResult? canAssignBedResult = global::SleepingBag.CanAssignBed(msg.player, this, num, 1, 0, null);
			if (canAssignBedResult != null)
			{
				global::BasePlayer basePlayer = global::RelationshipManager.FindByID(num);
				if (canAssignBedResult.Value.Result == global::SleepingBag.BagResultType.TooManyBags)
				{
					msg.player.ShowToast(GameTip.Styles.Red_Normal, this.cannotAssignBedPhrase, new string[] { ((basePlayer != null) ? basePlayer.displayName : null) ?? "other player" });
				}
				else if (canAssignBedResult.Value.Result == global::SleepingBag.BagResultType.BagBlocked)
				{
					msg.player.ShowToast(GameTip.Styles.Red_Normal, this.bedAssigningBlocked, Array.Empty<string>());
				}
				else
				{
					global::SleepingBag.CanAssignBedResult canAssignBedResult2;
					if (basePlayer != null)
					{
						global::BasePlayer basePlayer2 = basePlayer;
						GameTip.Styles styles = GameTip.Styles.Blue_Long;
						Translate.Phrase phrase = this.assignedBagPhrase;
						string[] array = new string[2];
						int num2 = 0;
						canAssignBedResult2 = canAssignBedResult.Value;
						array[num2] = canAssignBedResult2.Count.ToString();
						int num3 = 1;
						canAssignBedResult2 = canAssignBedResult.Value;
						array[num3] = canAssignBedResult2.Max.ToString();
						basePlayer2.ShowToast(styles, phrase, array);
					}
					global::BasePlayer player = msg.player;
					GameTip.Styles styles2 = GameTip.Styles.Blue_Long;
					Translate.Phrase phrase2 = global::SleepingBag.bagLimitPhrase;
					string[] array2 = new string[2];
					array2[0] = (global::SleepingBag.GetSleepingBagCount(msg.player.userID) - 1).ToString();
					int num4 = 1;
					canAssignBedResult2 = canAssignBedResult.Value;
					array2[num4] = canAssignBedResult2.Max.ToString();
					player.ShowToast(styles2, phrase2, array2);
				}
				if (canAssignBedResult.Value.Result != global::SleepingBag.BagResultType.Ok)
				{
					return;
				}
			}
		}
		ulong num5 = this.deployerUserID;
		this.deployerUserID = num;
		this.NotifyPlayer(num5);
		this.NotifyPlayer(this.deployerUserID);
		global::SleepingBag.OnBagChangedOwnership(this, num5);
		Analytics.Azure.OnSleepingBagAssigned(msg.player, this, num);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001259 RID: 4697 RVA: 0x00094450 File Offset: 0x00092650
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public virtual void RPC_MakePublic(global::BaseEntity.RPCMessage msg)
	{
		if (!this.canBePublic)
		{
			return;
		}
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (this.deployerUserID != msg.player.userID && !msg.player.CanBuild())
		{
			return;
		}
		bool flag = msg.read.Bit();
		if (flag == this.IsPublic())
		{
			return;
		}
		this.SetPublic(flag);
		if (!this.IsPublic())
		{
			if (ConVar.Server.max_sleeping_bags > 0)
			{
				global::SleepingBag.CanAssignBedResult? canAssignBedResult = global::SleepingBag.CanAssignBed(msg.player, this, msg.player.userID, 1, 0, this);
				if (canAssignBedResult != null)
				{
					if (canAssignBedResult.Value.Result == global::SleepingBag.BagResultType.Ok)
					{
						global::BasePlayer player = msg.player;
						GameTip.Styles styles = GameTip.Styles.Blue_Long;
						Translate.Phrase phrase = global::SleepingBag.bagLimitPhrase;
						string[] array = new string[2];
						int num = 0;
						global::SleepingBag.CanAssignBedResult canAssignBedResult2 = canAssignBedResult.Value;
						array[num] = canAssignBedResult2.Count.ToString();
						int num2 = 1;
						canAssignBedResult2 = canAssignBedResult.Value;
						array[num2] = canAssignBedResult2.Max.ToString();
						player.ShowToast(styles, phrase, array);
					}
					else
					{
						global::BasePlayer player2 = msg.player;
						GameTip.Styles styles2 = GameTip.Styles.Blue_Long;
						Translate.Phrase phrase2 = this.cannotMakeBedPhrase;
						string[] array2 = new string[2];
						int num3 = 0;
						global::SleepingBag.CanAssignBedResult canAssignBedResult2 = canAssignBedResult.Value;
						array2[num3] = canAssignBedResult2.Count.ToString();
						int num4 = 1;
						canAssignBedResult2 = canAssignBedResult.Value;
						array2[num4] = canAssignBedResult2.Max.ToString();
						player2.ShowToast(styles2, phrase2, array2);
					}
					if (canAssignBedResult.Value.Result != global::SleepingBag.BagResultType.Ok)
					{
						return;
					}
				}
			}
			ulong num5 = this.deployerUserID;
			this.deployerUserID = msg.player.userID;
			this.NotifyPlayer(num5);
			this.NotifyPlayer(this.deployerUserID);
			global::SleepingBag.OnBagChangedOwnership(this, num5);
			Analytics.Azure.OnSleepingBagAssigned(msg.player, this, this.deployerUserID = msg.player.userID);
		}
		else
		{
			Analytics.Azure.OnSleepingBagAssigned(msg.player, this, 0UL);
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600125A RID: 4698 RVA: 0x00094604 File Offset: 0x00092804
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_MakeBed(global::BaseEntity.RPCMessage msg)
	{
		if (!this.canBePublic || !this.IsPublic())
		{
			return;
		}
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (ConVar.Server.max_sleeping_bags > 0)
		{
			global::SleepingBag.CanAssignBedResult? canAssignBedResult = global::SleepingBag.CanAssignBed(msg.player, this, msg.player.userID, 1, 0, this);
			if (canAssignBedResult != null)
			{
				if (canAssignBedResult.Value.Result != global::SleepingBag.BagResultType.Ok)
				{
					msg.player.ShowToast(GameTip.Styles.Red_Normal, this.cannotMakeBedPhrase, Array.Empty<string>());
				}
				else
				{
					global::BasePlayer player = msg.player;
					GameTip.Styles styles = GameTip.Styles.Blue_Long;
					Translate.Phrase phrase = global::SleepingBag.bagLimitPhrase;
					string[] array = new string[2];
					int num = 0;
					global::SleepingBag.CanAssignBedResult canAssignBedResult2 = canAssignBedResult.Value;
					array[num] = canAssignBedResult2.Count.ToString();
					int num2 = 1;
					canAssignBedResult2 = canAssignBedResult.Value;
					array[num2] = canAssignBedResult2.Max.ToString();
					player.ShowToast(styles, phrase, array);
				}
				if (canAssignBedResult.Value.Result != global::SleepingBag.BagResultType.Ok)
				{
					return;
				}
			}
		}
		ulong num3 = this.deployerUserID;
		this.deployerUserID = msg.player.userID;
		this.NotifyPlayer(num3);
		this.NotifyPlayer(this.deployerUserID);
		global::SleepingBag.OnBagChangedOwnership(this, num3);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600125B RID: 4699 RVA: 0x00094713 File Offset: 0x00092913
	protected virtual void PostPlayerSpawn(global::BasePlayer p)
	{
		p.SendRespawnOptions();
	}

	// Token: 0x0600125C RID: 4700 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool IsOccupied()
	{
		return false;
	}

	// Token: 0x0600125D RID: 4701 RVA: 0x0009471C File Offset: 0x0009291C
	public virtual bool IsMobile()
	{
		global::BaseEntity parentEntity = base.GetParentEntity();
		return (parentEntity != null && parentEntity is global::BaseVehicle) || this.RespawnType == RespawnInformation.SpawnOptions.RespawnType.Camper;
	}

	// Token: 0x0600125E RID: 4702 RVA: 0x0009474C File Offset: 0x0009294C
	public override string Admin_Who()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(base.Admin_Who());
		stringBuilder.AppendLine(string.Format("Assigned bag ID: {0}", this.deployerUserID));
		stringBuilder.AppendLine("Assigned player name: " + Admin.GetPlayerName(this.deployerUserID));
		stringBuilder.AppendLine("Bag Name:" + this.niceName);
		return stringBuilder.ToString();
	}

	// Token: 0x0600125F RID: 4703 RVA: 0x000947C0 File Offset: 0x000929C0
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.sleepingBag != null)
		{
			this.niceName = info.msg.sleepingBag.name;
			this.deployerUserID = info.msg.sleepingBag.deployerID;
		}
	}

	// Token: 0x06001260 RID: 4704 RVA: 0x0009480D File Offset: 0x00092A0D
	public override bool CanPickup(global::BasePlayer player)
	{
		return base.CanPickup(player) && player.userID == this.deployerUserID;
	}

	// Token: 0x04000B54 RID: 2900
	[NonSerialized]
	public ulong deployerUserID;

	// Token: 0x04000B55 RID: 2901
	public GameObject renameDialog;

	// Token: 0x04000B56 RID: 2902
	public GameObject assignDialog;

	// Token: 0x04000B57 RID: 2903
	public float secondsBetweenReuses = 300f;

	// Token: 0x04000B58 RID: 2904
	public string niceName = "Unnamed Bag";

	// Token: 0x04000B59 RID: 2905
	public Vector3 spawnOffset = Vector3.zero;

	// Token: 0x04000B5A RID: 2906
	public RespawnInformation.SpawnOptions.RespawnType RespawnType = RespawnInformation.SpawnOptions.RespawnType.SleepingBag;

	// Token: 0x04000B5B RID: 2907
	public bool isStatic;

	// Token: 0x04000B5C RID: 2908
	public bool canBePublic;

	// Token: 0x04000B5D RID: 2909
	public const global::BaseEntity.Flags IsPublicFlag = global::BaseEntity.Flags.Reserved3;

	// Token: 0x04000B5E RID: 2910
	public static Translate.Phrase bagLimitPhrase = new Translate.Phrase("bag_limit_update", "You are now at {0}/{1} bags");

	// Token: 0x04000B5F RID: 2911
	public static Translate.Phrase bagLimitReachedPhrase = new Translate.Phrase("bag_limit_reached", "You have reached your bag limit!");

	// Token: 0x04000B60 RID: 2912
	public Translate.Phrase assignOtherBagPhrase = new Translate.Phrase("assigned_other_bag_limit", "You have assigned {0} a bag, they are now at {0}/{1} bags");

	// Token: 0x04000B61 RID: 2913
	public Translate.Phrase assignedBagPhrase = new Translate.Phrase("assigned_bag_limit", "You have been assigned a bag, you are now at {0}/{1} bags");

	// Token: 0x04000B62 RID: 2914
	public Translate.Phrase cannotAssignBedPhrase = new Translate.Phrase("cannot_assign_bag_limit", "You cannot assign {0} a bag, they have reached their bag limit!");

	// Token: 0x04000B63 RID: 2915
	public Translate.Phrase cannotMakeBedPhrase = new Translate.Phrase("cannot_make_bed_limit", "You cannot take ownership of the bed, you are at your bag limit");

	// Token: 0x04000B64 RID: 2916
	public Translate.Phrase bedAssigningBlocked = new Translate.Phrase("bag_assign_blocked", "That player has blocked bag assignment");

	// Token: 0x04000B65 RID: 2917
	internal float unlockTime;

	// Token: 0x04000B66 RID: 2918
	public static List<global::SleepingBag> sleepingBags = new List<global::SleepingBag>();

	// Token: 0x04000B67 RID: 2919
	private bool notifyPlayerOnServerInit;

	// Token: 0x04000B68 RID: 2920
	private static Dictionary<ulong, List<global::SleepingBag>> bagsPerPlayer = new Dictionary<ulong, List<global::SleepingBag>>();

	// Token: 0x02000C08 RID: 3080
	public enum BagAssignMode
	{
		// Token: 0x0400422D RID: 16941
		Allowed,
		// Token: 0x0400422E RID: 16942
		TeamAndFriendlyContacts,
		// Token: 0x0400422F RID: 16943
		None,
		// Token: 0x04004230 RID: 16944
		LAST = 2
	}

	// Token: 0x02000C09 RID: 3081
	public enum BagResultType
	{
		// Token: 0x04004232 RID: 16946
		Ok,
		// Token: 0x04004233 RID: 16947
		TooManyBags,
		// Token: 0x04004234 RID: 16948
		BagBlocked
	}

	// Token: 0x02000C0A RID: 3082
	public struct CanAssignBedResult
	{
		// Token: 0x04004235 RID: 16949
		public global::SleepingBag.BagResultType Result;

		// Token: 0x04004236 RID: 16950
		public int Count;

		// Token: 0x04004237 RID: 16951
		public int Max;
	}

	// Token: 0x02000C0B RID: 3083
	public struct CanBuildResult
	{
		// Token: 0x04004238 RID: 16952
		public bool Result;

		// Token: 0x04004239 RID: 16953
		public Translate.Phrase Phrase;

		// Token: 0x0400423A RID: 16954
		public string[] Arguments;
	}

	// Token: 0x02000C0C RID: 3084
	public enum SleepingBagResetReason
	{
		// Token: 0x0400423C RID: 16956
		Respawned,
		// Token: 0x0400423D RID: 16957
		Placed,
		// Token: 0x0400423E RID: 16958
		Death
	}
}
