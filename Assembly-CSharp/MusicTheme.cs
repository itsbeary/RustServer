using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000237 RID: 567
[CreateAssetMenu(menuName = "Rust/MusicTheme")]
public class MusicTheme : ScriptableObject
{
	// Token: 0x1700025C RID: 604
	// (get) Token: 0x06001C2A RID: 7210 RVA: 0x000C525E File Offset: 0x000C345E
	public int layerCount
	{
		get
		{
			return this.layers.Count;
		}
	}

	// Token: 0x1700025D RID: 605
	// (get) Token: 0x06001C2B RID: 7211 RVA: 0x000C526B File Offset: 0x000C346B
	public int samplesPerBar
	{
		get
		{
			return MusicUtil.BarsToSamples(this.tempo, 1f, 44100);
		}
	}

	// Token: 0x06001C2C RID: 7212 RVA: 0x000C5284 File Offset: 0x000C3484
	private void OnValidate()
	{
		this.audioClipDict.Clear();
		this.activeClips.Clear();
		this.UpdateLengthInBars();
		for (int i = 0; i < this.clips.Count; i++)
		{
			MusicTheme.PositionedClip positionedClip = this.clips[i];
			int num = this.ActiveClipCollectionID(positionedClip.startingBar - 8);
			int num2 = this.ActiveClipCollectionID(positionedClip.endingBar);
			for (int j = num; j <= num2; j++)
			{
				if (!this.activeClips.ContainsKey(j))
				{
					this.activeClips.Add(j, new List<MusicTheme.PositionedClip>());
				}
				if (!this.activeClips[j].Contains(positionedClip))
				{
					this.activeClips[j].Add(positionedClip);
				}
			}
			if (positionedClip.musicClip != null)
			{
				AudioClip audioClip = positionedClip.musicClip.audioClip;
				if (!this.audioClipDict.ContainsKey(audioClip))
				{
					this.audioClipDict.Add(audioClip, true);
				}
				if (positionedClip.startingBar < 8 && !this.firstAudioClips.Contains(audioClip))
				{
					this.firstAudioClips.Add(audioClip);
				}
				positionedClip.musicClip.lengthInBarsWithTail = Mathf.CeilToInt(MusicUtil.SecondsToBars(this.tempo, (double)positionedClip.musicClip.audioClip.length));
			}
		}
	}

	// Token: 0x06001C2D RID: 7213 RVA: 0x000C53CC File Offset: 0x000C35CC
	public List<MusicTheme.PositionedClip> GetActiveClipsForBar(int bar)
	{
		int num = this.ActiveClipCollectionID(bar);
		if (!this.activeClips.ContainsKey(num))
		{
			return null;
		}
		return this.activeClips[num];
	}

	// Token: 0x06001C2E RID: 7214 RVA: 0x000C53FD File Offset: 0x000C35FD
	private int ActiveClipCollectionID(int bar)
	{
		return Mathf.FloorToInt(Mathf.Max((float)(bar / 4), 0f));
	}

	// Token: 0x06001C2F RID: 7215 RVA: 0x000C5412 File Offset: 0x000C3612
	public MusicTheme.Layer LayerById(int id)
	{
		if (this.layers.Count <= id)
		{
			return null;
		}
		return this.layers[id];
	}

	// Token: 0x06001C30 RID: 7216 RVA: 0x000C5430 File Offset: 0x000C3630
	public void AddLayer()
	{
		MusicTheme.Layer layer = new MusicTheme.Layer();
		layer.name = "layer " + this.layers.Count;
		this.layers.Add(layer);
	}

	// Token: 0x06001C31 RID: 7217 RVA: 0x000C5470 File Offset: 0x000C3670
	private void UpdateLengthInBars()
	{
		int num = 0;
		for (int i = 0; i < this.clips.Count; i++)
		{
			MusicTheme.PositionedClip positionedClip = this.clips[i];
			if (!(positionedClip.musicClip == null))
			{
				int num2 = positionedClip.startingBar + positionedClip.musicClip.lengthInBars;
				if (num2 > num)
				{
					num = num2;
				}
			}
		}
		this.lengthInBars = num;
	}

	// Token: 0x06001C32 RID: 7218 RVA: 0x000C54D0 File Offset: 0x000C36D0
	public bool CanPlayInEnvironment(int currentBiome, int currentTopology, float currentRain, float currentSnow, float currentWind)
	{
		return (!TOD_Sky.Instance || this.time.Evaluate(TOD_Sky.Instance.Cycle.Hour) >= 0f) && (this.biomes == (TerrainBiome.Enum)(-1) || (this.biomes & (TerrainBiome.Enum)currentBiome) != (TerrainBiome.Enum)0) && (this.topologies == (TerrainTopology.Enum)(-1) || (this.topologies & (TerrainTopology.Enum)currentTopology) == (TerrainTopology.Enum)0) && ((this.rain.min <= 0f && this.rain.max >= 1f) || currentRain >= this.rain.min) && currentRain <= this.rain.max && ((this.snow.min <= 0f && this.snow.max >= 1f) || currentSnow >= this.snow.min) && currentSnow <= this.snow.max && ((this.wind.min <= 0f && this.wind.max >= 1f) || currentWind >= this.wind.min) && currentWind <= this.wind.max;
	}

	// Token: 0x06001C33 RID: 7219 RVA: 0x000C5604 File Offset: 0x000C3804
	public bool FirstClipsLoaded()
	{
		for (int i = 0; i < this.firstAudioClips.Count; i++)
		{
			if (this.firstAudioClips[i].loadState != AudioDataLoadState.Loaded)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001C34 RID: 7220 RVA: 0x000C563E File Offset: 0x000C383E
	public bool ContainsAudioClip(AudioClip clip)
	{
		return this.audioClipDict.ContainsKey(clip);
	}

	// Token: 0x0400146F RID: 5231
	[Header("Basic info")]
	public float tempo = 80f;

	// Token: 0x04001470 RID: 5232
	public int intensityHoldBars = 4;

	// Token: 0x04001471 RID: 5233
	public int lengthInBars;

	// Token: 0x04001472 RID: 5234
	[Header("Playback restrictions")]
	public bool canPlayInMenus = true;

	// Token: 0x04001473 RID: 5235
	[Horizontal(2, -1)]
	public MusicTheme.ValueRange rain = new MusicTheme.ValueRange(0f, 1f);

	// Token: 0x04001474 RID: 5236
	[Horizontal(2, -1)]
	public MusicTheme.ValueRange wind = new MusicTheme.ValueRange(0f, 1f);

	// Token: 0x04001475 RID: 5237
	[Horizontal(2, -1)]
	public MusicTheme.ValueRange snow = new MusicTheme.ValueRange(0f, 1f);

	// Token: 0x04001476 RID: 5238
	[InspectorFlags]
	public TerrainBiome.Enum biomes = (TerrainBiome.Enum)(-1);

	// Token: 0x04001477 RID: 5239
	[InspectorFlags]
	public TerrainTopology.Enum topologies = (TerrainTopology.Enum)(-1);

	// Token: 0x04001478 RID: 5240
	public AnimationCurve time = AnimationCurve.Linear(0f, 0f, 24f, 0f);

	// Token: 0x04001479 RID: 5241
	[Header("Clip data")]
	public List<MusicTheme.PositionedClip> clips = new List<MusicTheme.PositionedClip>();

	// Token: 0x0400147A RID: 5242
	public List<MusicTheme.Layer> layers = new List<MusicTheme.Layer>();

	// Token: 0x0400147B RID: 5243
	private Dictionary<int, List<MusicTheme.PositionedClip>> activeClips = new Dictionary<int, List<MusicTheme.PositionedClip>>();

	// Token: 0x0400147C RID: 5244
	private List<AudioClip> firstAudioClips = new List<AudioClip>();

	// Token: 0x0400147D RID: 5245
	private Dictionary<AudioClip, bool> audioClipDict = new Dictionary<AudioClip, bool>();

	// Token: 0x02000C93 RID: 3219
	[Serializable]
	public class Layer
	{
		// Token: 0x04004447 RID: 17479
		public string name = "layer";
	}

	// Token: 0x02000C94 RID: 3220
	[Serializable]
	public class PositionedClip
	{
		// Token: 0x17000699 RID: 1689
		// (get) Token: 0x06004F59 RID: 20313 RVA: 0x001A6647 File Offset: 0x001A4847
		public int endingBar
		{
			get
			{
				if (!(this.musicClip == null))
				{
					return this.startingBar + this.musicClip.lengthInBarsWithTail;
				}
				return this.startingBar;
			}
		}

		// Token: 0x06004F5A RID: 20314 RVA: 0x001A6670 File Offset: 0x001A4870
		public bool CanPlay(float intensity)
		{
			return (intensity > this.minIntensity || (this.minIntensity == 0f && intensity == 0f)) && intensity <= this.maxIntensity;
		}

		// Token: 0x1700069A RID: 1690
		// (get) Token: 0x06004F5B RID: 20315 RVA: 0x001A669E File Offset: 0x001A489E
		public bool isControlClip
		{
			get
			{
				return this.musicClip == null;
			}
		}

		// Token: 0x06004F5C RID: 20316 RVA: 0x001A66AC File Offset: 0x001A48AC
		public void CopySettingsFrom(MusicTheme.PositionedClip otherClip)
		{
			if (this.isControlClip != otherClip.isControlClip)
			{
				return;
			}
			if (otherClip == this)
			{
				return;
			}
			this.allowFadeIn = otherClip.allowFadeIn;
			this.fadeInTime = otherClip.fadeInTime;
			this.allowFadeOut = otherClip.allowFadeOut;
			this.fadeOutTime = otherClip.fadeOutTime;
			this.maxIntensity = otherClip.maxIntensity;
			this.minIntensity = otherClip.minIntensity;
			this.intensityReduction = otherClip.intensityReduction;
		}

		// Token: 0x04004448 RID: 17480
		public MusicTheme theme;

		// Token: 0x04004449 RID: 17481
		public MusicClip musicClip;

		// Token: 0x0400444A RID: 17482
		public int startingBar;

		// Token: 0x0400444B RID: 17483
		public int layerId;

		// Token: 0x0400444C RID: 17484
		public float minIntensity;

		// Token: 0x0400444D RID: 17485
		public float maxIntensity = 1f;

		// Token: 0x0400444E RID: 17486
		public bool allowFadeIn = true;

		// Token: 0x0400444F RID: 17487
		public bool allowFadeOut = true;

		// Token: 0x04004450 RID: 17488
		public float fadeInTime = 1f;

		// Token: 0x04004451 RID: 17489
		public float fadeOutTime = 0.5f;

		// Token: 0x04004452 RID: 17490
		public float intensityReduction;

		// Token: 0x04004453 RID: 17491
		public int jumpBarCount;

		// Token: 0x04004454 RID: 17492
		public float jumpMinimumIntensity = 0.5f;

		// Token: 0x04004455 RID: 17493
		public float jumpMaximumIntensity = 0.5f;
	}

	// Token: 0x02000C95 RID: 3221
	[Serializable]
	public class ValueRange
	{
		// Token: 0x06004F5E RID: 20318 RVA: 0x001A677C File Offset: 0x001A497C
		public ValueRange(float min, float max)
		{
			this.min = min;
			this.max = max;
		}

		// Token: 0x04004456 RID: 17494
		public float min;

		// Token: 0x04004457 RID: 17495
		public float max;
	}
}
