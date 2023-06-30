using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Token: 0x02000822 RID: 2082
public class DragReceiver : MonoBehaviour
{
	// Token: 0x04002EF6 RID: 12022
	public DragReceiver.TriggerEvent onEndDrag;

	// Token: 0x02000E97 RID: 3735
	[Serializable]
	public class TriggerEvent : UnityEvent<BaseEventData>
	{
	}
}
