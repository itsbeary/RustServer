using System;
using UnityEngine;

// Token: 0x02000229 RID: 553
[CreateAssetMenu(menuName = "Rust/Blended Engine Loop Definition")]
public class BlendedEngineLoopDefinition : ScriptableObject
{
	// Token: 0x040013F4 RID: 5108
	public BlendedEngineLoopDefinition.EngineLoopDefinition[] engineLoops;

	// Token: 0x040013F5 RID: 5109
	public float minRPM;

	// Token: 0x040013F6 RID: 5110
	public float maxRPM;

	// Token: 0x040013F7 RID: 5111
	public float RPMChangeRateUp = 0.5f;

	// Token: 0x040013F8 RID: 5112
	public float RPMChangeRateDown = 0.2f;

	// Token: 0x02000C88 RID: 3208
	[Serializable]
	public class EngineLoopDefinition
	{
		// Token: 0x06004F44 RID: 20292 RVA: 0x001A621E File Offset: 0x001A441E
		public float GetPitchForRPM(float targetRPM)
		{
			return targetRPM / this.RPM;
		}

		// Token: 0x04004405 RID: 17413
		public SoundDefinition soundDefinition;

		// Token: 0x04004406 RID: 17414
		public float RPM;

		// Token: 0x04004407 RID: 17415
		public float startRPM;

		// Token: 0x04004408 RID: 17416
		public float startFullRPM;

		// Token: 0x04004409 RID: 17417
		public float stopFullRPM;

		// Token: 0x0400440A RID: 17418
		public float stopRPM;
	}
}
