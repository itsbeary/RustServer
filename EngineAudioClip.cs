using System;
using System.Collections.Generic;
using JSON;
using UnityEngine;

// Token: 0x0200022C RID: 556
public class EngineAudioClip : MonoBehaviour, IClientComponent
{
	// Token: 0x06001C0E RID: 7182 RVA: 0x000C4A97 File Offset: 0x000C2C97
	private int GetBucketRPM(int RPM)
	{
		return Mathf.RoundToInt((float)(RPM / 25)) * 25;
	}

	// Token: 0x04001408 RID: 5128
	public AudioClip granularClip;

	// Token: 0x04001409 RID: 5129
	public AudioClip accelerationClip;

	// Token: 0x0400140A RID: 5130
	public TextAsset accelerationCyclesJson;

	// Token: 0x0400140B RID: 5131
	public List<EngineAudioClip.EngineCycle> accelerationCycles = new List<EngineAudioClip.EngineCycle>();

	// Token: 0x0400140C RID: 5132
	public List<EngineAudioClip.EngineCycleBucket> cycleBuckets = new List<EngineAudioClip.EngineCycleBucket>();

	// Token: 0x0400140D RID: 5133
	public Dictionary<int, EngineAudioClip.EngineCycleBucket> accelerationCyclesByRPM = new Dictionary<int, EngineAudioClip.EngineCycleBucket>();

	// Token: 0x0400140E RID: 5134
	public Dictionary<int, int> rpmBucketLookup = new Dictionary<int, int>();

	// Token: 0x0400140F RID: 5135
	public int sampleRate = 44100;

	// Token: 0x04001410 RID: 5136
	public int samplesUntilNextGrain;

	// Token: 0x04001411 RID: 5137
	public int lastCycleId;

	// Token: 0x04001412 RID: 5138
	public List<EngineAudioClip.Grain> grains = new List<EngineAudioClip.Grain>();

	// Token: 0x04001413 RID: 5139
	public int currentRPM;

	// Token: 0x04001414 RID: 5140
	public int targetRPM = 1500;

	// Token: 0x04001415 RID: 5141
	public int minRPM;

	// Token: 0x04001416 RID: 5142
	public int maxRPM;

	// Token: 0x04001417 RID: 5143
	public int cyclePadding;

	// Token: 0x04001418 RID: 5144
	[Range(0f, 1f)]
	public float RPMControl;

	// Token: 0x04001419 RID: 5145
	public AudioSource source;

	// Token: 0x0400141A RID: 5146
	public float rpmLerpSpeed = 0.025f;

	// Token: 0x0400141B RID: 5147
	public float rpmLerpSpeedDown = 0.01f;

	// Token: 0x02000C8B RID: 3211
	[Serializable]
	public class EngineCycle
	{
		// Token: 0x06004F48 RID: 20296 RVA: 0x001A6228 File Offset: 0x001A4428
		public EngineCycle(int RPM, int startSample, int endSample, float period, int id)
		{
			this.RPM = RPM;
			this.startSample = startSample;
			this.endSample = endSample;
			this.period = period;
			this.id = id;
		}

		// Token: 0x04004416 RID: 17430
		public int RPM;

		// Token: 0x04004417 RID: 17431
		public int startSample;

		// Token: 0x04004418 RID: 17432
		public int endSample;

		// Token: 0x04004419 RID: 17433
		public float period;

		// Token: 0x0400441A RID: 17434
		public int id;
	}

	// Token: 0x02000C8C RID: 3212
	public class EngineCycleBucket
	{
		// Token: 0x06004F49 RID: 20297 RVA: 0x001A6255 File Offset: 0x001A4455
		public EngineCycleBucket(int RPM)
		{
			this.RPM = RPM;
		}

		// Token: 0x06004F4A RID: 20298 RVA: 0x001A627C File Offset: 0x001A447C
		public EngineAudioClip.EngineCycle GetCycle(System.Random random, int lastCycleId)
		{
			if (this.remainingCycles.Count == 0)
			{
				this.ResetRemainingCycles(random);
			}
			int num = this.remainingCycles.Pop<int>();
			if (this.cycles[num].id == lastCycleId)
			{
				if (this.remainingCycles.Count == 0)
				{
					this.ResetRemainingCycles(random);
				}
				num = this.remainingCycles.Pop<int>();
			}
			return this.cycles[num];
		}

		// Token: 0x06004F4B RID: 20299 RVA: 0x001A62EC File Offset: 0x001A44EC
		private void ResetRemainingCycles(System.Random random)
		{
			for (int i = 0; i < this.cycles.Count; i++)
			{
				this.remainingCycles.Add(i);
			}
			this.remainingCycles.Shuffle((uint)random.Next());
		}

		// Token: 0x06004F4C RID: 20300 RVA: 0x001A632C File Offset: 0x001A452C
		public void Add(EngineAudioClip.EngineCycle cycle)
		{
			if (!this.cycles.Contains(cycle))
			{
				this.cycles.Add(cycle);
			}
		}

		// Token: 0x0400441B RID: 17435
		public int RPM;

		// Token: 0x0400441C RID: 17436
		public List<EngineAudioClip.EngineCycle> cycles = new List<EngineAudioClip.EngineCycle>();

		// Token: 0x0400441D RID: 17437
		public List<int> remainingCycles = new List<int>();
	}

	// Token: 0x02000C8D RID: 3213
	public class Grain
	{
		// Token: 0x17000697 RID: 1687
		// (get) Token: 0x06004F4D RID: 20301 RVA: 0x001A6348 File Offset: 0x001A4548
		public bool finished
		{
			get
			{
				return this.currentSample >= this.endSample;
			}
		}

		// Token: 0x06004F4E RID: 20302 RVA: 0x001A635C File Offset: 0x001A455C
		public void Init(float[] source, EngineAudioClip.EngineCycle cycle, int cyclePadding)
		{
			this.sourceData = source;
			this.startSample = cycle.startSample - cyclePadding;
			this.currentSample = this.startSample;
			this.attackTimeSamples = cyclePadding;
			this.sustainTimeSamples = cycle.endSample - cycle.startSample;
			this.releaseTimeSamples = cyclePadding;
			this.gainPerSampleAttack = 1f / (float)this.attackTimeSamples;
			this.gainPerSampleRelease = -1f / (float)this.releaseTimeSamples;
			this.attackEndSample = this.startSample + this.attackTimeSamples;
			this.releaseStartSample = this.attackEndSample + this.sustainTimeSamples;
			this.endSample = this.releaseStartSample + this.releaseTimeSamples;
			this.gain = 0f;
		}

		// Token: 0x06004F4F RID: 20303 RVA: 0x001A6418 File Offset: 0x001A4618
		public float GetSample()
		{
			if (this.currentSample >= this.sourceData.Length)
			{
				return 0f;
			}
			float num = this.sourceData[this.currentSample];
			if (this.currentSample <= this.attackEndSample)
			{
				this.gain += this.gainPerSampleAttack;
				if (this.gain > 0.8f)
				{
					this.gain = 0.8f;
				}
			}
			else if (this.currentSample >= this.releaseStartSample)
			{
				this.gain += this.gainPerSampleRelease;
				if (this.gain < 0f)
				{
					this.gain = 0f;
				}
			}
			this.currentSample++;
			return num * this.gain;
		}

		// Token: 0x0400441E RID: 17438
		private float[] sourceData;

		// Token: 0x0400441F RID: 17439
		private int startSample;

		// Token: 0x04004420 RID: 17440
		private int currentSample;

		// Token: 0x04004421 RID: 17441
		private int attackTimeSamples;

		// Token: 0x04004422 RID: 17442
		private int sustainTimeSamples;

		// Token: 0x04004423 RID: 17443
		private int releaseTimeSamples;

		// Token: 0x04004424 RID: 17444
		private float gain;

		// Token: 0x04004425 RID: 17445
		private float gainPerSampleAttack;

		// Token: 0x04004426 RID: 17446
		private float gainPerSampleRelease;

		// Token: 0x04004427 RID: 17447
		private int attackEndSample;

		// Token: 0x04004428 RID: 17448
		private int releaseStartSample;

		// Token: 0x04004429 RID: 17449
		private int endSample;
	}
}
