using System;
using ConVar;

// Token: 0x020003E5 RID: 997
public class DebrisEntity : BaseCombatEntity
{
	// Token: 0x0600225D RID: 8797 RVA: 0x000DE6EA File Offset: 0x000DC8EA
	public override void ServerInit()
	{
		this.ResetRemovalTime();
		base.ServerInit();
	}

	// Token: 0x0600225E RID: 8798 RVA: 0x00003384 File Offset: 0x00001584
	public void RemoveCorpse()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x0600225F RID: 8799 RVA: 0x000DE6F8 File Offset: 0x000DC8F8
	public void ResetRemovalTime(float dur)
	{
		using (TimeWarning.New("ResetRemovalTime", 0))
		{
			if (base.IsInvoking(new Action(this.RemoveCorpse)))
			{
				base.CancelInvoke(new Action(this.RemoveCorpse));
			}
			base.Invoke(new Action(this.RemoveCorpse), dur);
		}
	}

	// Token: 0x06002260 RID: 8800 RVA: 0x000DE768 File Offset: 0x000DC968
	public float GetRemovalTime()
	{
		if (this.DebrisDespawnOverride <= 0f)
		{
			return Server.debrisdespawn;
		}
		return this.DebrisDespawnOverride;
	}

	// Token: 0x06002261 RID: 8801 RVA: 0x000DE783 File Offset: 0x000DC983
	public void ResetRemovalTime()
	{
		this.ResetRemovalTime(this.GetRemovalTime());
	}

	// Token: 0x06002262 RID: 8802 RVA: 0x000DE791 File Offset: 0x000DC991
	public override string Categorize()
	{
		return "debris";
	}

	// Token: 0x04001A84 RID: 6788
	public float DebrisDespawnOverride;
}
