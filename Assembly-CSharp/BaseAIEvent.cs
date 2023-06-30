using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000371 RID: 881
public class BaseAIEvent
{
	// Token: 0x17000291 RID: 657
	// (get) Token: 0x06001FEA RID: 8170 RVA: 0x000D6CED File Offset: 0x000D4EED
	// (set) Token: 0x06001FEB RID: 8171 RVA: 0x000D6CF5 File Offset: 0x000D4EF5
	public AIEventType EventType { get; private set; }

	// Token: 0x17000292 RID: 658
	// (get) Token: 0x06001FEC RID: 8172 RVA: 0x000D6CFE File Offset: 0x000D4EFE
	// (set) Token: 0x06001FED RID: 8173 RVA: 0x000D6D06 File Offset: 0x000D4F06
	public int TriggerStateContainerID { get; private set; } = -1;

	// Token: 0x17000293 RID: 659
	// (get) Token: 0x06001FEE RID: 8174 RVA: 0x000D6D0F File Offset: 0x000D4F0F
	// (set) Token: 0x06001FEF RID: 8175 RVA: 0x000D6D17 File Offset: 0x000D4F17
	public BaseAIEvent.ExecuteRate Rate { get; protected set; } = BaseAIEvent.ExecuteRate.Normal;

	// Token: 0x17000294 RID: 660
	// (get) Token: 0x06001FF0 RID: 8176 RVA: 0x000D6D20 File Offset: 0x000D4F20
	public float ExecutionRate
	{
		get
		{
			switch (this.Rate)
			{
			case BaseAIEvent.ExecuteRate.Slow:
				return 1f;
			case BaseAIEvent.ExecuteRate.Normal:
				return 0.5f;
			case BaseAIEvent.ExecuteRate.Fast:
				return 0.25f;
			case BaseAIEvent.ExecuteRate.VeryFast:
				return 0.1f;
			default:
				return 0.5f;
			}
		}
	}

	// Token: 0x17000295 RID: 661
	// (get) Token: 0x06001FF1 RID: 8177 RVA: 0x000D6D69 File Offset: 0x000D4F69
	// (set) Token: 0x06001FF2 RID: 8178 RVA: 0x000D6D71 File Offset: 0x000D4F71
	public bool ShouldExecute { get; protected set; }

	// Token: 0x17000296 RID: 662
	// (get) Token: 0x06001FF3 RID: 8179 RVA: 0x000D6D7A File Offset: 0x000D4F7A
	// (set) Token: 0x06001FF4 RID: 8180 RVA: 0x000D6D82 File Offset: 0x000D4F82
	public bool Result { get; protected set; }

	// Token: 0x17000297 RID: 663
	// (get) Token: 0x06001FF5 RID: 8181 RVA: 0x000D6D8B File Offset: 0x000D4F8B
	// (set) Token: 0x06001FF6 RID: 8182 RVA: 0x000D6D93 File Offset: 0x000D4F93
	public bool Inverted { get; private set; }

	// Token: 0x17000298 RID: 664
	// (get) Token: 0x06001FF7 RID: 8183 RVA: 0x000D6D9C File Offset: 0x000D4F9C
	// (set) Token: 0x06001FF8 RID: 8184 RVA: 0x000D6DA4 File Offset: 0x000D4FA4
	public int OutputEntityMemorySlot { get; protected set; } = -1;

	// Token: 0x17000299 RID: 665
	// (get) Token: 0x06001FF9 RID: 8185 RVA: 0x000D6DAD File Offset: 0x000D4FAD
	public bool ShouldSetOutputEntityMemory
	{
		get
		{
			return this.OutputEntityMemorySlot > -1;
		}
	}

	// Token: 0x1700029A RID: 666
	// (get) Token: 0x06001FFA RID: 8186 RVA: 0x000D6DB8 File Offset: 0x000D4FB8
	// (set) Token: 0x06001FFB RID: 8187 RVA: 0x000D6DC0 File Offset: 0x000D4FC0
	public int InputEntityMemorySlot { get; protected set; } = -1;

	// Token: 0x1700029B RID: 667
	// (get) Token: 0x06001FFC RID: 8188 RVA: 0x000D6DC9 File Offset: 0x000D4FC9
	// (set) Token: 0x06001FFD RID: 8189 RVA: 0x000D6DD1 File Offset: 0x000D4FD1
	public int ID { get; protected set; }

	// Token: 0x1700029C RID: 668
	// (get) Token: 0x06001FFE RID: 8190 RVA: 0x000D6DDA File Offset: 0x000D4FDA
	// (set) Token: 0x06001FFF RID: 8191 RVA: 0x000D6DE2 File Offset: 0x000D4FE2
	public global::BaseEntity Owner { get; private set; }

	// Token: 0x1700029D RID: 669
	// (get) Token: 0x06002000 RID: 8192 RVA: 0x000D6DEB File Offset: 0x000D4FEB
	public bool HasValidTriggerState
	{
		get
		{
			return this.TriggerStateContainerID != -1;
		}
	}

	// Token: 0x06002001 RID: 8193 RVA: 0x000D6DF9 File Offset: 0x000D4FF9
	public BaseAIEvent(AIEventType type)
	{
		this.EventType = type;
	}

	// Token: 0x06002002 RID: 8194 RVA: 0x000D6E24 File Offset: 0x000D5024
	public virtual void Init(AIEventData data, global::BaseEntity owner)
	{
		this.Init(data.triggerStateContainer, data.id, owner, data.inputMemorySlot, data.outputMemorySlot, data.inverted);
	}

	// Token: 0x06002003 RID: 8195 RVA: 0x000D6E4B File Offset: 0x000D504B
	public virtual void Init(int triggerStateContainer, int id, global::BaseEntity owner, int inputMemorySlot, int outputMemorySlot, bool inverted)
	{
		this.TriggerStateContainerID = triggerStateContainer;
		this.ID = id;
		this.Owner = owner;
		this.InputEntityMemorySlot = inputMemorySlot;
		this.OutputEntityMemorySlot = outputMemorySlot;
		this.Inverted = inverted;
	}

	// Token: 0x06002004 RID: 8196 RVA: 0x000D6E7C File Offset: 0x000D507C
	public virtual AIEventData ToProto()
	{
		return new AIEventData
		{
			id = this.ID,
			eventType = (int)this.EventType,
			triggerStateContainer = this.TriggerStateContainerID,
			outputMemorySlot = this.OutputEntityMemorySlot,
			inputMemorySlot = this.InputEntityMemorySlot,
			inverted = this.Inverted
		};
	}

	// Token: 0x06002005 RID: 8197 RVA: 0x000D6ED6 File Offset: 0x000D50D6
	public virtual void Reset()
	{
		this.executeTimer = 0f;
		this.deltaTime = 0f;
		this.Result = false;
	}

	// Token: 0x06002006 RID: 8198 RVA: 0x000D6EF8 File Offset: 0x000D50F8
	public void Tick(float deltaTime, IAIEventListener listener)
	{
		this.deltaTime += deltaTime;
		this.executeTimer += deltaTime;
		float executionRate = this.ExecutionRate;
		if (this.executeTimer >= executionRate)
		{
			this.executeTimer = 0f;
			this.ShouldExecute = true;
			return;
		}
		this.ShouldExecute = false;
	}

	// Token: 0x06002007 RID: 8199 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
	}

	// Token: 0x06002008 RID: 8200 RVA: 0x000D6F4B File Offset: 0x000D514B
	public virtual void PostExecute()
	{
		this.deltaTime = 0f;
	}

	// Token: 0x06002009 RID: 8201 RVA: 0x000D6F58 File Offset: 0x000D5158
	public void TriggerStateChange(IAIEventListener listener, int sourceEventID)
	{
		listener.EventTriggeredStateChange(this.TriggerStateContainerID, sourceEventID);
	}

	// Token: 0x0600200A RID: 8202 RVA: 0x000D6F68 File Offset: 0x000D5168
	public static BaseAIEvent CreateEvent(AIEventType eventType)
	{
		switch (eventType)
		{
		case AIEventType.Timer:
			return new TimerAIEvent();
		case AIEventType.PlayerDetected:
			return new PlayerDetectedAIEvent();
		case AIEventType.StateError:
			return new StateErrorAIEvent();
		case AIEventType.Attacked:
			return new AttackedAIEvent();
		case AIEventType.StateFinished:
			return new StateFinishedAIEvent();
		case AIEventType.InAttackRange:
			return new InAttackRangeAIEvent();
		case AIEventType.HealthBelow:
			return new HealthBelowAIEvent();
		case AIEventType.InRange:
			return new InRangeAIEvent();
		case AIEventType.PerformedAttack:
			return new PerformedAttackAIEvent();
		case AIEventType.TirednessAbove:
			return new TirednessAboveAIEvent();
		case AIEventType.HungerAbove:
			return new HungerAboveAIEvent();
		case AIEventType.ThreatDetected:
			return new ThreatDetectedAIEvent();
		case AIEventType.TargetDetected:
			return new TargetDetectedAIEvent();
		case AIEventType.AmmoBelow:
			return new AmmoBelowAIEvent();
		case AIEventType.BestTargetDetected:
			return new BestTargetDetectedAIEvent();
		case AIEventType.IsVisible:
			return new IsVisibleAIEvent();
		case AIEventType.AttackTick:
			return new AttackTickAIEvent();
		case AIEventType.IsMounted:
			return new IsMountedAIEvent();
		case AIEventType.And:
			return new AndAIEvent();
		case AIEventType.Chance:
			return new ChanceAIEvent();
		case AIEventType.TargetLost:
			return new TargetLostAIEvent();
		case AIEventType.TimeSinceThreat:
			return new TimeSinceThreatAIEvent();
		case AIEventType.OnPositionMemorySet:
			return new OnPositionMemorySetAIEvent();
		case AIEventType.AggressionTimer:
			return new AggressionTimerAIEvent();
		case AIEventType.Reloading:
			return new ReloadingAIEvent();
		case AIEventType.InRangeOfHome:
			return new InRangeOfHomeAIEvent();
		case AIEventType.IsBlinded:
			return new IsBlindedAIEvent();
		default:
			Debug.LogWarning("No case for " + eventType + " event in BaseAIEvent.CreateEvent()!");
			return null;
		}
	}

	// Token: 0x04001955 RID: 6485
	private float executeTimer;

	// Token: 0x04001956 RID: 6486
	protected float deltaTime;

	// Token: 0x02000CCC RID: 3276
	public enum ExecuteRate
	{
		// Token: 0x04004569 RID: 17769
		Slow,
		// Token: 0x0400456A RID: 17770
		Normal,
		// Token: 0x0400456B RID: 17771
		Fast,
		// Token: 0x0400456C RID: 17772
		VeryFast
	}
}
