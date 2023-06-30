using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008A8 RID: 2216
public class PieOption : MonoBehaviour
{
	// Token: 0x17000458 RID: 1112
	// (get) Token: 0x06003706 RID: 14086 RVA: 0x0014B5B5 File Offset: 0x001497B5
	internal float midRadius
	{
		get
		{
			return (this.background.startRadius + this.background.endRadius) * 0.5f;
		}
	}

	// Token: 0x17000459 RID: 1113
	// (get) Token: 0x06003707 RID: 14087 RVA: 0x0014B5D4 File Offset: 0x001497D4
	internal float sliceSize
	{
		get
		{
			return this.background.endRadius - this.background.startRadius;
		}
	}

	// Token: 0x06003708 RID: 14088 RVA: 0x0014B5F0 File Offset: 0x001497F0
	public void UpdateOption(float startSlice, float sliceSize, float border, string optionTitle, float outerSize, float innerSize, float imageSize, Sprite sprite, bool showOverlay)
	{
		if (this.background == null)
		{
			return;
		}
		float num = this.background.rectTransform.rect.height * 0.5f;
		float num2 = num * (innerSize + (outerSize - innerSize) * 0.5f);
		float num3 = num * (outerSize - innerSize);
		this.background.startRadius = startSlice;
		this.background.endRadius = startSlice + sliceSize;
		this.background.border = border;
		this.background.outerSize = outerSize;
		this.background.innerSize = innerSize;
		this.background.color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 0f);
		float num4 = startSlice + sliceSize * 0.5f;
		float num5 = Mathf.Sin(num4 * 0.017453292f) * num2;
		float num6 = Mathf.Cos(num4 * 0.017453292f) * num2;
		this.imageIcon.rectTransform.localPosition = new Vector3(num5, num6);
		this.imageIcon.rectTransform.sizeDelta = new Vector2(num3 * imageSize, num3 * imageSize);
		this.imageIcon.sprite = sprite;
		this.overlayIcon.gameObject.SetActive(showOverlay);
		if (showOverlay)
		{
			this.overlayIcon.rectTransform.localPosition = this.imageIcon.rectTransform.localPosition;
			this.overlayIcon.rectTransform.sizeDelta = this.imageIcon.rectTransform.sizeDelta;
		}
	}

	// Token: 0x04003200 RID: 12800
	public PieShape background;

	// Token: 0x04003201 RID: 12801
	public Image imageIcon;

	// Token: 0x04003202 RID: 12802
	public Image overlayIcon;
}
