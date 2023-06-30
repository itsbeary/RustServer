using System;

// Token: 0x02000576 RID: 1398
public interface ISpawnGroup
{
	// Token: 0x06002AF0 RID: 10992
	void Clear();

	// Token: 0x06002AF1 RID: 10993
	void Fill();

	// Token: 0x06002AF2 RID: 10994
	void SpawnInitial();

	// Token: 0x06002AF3 RID: 10995
	void SpawnRepeating();

	// Token: 0x17000393 RID: 915
	// (get) Token: 0x06002AF4 RID: 10996
	int currentPopulation { get; }
}
