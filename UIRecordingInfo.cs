using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000810 RID: 2064
public class UIRecordingInfo : SingletonComponent<UIRecordingInfo>
{
	// Token: 0x060035C3 RID: 13763 RVA: 0x0014551A File Offset: 0x0014371A
	private void Start()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04002E8C RID: 11916
	public RustText CountdownText;

	// Token: 0x04002E8D RID: 11917
	public Slider TapeProgressSlider;

	// Token: 0x04002E8E RID: 11918
	public GameObject CountdownRoot;

	// Token: 0x04002E8F RID: 11919
	public GameObject RecordingRoot;

	// Token: 0x04002E90 RID: 11920
	public Transform Spinner;

	// Token: 0x04002E91 RID: 11921
	public float SpinSpeed = 180f;

	// Token: 0x04002E92 RID: 11922
	public Image CassetteImage;
}
