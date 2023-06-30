using System;
using UnityEngine;

namespace Facepunch.UI
{
	// Token: 0x02000AF9 RID: 2809
	public class ESPCanvas : SingletonComponent<ESPCanvas>
	{
		// Token: 0x170005FE RID: 1534
		// (get) Token: 0x060043BD RID: 17341 RVA: 0x0018F03C File Offset: 0x0018D23C
		// (set) Token: 0x060043BE RID: 17342 RVA: 0x0018F043 File Offset: 0x0018D243
		[ClientVar(ClientAdmin = true, Help = "Max amount of nameplates to show at once")]
		public static int MaxNameplates
		{
			get
			{
				return ESPCanvas.NameplateCount;
			}
			set
			{
				ESPCanvas.NameplateCount = Mathf.Clamp(value, 16, 150);
			}
		}

		// Token: 0x04003CE2 RID: 15586
		[Tooltip("Amount of times per second we should update the visible panels")]
		public float RefreshRate = 5f;

		// Token: 0x04003CE3 RID: 15587
		[Tooltip("This object will be duplicated in place")]
		public ESPPlayerInfo Source;

		// Token: 0x04003CE4 RID: 15588
		[Tooltip("Entities this far away won't be overlayed")]
		public float MaxDistance = 64f;

		// Token: 0x04003CE5 RID: 15589
		private static int NameplateCount = 32;

		// Token: 0x04003CE6 RID: 15590
		[ClientVar(ClientAdmin = true)]
		public static float OverrideMaxDisplayDistance = 0f;

		// Token: 0x04003CE7 RID: 15591
		[ClientVar(ClientAdmin = true)]
		public static bool DisableOcclusionChecks = false;

		// Token: 0x04003CE8 RID: 15592
		[ClientVar(ClientAdmin = true)]
		public static bool ShowHealth = false;

		// Token: 0x04003CE9 RID: 15593
		[ClientVar(ClientAdmin = true)]
		public static bool ColourCodeTeams = false;
	}
}
