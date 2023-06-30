using System;

// Token: 0x0200037D RID: 893
public class IsMountedAIEvent : BaseAIEvent
{
	// Token: 0x06002034 RID: 8244 RVA: 0x000D7674 File Offset: 0x000D5874
	public IsMountedAIEvent()
		: base(AIEventType.IsMounted)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06002035 RID: 8245 RVA: 0x000D7688 File Offset: 0x000D5888
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		IAIMounted iaimounted = memory.Entity.Get(base.InputEntityMemorySlot) as IAIMounted;
		base.Result = false;
		if (iaimounted == null)
		{
			return;
		}
		if (base.Inverted && !iaimounted.IsMounted())
		{
			base.Result = true;
		}
		if (!base.Inverted && iaimounted.IsMounted())
		{
			base.Result = true;
		}
		if (base.Result && base.ShouldSetOutputEntityMemory)
		{
			memory.Entity.Set(memory.Entity.Get(base.InputEntityMemorySlot), base.OutputEntityMemorySlot);
		}
	}
}
