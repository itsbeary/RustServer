using System;
using Rust.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000801 RID: 2049
public class SleepingBagMapMarker : MonoBehaviour
{
	// Token: 0x04002E37 RID: 11831
	public Image MapIcon;

	// Token: 0x04002E38 RID: 11832
	public Image SleepingBagIcon;

	// Token: 0x04002E39 RID: 11833
	public Sprite SleepingBagSprite;

	// Token: 0x04002E3A RID: 11834
	public Sprite BedSprite;

	// Token: 0x04002E3B RID: 11835
	public Sprite BeachTowelSprite;

	// Token: 0x04002E3C RID: 11836
	public Sprite CamperSprite;

	// Token: 0x04002E3D RID: 11837
	public Tooltip MarkerTooltip;

	// Token: 0x04002E3E RID: 11838
	public GameObject LockRoot;

	// Token: 0x04002E3F RID: 11839
	public TextMeshProUGUI LockTime;

	// Token: 0x04002E40 RID: 11840
	public GameObject OccupiedRoot;

	// Token: 0x04002E41 RID: 11841
	public Image CircleRim;

	// Token: 0x04002E42 RID: 11842
	public Image CircleFill;

	// Token: 0x04002E43 RID: 11843
	public RustButton DeleteButton;

	// Token: 0x04002E44 RID: 11844
	public Image ConfirmSlider;
}
