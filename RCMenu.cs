using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x0200010B RID: 267
public class RCMenu : ComputerMenu
{
	// Token: 0x04000E11 RID: 3601
	public Image backgroundOpaque;

	// Token: 0x04000E12 RID: 3602
	public InputField newBookmarkEntryField;

	// Token: 0x04000E13 RID: 3603
	public NeedsCursor needsCursor;

	// Token: 0x04000E14 RID: 3604
	public float hiddenOffset = -256f;

	// Token: 0x04000E15 RID: 3605
	public RectTransform devicesPanel;

	// Token: 0x04000E16 RID: 3606
	private Vector3 initialDevicesPosition;

	// Token: 0x04000E17 RID: 3607
	public static bool isControllingCamera;

	// Token: 0x04000E18 RID: 3608
	public CanvasGroup overExposure;

	// Token: 0x04000E19 RID: 3609
	public CanvasGroup interference;

	// Token: 0x04000E1A RID: 3610
	public float interferenceFadeDuration = 0.2f;

	// Token: 0x04000E1B RID: 3611
	public float rangeInterferenceScale = 10000f;

	// Token: 0x04000E1C RID: 3612
	public Text timeText;

	// Token: 0x04000E1D RID: 3613
	public Text watchedDurationText;

	// Token: 0x04000E1E RID: 3614
	public Text deviceNameText;

	// Token: 0x04000E1F RID: 3615
	public Text noSignalText;

	// Token: 0x04000E20 RID: 3616
	public Text healthText;

	// Token: 0x04000E21 RID: 3617
	public GameObject healthBarParent;

	// Token: 0x04000E22 RID: 3618
	public RectTransform healthBarBackground;

	// Token: 0x04000E23 RID: 3619
	public RectTransform healthBarFill;

	// Token: 0x04000E24 RID: 3620
	public SoundDefinition bookmarkPressedSoundDef;

	// Token: 0x04000E25 RID: 3621
	public GameObject[] hideIfStatic;

	// Token: 0x04000E26 RID: 3622
	public GameObject readOnlyIndicator;

	// Token: 0x04000E27 RID: 3623
	[FormerlySerializedAs("crosshair")]
	public GameObject aimCrosshair;

	// Token: 0x04000E28 RID: 3624
	public GameObject generalCrosshair;

	// Token: 0x04000E29 RID: 3625
	public float fogOverrideDensity = 0.1f;

	// Token: 0x04000E2A RID: 3626
	public float autoTurretFogDistance = 30f;

	// Token: 0x04000E2B RID: 3627
	public float autoTurretDotBaseScale = 2f;

	// Token: 0x04000E2C RID: 3628
	public float autoTurretDotGrowScale = 4f;

	// Token: 0x04000E2D RID: 3629
	public PingManager PingManager;

	// Token: 0x04000E2E RID: 3630
	public ScrollRectSettable scrollRect;
}
