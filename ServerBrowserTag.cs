using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000896 RID: 2198
public class ServerBrowserTag : MonoBehaviour
{
	// Token: 0x17000455 RID: 1109
	// (get) Token: 0x060036CF RID: 14031 RVA: 0x00149EE7 File Offset: 0x001480E7
	public bool IsActive
	{
		get
		{
			return this.button != null && this.button.IsPressed;
		}
	}

	// Token: 0x04003197 RID: 12695
	public string serverTag;

	// Token: 0x04003198 RID: 12696
	public RustButton button;
}
