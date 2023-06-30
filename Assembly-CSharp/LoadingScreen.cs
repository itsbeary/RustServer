using System;
using Rust.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200087D RID: 2173
public class LoadingScreen : SingletonComponent<LoadingScreen>
{
	// Token: 0x17000451 RID: 1105
	// (get) Token: 0x06003663 RID: 13923 RVA: 0x00148935 File Offset: 0x00146B35
	public static bool isOpen
	{
		get
		{
			return SingletonComponent<LoadingScreen>.Instance && SingletonComponent<LoadingScreen>.Instance.panel && SingletonComponent<LoadingScreen>.Instance.panel.gameObject.activeSelf;
		}
	}

	// Token: 0x17000452 RID: 1106
	// (get) Token: 0x06003664 RID: 13924 RVA: 0x0014896A File Offset: 0x00146B6A
	// (set) Token: 0x06003665 RID: 13925 RVA: 0x00148971 File Offset: 0x00146B71
	public static bool WantsSkip { get; private set; }

	// Token: 0x17000453 RID: 1107
	// (get) Token: 0x06003667 RID: 13927 RVA: 0x00148981 File Offset: 0x00146B81
	// (set) Token: 0x06003666 RID: 13926 RVA: 0x00148979 File Offset: 0x00146B79
	public static string Text { get; private set; }

	// Token: 0x06003668 RID: 13928 RVA: 0x000063A5 File Offset: 0x000045A5
	public static void Update(string strType)
	{
	}

	// Token: 0x06003669 RID: 13929 RVA: 0x000063A5 File Offset: 0x000045A5
	public static void Update(string strType, string strSubtitle)
	{
	}

	// Token: 0x04003115 RID: 12565
	public CanvasRenderer panel;

	// Token: 0x04003116 RID: 12566
	public TextMeshProUGUI title;

	// Token: 0x04003117 RID: 12567
	public TextMeshProUGUI subtitle;

	// Token: 0x04003118 RID: 12568
	public Button skipButton;

	// Token: 0x04003119 RID: 12569
	public Button cancelButton;

	// Token: 0x0400311A RID: 12570
	public GameObject performanceWarning;

	// Token: 0x0400311B RID: 12571
	public AudioSource music;

	// Token: 0x0400311C RID: 12572
	public RectTransform serverInfo;

	// Token: 0x0400311D RID: 12573
	public RustText serverName;

	// Token: 0x0400311E RID: 12574
	public RustText serverPlayers;

	// Token: 0x0400311F RID: 12575
	public RustLayout serverModeSection;

	// Token: 0x04003120 RID: 12576
	public RustText serverMode;

	// Token: 0x04003121 RID: 12577
	public RustText serverMap;

	// Token: 0x04003122 RID: 12578
	public RustLayout serverTagsSection;

	// Token: 0x04003123 RID: 12579
	public ServerBrowserTagList serverTags;

	// Token: 0x04003124 RID: 12580
	public RectTransform demoInfo;

	// Token: 0x04003125 RID: 12581
	public RustText demoName;

	// Token: 0x04003126 RID: 12582
	public RustText demoLength;

	// Token: 0x04003127 RID: 12583
	public RustText demoDate;

	// Token: 0x04003128 RID: 12584
	public RustText demoMap;

	// Token: 0x04003129 RID: 12585
	public RawImage backgroundImage;

	// Token: 0x0400312A RID: 12586
	public Texture2D defaultBackground;

	// Token: 0x0400312B RID: 12587
	public GameObject pingWarning;

	// Token: 0x0400312C RID: 12588
	public RustText pingWarningText;

	// Token: 0x0400312D RID: 12589
	[Tooltip("Ping must be at least this many ms higher than the server browser ping")]
	public int minPingDiffToShowWarning = 50;

	// Token: 0x0400312E RID: 12590
	[Tooltip("Ping must be this many times higher than the server browser ping")]
	public float pingDiffFactorToShowWarning = 2f;

	// Token: 0x0400312F RID: 12591
	[Tooltip("Number of ping samples required before showing the warning")]
	public int requiredPingSampleCount = 10;

	// Token: 0x04003130 RID: 12592
	public GameObject blackout;
}
