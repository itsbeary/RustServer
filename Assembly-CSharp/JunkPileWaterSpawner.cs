using System;

// Token: 0x0200016C RID: 364
public class JunkPileWaterSpawner : SpawnGroup
{
	// Token: 0x06001787 RID: 6023 RVA: 0x000B284A File Offset: 0x000B0A4A
	protected override void PostSpawnProcess(BaseEntity entity, BaseSpawnPoint spawnPoint)
	{
		base.PostSpawnProcess(entity, spawnPoint);
		if (this.attachToParent != null)
		{
			entity.SetParent(this.attachToParent, true, false);
		}
	}

	// Token: 0x0400102B RID: 4139
	public BaseEntity attachToParent;
}
