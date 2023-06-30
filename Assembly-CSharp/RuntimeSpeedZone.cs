using System;

// Token: 0x020001B2 RID: 434
public class RuntimeSpeedZone : IAIPathSpeedZone
{
	// Token: 0x060018FF RID: 6399 RVA: 0x000B8C11 File Offset: 0x000B6E11
	public float GetMaxSpeed()
	{
		return this.maxVelocityPerSec;
	}

	// Token: 0x06001900 RID: 6400 RVA: 0x000B8C19 File Offset: 0x000B6E19
	public OBB WorldSpaceBounds()
	{
		return this.worldOBBBounds;
	}

	// Token: 0x04001183 RID: 4483
	public OBB worldOBBBounds;

	// Token: 0x04001184 RID: 4484
	public float maxVelocityPerSec = 5f;
}
