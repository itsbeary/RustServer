using System;
using UnityEngine;

namespace Sonar
{
	// Token: 0x02000A21 RID: 2593
	public class SonarSystem : FacepunchBehaviour
	{
		// Token: 0x040037BB RID: 14267
		[SerializeField]
		private float range = 100f;

		// Token: 0x040037BC RID: 14268
		[SerializeField]
		private float maxDepth = float.PositiveInfinity;

		// Token: 0x040037BD RID: 14269
		[SerializeField]
		private ParticleSystem sonarPS;

		// Token: 0x040037BE RID: 14270
		[SerializeField]
		private ParticleSystem blipPS;

		// Token: 0x040037BF RID: 14271
		[SerializeField]
		private SonarObject us;

		// Token: 0x040037C0 RID: 14272
		[SerializeField]
		private Color greenBlip;

		// Token: 0x040037C1 RID: 14273
		[SerializeField]
		private Color redBlip;

		// Token: 0x040037C2 RID: 14274
		[SerializeField]
		private Color whiteBlip;

		// Token: 0x040037C3 RID: 14275
		[SerializeField]
		private SoundDefinition sonarBlipSound;

		// Token: 0x040037C4 RID: 14276
		[SerializeField]
		private GameObject sonarSoundParent;
	}
}
