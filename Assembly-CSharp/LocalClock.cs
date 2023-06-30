using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000946 RID: 2374
public class LocalClock
{
	// Token: 0x060038B6 RID: 14518 RVA: 0x00151804 File Offset: 0x0014FA04
	public void Add(float delta, float variance, Action action)
	{
		LocalClock.TimedEvent timedEvent = default(LocalClock.TimedEvent);
		timedEvent.time = Time.time + delta + UnityEngine.Random.Range(-variance, variance);
		timedEvent.delta = delta;
		timedEvent.variance = variance;
		timedEvent.action = action;
		this.events.Add(timedEvent);
	}

	// Token: 0x060038B7 RID: 14519 RVA: 0x00151854 File Offset: 0x0014FA54
	public void Tick()
	{
		for (int i = 0; i < this.events.Count; i++)
		{
			LocalClock.TimedEvent timedEvent = this.events[i];
			if (Time.time > timedEvent.time)
			{
				float delta = timedEvent.delta;
				float variance = timedEvent.variance;
				timedEvent.action();
				timedEvent.time = Time.time + delta + UnityEngine.Random.Range(-variance, variance);
				this.events[i] = timedEvent;
			}
		}
	}

	// Token: 0x040033B0 RID: 13232
	public List<LocalClock.TimedEvent> events = new List<LocalClock.TimedEvent>();

	// Token: 0x02000ED0 RID: 3792
	public struct TimedEvent
	{
		// Token: 0x04004D6E RID: 19822
		public float time;

		// Token: 0x04004D6F RID: 19823
		public float delta;

		// Token: 0x04004D70 RID: 19824
		public float variance;

		// Token: 0x04004D71 RID: 19825
		public Action action;
	}
}
