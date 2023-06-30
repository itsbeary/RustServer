using System;
using UnityEngine;

// Token: 0x0200091D RID: 2333
public class StartOfFrameHook : MonoBehaviour
{
	// Token: 0x0600382B RID: 14379 RVA: 0x0014E4AC File Offset: 0x0014C6AC
	private void OnEnable()
	{
		Action onStartOfFrame = StartOfFrameHook.OnStartOfFrame;
		if (onStartOfFrame == null)
		{
			return;
		}
		onStartOfFrame();
	}

	// Token: 0x0600382C RID: 14380 RVA: 0x0014E4BD File Offset: 0x0014C6BD
	private void Update()
	{
		base.gameObject.SetActive(false);
		base.gameObject.SetActive(true);
	}

	// Token: 0x04003396 RID: 13206
	public static Action OnStartOfFrame;
}
