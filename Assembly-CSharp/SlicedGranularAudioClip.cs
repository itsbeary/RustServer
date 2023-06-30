using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200023E RID: 574
public class SlicedGranularAudioClip : MonoBehaviour, IClientComponent
{
	// Token: 0x04001497 RID: 5271
	public AudioClip sourceClip;

	// Token: 0x04001498 RID: 5272
	public AudioClip granularClip;

	// Token: 0x04001499 RID: 5273
	public int sampleRate = 44100;

	// Token: 0x0400149A RID: 5274
	public float grainAttack = 0.1f;

	// Token: 0x0400149B RID: 5275
	public float grainSustain = 0.1f;

	// Token: 0x0400149C RID: 5276
	public float grainRelease = 0.1f;

	// Token: 0x0400149D RID: 5277
	public float grainFrequency = 0.1f;

	// Token: 0x0400149E RID: 5278
	public int grainAttackSamples;

	// Token: 0x0400149F RID: 5279
	public int grainSustainSamples;

	// Token: 0x040014A0 RID: 5280
	public int grainReleaseSamples;

	// Token: 0x040014A1 RID: 5281
	public int grainFrequencySamples;

	// Token: 0x040014A2 RID: 5282
	public int samplesUntilNextGrain;

	// Token: 0x040014A3 RID: 5283
	public List<SlicedGranularAudioClip.Grain> grains = new List<SlicedGranularAudioClip.Grain>();

	// Token: 0x040014A4 RID: 5284
	public List<int> startPositions = new List<int>();

	// Token: 0x040014A5 RID: 5285
	public int lastStartPositionIdx = int.MaxValue;

	// Token: 0x02000C96 RID: 3222
	public class Grain
	{
		// Token: 0x1700069B RID: 1691
		// (get) Token: 0x06004F5F RID: 20319 RVA: 0x001A6792 File Offset: 0x001A4992
		public bool finished
		{
			get
			{
				return this.currentSample >= this.endSample;
			}
		}

		// Token: 0x06004F60 RID: 20320 RVA: 0x001A67A8 File Offset: 0x001A49A8
		public void Init(float[] source, int start, int attack, int sustain, int release)
		{
			this.sourceData = source;
			this.startSample = start;
			this.currentSample = start;
			this.attackTimeSamples = attack;
			this.sustainTimeSamples = sustain;
			this.releaseTimeSamples = release;
			this.gainPerSampleAttack = 0.5f / (float)this.attackTimeSamples;
			this.gainPerSampleRelease = -0.5f / (float)this.releaseTimeSamples;
			this.attackEndSample = this.startSample + this.attackTimeSamples;
			this.releaseStartSample = this.attackEndSample + this.sustainTimeSamples;
			this.endSample = this.releaseStartSample + this.releaseTimeSamples;
			this.gain = 0f;
		}

		// Token: 0x06004F61 RID: 20321 RVA: 0x001A684C File Offset: 0x001A4A4C
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
				if (this.gain > 0.5f)
				{
					this.gain = 0.5f;
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

		// Token: 0x06004F62 RID: 20322 RVA: 0x001A6905 File Offset: 0x001A4B05
		public void FadeOut()
		{
			this.releaseStartSample = this.currentSample;
			this.endSample = this.releaseStartSample + this.releaseTimeSamples;
		}

		// Token: 0x04004458 RID: 17496
		private float[] sourceData;

		// Token: 0x04004459 RID: 17497
		private int startSample;

		// Token: 0x0400445A RID: 17498
		private int currentSample;

		// Token: 0x0400445B RID: 17499
		private int attackTimeSamples;

		// Token: 0x0400445C RID: 17500
		private int sustainTimeSamples;

		// Token: 0x0400445D RID: 17501
		private int releaseTimeSamples;

		// Token: 0x0400445E RID: 17502
		private float gain;

		// Token: 0x0400445F RID: 17503
		private float gainPerSampleAttack;

		// Token: 0x04004460 RID: 17504
		private float gainPerSampleRelease;

		// Token: 0x04004461 RID: 17505
		private int attackEndSample;

		// Token: 0x04004462 RID: 17506
		private int releaseStartSample;

		// Token: 0x04004463 RID: 17507
		private int endSample;
	}
}
