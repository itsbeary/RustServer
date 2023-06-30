using System;

// Token: 0x0200049F RID: 1183
public interface IVehicleLockUser
{
	// Token: 0x060026EA RID: 9962
	bool PlayerCanDestroyLock(BasePlayer player, BaseVehicleModule viaModule);

	// Token: 0x060026EB RID: 9963
	bool PlayerHasUnlockPermission(BasePlayer player);

	// Token: 0x060026EC RID: 9964
	bool PlayerCanUseThis(BasePlayer player, ModularCarCodeLock.LockType lockType);

	// Token: 0x060026ED RID: 9965
	void RemoveLock();
}
