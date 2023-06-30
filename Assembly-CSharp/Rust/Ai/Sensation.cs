using System;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x02000B4E RID: 2894
	public struct Sensation
	{
		// Token: 0x04003EF3 RID: 16115
		public SensationType Type;

		// Token: 0x04003EF4 RID: 16116
		public Vector3 Position;

		// Token: 0x04003EF5 RID: 16117
		public float Radius;

		// Token: 0x04003EF6 RID: 16118
		public float DamagePotential;

		// Token: 0x04003EF7 RID: 16119
		public BaseEntity Initiator;

		// Token: 0x04003EF8 RID: 16120
		public BasePlayer InitiatorPlayer;

		// Token: 0x04003EF9 RID: 16121
		public BaseEntity UsedEntity;
	}
}
