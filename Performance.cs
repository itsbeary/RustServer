using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using Rust.Workshop;
using UnityEngine;

// Token: 0x0200032E RID: 814
public class Performance : SingletonComponent<global::Performance>
{
	// Token: 0x06001F2F RID: 7983 RVA: 0x000D3D64 File Offset: 0x000D1F64
	private void Update()
	{
		global::Performance.frameTimes[Time.frameCount % 1000] = (int)(1000f * Time.deltaTime);
		using (TimeWarning.New("FPSTimer", 0))
		{
			this.FPSTimer();
		}
	}

	// Token: 0x06001F30 RID: 7984 RVA: 0x000D3DBC File Offset: 0x000D1FBC
	public List<int> GetFrameTimes(int requestedStart, int maxCount, out int startIndex)
	{
		startIndex = Math.Max(requestedStart, Math.Max(Time.frameCount - 1000 - 1, 0));
		int num = Math.Min(Math.Min(1000, maxCount), Time.frameCount);
		List<int> list = Pool.GetList<int>();
		for (int i = 0; i < num; i++)
		{
			int num2 = (startIndex + i) % 1000;
			list.Add(global::Performance.frameTimes[num2]);
		}
		return list;
	}

	// Token: 0x06001F31 RID: 7985 RVA: 0x000D3E28 File Offset: 0x000D2028
	private void FPSTimer()
	{
		this.frames++;
		this.time += Time.unscaledDeltaTime;
		if (this.time < 1f)
		{
			return;
		}
		long memoryCollections = global::Performance.current.memoryCollections;
		global::Performance.current.frameID = Time.frameCount;
		global::Performance.current.frameRate = this.frames;
		global::Performance.current.frameTime = this.time / (float)this.frames * 1000f;
		checked
		{
			global::Performance.frameRateHistory[(int)((IntPtr)(global::Performance.cycles % unchecked((long)global::Performance.frameRateHistory.Length)))] = global::Performance.current.frameRate;
			global::Performance.frameTimeHistory[(int)((IntPtr)(global::Performance.cycles % unchecked((long)global::Performance.frameTimeHistory.Length)))] = global::Performance.current.frameTime;
			global::Performance.current.frameRateAverage = this.AverageFrameRate();
			global::Performance.current.frameTimeAverage = this.AverageFrameTime();
		}
		global::Performance.current.memoryUsageSystem = (long)SystemInfoEx.systemMemoryUsed;
		global::Performance.current.memoryAllocations = Rust.GC.GetTotalMemory();
		global::Performance.current.memoryCollections = (long)Rust.GC.CollectionCount();
		global::Performance.current.loadBalancerTasks = (long)LoadBalancer.Count();
		global::Performance.current.invokeHandlerTasks = (long)InvokeHandler.Count();
		global::Performance.current.workshopSkinsQueued = (long)Rust.Workshop.WorkshopSkin.QueuedCount;
		global::Performance.current.gcTriggered = memoryCollections != global::Performance.current.memoryCollections;
		this.frames = 0;
		this.time = 0f;
		global::Performance.cycles += 1L;
		global::Performance.report = global::Performance.current;
	}

	// Token: 0x06001F32 RID: 7986 RVA: 0x000D3FAC File Offset: 0x000D21AC
	private float AverageFrameRate()
	{
		float num = 0f;
		int num2 = Math.Min(global::Performance.frameRateHistory.Length, (int)global::Performance.cycles);
		for (int i = 0; i < num2; i++)
		{
			num += (float)global::Performance.frameRateHistory[i];
		}
		return num / (float)num2;
	}

	// Token: 0x06001F33 RID: 7987 RVA: 0x000D3FF0 File Offset: 0x000D21F0
	private float AverageFrameTime()
	{
		float num = 0f;
		int num2 = Math.Min(global::Performance.frameTimeHistory.Length, (int)global::Performance.cycles);
		for (int i = 0; i < global::Performance.frameTimeHistory.Length; i++)
		{
			num += global::Performance.frameTimeHistory[i];
		}
		return num / (float)num2;
	}

	// Token: 0x0400181B RID: 6171
	public static global::Performance.Tick current;

	// Token: 0x0400181C RID: 6172
	public static global::Performance.Tick report;

	// Token: 0x0400181D RID: 6173
	public const int FrameHistoryCount = 1000;

	// Token: 0x0400181E RID: 6174
	private const int HistoryLength = 60;

	// Token: 0x0400181F RID: 6175
	private static long cycles = 0L;

	// Token: 0x04001820 RID: 6176
	private static int[] frameRateHistory = new int[60];

	// Token: 0x04001821 RID: 6177
	private static float[] frameTimeHistory = new float[60];

	// Token: 0x04001822 RID: 6178
	private static int[] frameTimes = new int[1000];

	// Token: 0x04001823 RID: 6179
	private int frames;

	// Token: 0x04001824 RID: 6180
	private float time;

	// Token: 0x02000CC3 RID: 3267
	public struct Tick
	{
		// Token: 0x0400453E RID: 17726
		public int frameID;

		// Token: 0x0400453F RID: 17727
		public int frameRate;

		// Token: 0x04004540 RID: 17728
		public float frameTime;

		// Token: 0x04004541 RID: 17729
		public float frameRateAverage;

		// Token: 0x04004542 RID: 17730
		public float frameTimeAverage;

		// Token: 0x04004543 RID: 17731
		public long memoryUsageSystem;

		// Token: 0x04004544 RID: 17732
		public long memoryAllocations;

		// Token: 0x04004545 RID: 17733
		public long memoryCollections;

		// Token: 0x04004546 RID: 17734
		public long loadBalancerTasks;

		// Token: 0x04004547 RID: 17735
		public long invokeHandlerTasks;

		// Token: 0x04004548 RID: 17736
		public long workshopSkinsQueued;

		// Token: 0x04004549 RID: 17737
		public int ping;

		// Token: 0x0400454A RID: 17738
		public bool gcTriggered;

		// Token: 0x0400454B RID: 17739
		public PerformanceSamplePoint performanceSample;
	}

	// Token: 0x02000CC4 RID: 3268
	private struct LagSpike
	{
		// Token: 0x0400454C RID: 17740
		public int Index;

		// Token: 0x0400454D RID: 17741
		public int Time;
	}
}
