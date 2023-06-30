using System;

// Token: 0x02000381 RID: 897
public class ReloadingAIEvent : BaseAIEvent
{
	// Token: 0x06002041 RID: 8257 RVA: 0x000D79D1 File Offset: 0x000D5BD1
	public ReloadingAIEvent()
		: base(AIEventType.Reloading)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06002042 RID: 8258 RVA: 0x000D79E4 File Offset: 0x000D5BE4
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		BaseEntity baseEntity = memory.Entity.Get(base.InputEntityMemorySlot);
		base.Result = false;
		NPCPlayer npcplayer = baseEntity as NPCPlayer;
		if (npcplayer == null)
		{
			return;
		}
		bool flag = npcplayer.IsReloading();
		base.Result = (base.Inverted ? (!flag) : flag);
	}
}
