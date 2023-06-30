using System;
using UnityEngine;

namespace Smaa
{
	// Token: 0x020009C3 RID: 2499
	public sealed class MinAttribute : PropertyAttribute
	{
		// Token: 0x06003BA8 RID: 15272 RVA: 0x00160116 File Offset: 0x0015E316
		public MinAttribute(float min)
		{
			this.min = min;
		}

		// Token: 0x0400368C RID: 13964
		public readonly float min;
	}
}
