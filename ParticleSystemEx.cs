using System;
using UnityEngine;

// Token: 0x02000929 RID: 2345
public static class ParticleSystemEx
{
	// Token: 0x0600384A RID: 14410 RVA: 0x0014F2BE File Offset: 0x0014D4BE
	public static void SetPlayingState(this ParticleSystem ps, bool play)
	{
		if (ps == null)
		{
			return;
		}
		if (play && !ps.isPlaying)
		{
			ps.Play();
			return;
		}
		if (!play && ps.isPlaying)
		{
			ps.Stop();
		}
	}

	// Token: 0x0600384B RID: 14411 RVA: 0x0014F2F0 File Offset: 0x0014D4F0
	public static void SetEmitterState(this ParticleSystem ps, bool enable)
	{
		if (enable != ps.emission.enabled)
		{
			ps.emission.enabled = enable;
		}
	}
}
