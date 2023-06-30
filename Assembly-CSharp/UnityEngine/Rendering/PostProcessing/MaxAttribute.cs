using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A49 RID: 2633
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class MaxAttribute : Attribute
	{
		// Token: 0x06003F57 RID: 16215 RVA: 0x001732C8 File Offset: 0x001714C8
		public MaxAttribute(float max)
		{
			this.max = max;
		}

		// Token: 0x040038BB RID: 14523
		public readonly float max;
	}
}
