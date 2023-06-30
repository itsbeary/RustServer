using System;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000B36 RID: 2870
	public class ModularCarAudio : GroundVehicleAudio
	{
		// Token: 0x04003E77 RID: 15991
		public bool showDebug;

		// Token: 0x04003E78 RID: 15992
		[Header("Skid")]
		[SerializeField]
		private SoundDefinition skidSoundLoop;

		// Token: 0x04003E79 RID: 15993
		[SerializeField]
		private SoundDefinition skidSoundDirtLoop;

		// Token: 0x04003E7A RID: 15994
		[SerializeField]
		private SoundDefinition skidSoundSnowLoop;

		// Token: 0x04003E7B RID: 15995
		[SerializeField]
		private float skidMinSlip = 10f;

		// Token: 0x04003E7C RID: 15996
		[SerializeField]
		private float skidMaxSlip = 25f;

		// Token: 0x04003E7D RID: 15997
		[Header("Movement & Suspension")]
		[SerializeField]
		private SoundDefinition movementStartOneshot;

		// Token: 0x04003E7E RID: 15998
		[SerializeField]
		private SoundDefinition movementStopOneshot;

		// Token: 0x04003E7F RID: 15999
		[SerializeField]
		private float movementStartStopMinTimeBetweenSounds = 0.25f;

		// Token: 0x04003E80 RID: 16000
		[SerializeField]
		private SoundDefinition movementRattleLoop;

		// Token: 0x04003E81 RID: 16001
		[SerializeField]
		private float movementRattleMaxSpeed = 10f;

		// Token: 0x04003E82 RID: 16002
		[SerializeField]
		private float movementRattleMaxAngSpeed = 10f;

		// Token: 0x04003E83 RID: 16003
		[SerializeField]
		private float movementRattleIdleGain = 0.3f;

		// Token: 0x04003E84 RID: 16004
		[SerializeField]
		private SoundDefinition suspensionLurchSound;

		// Token: 0x04003E85 RID: 16005
		[SerializeField]
		private float suspensionLurchMinExtensionDelta = 0.4f;

		// Token: 0x04003E86 RID: 16006
		[SerializeField]
		private float suspensionLurchMinTimeBetweenSounds = 0.25f;

		// Token: 0x04003E87 RID: 16007
		[Header("Wheels")]
		[SerializeField]
		private SoundDefinition tyreRollingSoundDef;

		// Token: 0x04003E88 RID: 16008
		[SerializeField]
		private SoundDefinition tyreRollingWaterSoundDef;

		// Token: 0x04003E89 RID: 16009
		[SerializeField]
		private SoundDefinition tyreRollingGrassSoundDef;

		// Token: 0x04003E8A RID: 16010
		[SerializeField]
		private SoundDefinition tyreRollingSnowSoundDef;

		// Token: 0x04003E8B RID: 16011
		[SerializeField]
		private AnimationCurve tyreRollGainCurve;
	}
}
