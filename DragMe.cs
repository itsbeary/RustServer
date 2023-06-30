using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020008E2 RID: 2274
public class DragMe : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
{
	// Token: 0x06003786 RID: 14214 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnBeginDrag(PointerEventData eventData)
	{
	}

	// Token: 0x17000464 RID: 1124
	// (get) Token: 0x06003787 RID: 14215 RVA: 0x0014CC76 File Offset: 0x0014AE76
	protected virtual Canvas TopCanvas
	{
		get
		{
			return UIRootScaled.DragOverlayCanvas;
		}
	}

	// Token: 0x06003788 RID: 14216 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnDrag(PointerEventData eventData)
	{
	}

	// Token: 0x06003789 RID: 14217 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnEndDrag(PointerEventData eventData)
	{
	}

	// Token: 0x0600378A RID: 14218 RVA: 0x0014CC7D File Offset: 0x0014AE7D
	public void CancelDrag()
	{
		this.OnEndDrag(new PointerEventData(EventSystem.current));
	}

	// Token: 0x040032EF RID: 13039
	public static DragMe dragging;

	// Token: 0x040032F0 RID: 13040
	public static GameObject dragIcon;

	// Token: 0x040032F1 RID: 13041
	public static object data;

	// Token: 0x040032F2 RID: 13042
	[NonSerialized]
	public string dragType = "generic";
}
