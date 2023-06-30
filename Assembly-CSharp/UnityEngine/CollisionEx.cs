using System;

namespace UnityEngine
{
	// Token: 0x02000A28 RID: 2600
	public static class CollisionEx
	{
		// Token: 0x06003DA5 RID: 15781 RVA: 0x00169557 File Offset: 0x00167757
		public static BaseEntity GetEntity(this Collision col)
		{
			return col.transform.ToBaseEntity();
		}
	}
}
