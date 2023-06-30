using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000954 RID: 2388
public class SynchronizedClock
{
	// Token: 0x1700048A RID: 1162
	// (get) Token: 0x06003941 RID: 14657 RVA: 0x000E635B File Offset: 0x000E455B
	private static float CurrentTime
	{
		get
		{
			return Time.realtimeSinceStartup;
		}
	}

	// Token: 0x06003942 RID: 14658 RVA: 0x00153D7C File Offset: 0x00151F7C
	public void Add(float delta, float variance, Action<uint> action)
	{
		SynchronizedClock.TimedEvent timedEvent = default(SynchronizedClock.TimedEvent);
		timedEvent.time = SynchronizedClock.CurrentTime;
		timedEvent.delta = delta;
		timedEvent.variance = variance;
		timedEvent.action = action;
		this.events.Add(timedEvent);
	}

	// Token: 0x06003943 RID: 14659 RVA: 0x00153DC4 File Offset: 0x00151FC4
	public void Tick()
	{
		for (int i = 0; i < this.events.Count; i++)
		{
			SynchronizedClock.TimedEvent timedEvent = this.events[i];
			float time = timedEvent.time;
			float currentTime = SynchronizedClock.CurrentTime;
			float delta = timedEvent.delta;
			float num = time - time % delta;
			uint num2 = (uint)(time / delta);
			SeedRandom.Wanghash(ref num2);
			SeedRandom.Wanghash(ref num2);
			SeedRandom.Wanghash(ref num2);
			float num3 = SeedRandom.Range(ref num2, -timedEvent.variance, timedEvent.variance);
			float num4 = num + delta + num3;
			if (time < num4 && currentTime >= num4)
			{
				timedEvent.action(num2);
				timedEvent.time = currentTime;
			}
			else if (currentTime > time || currentTime < num - 5f)
			{
				timedEvent.time = currentTime;
			}
			this.events[i] = timedEvent;
		}
	}

	// Token: 0x040033DD RID: 13277
	public List<SynchronizedClock.TimedEvent> events = new List<SynchronizedClock.TimedEvent>();

	// Token: 0x02000ED3 RID: 3795
	public struct TimedEvent
	{
		// Token: 0x04004D79 RID: 19833
		public float time;

		// Token: 0x04004D7A RID: 19834
		public float delta;

		// Token: 0x04004D7B RID: 19835
		public float variance;

		// Token: 0x04004D7C RID: 19836
		public Action<uint> action;
	}
}
