using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000799 RID: 1945
public class AspectRatioGuide : MonoBehaviour
{
	// Token: 0x060034EF RID: 13551 RVA: 0x00145538 File Offset: 0x00143738
	private void Populate()
	{
		this.aspect = CameraMan.GuideAspect;
		this.ratio = Mathf.Max(CameraMan.GuideRatio, 1f);
		this.aspectRatioFitter.aspectRatio = this.aspect / this.ratio;
		this.label.text = string.Format("{0}:{1}", this.aspect, this.ratio);
	}

	// Token: 0x060034F0 RID: 13552 RVA: 0x001455A8 File Offset: 0x001437A8
	public void Awake()
	{
		this.Populate();
	}

	// Token: 0x060034F1 RID: 13553 RVA: 0x001455A8 File Offset: 0x001437A8
	public void Update()
	{
		this.Populate();
	}

	// Token: 0x04002B7E RID: 11134
	public AspectRatioFitter aspectRatioFitter;

	// Token: 0x04002B7F RID: 11135
	public RustText label;

	// Token: 0x04002B80 RID: 11136
	public float aspect;

	// Token: 0x04002B81 RID: 11137
	public float ratio;
}
