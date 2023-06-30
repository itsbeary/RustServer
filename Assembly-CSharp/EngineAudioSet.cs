using System;
using UnityEngine;

// Token: 0x0200049A RID: 1178
[CreateAssetMenu(fileName = "Engine Audio Preset", menuName = "Rust/Vehicles/Engine Audio Preset")]
public class EngineAudioSet : ScriptableObject
{
	// Token: 0x060026E1 RID: 9953 RVA: 0x000F4030 File Offset: 0x000F2230
	public BlendedEngineLoopDefinition GetEngineLoopDef(int numEngines)
	{
		int num = (numEngines - 1) % this.engineAudioLoops.Length;
		return this.engineAudioLoops[num];
	}

	// Token: 0x04001F37 RID: 7991
	public BlendedEngineLoopDefinition[] engineAudioLoops;

	// Token: 0x04001F38 RID: 7992
	public int priority;

	// Token: 0x04001F39 RID: 7993
	public float idleVolume = 0.4f;

	// Token: 0x04001F3A RID: 7994
	public float maxVolume = 0.6f;

	// Token: 0x04001F3B RID: 7995
	public float volumeChangeRateUp = 48f;

	// Token: 0x04001F3C RID: 7996
	public float volumeChangeRateDown = 16f;

	// Token: 0x04001F3D RID: 7997
	public float idlePitch = 0.25f;

	// Token: 0x04001F3E RID: 7998
	public float maxPitch = 1.5f;

	// Token: 0x04001F3F RID: 7999
	public float idleRpm = 600f;

	// Token: 0x04001F40 RID: 8000
	public float gearUpRpm = 5000f;

	// Token: 0x04001F41 RID: 8001
	public float gearDownRpm = 2500f;

	// Token: 0x04001F42 RID: 8002
	public int numGears = 5;

	// Token: 0x04001F43 RID: 8003
	public float maxRpm = 6000f;

	// Token: 0x04001F44 RID: 8004
	public float gearUpRpmRate = 5f;

	// Token: 0x04001F45 RID: 8005
	public float gearDownRpmRate = 6f;

	// Token: 0x04001F46 RID: 8006
	public SoundDefinition badPerformanceLoop;
}
