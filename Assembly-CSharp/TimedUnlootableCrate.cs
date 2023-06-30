using System;

// Token: 0x02000425 RID: 1061
public class TimedUnlootableCrate : LootContainer
{
	// Token: 0x06002415 RID: 9237 RVA: 0x000E69C9 File Offset: 0x000E4BC9
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.unlootableOnSpawn)
		{
			this.SetUnlootableFor(this.unlootableDuration);
		}
	}

	// Token: 0x06002416 RID: 9238 RVA: 0x000E69E5 File Offset: 0x000E4BE5
	public void SetUnlootableFor(float duration)
	{
		base.SetFlag(BaseEntity.Flags.OnFire, true, false, true);
		base.SetFlag(BaseEntity.Flags.Locked, true, false, true);
		this.unlootableDuration = duration;
		base.Invoke(new Action(this.MakeLootable), duration);
	}

	// Token: 0x06002417 RID: 9239 RVA: 0x000E6A16 File Offset: 0x000E4C16
	public void MakeLootable()
	{
		base.SetFlag(BaseEntity.Flags.OnFire, false, false, true);
		base.SetFlag(BaseEntity.Flags.Locked, false, false, true);
	}

	// Token: 0x04001C1B RID: 7195
	public bool unlootableOnSpawn = true;

	// Token: 0x04001C1C RID: 7196
	public float unlootableDuration = 300f;
}
