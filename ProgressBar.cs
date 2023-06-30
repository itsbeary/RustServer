using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020008B2 RID: 2226
public class ProgressBar : UIBehaviour
{
	// Token: 0x1700045B RID: 1115
	// (get) Token: 0x06003715 RID: 14101 RVA: 0x0014BA9E File Offset: 0x00149C9E
	public bool InstanceIsOpen
	{
		get
		{
			if (ProgressBar.Instance == this)
			{
				return this.isOpen;
			}
			return ProgressBar.Instance.InstanceIsOpen;
		}
	}

	// Token: 0x04003237 RID: 12855
	public static ProgressBar Instance;

	// Token: 0x04003238 RID: 12856
	private Action<BasePlayer> action;

	// Token: 0x04003239 RID: 12857
	public float timeFinished;

	// Token: 0x0400323A RID: 12858
	private float timeCounter;

	// Token: 0x0400323B RID: 12859
	public GameObject scaleTarget;

	// Token: 0x0400323C RID: 12860
	public Image progressField;

	// Token: 0x0400323D RID: 12861
	public Image iconField;

	// Token: 0x0400323E RID: 12862
	public Text leftField;

	// Token: 0x0400323F RID: 12863
	public Text rightField;

	// Token: 0x04003240 RID: 12864
	public SoundDefinition clipOpen;

	// Token: 0x04003241 RID: 12865
	public SoundDefinition clipCancel;

	// Token: 0x04003242 RID: 12866
	private bool isOpen;
}
