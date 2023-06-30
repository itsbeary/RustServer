using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A4B RID: 2635
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class MinMaxAttribute : Attribute
	{
		// Token: 0x06003F59 RID: 16217 RVA: 0x001732E6 File Offset: 0x001714E6
		public MinMaxAttribute(float min, float max)
		{
			this.min = min;
			this.max = max;
		}

		// Token: 0x040038BD RID: 14525
		public readonly float min;

		// Token: 0x040038BE RID: 14526
		public readonly float max;
	}
}
