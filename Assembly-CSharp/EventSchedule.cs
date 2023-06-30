using System;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x020004EA RID: 1258
public class EventSchedule : BaseMonoBehaviour
{
	// Token: 0x060028CB RID: 10443 RVA: 0x000FBF6C File Offset: 0x000FA16C
	private void OnEnable()
	{
		this.hoursRemaining = UnityEngine.Random.Range(this.minimumHoursBetween, this.maxmumHoursBetween);
		base.InvokeRepeating(new Action(this.RunSchedule), 1f, 1f);
	}

	// Token: 0x060028CC RID: 10444 RVA: 0x000FBFA2 File Offset: 0x000FA1A2
	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		base.CancelInvoke(new Action(this.RunSchedule));
	}

	// Token: 0x060028CD RID: 10445 RVA: 0x000FBFBF File Offset: 0x000FA1BF
	public virtual void RunSchedule()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (!ConVar.Server.events)
		{
			return;
		}
		this.CountHours();
		if (this.hoursRemaining > 0f)
		{
			return;
		}
		this.Trigger();
	}

	// Token: 0x060028CE RID: 10446 RVA: 0x000FBFEC File Offset: 0x000FA1EC
	private void Trigger()
	{
		this.hoursRemaining = UnityEngine.Random.Range(this.minimumHoursBetween, this.maxmumHoursBetween);
		TriggeredEvent[] components = base.GetComponents<TriggeredEvent>();
		if (components.Length == 0)
		{
			return;
		}
		TriggeredEvent triggeredEvent = components[UnityEngine.Random.Range(0, components.Length)];
		if (triggeredEvent == null)
		{
			return;
		}
		triggeredEvent.SendMessage("RunEvent", SendMessageOptions.DontRequireReceiver);
	}

	// Token: 0x060028CF RID: 10447 RVA: 0x000FC040 File Offset: 0x000FA240
	private void CountHours()
	{
		if (!TOD_Sky.Instance)
		{
			return;
		}
		if (this.lastRun != 0L)
		{
			TimeSpan timeSpan = TOD_Sky.Instance.Cycle.DateTime.Subtract(DateTime.FromBinary(this.lastRun));
			this.hoursRemaining -= (float)timeSpan.TotalSeconds / 60f / 60f;
		}
		this.lastRun = TOD_Sky.Instance.Cycle.DateTime.ToBinary();
	}

	// Token: 0x0400210F RID: 8463
	[Tooltip("The minimum amount of hours between events")]
	public float minimumHoursBetween = 12f;

	// Token: 0x04002110 RID: 8464
	[Tooltip("The maximum amount of hours between events")]
	public float maxmumHoursBetween = 24f;

	// Token: 0x04002111 RID: 8465
	private float hoursRemaining;

	// Token: 0x04002112 RID: 8466
	private long lastRun;
}
