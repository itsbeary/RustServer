using System;

// Token: 0x020003CA RID: 970
public class EntityFlag_TOD : EntityComponent<BaseEntity>
{
	// Token: 0x060021D5 RID: 8661 RVA: 0x000DC5D7 File Offset: 0x000DA7D7
	public void Start()
	{
		base.Invoke(new Action(this.Initialize), 1f);
	}

	// Token: 0x060021D6 RID: 8662 RVA: 0x000DC5F0 File Offset: 0x000DA7F0
	public void Initialize()
	{
		if (base.baseEntity == null || base.baseEntity.isClient)
		{
			return;
		}
		base.InvokeRandomized(new Action(this.DoTimeCheck), 0f, 5f, 1f);
	}

	// Token: 0x060021D7 RID: 8663 RVA: 0x000DC630 File Offset: 0x000DA830
	public bool WantsOn()
	{
		if (TOD_Sky.Instance == null)
		{
			return false;
		}
		bool isNight = TOD_Sky.Instance.IsNight;
		return this.onAtNight == isNight;
	}

	// Token: 0x060021D8 RID: 8664 RVA: 0x000DC664 File Offset: 0x000DA864
	private void DoTimeCheck()
	{
		bool flag = base.baseEntity.HasFlag(this.desiredFlag);
		bool flag2 = this.WantsOn();
		if (flag != flag2)
		{
			base.baseEntity.SetFlag(this.desiredFlag, flag2, false, true);
		}
	}

	// Token: 0x04001A39 RID: 6713
	public BaseEntity.Flags desiredFlag;

	// Token: 0x04001A3A RID: 6714
	public bool onAtNight = true;
}
