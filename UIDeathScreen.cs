using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008CE RID: 2254
public class UIDeathScreen : SingletonComponent<UIDeathScreen>, IUIScreen
{
	// Token: 0x040032C0 RID: 12992
	public LifeInfographic previousLifeInfographic;

	// Token: 0x040032C1 RID: 12993
	public Animator screenAnimator;

	// Token: 0x040032C2 RID: 12994
	public bool fadeIn;

	// Token: 0x040032C3 RID: 12995
	public Button ReportCheatButton;

	// Token: 0x040032C4 RID: 12996
	public MapView View;

	// Token: 0x040032C5 RID: 12997
	public List<SleepingBagButton> sleepingBagButtons = new List<SleepingBagButton>();

	// Token: 0x040032C6 RID: 12998
	public UIDeathScreen.RespawnColourScheme[] RespawnColourSchemes;

	// Token: 0x040032C7 RID: 12999
	public GameObject RespawnScrollGradient;

	// Token: 0x040032C8 RID: 13000
	public ScrollRect RespawnScrollRect;

	// Token: 0x040032C9 RID: 13001
	public ExpandedLifeStats ExpandedStats;

	// Token: 0x040032CA RID: 13002
	public CanvasGroup StreamerModeContainer;

	// Token: 0x02000EBC RID: 3772
	[Serializable]
	public struct RespawnColourScheme
	{
		// Token: 0x04004D23 RID: 19747
		public Color BackgroundColour;

		// Token: 0x04004D24 RID: 19748
		public Color CircleRimColour;

		// Token: 0x04004D25 RID: 19749
		public Color CircleFillColour;
	}
}
