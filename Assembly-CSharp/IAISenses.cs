using System;

// Token: 0x0200038B RID: 907
internal interface IAISenses
{
	// Token: 0x06002069 RID: 8297
	bool IsThreat(BaseEntity entity);

	// Token: 0x0600206A RID: 8298
	bool IsTarget(BaseEntity entity);

	// Token: 0x0600206B RID: 8299
	bool IsFriendly(BaseEntity entity);
}
