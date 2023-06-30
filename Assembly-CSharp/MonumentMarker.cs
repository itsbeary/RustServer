using System;
using Facepunch;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007FE RID: 2046
public class MonumentMarker : MonoBehaviour
{
	// Token: 0x060035A0 RID: 13728 RVA: 0x00146E18 File Offset: 0x00145018
	public void Setup(LandmarkInfo info)
	{
		this.text.text = (info.displayPhrase.IsValid() ? info.displayPhrase.translated : info.transform.root.name);
		if (info.mapIcon != null)
		{
			this.image.sprite = info.mapIcon;
			this.text.SetActive(false);
			this.imageBackground.SetActive(true);
		}
		else
		{
			this.text.SetActive(true);
			this.imageBackground.SetActive(false);
		}
		this.SetNightMode(false);
	}

	// Token: 0x060035A1 RID: 13729 RVA: 0x00146EB4 File Offset: 0x001450B4
	public void SetNightMode(bool nightMode)
	{
		Color color = (nightMode ? this.nightColor : this.dayColor);
		Color color2 = (nightMode ? this.dayColor : this.nightColor);
		if (this.text != null)
		{
			this.text.color = color;
		}
		if (this.image != null)
		{
			this.image.color = color;
		}
		if (this.imageBackground != null)
		{
			this.imageBackground.color = color2;
		}
	}

	// Token: 0x04002E20 RID: 11808
	public Text text;

	// Token: 0x04002E21 RID: 11809
	public Image imageBackground;

	// Token: 0x04002E22 RID: 11810
	public Image image;

	// Token: 0x04002E23 RID: 11811
	public Color dayColor;

	// Token: 0x04002E24 RID: 11812
	public Color nightColor;
}
