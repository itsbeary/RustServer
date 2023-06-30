using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020008E5 RID: 2277
public class DropMe : MonoBehaviour, IDropHandler, IEventSystemHandler
{
	// Token: 0x0600378D RID: 14221 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnDrop(PointerEventData eventData)
	{
	}

	// Token: 0x040032F5 RID: 13045
	public string[] droppableTypes;
}
