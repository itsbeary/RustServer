using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AC3 RID: 2755
	[ConsoleSystem.Factory("fps")]
	public class FPS : ConsoleSystem
	{
		// Token: 0x170005A9 RID: 1449
		// (get) Token: 0x060041EA RID: 16874 RVA: 0x0018652D File Offset: 0x0018472D
		// (set) Token: 0x060041EB RID: 16875 RVA: 0x00186546 File Offset: 0x00184746
		[ClientVar(Saved = true)]
		[ServerVar(Saved = true)]
		public static int limit
		{
			get
			{
				if (FPS._limit == -1)
				{
					FPS._limit = Application.targetFrameRate;
				}
				return FPS._limit;
			}
			set
			{
				FPS._limit = value;
				Application.targetFrameRate = FPS._limit;
			}
		}

		// Token: 0x170005AA RID: 1450
		// (get) Token: 0x060041EC RID: 16876 RVA: 0x00186558 File Offset: 0x00184758
		// (set) Token: 0x060041ED RID: 16877 RVA: 0x00186560 File Offset: 0x00184760
		[ClientVar]
		public static int graph
		{
			get
			{
				return FPS.m_graph;
			}
			set
			{
				FPS.m_graph = value;
				if (!MainCamera.mainCamera)
				{
					return;
				}
				FPSGraph component = MainCamera.mainCamera.GetComponent<FPSGraph>();
				if (!component)
				{
					return;
				}
				component.Refresh();
			}
		}

		// Token: 0x04003BA6 RID: 15270
		private static int _limit = 240;

		// Token: 0x04003BA7 RID: 15271
		private static int m_graph;
	}
}
