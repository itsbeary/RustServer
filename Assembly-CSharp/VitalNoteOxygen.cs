using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008EE RID: 2286
public class VitalNoteOxygen : MonoBehaviour, IClientComponent, IVitalNotice
{
	// Token: 0x04003303 RID: 13059
	[SerializeField]
	private float refreshTime = 1f;

	// Token: 0x04003304 RID: 13060
	[SerializeField]
	private TextMeshProUGUI valueText;

	// Token: 0x04003305 RID: 13061
	[SerializeField]
	private Animator animator;

	// Token: 0x04003306 RID: 13062
	[SerializeField]
	private Image airIcon;

	// Token: 0x04003307 RID: 13063
	[SerializeField]
	private RectTransform airIconTr;

	// Token: 0x04003308 RID: 13064
	[SerializeField]
	private Image backgroundImage;

	// Token: 0x04003309 RID: 13065
	[SerializeField]
	private Color baseColour;

	// Token: 0x0400330A RID: 13066
	[SerializeField]
	private Color badColour;

	// Token: 0x0400330B RID: 13067
	[SerializeField]
	private Image iconImage;

	// Token: 0x0400330C RID: 13068
	[SerializeField]
	private Color iconBaseColour;

	// Token: 0x0400330D RID: 13069
	[SerializeField]
	private Color iconBadColour;

	// Token: 0x0400330E RID: 13070
	protected bool show = true;
}
