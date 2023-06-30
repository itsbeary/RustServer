using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200022F RID: 559
public class GranularAudioClip : MonoBehaviour
{
	// Token: 0x06001C12 RID: 7186 RVA: 0x000C4B3C File Offset: 0x000C2D3C
	private void Update()
	{
		if (!this.inited && this.sourceClip.loadState == AudioDataLoadState.Loaded)
		{
			this.sampleRate = this.sourceClip.frequency;
			this.sourceAudioData = new float[this.sourceClip.samples * this.sourceClip.channels];
			this.sourceClip.GetData(this.sourceAudioData, 0);
			this.InitAudioClip();
			AudioSource component = base.GetComponent<AudioSource>();
			component.clip = this.granularClip;
			component.loop = true;
			component.Play();
			this.inited = true;
		}
		this.RefreshCachedData();
	}

	// Token: 0x06001C13 RID: 7187 RVA: 0x000C4BD8 File Offset: 0x000C2DD8
	private void RefreshCachedData()
	{
		this.grainAttackSamples = Mathf.FloorToInt(this.grainAttack * (float)this.sampleRate * (float)this.sourceChannels);
		this.grainSustainSamples = Mathf.FloorToInt(this.grainSustain * (float)this.sampleRate * (float)this.sourceChannels);
		this.grainReleaseSamples = Mathf.FloorToInt(this.grainRelease * (float)this.sampleRate * (float)this.sourceChannels);
		this.grainFrequencySamples = Mathf.FloorToInt(this.grainFrequency * (float)this.sampleRate * (float)this.sourceChannels);
	}

	// Token: 0x06001C14 RID: 7188 RVA: 0x000C4C6C File Offset: 0x000C2E6C
	private void InitAudioClip()
	{
		int num = 1;
		int num2 = 1;
		UnityEngine.AudioSettings.GetDSPBufferSize(out num, out num2);
		this.granularClip = AudioClip.Create(this.sourceClip.name + " (granular)", num, this.sourceClip.channels, this.sampleRate, true, new AudioClip.PCMReaderCallback(this.OnAudioRead));
		this.sourceChannels = this.sourceClip.channels;
	}

	// Token: 0x06001C15 RID: 7189 RVA: 0x000C4CD8 File Offset: 0x000C2ED8
	private void OnAudioRead(float[] data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			if (this.samplesUntilNextGrain <= 0)
			{
				this.SpawnGrain();
			}
			float num = 0f;
			for (int j = 0; j < this.grains.Count; j++)
			{
				num += this.grains[j].GetSample();
			}
			data[i] = num;
			this.samplesUntilNextGrain--;
		}
		this.CleanupFinishedGrains();
	}

	// Token: 0x06001C16 RID: 7190 RVA: 0x000C4D4C File Offset: 0x000C2F4C
	private void SpawnGrain()
	{
		if (this.grainFrequencySamples == 0)
		{
			return;
		}
		float num = (float)(this.random.NextDouble() * (double)this.sourceTimeVariation * 2.0) - this.sourceTimeVariation;
		int num2 = Mathf.FloorToInt((this.sourceTime + num) * (float)this.sampleRate / (float)this.sourceChannels);
		GranularAudioClip.Grain grain = Pool.Get<GranularAudioClip.Grain>();
		grain.Init(this.sourceAudioData, num2, this.grainAttackSamples, this.grainSustainSamples, this.grainReleaseSamples);
		this.grains.Add(grain);
		this.samplesUntilNextGrain = this.grainFrequencySamples;
	}

	// Token: 0x06001C17 RID: 7191 RVA: 0x000C4DE4 File Offset: 0x000C2FE4
	private void CleanupFinishedGrains()
	{
		for (int i = this.grains.Count - 1; i >= 0; i--)
		{
			GranularAudioClip.Grain grain = this.grains[i];
			if (grain.finished)
			{
				Pool.Free<GranularAudioClip.Grain>(ref grain);
				this.grains.RemoveAt(i);
			}
		}
	}

	// Token: 0x04001424 RID: 5156
	public AudioClip sourceClip;

	// Token: 0x04001425 RID: 5157
	private float[] sourceAudioData;

	// Token: 0x04001426 RID: 5158
	private int sourceChannels = 1;

	// Token: 0x04001427 RID: 5159
	public AudioClip granularClip;

	// Token: 0x04001428 RID: 5160
	public int sampleRate = 44100;

	// Token: 0x04001429 RID: 5161
	public float sourceTime = 0.5f;

	// Token: 0x0400142A RID: 5162
	public float sourceTimeVariation = 0.1f;

	// Token: 0x0400142B RID: 5163
	public float grainAttack = 0.1f;

	// Token: 0x0400142C RID: 5164
	public float grainSustain = 0.1f;

	// Token: 0x0400142D RID: 5165
	public float grainRelease = 0.1f;

	// Token: 0x0400142E RID: 5166
	public float grainFrequency = 0.1f;

	// Token: 0x0400142F RID: 5167
	public int grainAttackSamples;

	// Token: 0x04001430 RID: 5168
	public int grainSustainSamples;

	// Token: 0x04001431 RID: 5169
	public int grainReleaseSamples;

	// Token: 0x04001432 RID: 5170
	public int grainFrequencySamples;

	// Token: 0x04001433 RID: 5171
	public int samplesUntilNextGrain;

	// Token: 0x04001434 RID: 5172
	public List<GranularAudioClip.Grain> grains = new List<GranularAudioClip.Grain>();

	// Token: 0x04001435 RID: 5173
	private System.Random random = new System.Random();

	// Token: 0x04001436 RID: 5174
	private bool inited;

	// Token: 0x02000C8F RID: 3215
	public class Grain
	{
		// Token: 0x17000698 RID: 1688
		// (get) Token: 0x06004F51 RID: 20305 RVA: 0x001A64D1 File Offset: 0x001A46D1
		public bool finished
		{
			get
			{
				return this.currentSample >= this.endSample;
			}
		}

		// Token: 0x06004F52 RID: 20306 RVA: 0x001A64E4 File Offset: 0x001A46E4
		public void Init(float[] source, int start, int attack, int sustain, int release)
		{
			this.sourceData = source;
			this.sourceDataLength = this.sourceData.Length;
			this.startSample = start;
			this.currentSample = start;
			this.attackTimeSamples = attack;
			this.sustainTimeSamples = sustain;
			this.releaseTimeSamples = release;
			this.gainPerSampleAttack = 1f / (float)this.attackTimeSamples;
			this.gainPerSampleRelease = -1f / (float)this.releaseTimeSamples;
			this.attackEndSample = this.startSample + this.attackTimeSamples;
			this.releaseStartSample = this.attackEndSample + this.sustainTimeSamples;
			this.endSample = this.releaseStartSample + this.releaseTimeSamples;
			this.gain = 0f;
		}

		// Token: 0x06004F53 RID: 20307 RVA: 0x001A6598 File Offset: 0x001A4798
		public float GetSample()
		{
			int num = this.currentSample % this.sourceDataLength;
			if (num < 0)
			{
				num += this.sourceDataLength;
			}
			float num2 = this.sourceData[num];
			if (this.currentSample <= this.attackEndSample)
			{
				this.gain += this.gainPerSampleAttack;
			}
			else if (this.currentSample >= this.releaseStartSample)
			{
				this.gain += this.gainPerSampleRelease;
			}
			this.currentSample++;
			return num2 * this.gain;
		}

		// Token: 0x0400442E RID: 17454
		private float[] sourceData;

		// Token: 0x0400442F RID: 17455
		private int sourceDataLength;

		// Token: 0x04004430 RID: 17456
		private int startSample;

		// Token: 0x04004431 RID: 17457
		private int currentSample;

		// Token: 0x04004432 RID: 17458
		private int attackTimeSamples;

		// Token: 0x04004433 RID: 17459
		private int sustainTimeSamples;

		// Token: 0x04004434 RID: 17460
		private int releaseTimeSamples;

		// Token: 0x04004435 RID: 17461
		private float gain;

		// Token: 0x04004436 RID: 17462
		private float gainPerSampleAttack;

		// Token: 0x04004437 RID: 17463
		private float gainPerSampleRelease;

		// Token: 0x04004438 RID: 17464
		private int attackEndSample;

		// Token: 0x04004439 RID: 17465
		private int releaseStartSample;

		// Token: 0x0400443A RID: 17466
		private int endSample;
	}
}
