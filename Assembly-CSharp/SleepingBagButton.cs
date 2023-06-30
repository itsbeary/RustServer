using System;
using Rust.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008BC RID: 2236
public class SleepingBagButton : MonoBehaviour
{
	// Token: 0x04003260 RID: 12896
	public GameObject TimeLockRoot;

	// Token: 0x04003261 RID: 12897
	public GameObject LockRoot;

	// Token: 0x04003262 RID: 12898
	public GameObject OccupiedRoot;

	// Token: 0x04003263 RID: 12899
	public Button ClickButton;

	// Token: 0x04003264 RID: 12900
	public TextMeshProUGUI BagName;

	// Token: 0x04003265 RID: 12901
	public TextMeshProUGUI LockTime;

	// Token: 0x04003266 RID: 12902
	public Image Icon;

	// Token: 0x04003267 RID: 12903
	public Sprite SleepingBagSprite;

	// Token: 0x04003268 RID: 12904
	public Sprite BedSprite;

	// Token: 0x04003269 RID: 12905
	public Sprite BeachTowelSprite;

	// Token: 0x0400326A RID: 12906
	public Sprite CamperSprite;

	// Token: 0x0400326B RID: 12907
	public Image CircleRim;

	// Token: 0x0400326C RID: 12908
	public Image CircleFill;

	// Token: 0x0400326D RID: 12909
	public Image Background;

	// Token: 0x0400326E RID: 12910
	public RustButton DeleteButton;

	// Token: 0x0400326F RID: 12911
	public Image ConfirmSlider;

	// Token: 0x04003270 RID: 12912
	public static Translate.Phrase toastHoldToUnclaimBag = new Translate.Phrase("hold_unclaim_bag", "Hold down the delete button to unclaim a sleeping bag");
}
