using System;
using UnityEngine;

// Token: 0x02000385 RID: 901
public class TargetLostAIEvent : BaseAIEvent
{
	// Token: 0x170002A5 RID: 677
	// (get) Token: 0x0600204D RID: 8269 RVA: 0x000D7BA6 File Offset: 0x000D5DA6
	// (set) Token: 0x0600204E RID: 8270 RVA: 0x000D7BAE File Offset: 0x000D5DAE
	public float Range { get; set; }

	// Token: 0x0600204F RID: 8271 RVA: 0x000D7BB7 File Offset: 0x000D5DB7
	public TargetLostAIEvent()
		: base(AIEventType.TargetLost)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06002050 RID: 8272 RVA: 0x000D7BC8 File Offset: 0x000D5DC8
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		BaseEntity baseEntity = memory.Entity.Get(base.InputEntityMemorySlot);
		if (baseEntity == null)
		{
			base.Result = !base.Inverted;
			return;
		}
		if (Vector3.Distance(baseEntity.transform.position, base.Owner.transform.position) > senses.TargetLostRange)
		{
			base.Result = !base.Inverted;
			return;
		}
		BasePlayer basePlayer = baseEntity as BasePlayer;
		if (baseEntity.Health() <= 0f || (basePlayer != null && basePlayer.IsDead()))
		{
			base.Result = !base.Inverted;
			return;
		}
		if (senses.ignoreSafeZonePlayers && basePlayer != null && basePlayer.InSafeZone())
		{
			base.Result = !base.Inverted;
			return;
		}
	}
}
