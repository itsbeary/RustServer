using System;
using UnityEngine;

// Token: 0x0200080F RID: 2063
public class UIHUD : SingletonComponent<UIHUD>, IUIScreen
{
	// Token: 0x04002E80 RID: 11904
	public UIChat chatPanel;

	// Token: 0x04002E81 RID: 11905
	public HudElement Hunger;

	// Token: 0x04002E82 RID: 11906
	public HudElement Thirst;

	// Token: 0x04002E83 RID: 11907
	public HudElement Health;

	// Token: 0x04002E84 RID: 11908
	public HudElement PendingHealth;

	// Token: 0x04002E85 RID: 11909
	public HudElement VehicleHealth;

	// Token: 0x04002E86 RID: 11910
	public HudElement AnimalStamina;

	// Token: 0x04002E87 RID: 11911
	public HudElement AnimalStaminaMax;

	// Token: 0x04002E88 RID: 11912
	public RectTransform vitalsRect;

	// Token: 0x04002E89 RID: 11913
	public Canvas healthCanvas;

	// Token: 0x04002E8A RID: 11914
	public UICompass CompassWidget;

	// Token: 0x04002E8B RID: 11915
	public GameObject KeyboardCaptureMode;
}
