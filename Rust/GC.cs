using System;
using UnityEngine;

namespace Rust
{
	// Token: 0x02000B0D RID: 2829
	public class GC : MonoBehaviour, IClientComponent
	{
		// Token: 0x1700063A RID: 1594
		// (get) Token: 0x06004500 RID: 17664 RVA: 0x0000441C File Offset: 0x0000261C
		public static bool Enabled
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06004501 RID: 17665 RVA: 0x00194836 File Offset: 0x00192A36
		public static void Collect()
		{
			GC.Collect();
		}

		// Token: 0x06004502 RID: 17666 RVA: 0x0019483D File Offset: 0x00192A3D
		public static long GetTotalMemory()
		{
			return GC.GetTotalMemory(false) / 1048576L;
		}

		// Token: 0x06004503 RID: 17667 RVA: 0x0019484C File Offset: 0x00192A4C
		public static int CollectionCount()
		{
			return GC.CollectionCount(0);
		}
	}
}
