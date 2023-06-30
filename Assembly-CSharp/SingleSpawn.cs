using System;

// Token: 0x0200057C RID: 1404
public class SingleSpawn : SpawnGroup
{
	// Token: 0x06002B13 RID: 11027 RVA: 0x00007A44 File Offset: 0x00005C44
	public override bool WantsInitialSpawn()
	{
		return false;
	}

	// Token: 0x06002B14 RID: 11028 RVA: 0x00106163 File Offset: 0x00104363
	public void FillDelay(float delay)
	{
		base.Invoke(new Action(this.Fill), delay);
	}
}
