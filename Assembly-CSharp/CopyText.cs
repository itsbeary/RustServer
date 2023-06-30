using System;
using Rust.UI;
using UnityEngine;

// Token: 0x020007A5 RID: 1957
public class CopyText : MonoBehaviour
{
	// Token: 0x06003502 RID: 13570 RVA: 0x00145733 File Offset: 0x00143933
	public void TriggerCopy()
	{
		if (this.TargetText != null)
		{
			GUIUtility.systemCopyBuffer = this.TargetText.text;
		}
	}

	// Token: 0x04002BAF RID: 11183
	public RustText TargetText;
}
