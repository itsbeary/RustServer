using System;

// Token: 0x020004CD RID: 1229
public class DummySwitch : IOEntity
{
	// Token: 0x0600282B RID: 10283 RVA: 0x00007649 File Offset: 0x00005849
	public override bool WantsPower()
	{
		return base.IsOn();
	}

	// Token: 0x0600282C RID: 10284 RVA: 0x00062B09 File Offset: 0x00060D09
	public override void ResetIOState()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x0600282D RID: 10285 RVA: 0x00062B15 File Offset: 0x00060D15
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.GetCurrentEnergy();
	}

	// Token: 0x0600282E RID: 10286 RVA: 0x000FA15D File Offset: 0x000F835D
	public void SetOn(bool wantsOn)
	{
		base.SetFlag(BaseEntity.Flags.On, wantsOn, false, true);
		this.MarkDirty();
		if (base.IsOn() && this.duration != -1f)
		{
			base.Invoke(new Action(this.SetOff), this.duration);
		}
	}

	// Token: 0x0600282F RID: 10287 RVA: 0x000FA19C File Offset: 0x000F839C
	public void SetOff()
	{
		this.SetOn(false);
	}

	// Token: 0x06002830 RID: 10288 RVA: 0x000FA1A8 File Offset: 0x000F83A8
	public override void OnEntityMessage(BaseEntity from, string msg)
	{
		if (msg == this.listenString)
		{
			if (base.IsOn())
			{
				this.SetOn(false);
			}
			this.SetOn(true);
			return;
		}
		if (msg == this.listenStringOff && this.listenStringOff != "" && base.IsOn())
		{
			this.SetOn(false);
		}
	}

	// Token: 0x040020A4 RID: 8356
	public string listenString = "";

	// Token: 0x040020A5 RID: 8357
	public string listenStringOff = "";

	// Token: 0x040020A6 RID: 8358
	public float duration = -1f;
}
