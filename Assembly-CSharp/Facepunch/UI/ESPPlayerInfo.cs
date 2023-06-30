using System;
using TMPro;
using UnityEngine;

namespace Facepunch.UI
{
	// Token: 0x02000AFA RID: 2810
	public class ESPPlayerInfo : MonoBehaviour
	{
		// Token: 0x170005FF RID: 1535
		// (get) Token: 0x060043C1 RID: 17345 RVA: 0x0018F09A File Offset: 0x0018D29A
		// (set) Token: 0x060043C2 RID: 17346 RVA: 0x0018F0A2 File Offset: 0x0018D2A2
		public BasePlayer Entity { get; set; }

		// Token: 0x04003CEA RID: 15594
		public Vector3 WorldOffset;

		// Token: 0x04003CEB RID: 15595
		public TextMeshProUGUI Text;

		// Token: 0x04003CEC RID: 15596
		public TextMeshProUGUI Image;

		// Token: 0x04003CED RID: 15597
		public CanvasGroup group;

		// Token: 0x04003CEE RID: 15598
		public Gradient gradientNormal;

		// Token: 0x04003CEF RID: 15599
		public Gradient gradientTeam;

		// Token: 0x04003CF0 RID: 15600
		public Color TeamColor;

		// Token: 0x04003CF1 RID: 15601
		public Color AllyColor = Color.blue;

		// Token: 0x04003CF2 RID: 15602
		public Color EnemyColor;

		// Token: 0x04003CF3 RID: 15603
		public QueryVis visCheck;
	}
}
