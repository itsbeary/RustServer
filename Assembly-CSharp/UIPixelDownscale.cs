using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008D4 RID: 2260
public class UIPixelDownscale : MonoBehaviour
{
	// Token: 0x06003769 RID: 14185 RVA: 0x0014C96C File Offset: 0x0014AB6C
	private void Awake()
	{
		if (this.CanvasScaler == null)
		{
			this.CanvasScaler = base.GetComponent<CanvasScaler>();
			if (this.CanvasScaler == null)
			{
				Debug.LogError(base.GetType().Name + " is attached to a gameobject that is missing a canvas scaler");
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	// Token: 0x0600376A RID: 14186 RVA: 0x0014C9C8 File Offset: 0x0014ABC8
	private void Update()
	{
		if ((float)Screen.width < this.CanvasScaler.referenceResolution.x || (float)Screen.height < this.CanvasScaler.referenceResolution.y)
		{
			this.CanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
			return;
		}
		this.CanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
	}

	// Token: 0x040032DA RID: 13018
	public CanvasScaler CanvasScaler;
}
