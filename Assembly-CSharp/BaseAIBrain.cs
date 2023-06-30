using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000037 RID: 55
public class BaseAIBrain : EntityComponent<global::BaseEntity>, IPet, IAISleepable, IAIDesign, IAIGroupable, IAIEventListener
{
	// Token: 0x060001DD RID: 477 RVA: 0x00025AFC File Offset: 0x00023CFC
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseAIBrain.OnRpcMessage", 0))
		{
			if (rpc == 66191493U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RequestAIDesign ");
				}
				using (TimeWarning.New("RequestAIDesign", 0))
				{
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
							this.RequestAIDesign(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RequestAIDesign");
					}
				}
				return true;
			}
			if (rpc == 2122228512U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - StopAIDesign ");
				}
				using (TimeWarning.New("StopAIDesign", 0))
				{
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
							this.StopAIDesign(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in StopAIDesign");
					}
				}
				return true;
			}
			if (rpc == 657290375U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SubmitAIDesign ");
				}
				using (TimeWarning.New("SubmitAIDesign", 0))
				{
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
							this.SubmitAIDesign(rpcmessage3);
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in SubmitAIDesign");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060001DE RID: 478 RVA: 0x00025E68 File Offset: 0x00024068
	public bool IsPet()
	{
		return this.Pet;
	}

	// Token: 0x060001DF RID: 479 RVA: 0x00025E70 File Offset: 0x00024070
	public void SetPetOwner(global::BasePlayer player)
	{
		global::BaseEntity baseEntity = this.GetBaseEntity();
		player.PetEntity = baseEntity;
		baseEntity.OwnerID = player.userID;
		BasePet.ActivePetByOwnerID[player.userID] = baseEntity as BasePet;
	}

	// Token: 0x060001E0 RID: 480 RVA: 0x00025EAD File Offset: 0x000240AD
	public bool IsOwnedBy(global::BasePlayer player)
	{
		return !(this.OwningPlayer == null) && !(player == null) && this != null && this.OwningPlayer == player;
	}

	// Token: 0x060001E1 RID: 481 RVA: 0x00025EDC File Offset: 0x000240DC
	public bool IssuePetCommand(PetCommandType cmd, int param, Ray? ray)
	{
		if (ray != null)
		{
			int num = 10551296;
			RaycastHit raycastHit;
			if (UnityEngine.Physics.Raycast(ray.Value, out raycastHit, 75f, num))
			{
				this.Events.Memory.Position.Set(raycastHit.point, 6);
			}
			else
			{
				this.Events.Memory.Position.Set(base.transform.position, 6);
			}
		}
		switch (cmd)
		{
		case PetCommandType.LoadDesign:
			if (param < 0 || param >= this.Designs.Count)
			{
				return false;
			}
			this.LoadAIDesign(AIDesigns.GetByNameOrInstance(this.Designs[param].Filename, this.InstanceSpecificDesign), null, param);
			return true;
		case PetCommandType.SetState:
		{
			global::AIStateContainer stateContainerByID = this.AIDesign.GetStateContainerByID(param);
			return stateContainerByID != null && this.SwitchToState(stateContainerByID.State, param);
		}
		case PetCommandType.Destroy:
			this.GetBaseEntity().Kill(global::BaseNetworkable.DestroyMode.None);
			return true;
		default:
			return false;
		}
	}

	// Token: 0x17000039 RID: 57
	// (get) Token: 0x060001E2 RID: 482 RVA: 0x00025FCC File Offset: 0x000241CC
	// (set) Token: 0x060001E3 RID: 483 RVA: 0x00025FD4 File Offset: 0x000241D4
	public BaseAIBrain.BasicAIState CurrentState { get; private set; }

	// Token: 0x1700003A RID: 58
	// (get) Token: 0x060001E4 RID: 484 RVA: 0x00025FDD File Offset: 0x000241DD
	// (set) Token: 0x060001E5 RID: 485 RVA: 0x00025FE5 File Offset: 0x000241E5
	public AIThinkMode ThinkMode { get; protected set; } = AIThinkMode.Interval;

	// Token: 0x1700003B RID: 59
	// (get) Token: 0x060001E6 RID: 486 RVA: 0x00025FEE File Offset: 0x000241EE
	// (set) Token: 0x060001E7 RID: 487 RVA: 0x00025FF6 File Offset: 0x000241F6
	public float Age { get; private set; }

	// Token: 0x060001E8 RID: 488 RVA: 0x00025FFF File Offset: 0x000241FF
	public void ForceSetAge(float age)
	{
		this.Age = age;
	}

	// Token: 0x1700003C RID: 60
	// (get) Token: 0x060001E9 RID: 489 RVA: 0x00026008 File Offset: 0x00024208
	// (set) Token: 0x060001EA RID: 490 RVA: 0x00026010 File Offset: 0x00024210
	public AIBrainSenses Senses { get; private set; } = new AIBrainSenses();

	// Token: 0x1700003D RID: 61
	// (get) Token: 0x060001EB RID: 491 RVA: 0x00026019 File Offset: 0x00024219
	// (set) Token: 0x060001EC RID: 492 RVA: 0x00026021 File Offset: 0x00024221
	public BasePathFinder PathFinder { get; protected set; }

	// Token: 0x1700003E RID: 62
	// (get) Token: 0x060001ED RID: 493 RVA: 0x0002602A File Offset: 0x0002422A
	// (set) Token: 0x060001EE RID: 494 RVA: 0x00026032 File Offset: 0x00024232
	public AIEvents Events { get; private set; }

	// Token: 0x1700003F RID: 63
	// (get) Token: 0x060001EF RID: 495 RVA: 0x0002603B File Offset: 0x0002423B
	// (set) Token: 0x060001F0 RID: 496 RVA: 0x00026043 File Offset: 0x00024243
	public global::AIDesign AIDesign { get; private set; }

	// Token: 0x17000040 RID: 64
	// (get) Token: 0x060001F1 RID: 497 RVA: 0x0002604C File Offset: 0x0002424C
	// (set) Token: 0x060001F2 RID: 498 RVA: 0x00026054 File Offset: 0x00024254
	public global::BasePlayer DesigningPlayer { get; private set; }

	// Token: 0x17000041 RID: 65
	// (get) Token: 0x060001F3 RID: 499 RVA: 0x0002605D File Offset: 0x0002425D
	// (set) Token: 0x060001F4 RID: 500 RVA: 0x00026065 File Offset: 0x00024265
	public global::BasePlayer OwningPlayer { get; private set; }

	// Token: 0x17000042 RID: 66
	// (get) Token: 0x060001F5 RID: 501 RVA: 0x0002606E File Offset: 0x0002426E
	// (set) Token: 0x060001F6 RID: 502 RVA: 0x00026076 File Offset: 0x00024276
	public bool IsGroupLeader { get; private set; }

	// Token: 0x17000043 RID: 67
	// (get) Token: 0x060001F7 RID: 503 RVA: 0x0002607F File Offset: 0x0002427F
	// (set) Token: 0x060001F8 RID: 504 RVA: 0x00026087 File Offset: 0x00024287
	public bool IsGrouped { get; private set; }

	// Token: 0x17000044 RID: 68
	// (get) Token: 0x060001F9 RID: 505 RVA: 0x00026090 File Offset: 0x00024290
	// (set) Token: 0x060001FA RID: 506 RVA: 0x00026098 File Offset: 0x00024298
	public IAIGroupable GroupLeader { get; private set; }

	// Token: 0x060001FB RID: 507 RVA: 0x000260A1 File Offset: 0x000242A1
	public int LoadedDesignIndex()
	{
		return this.loadedDesignIndex;
	}

	// Token: 0x17000045 RID: 69
	// (get) Token: 0x060001FC RID: 508 RVA: 0x000260A9 File Offset: 0x000242A9
	// (set) Token: 0x060001FD RID: 509 RVA: 0x000260B1 File Offset: 0x000242B1
	public BaseNavigator Navigator { get; private set; }

	// Token: 0x060001FE RID: 510 RVA: 0x000260BA File Offset: 0x000242BA
	public void SetEnabled(bool flag)
	{
		this.disabled = !flag;
	}

	// Token: 0x060001FF RID: 511 RVA: 0x000260C6 File Offset: 0x000242C6
	bool IAIDesign.CanPlayerDesignAI(global::BasePlayer player)
	{
		return this.PlayerCanDesignAI(player);
	}

	// Token: 0x06000200 RID: 512 RVA: 0x000260CF File Offset: 0x000242CF
	private bool PlayerCanDesignAI(global::BasePlayer player)
	{
		return AI.allowdesigning && !(player == null) && this.UseAIDesign && !(this.DesigningPlayer != null) && player.IsDeveloper;
	}

	// Token: 0x06000201 RID: 513 RVA: 0x0002610C File Offset: 0x0002430C
	[global::BaseEntity.RPC_Server]
	private void RequestAIDesign(global::BaseEntity.RPCMessage msg)
	{
		if (!this.UseAIDesign)
		{
			return;
		}
		if (msg.player == null)
		{
			return;
		}
		if (this.AIDesign == null)
		{
			return;
		}
		if (!this.PlayerCanDesignAI(msg.player))
		{
			return;
		}
		msg.player.designingAIEntity = this.GetBaseEntity();
		msg.player.ClientRPCPlayer<ProtoBuf.AIDesign>(null, msg.player, "StartDesigningAI", this.AIDesign.ToProto(this.currentStateContainerID));
		this.DesigningPlayer = msg.player;
		this.SetOwningPlayer(msg.player);
	}

	// Token: 0x06000202 RID: 514 RVA: 0x0002619C File Offset: 0x0002439C
	[global::BaseEntity.RPC_Server]
	private void SubmitAIDesign(global::BaseEntity.RPCMessage msg)
	{
		ProtoBuf.AIDesign aidesign = ProtoBuf.AIDesign.Deserialize(msg.read);
		if (!this.LoadAIDesign(aidesign, msg.player, this.loadedDesignIndex))
		{
			return;
		}
		this.SaveDesign();
		if (aidesign.scope == 2)
		{
			return;
		}
		global::BaseEntity baseEntity = this.GetBaseEntity();
		global::BaseEntity[] array = global::BaseEntity.Util.FindTargets(baseEntity.ShortPrefabName, false);
		if (array == null || array.Length == 0)
		{
			return;
		}
		foreach (global::BaseEntity baseEntity2 in array)
		{
			if (!(baseEntity2 == null) && !(baseEntity2 == baseEntity))
			{
				EntityComponentBase[] components = baseEntity2.Components;
				if (components != null)
				{
					EntityComponentBase[] array3 = components;
					for (int j = 0; j < array3.Length; j++)
					{
						IAIDesign iaidesign;
						if ((iaidesign = array3[j] as IAIDesign) != null)
						{
							iaidesign.LoadAIDesign(aidesign, null);
							break;
						}
					}
				}
			}
		}
	}

	// Token: 0x06000203 RID: 515 RVA: 0x00026265 File Offset: 0x00024465
	void IAIDesign.StopDesigning()
	{
		this.ClearDesigningPlayer();
	}

	// Token: 0x06000204 RID: 516 RVA: 0x0002626D File Offset: 0x0002446D
	void IAIDesign.LoadAIDesign(ProtoBuf.AIDesign design, global::BasePlayer player)
	{
		this.LoadAIDesign(design, player, this.loadedDesignIndex);
	}

	// Token: 0x06000205 RID: 517 RVA: 0x0002627E File Offset: 0x0002447E
	public bool LoadDefaultAIDesign()
	{
		return this.loadedDesignIndex == 0 || this.LoadAIDesignAtIndex(0);
	}

	// Token: 0x06000206 RID: 518 RVA: 0x00026294 File Offset: 0x00024494
	public bool LoadAIDesignAtIndex(int index)
	{
		return this.Designs != null && index >= 0 && index < this.Designs.Count && this.LoadAIDesign(AIDesigns.GetByNameOrInstance(this.Designs[index].Filename, this.InstanceSpecificDesign), null, index);
	}

	// Token: 0x06000207 RID: 519 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnAIDesignLoadedAtIndex(int index)
	{
	}

	// Token: 0x06000208 RID: 520 RVA: 0x000262E4 File Offset: 0x000244E4
	protected bool LoadAIDesign(ProtoBuf.AIDesign design, global::BasePlayer player, int index)
	{
		if (design == null)
		{
			Debug.LogError(this.GetBaseEntity().gameObject.name + " failed to load AI design!");
			return false;
		}
		if (player != null)
		{
			AIDesignScope scope = (AIDesignScope)design.scope;
			if (scope == AIDesignScope.Default && !player.IsDeveloper)
			{
				return false;
			}
			if (scope == AIDesignScope.EntityServerWide && !player.IsDeveloper && !player.IsAdmin)
			{
				return false;
			}
		}
		if (this.AIDesign == null)
		{
			return false;
		}
		this.AIDesign.Load(design, base.baseEntity);
		global::AIStateContainer defaultStateContainer = this.AIDesign.GetDefaultStateContainer();
		if (defaultStateContainer != null)
		{
			this.SwitchToState(defaultStateContainer.State, defaultStateContainer.ID);
		}
		this.loadedDesignIndex = index;
		this.OnAIDesignLoadedAtIndex(this.loadedDesignIndex);
		return true;
	}

	// Token: 0x06000209 RID: 521 RVA: 0x0002639C File Offset: 0x0002459C
	public void SaveDesign()
	{
		if (this.AIDesign == null)
		{
			return;
		}
		ProtoBuf.AIDesign aidesign = this.AIDesign.ToProto(this.currentStateContainerID);
		string text = "cfg/ai/";
		string text2 = this.Designs[this.loadedDesignIndex].Filename;
		switch (this.AIDesign.Scope)
		{
		case AIDesignScope.Default:
			text += text2;
			try
			{
				using (FileStream fileStream = File.Create(text))
				{
					ProtoBuf.AIDesign.Serialize(fileStream, aidesign);
				}
				AIDesigns.RefreshCache(text2, aidesign);
				return;
			}
			catch (Exception)
			{
				Debug.LogWarning("Error trying to save default AI Design: " + text);
				return;
			}
			break;
		case AIDesignScope.EntityServerWide:
			break;
		case AIDesignScope.EntityInstance:
			return;
		default:
			return;
		}
		text2 += "_custom";
		text += text2;
		try
		{
			using (FileStream fileStream2 = File.Create(text))
			{
				ProtoBuf.AIDesign.Serialize(fileStream2, aidesign);
			}
			AIDesigns.RefreshCache(text2, aidesign);
		}
		catch (Exception)
		{
			Debug.LogWarning("Error trying to save server-wide AI Design: " + text);
		}
	}

	// Token: 0x0600020A RID: 522 RVA: 0x000264C4 File Offset: 0x000246C4
	[global::BaseEntity.RPC_Server]
	private void StopAIDesign(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player == this.DesigningPlayer)
		{
			this.ClearDesigningPlayer();
		}
	}

	// Token: 0x0600020B RID: 523 RVA: 0x000264DF File Offset: 0x000246DF
	private void ClearDesigningPlayer()
	{
		this.DesigningPlayer = null;
	}

	// Token: 0x0600020C RID: 524 RVA: 0x000264E8 File Offset: 0x000246E8
	public void SetOwningPlayer(global::BasePlayer owner)
	{
		this.OwningPlayer = owner;
		this.Events.Memory.Entity.Set(this.OwningPlayer, 5);
		if (this != null && ((IPet)this).IsPet())
		{
			((IPet)this).SetPetOwner(owner);
			owner.Pet = this;
		}
	}

	// Token: 0x0600020D RID: 525 RVA: 0x00026533 File Offset: 0x00024733
	public virtual bool ShouldServerThink()
	{
		return this.ThinkMode == AIThinkMode.Interval && UnityEngine.Time.time > this.lastThinkTime + this.thinkRate;
	}

	// Token: 0x0600020E RID: 526 RVA: 0x00026558 File Offset: 0x00024758
	public virtual void DoThink()
	{
		float num = UnityEngine.Time.time - this.lastThinkTime;
		this.Think(num);
	}

	// Token: 0x0600020F RID: 527 RVA: 0x00026579 File Offset: 0x00024779
	public List<AIState> GetStateList()
	{
		return this.states.Keys.ToList<AIState>();
	}

	// Token: 0x06000210 RID: 528 RVA: 0x0002658B File Offset: 0x0002478B
	public bool Blinded()
	{
		return UnityEngine.Time.time < this.unblindTime;
	}

	// Token: 0x06000211 RID: 529 RVA: 0x0002659C File Offset: 0x0002479C
	public void SetBlinded(float duration)
	{
		if (!this.CanBeBlinded)
		{
			return;
		}
		if (this.Blinded())
		{
			return;
		}
		this.unblindTime = UnityEngine.Time.time + duration;
		if (this.HasState(AIState.Blinded) && this.AIDesign != null)
		{
			BaseAIBrain.BasicAIState basicAIState = this.states[AIState.Blinded];
			global::AIStateContainer firstStateContainerOfType = this.AIDesign.GetFirstStateContainerOfType(AIState.Blinded);
			if (basicAIState != null && firstStateContainerOfType != null)
			{
				this.SwitchToState(basicAIState, firstStateContainerOfType.ID);
			}
		}
	}

	// Token: 0x06000212 RID: 530 RVA: 0x0002660A File Offset: 0x0002480A
	public void Start()
	{
		this.AddStates();
		this.InitializeAI();
	}

	// Token: 0x06000213 RID: 531 RVA: 0x00026618 File Offset: 0x00024818
	public virtual void AddStates()
	{
		this.states = new Dictionary<AIState, BaseAIBrain.BasicAIState>();
	}

	// Token: 0x06000214 RID: 532 RVA: 0x00026628 File Offset: 0x00024828
	public virtual void InitializeAI()
	{
		global::BaseEntity baseEntity = this.GetBaseEntity();
		baseEntity.HasBrain = true;
		this.Navigator = base.GetComponent<BaseNavigator>();
		if (this.UseAIDesign)
		{
			this.AIDesign = new global::AIDesign();
			this.AIDesign.SetAvailableStates(this.GetStateList());
			if (this.Events == null)
			{
				this.Events = new AIEvents();
			}
			bool flag = this.MaxGroupSize > 0;
			this.Senses.Init(baseEntity, this, this.MemoryDuration, this.SenseRange, this.TargetLostRange, this.VisionCone, this.CheckVisionCone, this.CheckLOS, this.IgnoreNonVisionSneakers, this.ListenRange, this.HostileTargetsOnly, flag, this.IgnoreSafeZonePlayers, this.SenseTypes, this.RefreshKnownLOS);
			if (this.DefaultDesignSO == null && this.Designs.Count == 0)
			{
				Debug.LogWarning("Brain on " + base.gameObject.name + " is trying to load a null AI design!");
				return;
			}
			this.Events.Memory.Position.Set(base.transform.position, 4);
			if (this.Designs.Count == 0)
			{
				this.Designs.Add(this.DefaultDesignSO);
			}
			this.loadedDesignIndex = 0;
			this.LoadAIDesign(AIDesigns.GetByNameOrInstance(this.Designs[this.loadedDesignIndex].Filename, this.InstanceSpecificDesign), null, this.loadedDesignIndex);
			AIInformationZone forPoint = AIInformationZone.GetForPoint(base.transform.position, false);
			if (forPoint != null)
			{
				forPoint.RegisterSleepableEntity(this);
			}
		}
		global::BaseEntity.Query.Server.AddBrain(baseEntity);
		this.StartMovementTick();
	}

	// Token: 0x06000215 RID: 533 RVA: 0x000267CC File Offset: 0x000249CC
	public global::BaseEntity GetBrainBaseEntity()
	{
		return this.GetBaseEntity();
	}

	// Token: 0x06000216 RID: 534 RVA: 0x000267D4 File Offset: 0x000249D4
	public virtual void OnDestroy()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		global::BaseEntity.Query.Server.RemoveBrain(this.GetBaseEntity());
		AIInformationZone aiinformationZone = null;
		HumanNPC humanNPC = this.GetBaseEntity() as HumanNPC;
		if (humanNPC != null)
		{
			aiinformationZone = humanNPC.VirtualInfoZone;
		}
		if (aiinformationZone == null)
		{
			aiinformationZone = AIInformationZone.GetForPoint(base.transform.position, true);
		}
		if (aiinformationZone != null)
		{
			aiinformationZone.UnregisterSleepableEntity(this);
		}
		this.LeaveGroup();
	}

	// Token: 0x06000217 RID: 535 RVA: 0x00026848 File Offset: 0x00024A48
	private void StartMovementTick()
	{
		base.CancelInvoke(new Action(this.TickMovement));
		base.InvokeRandomized(new Action(this.TickMovement), 1f, 0.1f, 0.010000001f);
	}

	// Token: 0x06000218 RID: 536 RVA: 0x0002687D File Offset: 0x00024A7D
	private void StopMovementTick()
	{
		base.CancelInvoke(new Action(this.TickMovement));
	}

	// Token: 0x06000219 RID: 537 RVA: 0x00026894 File Offset: 0x00024A94
	public void TickMovement()
	{
		if (BasePet.queuedMovementsAllowed && this.UseQueuedMovementUpdates && this.Navigator != null)
		{
			if (BasePet.onlyQueueBaseNavMovements && this.Navigator.CurrentNavigationType != BaseNavigator.NavigationType.Base)
			{
				this.DoMovementTick();
				return;
			}
			BasePet basePet = this.GetBaseEntity() as BasePet;
			if (basePet != null && !basePet.inQueue)
			{
				BasePet._movementProcessQueue.Enqueue(basePet);
				basePet.inQueue = true;
				return;
			}
		}
		else
		{
			this.DoMovementTick();
		}
	}

	// Token: 0x0600021A RID: 538 RVA: 0x00026910 File Offset: 0x00024B10
	public void DoMovementTick()
	{
		float num = UnityEngine.Time.realtimeSinceStartup - this.lastMovementTickTime;
		this.lastMovementTickTime = UnityEngine.Time.realtimeSinceStartup;
		if (this.Navigator != null)
		{
			this.Navigator.Think(num);
		}
	}

	// Token: 0x0600021B RID: 539 RVA: 0x00026950 File Offset: 0x00024B50
	public void AddState(BaseAIBrain.BasicAIState newState)
	{
		if (this.states.ContainsKey(newState.StateType))
		{
			Debug.LogWarning("Trying to add duplicate state: " + newState.StateType.ToString() + " to " + this.GetBaseEntity().PrefabName);
			return;
		}
		newState.brain = this;
		newState.Reset();
		this.states.Add(newState.StateType, newState);
	}

	// Token: 0x0600021C RID: 540 RVA: 0x000269C3 File Offset: 0x00024BC3
	public bool HasState(AIState state)
	{
		return this.states.ContainsKey(state);
	}

	// Token: 0x0600021D RID: 541 RVA: 0x000269D1 File Offset: 0x00024BD1
	protected bool SwitchToState(AIState newState, int stateContainerID = -1)
	{
		if (!this.HasState(newState))
		{
			return false;
		}
		bool flag = this.SwitchToState(this.states[newState], stateContainerID);
		if (flag)
		{
			this.OnStateChanged();
		}
		return flag;
	}

	// Token: 0x0600021E RID: 542 RVA: 0x000269FC File Offset: 0x00024BFC
	private bool SwitchToState(BaseAIBrain.BasicAIState newState, int stateContainerID = -1)
	{
		if (newState == null || !newState.CanEnter())
		{
			return false;
		}
		if (this.CurrentState != null)
		{
			if (!this.CurrentState.CanLeave())
			{
				return false;
			}
			if (this.CurrentState == newState && !this.UseAIDesign)
			{
				return false;
			}
			this.CurrentState.StateLeave(this, this.GetBaseEntity());
		}
		this.AddEvents(stateContainerID);
		this.CurrentState = newState;
		this.CurrentState.StateEnter(this, this.GetBaseEntity());
		this.currentStateContainerID = stateContainerID;
		return true;
	}

	// Token: 0x0600021F RID: 543 RVA: 0x00026A7C File Offset: 0x00024C7C
	protected virtual void OnStateChanged()
	{
		if (this.SendClientCurrentState)
		{
			global::BaseEntity baseEntity = this.GetBaseEntity();
			if (baseEntity != null)
			{
				baseEntity.ClientRPC<int>(null, "ClientChangeState", (int)((this.CurrentState != null) ? this.CurrentState.StateType : AIState.None));
			}
		}
	}

	// Token: 0x06000220 RID: 544 RVA: 0x00026AC3 File Offset: 0x00024CC3
	private void AddEvents(int stateContainerID)
	{
		if (!this.UseAIDesign)
		{
			return;
		}
		if (this.AIDesign == null)
		{
			return;
		}
		this.Events.Init(this, this.AIDesign.GetStateContainerByID(stateContainerID), base.baseEntity, this.Senses);
	}

	// Token: 0x06000221 RID: 545 RVA: 0x00026AFC File Offset: 0x00024CFC
	public virtual void Think(float delta)
	{
		if (!AI.think)
		{
			return;
		}
		this.lastThinkTime = UnityEngine.Time.time;
		if (this.sleeping || this.disabled)
		{
			return;
		}
		this.Age += delta;
		if (this.UseAIDesign)
		{
			this.Senses.Update();
			this.UpdateGroup();
		}
		if (this.CurrentState != null)
		{
			this.UpdateAgressionTimer(delta);
			StateStatus stateStatus = this.CurrentState.StateThink(delta, this, this.GetBaseEntity());
			if (this.Events != null)
			{
				this.Events.Tick(delta, stateStatus);
			}
		}
		if (!this.UseAIDesign && (this.CurrentState == null || this.CurrentState.CanLeave()))
		{
			float num = 0f;
			BaseAIBrain.BasicAIState basicAIState = null;
			foreach (BaseAIBrain.BasicAIState basicAIState2 in this.states.Values)
			{
				if (basicAIState2 != null && basicAIState2.CanEnter())
				{
					float weight = basicAIState2.GetWeight();
					if (weight > num)
					{
						num = weight;
						basicAIState = basicAIState2;
					}
				}
			}
			if (basicAIState != this.CurrentState)
			{
				this.SwitchToState(basicAIState, -1);
			}
		}
	}

	// Token: 0x06000222 RID: 546 RVA: 0x00026C2C File Offset: 0x00024E2C
	private void UpdateAgressionTimer(float delta)
	{
		if (this.CurrentState == null)
		{
			this.Senses.TimeInAgressiveState = 0f;
			return;
		}
		if (this.CurrentState.AgrresiveState)
		{
			this.Senses.TimeInAgressiveState += delta;
			return;
		}
		this.Senses.TimeInAgressiveState = 0f;
	}

	// Token: 0x06000223 RID: 547 RVA: 0x00026C83 File Offset: 0x00024E83
	bool IAISleepable.AllowedToSleep()
	{
		return this.AllowedToSleep;
	}

	// Token: 0x06000224 RID: 548 RVA: 0x00026C8B File Offset: 0x00024E8B
	void IAISleepable.SleepAI()
	{
		if (this.sleeping)
		{
			return;
		}
		this.sleeping = true;
		if (this.Navigator != null)
		{
			this.Navigator.Pause();
		}
		this.StopMovementTick();
	}

	// Token: 0x06000225 RID: 549 RVA: 0x00026CBC File Offset: 0x00024EBC
	void IAISleepable.WakeAI()
	{
		if (!this.sleeping)
		{
			return;
		}
		this.sleeping = false;
		if (this.Navigator != null)
		{
			this.Navigator.Resume();
		}
		this.StartMovementTick();
	}

	// Token: 0x06000226 RID: 550 RVA: 0x00026CF0 File Offset: 0x00024EF0
	private void UpdateGroup()
	{
		if (!AI.groups)
		{
			return;
		}
		if (this.MaxGroupSize <= 0)
		{
			return;
		}
		if (!this.InGroup() && this.Senses.Memory.Friendlies.Count > 0)
		{
			IAIGroupable iaigroupable = null;
			foreach (global::BaseEntity baseEntity in this.Senses.Memory.Friendlies)
			{
				if (!(baseEntity == null))
				{
					IAIGroupable component = baseEntity.GetComponent<IAIGroupable>();
					if (component != null)
					{
						if (component.InGroup() && component.AddMember(this))
						{
							break;
						}
						if (iaigroupable == null && !component.InGroup())
						{
							iaigroupable = component;
						}
					}
				}
			}
			if (!this.InGroup() && iaigroupable != null)
			{
				this.AddMember(iaigroupable);
			}
		}
	}

	// Token: 0x06000227 RID: 551 RVA: 0x00026DC8 File Offset: 0x00024FC8
	public bool AddMember(IAIGroupable member)
	{
		if (this.InGroup() && !this.IsGroupLeader)
		{
			return this.GroupLeader.AddMember(member);
		}
		if (this.MaxGroupSize <= 0)
		{
			return false;
		}
		if (this.groupMembers.Contains(member))
		{
			return true;
		}
		if (this.groupMembers.Count + 1 >= this.MaxGroupSize)
		{
			return false;
		}
		this.groupMembers.Add(member);
		this.IsGrouped = true;
		this.IsGroupLeader = true;
		this.GroupLeader = this;
		global::BaseEntity baseEntity = this.GetBaseEntity();
		this.Events.Memory.Entity.Set(baseEntity, 6);
		member.JoinGroup(this, baseEntity);
		return true;
	}

	// Token: 0x06000228 RID: 552 RVA: 0x00026E6C File Offset: 0x0002506C
	public void JoinGroup(IAIGroupable leader, global::BaseEntity leaderEntity)
	{
		this.Events.Memory.Entity.Set(leaderEntity, 6);
		this.GroupLeader = leader;
		this.IsGroupLeader = false;
		this.IsGrouped = true;
	}

	// Token: 0x06000229 RID: 553 RVA: 0x00026E9C File Offset: 0x0002509C
	public void SetGroupRoamRootPosition(Vector3 rootPos)
	{
		if (this.IsGroupLeader)
		{
			foreach (IAIGroupable iaigroupable in this.groupMembers)
			{
				iaigroupable.SetGroupRoamRootPosition(rootPos);
			}
		}
		this.Events.Memory.Position.Set(rootPos, 5);
	}

	// Token: 0x0600022A RID: 554 RVA: 0x00026F0C File Offset: 0x0002510C
	public bool InGroup()
	{
		return this.IsGrouped;
	}

	// Token: 0x0600022B RID: 555 RVA: 0x00026F14 File Offset: 0x00025114
	public void LeaveGroup()
	{
		if (!this.InGroup())
		{
			return;
		}
		if (!this.IsGroupLeader)
		{
			if (this.GroupLeader != null)
			{
				this.GroupLeader.RemoveMember(base.GetComponent<IAIGroupable>());
			}
			return;
		}
		if (this.groupMembers.Count == 0)
		{
			return;
		}
		IAIGroupable iaigroupable = this.groupMembers[0];
		if (iaigroupable == null)
		{
			return;
		}
		this.RemoveMember(iaigroupable);
		for (int i = this.groupMembers.Count - 1; i >= 0; i--)
		{
			IAIGroupable iaigroupable2 = this.groupMembers[i];
			if (iaigroupable2 != null && iaigroupable2 != iaigroupable)
			{
				this.RemoveMember(iaigroupable2);
				iaigroupable.AddMember(iaigroupable2);
			}
		}
		this.groupMembers.Clear();
	}

	// Token: 0x0600022C RID: 556 RVA: 0x00026FB8 File Offset: 0x000251B8
	public void RemoveMember(IAIGroupable member)
	{
		if (member == null)
		{
			return;
		}
		if (!this.IsGroupLeader)
		{
			return;
		}
		if (!this.groupMembers.Contains(member))
		{
			return;
		}
		this.groupMembers.Remove(member);
		member.SetUngrouped();
		if (this.groupMembers.Count == 0)
		{
			this.SetUngrouped();
		}
	}

	// Token: 0x0600022D RID: 557 RVA: 0x00027007 File Offset: 0x00025207
	public void SetUngrouped()
	{
		this.IsGrouped = false;
		this.IsGroupLeader = false;
		this.GroupLeader = null;
	}

	// Token: 0x0600022E RID: 558 RVA: 0x0002701E File Offset: 0x0002521E
	public override void LoadComponent(global::BaseNetworkable.LoadInfo info)
	{
		base.LoadComponent(info);
	}

	// Token: 0x0600022F RID: 559 RVA: 0x00027028 File Offset: 0x00025228
	public override void SaveComponent(global::BaseNetworkable.SaveInfo info)
	{
		base.SaveComponent(info);
		if (this.SendClientCurrentState && this.CurrentState != null)
		{
			info.msg.brainComponent = Facepunch.Pool.Get<BrainComponent>();
			info.msg.brainComponent.currentState = (int)this.CurrentState.StateType;
		}
	}

	// Token: 0x06000230 RID: 560 RVA: 0x00027077 File Offset: 0x00025277
	private void SendStateChangeEvent(int previousStateID, int newStateID, int sourceEventID)
	{
		if (this.DesigningPlayer != null)
		{
			this.DesigningPlayer.ClientRPCPlayer<int, int, int>(null, this.DesigningPlayer, "OnDebugAIEventTriggeredStateChange", previousStateID, newStateID, sourceEventID);
		}
	}

	// Token: 0x06000231 RID: 561 RVA: 0x000270A4 File Offset: 0x000252A4
	public void EventTriggeredStateChange(int newStateContainerID, int sourceEventID)
	{
		if (this.AIDesign == null)
		{
			return;
		}
		if (newStateContainerID == -1)
		{
			return;
		}
		global::AIStateContainer stateContainerByID = this.AIDesign.GetStateContainerByID(newStateContainerID);
		int num = this.currentStateContainerID;
		this.SwitchToState(stateContainerByID.State, newStateContainerID);
		this.SendStateChangeEvent(num, this.currentStateContainerID, sourceEventID);
	}

	// Token: 0x040001E3 RID: 483
	public bool SendClientCurrentState;

	// Token: 0x040001E4 RID: 484
	public bool UseQueuedMovementUpdates;

	// Token: 0x040001E5 RID: 485
	public bool AllowedToSleep = true;

	// Token: 0x040001E6 RID: 486
	public AIDesignSO DefaultDesignSO;

	// Token: 0x040001E7 RID: 487
	public List<AIDesignSO> Designs = new List<AIDesignSO>();

	// Token: 0x040001E8 RID: 488
	public ProtoBuf.AIDesign InstanceSpecificDesign;

	// Token: 0x040001E9 RID: 489
	public float SenseRange = 10f;

	// Token: 0x040001EA RID: 490
	public float AttackRangeMultiplier = 1f;

	// Token: 0x040001EB RID: 491
	public float TargetLostRange = 40f;

	// Token: 0x040001EC RID: 492
	public float VisionCone = -0.8f;

	// Token: 0x040001ED RID: 493
	public bool CheckVisionCone;

	// Token: 0x040001EE RID: 494
	public bool CheckLOS;

	// Token: 0x040001EF RID: 495
	public bool IgnoreNonVisionSneakers = true;

	// Token: 0x040001F0 RID: 496
	public float IgnoreSneakersMaxDistance = 4f;

	// Token: 0x040001F1 RID: 497
	public float IgnoreNonVisionMaxDistance = 15f;

	// Token: 0x040001F2 RID: 498
	public float ListenRange;

	// Token: 0x040001F3 RID: 499
	public EntityType SenseTypes;

	// Token: 0x040001F4 RID: 500
	public bool HostileTargetsOnly;

	// Token: 0x040001F5 RID: 501
	public bool IgnoreSafeZonePlayers;

	// Token: 0x040001F6 RID: 502
	public int MaxGroupSize;

	// Token: 0x040001F7 RID: 503
	public float MemoryDuration = 10f;

	// Token: 0x040001F8 RID: 504
	public bool RefreshKnownLOS;

	// Token: 0x040001F9 RID: 505
	public bool CanBeBlinded = true;

	// Token: 0x040001FA RID: 506
	public float BlindDurationMultiplier = 1f;

	// Token: 0x040001FC RID: 508
	public AIState ClientCurrentState;

	// Token: 0x040001FD RID: 509
	public Vector3 mainInterestPoint;

	// Token: 0x04000202 RID: 514
	public bool UseAIDesign;

	// Token: 0x0400020A RID: 522
	public bool Pet;

	// Token: 0x0400020B RID: 523
	private List<IAIGroupable> groupMembers = new List<IAIGroupable>();

	// Token: 0x0400020C RID: 524
	[Header("Healing")]
	public bool CanUseHealingItems;

	// Token: 0x0400020D RID: 525
	public float HealChance = 0.5f;

	// Token: 0x0400020E RID: 526
	public float HealBelowHealthFraction = 0.5f;

	// Token: 0x0400020F RID: 527
	protected int loadedDesignIndex;

	// Token: 0x04000211 RID: 529
	private int currentStateContainerID = -1;

	// Token: 0x04000212 RID: 530
	private float lastMovementTickTime;

	// Token: 0x04000213 RID: 531
	private bool sleeping;

	// Token: 0x04000214 RID: 532
	private bool disabled;

	// Token: 0x04000215 RID: 533
	protected Dictionary<AIState, BaseAIBrain.BasicAIState> states;

	// Token: 0x04000216 RID: 534
	protected float thinkRate = 0.25f;

	// Token: 0x04000217 RID: 535
	protected float lastThinkTime;

	// Token: 0x04000218 RID: 536
	protected float unblindTime;

	// Token: 0x02000B6A RID: 2922
	public class BaseAttackState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004CE1 RID: 19681 RVA: 0x0019F562 File Offset: 0x0019D762
		public BaseAttackState()
			: base(AIState.Attack)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004CE2 RID: 19682 RVA: 0x0019F574 File Offset: 0x0019D774
		public override void StateEnter(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.attack = entity as IAIAttack;
			global::BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity != null)
			{
				Vector3 aimDirection = BaseAIBrain.BaseAttackState.GetAimDirection(brain.Navigator.transform.position, baseEntity.transform.position);
				brain.Navigator.SetFacingDirectionOverride(aimDirection);
				if (this.attack.CanAttack(baseEntity))
				{
					this.StartAttacking(baseEntity);
				}
				brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
			}
		}

		// Token: 0x06004CE3 RID: 19683 RVA: 0x0019F623 File Offset: 0x0019D823
		public override void StateLeave(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			brain.Navigator.ClearFacingDirectionOverride();
			brain.Navigator.Stop();
			this.StopAttacking();
		}

		// Token: 0x06004CE4 RID: 19684 RVA: 0x0019F649 File Offset: 0x0019D849
		private void StopAttacking()
		{
			this.attack.StopAttacking();
		}

		// Token: 0x06004CE5 RID: 19685 RVA: 0x0019F658 File Offset: 0x0019D858
		public override StateStatus StateThink(float delta, BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			global::BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (this.attack == null)
			{
				return StateStatus.Error;
			}
			if (baseEntity == null)
			{
				brain.Navigator.ClearFacingDirectionOverride();
				this.StopAttacking();
				return StateStatus.Finished;
			}
			if (brain.Senses.ignoreSafeZonePlayers)
			{
				global::BasePlayer basePlayer = baseEntity as global::BasePlayer;
				if (basePlayer != null && basePlayer.InSafeZone())
				{
					return StateStatus.Error;
				}
			}
			if (!brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0.25f, 0f))
			{
				return StateStatus.Error;
			}
			Vector3 aimDirection = BaseAIBrain.BaseAttackState.GetAimDirection(brain.Navigator.transform.position, baseEntity.transform.position);
			brain.Navigator.SetFacingDirectionOverride(aimDirection);
			if (this.attack.CanAttack(baseEntity))
			{
				this.StartAttacking(baseEntity);
			}
			else
			{
				this.StopAttacking();
			}
			return StateStatus.Running;
		}

		// Token: 0x06004CE6 RID: 19686 RVA: 0x0019F74D File Offset: 0x0019D94D
		private static Vector3 GetAimDirection(Vector3 from, Vector3 target)
		{
			return Vector3Ex.Direction2D(target, from);
		}

		// Token: 0x06004CE7 RID: 19687 RVA: 0x0019F756 File Offset: 0x0019D956
		private void StartAttacking(global::BaseEntity entity)
		{
			this.attack.StartAttacking(entity);
		}

		// Token: 0x04003F7C RID: 16252
		private IAIAttack attack;
	}

	// Token: 0x02000B6B RID: 2923
	public class BaseBlindedState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004CE8 RID: 19688 RVA: 0x0019F765 File Offset: 0x0019D965
		public BaseBlindedState()
			: base(AIState.Blinded)
		{
		}
	}

	// Token: 0x02000B6C RID: 2924
	public class BaseChaseState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004CE9 RID: 19689 RVA: 0x0019F76F File Offset: 0x0019D96F
		public BaseChaseState()
			: base(AIState.Chase)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004CEA RID: 19690 RVA: 0x0019F780 File Offset: 0x0019D980
		public override void StateEnter(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			global::BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity != null)
			{
				brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
			}
		}

		// Token: 0x06004CEB RID: 19691 RVA: 0x0019F7E1 File Offset: 0x0019D9E1
		public override void StateLeave(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004CEC RID: 19692 RVA: 0x0019F7F1 File Offset: 0x0019D9F1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004CED RID: 19693 RVA: 0x0019F804 File Offset: 0x0019DA04
		public override StateStatus StateThink(float delta, BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			global::BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				this.Stop();
				return StateStatus.Error;
			}
			if (!brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0.25f, 0f))
			{
				return StateStatus.Error;
			}
			if (!brain.Navigator.Moving)
			{
				return StateStatus.Finished;
			}
			return StateStatus.Running;
		}
	}

	// Token: 0x02000B6D RID: 2925
	public class BaseCooldownState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004CEE RID: 19694 RVA: 0x0019F882 File Offset: 0x0019DA82
		public BaseCooldownState()
			: base(AIState.Cooldown)
		{
		}
	}

	// Token: 0x02000B6E RID: 2926
	public class BaseDismountedState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004CEF RID: 19695 RVA: 0x0019F88C File Offset: 0x0019DA8C
		public BaseDismountedState()
			: base(AIState.Dismounted)
		{
		}
	}

	// Token: 0x02000B6F RID: 2927
	public class BaseFleeState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004CF0 RID: 19696 RVA: 0x0019F896 File Offset: 0x0019DA96
		public BaseFleeState()
			: base(AIState.Flee)
		{
		}

		// Token: 0x06004CF1 RID: 19697 RVA: 0x0019F8AC File Offset: 0x0019DAAC
		public override void StateEnter(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			global::BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity != null)
			{
				this.stopFleeDistance = UnityEngine.Random.Range(80f, 100f) + Mathf.Clamp(Vector3Ex.Distance2D(brain.Navigator.transform.position, baseEntity.transform.position), 0f, 50f);
			}
			this.FleeFrom(brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot), entity);
		}

		// Token: 0x06004CF2 RID: 19698 RVA: 0x0019F958 File Offset: 0x0019DB58
		public override void StateLeave(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004CF3 RID: 19699 RVA: 0x0019F7F1 File Offset: 0x0019D9F1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004CF4 RID: 19700 RVA: 0x0019F968 File Offset: 0x0019DB68
		public override StateStatus StateThink(float delta, BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			global::BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				return StateStatus.Finished;
			}
			if (Vector3Ex.Distance2D(brain.Navigator.transform.position, baseEntity.transform.position) >= this.stopFleeDistance)
			{
				return StateStatus.Finished;
			}
			if ((brain.Navigator.UpdateIntervalElapsed(this.nextInterval) || !brain.Navigator.Moving) && !this.FleeFrom(baseEntity, entity))
			{
				return StateStatus.Error;
			}
			return StateStatus.Running;
		}

		// Token: 0x06004CF5 RID: 19701 RVA: 0x0019FA08 File Offset: 0x0019DC08
		private bool FleeFrom(global::BaseEntity fleeFromEntity, global::BaseEntity thisEntity)
		{
			if (thisEntity == null || fleeFromEntity == null)
			{
				return false;
			}
			this.nextInterval = UnityEngine.Random.Range(3f, 6f);
			Vector3 vector;
			if (!this.brain.PathFinder.GetBestFleePosition(this.brain.Navigator, this.brain.Senses, fleeFromEntity, this.brain.Events.Memory.Position.Get(4), 50f, 100f, out vector))
			{
				return false;
			}
			bool flag = this.brain.Navigator.SetDestination(vector, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
			if (!flag)
			{
				this.Stop();
			}
			return flag;
		}

		// Token: 0x04003F7D RID: 16253
		private float nextInterval = 2f;

		// Token: 0x04003F7E RID: 16254
		private float stopFleeDistance;
	}

	// Token: 0x02000B70 RID: 2928
	public class BaseFollowPathState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004CF6 RID: 19702 RVA: 0x0019FAB5 File Offset: 0x0019DCB5
		public BaseFollowPathState()
			: base(AIState.FollowPath)
		{
		}

		// Token: 0x06004CF7 RID: 19703 RVA: 0x0019FAC0 File Offset: 0x0019DCC0
		public override void StateEnter(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.status = StateStatus.Error;
			brain.Navigator.SetBrakingEnabled(false);
			this.path = brain.Navigator.Path;
			if (this.path == null)
			{
				AIInformationZone forPoint = AIInformationZone.GetForPoint(entity.ServerPosition, true);
				if (forPoint == null)
				{
					return;
				}
				this.path = forPoint.GetNearestPath(entity.ServerPosition);
				if (this.path == null)
				{
					return;
				}
			}
			this.currentNodeIndex = this.path.FindNearestPointIndex(entity.ServerPosition);
			this.currentTargetPoint = this.path.FindNearestPoint(entity.ServerPosition);
			if (this.currentTargetPoint == null)
			{
				return;
			}
			this.status = StateStatus.Running;
			this.currentWaitTime = 0f;
			brain.Navigator.SetDestination(this.currentTargetPoint.transform.position, BaseNavigator.NavigationSpeed.Slow, 0f, 0f);
		}

		// Token: 0x06004CF8 RID: 19704 RVA: 0x0019FBB5 File Offset: 0x0019DDB5
		public override void StateLeave(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			brain.Navigator.ClearFacingDirectionOverride();
			brain.Navigator.SetBrakingEnabled(true);
		}

		// Token: 0x06004CF9 RID: 19705 RVA: 0x0019FBD8 File Offset: 0x0019DDD8
		public override StateStatus StateThink(float delta, BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			if (this.status == StateStatus.Error)
			{
				return this.status;
			}
			if (!brain.Navigator.Moving)
			{
				if (this.currentWaitTime <= 0f && this.currentTargetPoint.HasLookAtPoints())
				{
					Transform randomLookAtPoint = this.currentTargetPoint.GetRandomLookAtPoint();
					if (randomLookAtPoint != null)
					{
						brain.Navigator.SetFacingDirectionOverride(Vector3Ex.Direction2D(randomLookAtPoint.transform.position, entity.ServerPosition));
					}
				}
				if (this.currentTargetPoint.WaitTime > 0f)
				{
					this.currentWaitTime += delta;
				}
				if (this.currentTargetPoint.WaitTime <= 0f || this.currentWaitTime >= this.currentTargetPoint.WaitTime)
				{
					brain.Navigator.ClearFacingDirectionOverride();
					this.currentWaitTime = 0f;
					int num = this.currentNodeIndex;
					this.currentNodeIndex = this.path.GetNextPointIndex(this.currentNodeIndex, ref this.pathDirection);
					this.currentTargetPoint = this.path.GetPointAtIndex(this.currentNodeIndex);
					if ((!(this.currentTargetPoint != null) || this.currentNodeIndex != num) && (this.currentTargetPoint == null || !brain.Navigator.SetDestination(this.currentTargetPoint.transform.position, BaseNavigator.NavigationSpeed.Slow, 0f, 0f)))
					{
						return StateStatus.Error;
					}
				}
			}
			else if (this.currentTargetPoint != null)
			{
				brain.Navigator.SetDestination(this.currentTargetPoint.transform.position, BaseNavigator.NavigationSpeed.Slow, 1f, 0f);
			}
			return StateStatus.Running;
		}

		// Token: 0x04003F7F RID: 16255
		private AIMovePointPath path;

		// Token: 0x04003F80 RID: 16256
		private StateStatus status;

		// Token: 0x04003F81 RID: 16257
		private AIMovePoint currentTargetPoint;

		// Token: 0x04003F82 RID: 16258
		private float currentWaitTime;

		// Token: 0x04003F83 RID: 16259
		private AIMovePointPath.PathDirection pathDirection;

		// Token: 0x04003F84 RID: 16260
		private int currentNodeIndex;
	}

	// Token: 0x02000B71 RID: 2929
	public class BaseIdleState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004CFA RID: 19706 RVA: 0x0019FD7D File Offset: 0x0019DF7D
		public BaseIdleState()
			: base(AIState.Idle)
		{
		}
	}

	// Token: 0x02000B72 RID: 2930
	public class BaseMountedState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004CFB RID: 19707 RVA: 0x0019FD86 File Offset: 0x0019DF86
		public BaseMountedState()
			: base(AIState.Mounted)
		{
		}

		// Token: 0x06004CFC RID: 19708 RVA: 0x0019FD8F File Offset: 0x0019DF8F
		public override void StateEnter(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			brain.Navigator.Stop();
		}
	}

	// Token: 0x02000B73 RID: 2931
	public class BaseMoveTorwardsState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004CFD RID: 19709 RVA: 0x0019FDA4 File Offset: 0x0019DFA4
		public BaseMoveTorwardsState()
			: base(AIState.MoveTowards)
		{
		}

		// Token: 0x06004CFE RID: 19710 RVA: 0x0019FDAE File Offset: 0x0019DFAE
		public override void StateLeave(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004CFF RID: 19711 RVA: 0x0019F7F1 File Offset: 0x0019D9F1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004D00 RID: 19712 RVA: 0x0019FDC0 File Offset: 0x0019DFC0
		public override StateStatus StateThink(float delta, BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			global::BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				this.Stop();
				return StateStatus.Error;
			}
			this.FaceTarget();
			if (!brain.Navigator.SetDestination(baseEntity.transform.position, brain.Navigator.MoveTowardsSpeed, 0.25f, 0f))
			{
				return StateStatus.Error;
			}
			if (!brain.Navigator.Moving)
			{
				return StateStatus.Finished;
			}
			return StateStatus.Running;
		}

		// Token: 0x06004D01 RID: 19713 RVA: 0x0019FE50 File Offset: 0x0019E050
		private void FaceTarget()
		{
			if (!this.brain.Navigator.FaceMoveTowardsTarget)
			{
				return;
			}
			global::BaseEntity baseEntity = this.brain.Events.Memory.Entity.Get(this.brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				this.brain.Navigator.ClearFacingDirectionOverride();
				return;
			}
			if (Vector3.Distance(baseEntity.transform.position, this.brain.transform.position) <= 1.5f)
			{
				this.brain.Navigator.SetFacingDirectionEntity(baseEntity);
			}
		}
	}

	// Token: 0x02000B74 RID: 2932
	public class BaseNavigateHomeState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004D02 RID: 19714 RVA: 0x0019FEED File Offset: 0x0019E0ED
		public BaseNavigateHomeState()
			: base(AIState.NavigateHome)
		{
		}

		// Token: 0x06004D03 RID: 19715 RVA: 0x0019FEF8 File Offset: 0x0019E0F8
		public override void StateEnter(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			Vector3 vector = brain.Events.Memory.Position.Get(4);
			this.status = StateStatus.Running;
			if (!brain.Navigator.SetDestination(vector, BaseNavigator.NavigationSpeed.Normal, 0f, 0f))
			{
				this.status = StateStatus.Error;
			}
		}

		// Token: 0x06004D04 RID: 19716 RVA: 0x0019FF4B File Offset: 0x0019E14B
		public override void StateLeave(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004D05 RID: 19717 RVA: 0x0019F7F1 File Offset: 0x0019D9F1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004D06 RID: 19718 RVA: 0x0019FF5B File Offset: 0x0019E15B
		public override StateStatus StateThink(float delta, BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			if (this.status == StateStatus.Error)
			{
				return this.status;
			}
			if (!brain.Navigator.Moving)
			{
				return StateStatus.Finished;
			}
			return StateStatus.Running;
		}

		// Token: 0x04003F85 RID: 16261
		private StateStatus status;
	}

	// Token: 0x02000B75 RID: 2933
	public class BasePatrolState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004D07 RID: 19719 RVA: 0x0019FF87 File Offset: 0x0019E187
		public BasePatrolState()
			: base(AIState.Patrol)
		{
		}
	}

	// Token: 0x02000B76 RID: 2934
	public class BaseRoamState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004D08 RID: 19720 RVA: 0x0019FF90 File Offset: 0x0019E190
		public BaseRoamState()
			: base(AIState.Roam)
		{
		}

		// Token: 0x06004D09 RID: 19721 RVA: 0x00029EBC File Offset: 0x000280BC
		public override float GetWeight()
		{
			return 0f;
		}

		// Token: 0x06004D0A RID: 19722 RVA: 0x0019FFA4 File Offset: 0x0019E1A4
		public override void StateEnter(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.nextRoamPositionTime = -1f;
			this.lastDestinationTime = UnityEngine.Time.time;
		}

		// Token: 0x06004D0B RID: 19723 RVA: 0x0002C05D File Offset: 0x0002A25D
		public virtual Vector3 GetDestination()
		{
			return Vector3.zero;
		}

		// Token: 0x06004D0C RID: 19724 RVA: 0x0019FFC4 File Offset: 0x0019E1C4
		public virtual Vector3 GetForwardDirection()
		{
			return Vector3.forward;
		}

		// Token: 0x06004D0D RID: 19725 RVA: 0x000063A5 File Offset: 0x000045A5
		public virtual void SetDestination(Vector3 destination)
		{
		}

		// Token: 0x06004D0E RID: 19726 RVA: 0x0019FFCB File Offset: 0x0019E1CB
		public override void DrawGizmos()
		{
			base.DrawGizmos();
			this.brain.PathFinder.DebugDraw();
		}

		// Token: 0x06004D0F RID: 19727 RVA: 0x0019FFE4 File Offset: 0x0019E1E4
		public virtual Vector3 GetRoamAnchorPosition()
		{
			if (this.brain.Navigator.MaxRoamDistanceFromHome > -1f)
			{
				return this.brain.Events.Memory.Position.Get(4);
			}
			return this.brain.GetBaseEntity().transform.position;
		}

		// Token: 0x06004D10 RID: 19728 RVA: 0x001A003C File Offset: 0x0019E23C
		public override StateStatus StateThink(float delta, BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			bool flag = UnityEngine.Time.time - this.lastDestinationTime > 25f;
			if ((Vector3.Distance(this.GetDestination(), entity.transform.position) < 2f || flag) && this.nextRoamPositionTime == -1f)
			{
				this.nextRoamPositionTime = UnityEngine.Time.time + UnityEngine.Random.Range(5f, 10f);
			}
			if (this.nextRoamPositionTime != -1f && UnityEngine.Time.time > this.nextRoamPositionTime)
			{
				AIMovePoint bestRoamPoint = brain.PathFinder.GetBestRoamPoint(this.GetRoamAnchorPosition(), entity.ServerPosition, this.GetForwardDirection(), brain.Navigator.MaxRoamDistanceFromHome, brain.Navigator.BestRoamPointMaxDistance);
				if (bestRoamPoint)
				{
					float num = Vector3.Distance(bestRoamPoint.transform.position, entity.transform.position) / 1.5f;
					bestRoamPoint.SetUsedBy(entity, num + 11f);
				}
				this.lastDestinationTime = UnityEngine.Time.time;
				Vector3 insideUnitSphere = UnityEngine.Random.insideUnitSphere;
				insideUnitSphere.y = 0f;
				insideUnitSphere.Normalize();
				Vector3 vector = ((bestRoamPoint == null) ? entity.transform.position : (bestRoamPoint.transform.position + insideUnitSphere * bestRoamPoint.radius));
				this.SetDestination(vector);
				this.nextRoamPositionTime = -1f;
			}
			return StateStatus.Running;
		}

		// Token: 0x04003F86 RID: 16262
		private float nextRoamPositionTime = -1f;

		// Token: 0x04003F87 RID: 16263
		private float lastDestinationTime;
	}

	// Token: 0x02000B77 RID: 2935
	public class BaseSleepState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004D11 RID: 19729 RVA: 0x001A01A8 File Offset: 0x0019E3A8
		public BaseSleepState()
			: base(AIState.Sleep)
		{
		}

		// Token: 0x06004D12 RID: 19730 RVA: 0x001A01BC File Offset: 0x0019E3BC
		public override void StateEnter(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.status = StateStatus.Error;
			IAISleep iaisleep;
			if ((iaisleep = entity as IAISleep) == null)
			{
				return;
			}
			iaisleep.StartSleeping();
			this.status = StateStatus.Running;
		}

		// Token: 0x06004D13 RID: 19731 RVA: 0x001A01F0 File Offset: 0x0019E3F0
		public override void StateLeave(BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			IAISleep iaisleep;
			if ((iaisleep = entity as IAISleep) == null)
			{
				return;
			}
			iaisleep.StopSleeping();
		}

		// Token: 0x06004D14 RID: 19732 RVA: 0x001A0216 File Offset: 0x0019E416
		public override StateStatus StateThink(float delta, BaseAIBrain brain, global::BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			return this.status;
		}

		// Token: 0x04003F88 RID: 16264
		private StateStatus status = StateStatus.Error;
	}

	// Token: 0x02000B78 RID: 2936
	public class BasicAIState
	{
		// Token: 0x1700066D RID: 1645
		// (get) Token: 0x06004D15 RID: 19733 RVA: 0x001A0228 File Offset: 0x0019E428
		// (set) Token: 0x06004D16 RID: 19734 RVA: 0x001A0230 File Offset: 0x0019E430
		public AIState StateType { get; private set; }

		// Token: 0x06004D17 RID: 19735 RVA: 0x001A0239 File Offset: 0x0019E439
		public virtual void StateEnter(BaseAIBrain brain, global::BaseEntity entity)
		{
			this.TimeInState = 0f;
		}

		// Token: 0x06004D18 RID: 19736 RVA: 0x001A0246 File Offset: 0x0019E446
		public virtual StateStatus StateThink(float delta, BaseAIBrain brain, global::BaseEntity entity)
		{
			this.TimeInState += delta;
			return StateStatus.Running;
		}

		// Token: 0x06004D19 RID: 19737 RVA: 0x001A0257 File Offset: 0x0019E457
		public virtual void StateLeave(BaseAIBrain brain, global::BaseEntity entity)
		{
			this.TimeInState = 0f;
			this._lastStateExitTime = UnityEngine.Time.time;
		}

		// Token: 0x06004D1A RID: 19738 RVA: 0x0000441C File Offset: 0x0000261C
		public virtual bool CanInterrupt()
		{
			return true;
		}

		// Token: 0x06004D1B RID: 19739 RVA: 0x0000441C File Offset: 0x0000261C
		public virtual bool CanEnter()
		{
			return true;
		}

		// Token: 0x06004D1C RID: 19740 RVA: 0x001A026F File Offset: 0x0019E46F
		public virtual bool CanLeave()
		{
			return this.CanInterrupt();
		}

		// Token: 0x06004D1D RID: 19741 RVA: 0x00029EBC File Offset: 0x000280BC
		public virtual float GetWeight()
		{
			return 0f;
		}

		// Token: 0x1700066E RID: 1646
		// (get) Token: 0x06004D1E RID: 19742 RVA: 0x001A0277 File Offset: 0x0019E477
		// (set) Token: 0x06004D1F RID: 19743 RVA: 0x001A027F File Offset: 0x0019E47F
		public float TimeInState { get; private set; }

		// Token: 0x06004D20 RID: 19744 RVA: 0x001A0288 File Offset: 0x0019E488
		public float TimeSinceState()
		{
			return UnityEngine.Time.time - this._lastStateExitTime;
		}

		// Token: 0x1700066F RID: 1647
		// (get) Token: 0x06004D21 RID: 19745 RVA: 0x001A0296 File Offset: 0x0019E496
		// (set) Token: 0x06004D22 RID: 19746 RVA: 0x001A029E File Offset: 0x0019E49E
		public bool AgrresiveState { get; protected set; }

		// Token: 0x06004D23 RID: 19747 RVA: 0x001A02A7 File Offset: 0x0019E4A7
		public BasicAIState(AIState state)
		{
			this.StateType = state;
		}

		// Token: 0x06004D24 RID: 19748 RVA: 0x001A0239 File Offset: 0x0019E439
		public void Reset()
		{
			this.TimeInState = 0f;
		}

		// Token: 0x06004D25 RID: 19749 RVA: 0x001A02B6 File Offset: 0x0019E4B6
		public bool IsInState()
		{
			return this.brain != null && this.brain.CurrentState != null && this.brain.CurrentState == this;
		}

		// Token: 0x06004D26 RID: 19750 RVA: 0x000063A5 File Offset: 0x000045A5
		public virtual void DrawGizmos()
		{
		}

		// Token: 0x04003F8A RID: 16266
		public BaseAIBrain brain;

		// Token: 0x04003F8C RID: 16268
		protected float _lastStateExitTime;
	}
}
