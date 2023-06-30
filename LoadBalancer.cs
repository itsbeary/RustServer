using System;
using System.Collections.Generic;
using System.Diagnostics;
using Rust;
using UnityEngine;

// Token: 0x02000906 RID: 2310
public class LoadBalancer : SingletonComponent<LoadBalancer>
{
	// Token: 0x060037E1 RID: 14305 RVA: 0x0014DA08 File Offset: 0x0014BC08
	protected void LateUpdate()
	{
		if (Rust.Application.isReceiving)
		{
			return;
		}
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (LoadBalancer.Paused)
		{
			return;
		}
		int num = LoadBalancer.Count();
		float num2 = Mathf.InverseLerp(1000f, 100000f, (float)num);
		float num3 = Mathf.SmoothStep(1f, 100f, num2);
		this.watch.Reset();
		this.watch.Start();
		for (int i = 0; i < this.queues.Length; i++)
		{
			Queue<DeferredAction> queue = this.queues[i];
			while (queue.Count > 0)
			{
				queue.Dequeue().Action();
				if (this.watch.Elapsed.TotalMilliseconds > (double)num3)
				{
					return;
				}
			}
		}
	}

	// Token: 0x060037E2 RID: 14306 RVA: 0x0014DABC File Offset: 0x0014BCBC
	public static int Count()
	{
		if (!SingletonComponent<LoadBalancer>.Instance)
		{
			return 0;
		}
		Queue<DeferredAction>[] array = SingletonComponent<LoadBalancer>.Instance.queues;
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			num += array[i].Count;
		}
		return num;
	}

	// Token: 0x060037E3 RID: 14307 RVA: 0x0014DB00 File Offset: 0x0014BD00
	public static void ProcessAll()
	{
		if (!SingletonComponent<LoadBalancer>.Instance)
		{
			LoadBalancer.CreateInstance();
		}
		foreach (Queue<DeferredAction> queue in SingletonComponent<LoadBalancer>.Instance.queues)
		{
			while (queue.Count > 0)
			{
				queue.Dequeue().Action();
			}
		}
	}

	// Token: 0x060037E4 RID: 14308 RVA: 0x0014DB51 File Offset: 0x0014BD51
	public static void Enqueue(DeferredAction action)
	{
		if (!SingletonComponent<LoadBalancer>.Instance)
		{
			LoadBalancer.CreateInstance();
		}
		SingletonComponent<LoadBalancer>.Instance.queues[action.Index].Enqueue(action);
	}

	// Token: 0x060037E5 RID: 14309 RVA: 0x0014DB7B File Offset: 0x0014BD7B
	private static void CreateInstance()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "LoadBalancer";
		gameObject.AddComponent<LoadBalancer>();
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
	}

	// Token: 0x0400333D RID: 13117
	public static bool Paused;

	// Token: 0x0400333E RID: 13118
	private const float MinMilliseconds = 1f;

	// Token: 0x0400333F RID: 13119
	private const float MaxMilliseconds = 100f;

	// Token: 0x04003340 RID: 13120
	private const int MinBacklog = 1000;

	// Token: 0x04003341 RID: 13121
	private const int MaxBacklog = 100000;

	// Token: 0x04003342 RID: 13122
	private Queue<DeferredAction>[] queues = new Queue<DeferredAction>[]
	{
		new Queue<DeferredAction>(),
		new Queue<DeferredAction>(),
		new Queue<DeferredAction>(),
		new Queue<DeferredAction>(),
		new Queue<DeferredAction>()
	};

	// Token: 0x04003343 RID: 13123
	private Stopwatch watch = Stopwatch.StartNew();
}
