using System;
using System.Collections.Generic;
using ProtoBuf;

// Token: 0x0200038E RID: 910
public class AIStateContainer
{
	// Token: 0x170002AB RID: 683
	// (get) Token: 0x06002073 RID: 8307 RVA: 0x000D801C File Offset: 0x000D621C
	// (set) Token: 0x06002074 RID: 8308 RVA: 0x000D8024 File Offset: 0x000D6224
	public int ID { get; private set; }

	// Token: 0x170002AC RID: 684
	// (get) Token: 0x06002075 RID: 8309 RVA: 0x000D802D File Offset: 0x000D622D
	// (set) Token: 0x06002076 RID: 8310 RVA: 0x000D8035 File Offset: 0x000D6235
	public AIState State { get; private set; }

	// Token: 0x170002AD RID: 685
	// (get) Token: 0x06002077 RID: 8311 RVA: 0x000D803E File Offset: 0x000D623E
	// (set) Token: 0x06002078 RID: 8312 RVA: 0x000D8046 File Offset: 0x000D6246
	public int InputMemorySlot { get; private set; } = -1;

	// Token: 0x06002079 RID: 8313 RVA: 0x000D8050 File Offset: 0x000D6250
	public void Init(ProtoBuf.AIStateContainer container, global::BaseEntity owner)
	{
		this.ID = container.id;
		this.State = (AIState)container.state;
		this.InputMemorySlot = container.inputMemorySlot;
		this.Events = new List<BaseAIEvent>();
		if (container.events == null)
		{
			return;
		}
		foreach (AIEventData aieventData in container.events)
		{
			BaseAIEvent baseAIEvent = BaseAIEvent.CreateEvent((AIEventType)aieventData.eventType);
			baseAIEvent.Init(aieventData, owner);
			baseAIEvent.Reset();
			this.Events.Add(baseAIEvent);
		}
	}

	// Token: 0x0600207A RID: 8314 RVA: 0x000D80FC File Offset: 0x000D62FC
	public ProtoBuf.AIStateContainer ToProto()
	{
		ProtoBuf.AIStateContainer aistateContainer = new ProtoBuf.AIStateContainer();
		aistateContainer.id = this.ID;
		aistateContainer.state = (int)this.State;
		aistateContainer.events = new List<AIEventData>();
		aistateContainer.inputMemorySlot = this.InputMemorySlot;
		foreach (BaseAIEvent baseAIEvent in this.Events)
		{
			aistateContainer.events.Add(baseAIEvent.ToProto());
		}
		return aistateContainer;
	}

	// Token: 0x0400196B RID: 6507
	public List<BaseAIEvent> Events;
}
