using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000856 RID: 2134
public class SquareBorder : MonoBehaviour
{
	// Token: 0x0600361D RID: 13853 RVA: 0x00148048 File Offset: 0x00146248
	private void Update()
	{
		if (this._lastSize != this.Size)
		{
			this.Top.offsetMin = new Vector2(0f, -this.Size);
			this.Bottom.offsetMax = new Vector2(0f, this.Size);
			this.Left.offsetMin = new Vector2(0f, this.Size);
			this.Left.offsetMax = new Vector2(this.Size, -this.Size);
			this.Right.offsetMin = new Vector2(-this.Size, this.Size);
			this.Right.offsetMax = new Vector2(0f, -this.Size);
			this._lastSize = this.Size;
		}
		if (this._lastColor != this.Color)
		{
			this.TopImage.color = this.Color;
			this.BottomImage.color = this.Color;
			this.LeftImage.color = this.Color;
			this.RightImage.color = this.Color;
			this._lastColor = this.Color;
		}
	}

	// Token: 0x04003020 RID: 12320
	public float Size;

	// Token: 0x04003021 RID: 12321
	public Color Color;

	// Token: 0x04003022 RID: 12322
	public RectTransform Top;

	// Token: 0x04003023 RID: 12323
	public RectTransform Bottom;

	// Token: 0x04003024 RID: 12324
	public RectTransform Left;

	// Token: 0x04003025 RID: 12325
	public RectTransform Right;

	// Token: 0x04003026 RID: 12326
	public Image TopImage;

	// Token: 0x04003027 RID: 12327
	public Image BottomImage;

	// Token: 0x04003028 RID: 12328
	public Image LeftImage;

	// Token: 0x04003029 RID: 12329
	public Image RightImage;

	// Token: 0x0400302A RID: 12330
	private float _lastSize;

	// Token: 0x0400302B RID: 12331
	private Color _lastColor;
}
