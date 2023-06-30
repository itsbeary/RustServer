using System;

// Token: 0x02000574 RID: 1396
public class GameModeSpawnGroup : SpawnGroup
{
	// Token: 0x06002AE7 RID: 10983 RVA: 0x0010594D File Offset: 0x00103B4D
	public void ResetSpawnGroup()
	{
		base.Clear();
		this.SpawnInitial();
	}

	// Token: 0x06002AE8 RID: 10984 RVA: 0x0010595C File Offset: 0x00103B5C
	public bool ShouldSpawn()
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		return !(activeGameMode == null) && (this.gameModeTags.Length == 0 || activeGameMode.HasAnyGameModeTag(this.gameModeTags));
	}

	// Token: 0x06002AE9 RID: 10985 RVA: 0x00105997 File Offset: 0x00103B97
	protected override void Spawn(int numToSpawn)
	{
		if (this.ShouldSpawn())
		{
			base.Spawn(numToSpawn);
		}
	}

	// Token: 0x04002310 RID: 8976
	public string[] gameModeTags;
}
