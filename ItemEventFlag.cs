using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020005CD RID: 1485
public class ItemEventFlag : MonoBehaviour, IItemUpdate
{
	// Token: 0x06002C98 RID: 11416 RVA: 0x0010E7C4 File Offset: 0x0010C9C4
	public virtual void OnItemUpdate(Item item)
	{
		bool flag = item.HasFlag(this.flag);
		if (!this.firstRun && flag == this.lastState)
		{
			return;
		}
		if (flag)
		{
			this.onEnabled.Invoke();
		}
		else
		{
			this.onDisable.Invoke();
		}
		this.lastState = flag;
		this.firstRun = false;
	}

	// Token: 0x04002480 RID: 9344
	public Item.Flag flag;

	// Token: 0x04002481 RID: 9345
	public UnityEvent onEnabled = new UnityEvent();

	// Token: 0x04002482 RID: 9346
	public UnityEvent onDisable = new UnityEvent();

	// Token: 0x04002483 RID: 9347
	internal bool firstRun = true;

	// Token: 0x04002484 RID: 9348
	internal bool lastState;
}
