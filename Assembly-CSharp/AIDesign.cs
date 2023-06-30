using System;
using System.Collections.Generic;
using ProtoBuf;

// Token: 0x02000363 RID: 867
public class AIDesign
{
	// Token: 0x1700028D RID: 653
	// (get) Token: 0x06001FBB RID: 8123 RVA: 0x000D623D File Offset: 0x000D443D
	// (set) Token: 0x06001FBC RID: 8124 RVA: 0x000D6245 File Offset: 0x000D4445
	public AIDesignScope Scope { get; private set; }

	// Token: 0x1700028E RID: 654
	// (get) Token: 0x06001FBD RID: 8125 RVA: 0x000D624E File Offset: 0x000D444E
	// (set) Token: 0x06001FBE RID: 8126 RVA: 0x000D6256 File Offset: 0x000D4456
	public string Description { get; private set; }

	// Token: 0x06001FBF RID: 8127 RVA: 0x000D625F File Offset: 0x000D445F
	public void SetAvailableStates(List<AIState> states)
	{
		this.AvailableStates = new List<AIState>();
		this.AvailableStates.AddRange(states);
	}

	// Token: 0x06001FC0 RID: 8128 RVA: 0x000D6278 File Offset: 0x000D4478
	public void Load(ProtoBuf.AIDesign design, global::BaseEntity owner)
	{
		this.Scope = (AIDesignScope)design.scope;
		this.DefaultStateContainerID = design.defaultStateContainer;
		this.Description = design.description;
		this.InitStateContainers(design, owner);
	}

	// Token: 0x06001FC1 RID: 8129 RVA: 0x000D62A8 File Offset: 0x000D44A8
	private void InitStateContainers(ProtoBuf.AIDesign design, global::BaseEntity owner)
	{
		this.stateContainers = new Dictionary<int, global::AIStateContainer>();
		if (design.stateContainers == null)
		{
			return;
		}
		foreach (ProtoBuf.AIStateContainer aistateContainer in design.stateContainers)
		{
			global::AIStateContainer aistateContainer2 = new global::AIStateContainer();
			aistateContainer2.Init(aistateContainer, owner);
			this.stateContainers.Add(aistateContainer2.ID, aistateContainer2);
		}
	}

	// Token: 0x06001FC2 RID: 8130 RVA: 0x000D6328 File Offset: 0x000D4528
	public global::AIStateContainer GetDefaultStateContainer()
	{
		return this.GetStateContainerByID(this.DefaultStateContainerID);
	}

	// Token: 0x06001FC3 RID: 8131 RVA: 0x000D6336 File Offset: 0x000D4536
	public global::AIStateContainer GetStateContainerByID(int id)
	{
		if (!this.stateContainers.ContainsKey(id))
		{
			return null;
		}
		return this.stateContainers[id];
	}

	// Token: 0x06001FC4 RID: 8132 RVA: 0x000D6354 File Offset: 0x000D4554
	public global::AIStateContainer GetFirstStateContainerOfType(AIState stateType)
	{
		foreach (global::AIStateContainer aistateContainer in this.stateContainers.Values)
		{
			if (aistateContainer.State == stateType)
			{
				return aistateContainer;
			}
		}
		return null;
	}

	// Token: 0x06001FC5 RID: 8133 RVA: 0x000D63B8 File Offset: 0x000D45B8
	public ProtoBuf.AIDesign ToProto(int currentStateID)
	{
		ProtoBuf.AIDesign aidesign = new ProtoBuf.AIDesign();
		aidesign.description = this.Description;
		aidesign.scope = (int)this.Scope;
		aidesign.defaultStateContainer = this.DefaultStateContainerID;
		aidesign.availableStates = new List<int>();
		foreach (AIState aistate in this.AvailableStates)
		{
			aidesign.availableStates.Add((int)aistate);
		}
		aidesign.stateContainers = new List<ProtoBuf.AIStateContainer>();
		foreach (global::AIStateContainer aistateContainer in this.stateContainers.Values)
		{
			aidesign.stateContainers.Add(aistateContainer.ToProto());
		}
		aidesign.intialViewStateID = currentStateID;
		return aidesign;
	}

	// Token: 0x0400190A RID: 6410
	public List<AIState> AvailableStates = new List<AIState>();

	// Token: 0x0400190B RID: 6411
	public int DefaultStateContainerID;

	// Token: 0x0400190C RID: 6412
	private Dictionary<int, global::AIStateContainer> stateContainers = new Dictionary<int, global::AIStateContainer>();
}
