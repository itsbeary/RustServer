using System;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
	// Token: 0x02000A1C RID: 2588
	public sealed class MinAttribute : PropertyAttribute
	{
		// Token: 0x06003D77 RID: 15735 RVA: 0x00168D47 File Offset: 0x00166F47
		public MinAttribute(float min)
		{
			this.min = min;
		}

		// Token: 0x040037B2 RID: 14258
		public readonly float min;
	}
}
