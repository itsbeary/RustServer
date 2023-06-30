using System;
using UnityEngine;

// Token: 0x0200022A RID: 554
public class BlendedLoopEngineSound : MonoBehaviour, IClientComponent
{
	// Token: 0x06001C08 RID: 7176 RVA: 0x000C4998 File Offset: 0x000C2B98
	public BlendedLoopEngineSound.EngineLoop[] GetEngineLoops()
	{
		return this.engineLoops;
	}

	// Token: 0x06001C09 RID: 7177 RVA: 0x000C49A0 File Offset: 0x000C2BA0
	public float GetLoopGain(int idx)
	{
		if (this.engineLoops != null && this.engineLoops[idx] != null && this.engineLoops[idx].gainMod != null)
		{
			return this.engineLoops[idx].gainMod.value;
		}
		return 0f;
	}

	// Token: 0x06001C0A RID: 7178 RVA: 0x000C49DB File Offset: 0x000C2BDB
	public float GetLoopPitch(int idx)
	{
		if (this.engineLoops != null && this.engineLoops[idx] != null && this.engineLoops[idx].pitchMod != null)
		{
			return this.engineLoops[idx].pitchMod.value;
		}
		return 0f;
	}

	// Token: 0x17000259 RID: 601
	// (get) Token: 0x06001C0B RID: 7179 RVA: 0x000C4A16 File Offset: 0x000C2C16
	public float maxDistance
	{
		get
		{
			return this.loopDefinition.engineLoops[0].soundDefinition.maxDistance;
		}
	}

	// Token: 0x040013F9 RID: 5113
	public BlendedEngineLoopDefinition loopDefinition;

	// Token: 0x040013FA RID: 5114
	public bool engineOn;

	// Token: 0x040013FB RID: 5115
	[Range(0f, 1f)]
	public float RPMControl;

	// Token: 0x040013FC RID: 5116
	public float smoothedRPMControl;

	// Token: 0x040013FD RID: 5117
	private BlendedLoopEngineSound.EngineLoop[] engineLoops;

	// Token: 0x02000C89 RID: 3209
	public class EngineLoop
	{
		// Token: 0x0400440B RID: 17419
		public BlendedEngineLoopDefinition.EngineLoopDefinition definition;

		// Token: 0x0400440C RID: 17420
		public BlendedLoopEngineSound parent;

		// Token: 0x0400440D RID: 17421
		public Sound sound;

		// Token: 0x0400440E RID: 17422
		public SoundModulation.Modulator gainMod;

		// Token: 0x0400440F RID: 17423
		public SoundModulation.Modulator pitchMod;
	}
}
