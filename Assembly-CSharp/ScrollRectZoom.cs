using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020008BB RID: 2235
public class ScrollRectZoom : MonoBehaviour, IScrollHandler, IEventSystemHandler
{
	// Token: 0x1700045C RID: 1116
	// (get) Token: 0x06003720 RID: 14112 RVA: 0x0014BAFB File Offset: 0x00149CFB
	public RectTransform rectTransform
	{
		get
		{
			return this.scrollRect.transform as RectTransform;
		}
	}

	// Token: 0x06003721 RID: 14113 RVA: 0x0014BB0D File Offset: 0x00149D0D
	private void OnEnable()
	{
		this.SetZoom(this.zoom, true);
	}

	// Token: 0x06003722 RID: 14114 RVA: 0x0014BB1C File Offset: 0x00149D1C
	public void OnScroll(PointerEventData data)
	{
		if (this.mouseWheelZoom)
		{
			this.SetZoom(this.zoom + this.scrollAmount * data.scrollDelta.y, true);
		}
	}

	// Token: 0x06003723 RID: 14115 RVA: 0x0014BB48 File Offset: 0x00149D48
	public void SetZoom(float z, bool expZoom = true)
	{
		z = Mathf.Clamp(z, this.min, this.max);
		this.zoom = z;
		Vector2 normalizedPosition = this.scrollRect.normalizedPosition;
		if (expZoom)
		{
			this.scrollRect.content.localScale = Vector3.one * Mathf.Exp(this.zoom);
		}
		else
		{
			this.scrollRect.content.localScale = Vector3.one * this.zoom;
		}
		this.scrollRect.normalizedPosition = normalizedPosition;
	}

	// Token: 0x0400325A RID: 12890
	public ScrollRectEx scrollRect;

	// Token: 0x0400325B RID: 12891
	public float zoom = 1f;

	// Token: 0x0400325C RID: 12892
	public float max = 1.5f;

	// Token: 0x0400325D RID: 12893
	public float min = 0.5f;

	// Token: 0x0400325E RID: 12894
	public bool mouseWheelZoom = true;

	// Token: 0x0400325F RID: 12895
	public float scrollAmount = 0.2f;
}
