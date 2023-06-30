using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Token: 0x020008E9 RID: 2281
public class RightClickReceiver : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x0600379C RID: 14236 RVA: 0x0014CDAE File Offset: 0x0014AFAE
	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			UnityEvent clickReceiver = this.ClickReceiver;
			if (clickReceiver == null)
			{
				return;
			}
			clickReceiver.Invoke();
		}
	}

	// Token: 0x040032FA RID: 13050
	public UnityEvent ClickReceiver;
}
