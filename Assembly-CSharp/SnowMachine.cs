using System;

// Token: 0x02000192 RID: 402
public class SnowMachine : FogMachine
{
	// Token: 0x06001813 RID: 6163 RVA: 0x00007A44 File Offset: 0x00005C44
	public override bool MotionModeEnabled()
	{
		return false;
	}

	// Token: 0x06001814 RID: 6164 RVA: 0x000B50C9 File Offset: 0x000B32C9
	public override void EnableFogField()
	{
		base.EnableFogField();
		this.tempTrigger.gameObject.SetActive(true);
	}

	// Token: 0x06001815 RID: 6165 RVA: 0x000B50E2 File Offset: 0x000B32E2
	public override void FinishFogging()
	{
		base.FinishFogging();
		this.tempTrigger.gameObject.SetActive(false);
	}

	// Token: 0x040010E7 RID: 4327
	public AdaptMeshToTerrain snowMesh;

	// Token: 0x040010E8 RID: 4328
	public TriggerTemperature tempTrigger;
}
