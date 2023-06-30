using System;
using UnityEngine;

// Token: 0x02000355 RID: 853
public class ScaleRenderer : MonoBehaviour
{
	// Token: 0x06001F6E RID: 8046 RVA: 0x000D4D3C File Offset: 0x000D2F3C
	private bool ScaleDifferent(float newScale)
	{
		return newScale != this.lastScale;
	}

	// Token: 0x06001F6F RID: 8047 RVA: 0x000D4D4A File Offset: 0x000D2F4A
	public void Start()
	{
		if (this.useRandomScale)
		{
			this.SetScale(UnityEngine.Random.Range(this.scaleMin, this.scaleMax));
		}
	}

	// Token: 0x06001F70 RID: 8048 RVA: 0x000D4D6C File Offset: 0x000D2F6C
	public void SetScale(float scale)
	{
		if (!this.hasInitialValues)
		{
			this.GatherInitialValues();
		}
		if (this.ScaleDifferent(scale) || (scale > 0f && !this.myRenderer.enabled))
		{
			this.SetRendererEnabled(scale != 0f);
			this.SetScale_Internal(scale);
		}
	}

	// Token: 0x06001F71 RID: 8049 RVA: 0x000D4DBD File Offset: 0x000D2FBD
	public virtual void SetScale_Internal(float scale)
	{
		this.lastScale = scale;
	}

	// Token: 0x06001F72 RID: 8050 RVA: 0x000D4DC6 File Offset: 0x000D2FC6
	public virtual void SetRendererEnabled(bool isEnabled)
	{
		if (this.myRenderer && this.myRenderer.enabled != isEnabled)
		{
			this.myRenderer.enabled = isEnabled;
		}
	}

	// Token: 0x06001F73 RID: 8051 RVA: 0x000D4DEF File Offset: 0x000D2FEF
	public virtual void GatherInitialValues()
	{
		this.hasInitialValues = true;
	}

	// Token: 0x040018B5 RID: 6325
	public bool useRandomScale;

	// Token: 0x040018B6 RID: 6326
	public float scaleMin = 1f;

	// Token: 0x040018B7 RID: 6327
	public float scaleMax = 1f;

	// Token: 0x040018B8 RID: 6328
	private float lastScale = -1f;

	// Token: 0x040018B9 RID: 6329
	protected bool hasInitialValues;

	// Token: 0x040018BA RID: 6330
	public Renderer myRenderer;
}
