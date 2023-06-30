using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000366 RID: 870
public class AIEvents
{
	// Token: 0x1700028F RID: 655
	// (get) Token: 0x06001FCB RID: 8139 RVA: 0x000D65C0 File Offset: 0x000D47C0
	// (set) Token: 0x06001FCC RID: 8140 RVA: 0x000D65C8 File Offset: 0x000D47C8
	public int CurrentInputMemorySlot { get; private set; } = -1;

	// Token: 0x06001FCD RID: 8141 RVA: 0x000D65D4 File Offset: 0x000D47D4
	public void Init(IAIEventListener listener, AIStateContainer stateContainer, BaseEntity owner, AIBrainSenses senses)
	{
		this.CurrentInputMemorySlot = stateContainer.InputMemorySlot;
		this.eventListener = listener;
		this.RemoveAll();
		this.AddStateEvents(stateContainer.Events, owner);
		this.Memory.Entity.Set(owner, 4);
		this.senses = senses;
	}

	// Token: 0x06001FCE RID: 8142 RVA: 0x000D6621 File Offset: 0x000D4821
	private void RemoveAll()
	{
		this.events.Clear();
	}

	// Token: 0x06001FCF RID: 8143 RVA: 0x000D6630 File Offset: 0x000D4830
	private void AddStateEvents(List<BaseAIEvent> events, BaseEntity owner)
	{
		foreach (BaseAIEvent baseAIEvent in events)
		{
			this.Add(baseAIEvent);
		}
	}

	// Token: 0x06001FD0 RID: 8144 RVA: 0x000D6680 File Offset: 0x000D4880
	private void Add(BaseAIEvent aiEvent)
	{
		if (this.events.Contains(aiEvent))
		{
			Debug.LogWarning("Attempting to add duplicate AI event: " + aiEvent.EventType);
			return;
		}
		aiEvent.Reset();
		this.events.Add(aiEvent);
	}

	// Token: 0x06001FD1 RID: 8145 RVA: 0x000D66C0 File Offset: 0x000D48C0
	public void Tick(float deltaTime, StateStatus stateStatus)
	{
		foreach (BaseAIEvent baseAIEvent in this.events)
		{
			baseAIEvent.Tick(deltaTime, this.eventListener);
		}
		this.inBlock = false;
		this.currentEventIndex = 0;
		this.currentEventIndex = 0;
		while (this.currentEventIndex < this.events.Count)
		{
			BaseAIEvent baseAIEvent2 = this.events[this.currentEventIndex];
			BaseAIEvent baseAIEvent3 = ((this.currentEventIndex < this.events.Count - 1) ? this.events[this.currentEventIndex + 1] : null);
			if (baseAIEvent3 != null && baseAIEvent3.EventType == AIEventType.And && !this.inBlock)
			{
				this.inBlock = true;
			}
			if (baseAIEvent2.EventType != AIEventType.And)
			{
				if (baseAIEvent2.ShouldExecute)
				{
					baseAIEvent2.Execute(this.Memory, this.senses, stateStatus);
					baseAIEvent2.PostExecute();
				}
				bool result = baseAIEvent2.Result;
				if (this.inBlock)
				{
					if (result)
					{
						if ((baseAIEvent3 != null && baseAIEvent3.EventType != AIEventType.And) || baseAIEvent3 == null)
						{
							this.inBlock = false;
							if (baseAIEvent2.HasValidTriggerState)
							{
								baseAIEvent2.TriggerStateChange(this.eventListener, baseAIEvent2.ID);
								return;
							}
						}
					}
					else
					{
						this.inBlock = false;
						this.currentEventIndex = this.FindNextEventBlock() - 1;
					}
				}
				else if (result && baseAIEvent2.HasValidTriggerState)
				{
					baseAIEvent2.TriggerStateChange(this.eventListener, baseAIEvent2.ID);
					return;
				}
			}
			this.currentEventIndex++;
		}
	}

	// Token: 0x06001FD2 RID: 8146 RVA: 0x000D6858 File Offset: 0x000D4A58
	private int FindNextEventBlock()
	{
		for (int i = this.currentEventIndex; i < this.events.Count; i++)
		{
			BaseAIEvent baseAIEvent = this.events[i];
			BaseAIEvent baseAIEvent2 = ((i < this.events.Count - 1) ? this.events[i + 1] : null);
			if (baseAIEvent2 != null && baseAIEvent2.EventType != AIEventType.And && baseAIEvent.EventType != AIEventType.And)
			{
				return i + 1;
			}
		}
		return this.events.Count + 1;
	}

	// Token: 0x0400192B RID: 6443
	public AIMemory Memory = new AIMemory();

	// Token: 0x0400192D RID: 6445
	private List<BaseAIEvent> events = new List<BaseAIEvent>();

	// Token: 0x0400192E RID: 6446
	private IAIEventListener eventListener;

	// Token: 0x0400192F RID: 6447
	private AIBrainSenses senses;

	// Token: 0x04001930 RID: 6448
	private int currentEventIndex;

	// Token: 0x04001931 RID: 6449
	private bool inBlock;
}
