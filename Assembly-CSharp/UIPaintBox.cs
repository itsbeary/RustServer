using System;
using Painting;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000807 RID: 2055
public class UIPaintBox : MonoBehaviour
{
	// Token: 0x060035AB RID: 13739 RVA: 0x00146F48 File Offset: 0x00145148
	public void UpdateBrushSize(int size)
	{
		this.brush.brushSize = Vector2.one * (float)size;
		this.brush.spacing = Mathf.Clamp((float)size * 0.1f, 1f, 3f);
		this.OnChanged();
	}

	// Token: 0x060035AC RID: 13740 RVA: 0x00146F94 File Offset: 0x00145194
	public void UpdateBrushTexture(Texture2D tex)
	{
		this.brush.texture = tex;
		this.OnChanged();
	}

	// Token: 0x060035AD RID: 13741 RVA: 0x00146FA8 File Offset: 0x001451A8
	public void UpdateBrushColor(Color col)
	{
		this.brush.color.r = col.r;
		this.brush.color.g = col.g;
		this.brush.color.b = col.b;
		this.OnChanged();
	}

	// Token: 0x060035AE RID: 13742 RVA: 0x00146FFD File Offset: 0x001451FD
	public void UpdateBrushAlpha(float a)
	{
		this.brush.color.a = a;
		this.OnChanged();
	}

	// Token: 0x060035AF RID: 13743 RVA: 0x00147016 File Offset: 0x00145216
	public void UpdateBrushEraser(bool b)
	{
		this.brush.erase = b;
	}

	// Token: 0x060035B0 RID: 13744 RVA: 0x00147024 File Offset: 0x00145224
	private void OnChanged()
	{
		this.onBrushChanged.Invoke(this.brush);
	}

	// Token: 0x04002E56 RID: 11862
	public UIPaintBox.OnBrushChanged onBrushChanged = new UIPaintBox.OnBrushChanged();

	// Token: 0x04002E57 RID: 11863
	public Brush brush;

	// Token: 0x02000E96 RID: 3734
	[Serializable]
	public class OnBrushChanged : UnityEvent<Brush>
	{
	}
}
