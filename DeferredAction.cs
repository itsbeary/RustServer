using System;
using UnityEngine;

// Token: 0x02000908 RID: 2312
public class DeferredAction
{
	// Token: 0x1700046D RID: 1133
	// (get) Token: 0x060037E8 RID: 14312 RVA: 0x0014DBEE File Offset: 0x0014BDEE
	// (set) Token: 0x060037E9 RID: 14313 RVA: 0x0014DBF6 File Offset: 0x0014BDF6
	public bool Idle { get; private set; }

	// Token: 0x1700046E RID: 1134
	// (get) Token: 0x060037EA RID: 14314 RVA: 0x0014DBFF File Offset: 0x0014BDFF
	public int Index
	{
		get
		{
			return (int)this.priority;
		}
	}

	// Token: 0x060037EB RID: 14315 RVA: 0x0014DC07 File Offset: 0x0014BE07
	public DeferredAction(UnityEngine.Object sender, Action action, ActionPriority priority = ActionPriority.Medium)
	{
		this.sender = sender;
		this.action = action;
		this.priority = priority;
		this.Idle = true;
	}

	// Token: 0x060037EC RID: 14316 RVA: 0x0014DC32 File Offset: 0x0014BE32
	public void Action()
	{
		if (this.Idle)
		{
			throw new Exception("Double invocation of a deferred action.");
		}
		this.Idle = true;
		if (this.sender)
		{
			this.action();
		}
	}

	// Token: 0x060037ED RID: 14317 RVA: 0x0014DC66 File Offset: 0x0014BE66
	public void Invoke()
	{
		if (!this.Idle)
		{
			throw new Exception("Double invocation of a deferred action.");
		}
		LoadBalancer.Enqueue(this);
		this.Idle = false;
	}

	// Token: 0x060037EE RID: 14318 RVA: 0x0014DC88 File Offset: 0x0014BE88
	public static implicit operator bool(DeferredAction obj)
	{
		return obj != null;
	}

	// Token: 0x060037EF RID: 14319 RVA: 0x0014DC8E File Offset: 0x0014BE8E
	public static void Invoke(UnityEngine.Object sender, Action action, ActionPriority priority = ActionPriority.Medium)
	{
		new DeferredAction(sender, action, priority).Invoke();
	}

	// Token: 0x0400334A RID: 13130
	private UnityEngine.Object sender;

	// Token: 0x0400334B RID: 13131
	private Action action;

	// Token: 0x0400334C RID: 13132
	private ActionPriority priority = ActionPriority.Medium;
}
