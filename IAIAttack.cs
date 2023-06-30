using System;

// Token: 0x02000397 RID: 919
public interface IAIAttack
{
	// Token: 0x06002090 RID: 8336
	void AttackTick(float delta, BaseEntity target, bool targetIsLOS);

	// Token: 0x06002091 RID: 8337
	BaseEntity GetBestTarget();

	// Token: 0x06002092 RID: 8338
	bool CanAttack(BaseEntity entity);

	// Token: 0x06002093 RID: 8339
	float EngagementRange();

	// Token: 0x06002094 RID: 8340
	bool IsTargetInRange(BaseEntity entity, out float dist);

	// Token: 0x06002095 RID: 8341
	bool CanSeeTarget(BaseEntity entity);

	// Token: 0x06002096 RID: 8342
	float GetAmmoFraction();

	// Token: 0x06002097 RID: 8343
	bool NeedsToReload();

	// Token: 0x06002098 RID: 8344
	bool Reload();

	// Token: 0x06002099 RID: 8345
	float CooldownDuration();

	// Token: 0x0600209A RID: 8346
	bool IsOnCooldown();

	// Token: 0x0600209B RID: 8347
	bool StartAttacking(BaseEntity entity);

	// Token: 0x0600209C RID: 8348
	void StopAttacking();
}
