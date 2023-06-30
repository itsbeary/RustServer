using System;
using UnityEngine.Events;

// Token: 0x020008CF RID: 2255
public class UIEscapeCapture : ListComponent<UIEscapeCapture>
{
	// Token: 0x0600375A RID: 14170 RVA: 0x0014C4C8 File Offset: 0x0014A6C8
	public static bool EscapePressed()
	{
		using (ListHashSet<UIEscapeCapture>.Enumerator enumerator = ListComponent<UIEscapeCapture>.InstanceList.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				enumerator.Current.onEscape.Invoke();
				return true;
			}
		}
		return false;
	}

	// Token: 0x040032CB RID: 13003
	public UnityEvent onEscape = new UnityEvent();
}
