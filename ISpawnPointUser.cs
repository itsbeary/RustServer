using System;

// Token: 0x02000577 RID: 1399
public interface ISpawnPointUser
{
	// Token: 0x06002AF5 RID: 10997
	void ObjectSpawned(SpawnPointInstance instance);

	// Token: 0x06002AF6 RID: 10998
	void ObjectRetired(SpawnPointInstance instance);
}
