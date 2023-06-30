using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A48 RID: 2632
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class DisplayNameAttribute : Attribute
	{
		// Token: 0x06003F56 RID: 16214 RVA: 0x001732B9 File Offset: 0x001714B9
		public DisplayNameAttribute(string displayName)
		{
			this.displayName = displayName;
		}

		// Token: 0x040038BA RID: 14522
		public readonly string displayName;
	}
}
