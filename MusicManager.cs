using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000236 RID: 566
public class MusicManager : SingletonComponent<MusicManager>, IClientComponent
{
	// Token: 0x1700025A RID: 602
	// (get) Token: 0x06001C25 RID: 7205 RVA: 0x000C51CA File Offset: 0x000C33CA
	public double currentThemeTime
	{
		get
		{
			return UnityEngine.AudioSettings.dspTime - this.themeStartTime;
		}
	}

	// Token: 0x1700025B RID: 603
	// (get) Token: 0x06001C26 RID: 7206 RVA: 0x000C51D8 File Offset: 0x000C33D8
	public int themeBar
	{
		get
		{
			return this.currentBar + this.barOffset;
		}
	}

	// Token: 0x06001C27 RID: 7207 RVA: 0x000063A5 File Offset: 0x000045A5
	public static void RaiseIntensityTo(float amount, int holdLengthBars = 0)
	{
	}

	// Token: 0x06001C28 RID: 7208 RVA: 0x000063A5 File Offset: 0x000045A5
	public void StopMusic()
	{
	}

	// Token: 0x04001455 RID: 5205
	public AudioMixerGroup mixerGroup;

	// Token: 0x04001456 RID: 5206
	public List<MusicTheme> themes;

	// Token: 0x04001457 RID: 5207
	public MusicTheme currentTheme;

	// Token: 0x04001458 RID: 5208
	public List<AudioSource> sources = new List<AudioSource>();

	// Token: 0x04001459 RID: 5209
	public double nextMusic;

	// Token: 0x0400145A RID: 5210
	public double nextMusicFromIntensityRaise;

	// Token: 0x0400145B RID: 5211
	[Range(0f, 1f)]
	public float intensity;

	// Token: 0x0400145C RID: 5212
	public Dictionary<MusicTheme.PositionedClip, MusicManager.ClipPlaybackData> clipPlaybackData = new Dictionary<MusicTheme.PositionedClip, MusicManager.ClipPlaybackData>();

	// Token: 0x0400145D RID: 5213
	public int holdIntensityUntilBar;

	// Token: 0x0400145E RID: 5214
	public bool musicPlaying;

	// Token: 0x0400145F RID: 5215
	public bool loadingFirstClips;

	// Token: 0x04001460 RID: 5216
	public MusicTheme nextTheme;

	// Token: 0x04001461 RID: 5217
	public double lastClipUpdate;

	// Token: 0x04001462 RID: 5218
	public float clipUpdateInterval = 0.1f;

	// Token: 0x04001463 RID: 5219
	public double themeStartTime;

	// Token: 0x04001464 RID: 5220
	public int lastActiveClipRefresh = -10;

	// Token: 0x04001465 RID: 5221
	public int activeClipRefreshInterval = 1;

	// Token: 0x04001466 RID: 5222
	public bool forceThemeChange;

	// Token: 0x04001467 RID: 5223
	public float randomIntensityJumpChance;

	// Token: 0x04001468 RID: 5224
	public int clipScheduleBarsEarly = 1;

	// Token: 0x04001469 RID: 5225
	public List<MusicTheme.PositionedClip> activeClips = new List<MusicTheme.PositionedClip>();

	// Token: 0x0400146A RID: 5226
	public List<MusicTheme.PositionedClip> activeMusicClips = new List<MusicTheme.PositionedClip>();

	// Token: 0x0400146B RID: 5227
	public List<MusicTheme.PositionedClip> activeControlClips = new List<MusicTheme.PositionedClip>();

	// Token: 0x0400146C RID: 5228
	public List<MusicZone> currentMusicZones = new List<MusicZone>();

	// Token: 0x0400146D RID: 5229
	public int currentBar;

	// Token: 0x0400146E RID: 5230
	public int barOffset;

	// Token: 0x02000C92 RID: 3218
	[Serializable]
	public class ClipPlaybackData
	{
		// Token: 0x04004440 RID: 17472
		public AudioSource source;

		// Token: 0x04004441 RID: 17473
		public MusicTheme.PositionedClip positionedClip;

		// Token: 0x04004442 RID: 17474
		public bool isActive;

		// Token: 0x04004443 RID: 17475
		public bool fadingIn;

		// Token: 0x04004444 RID: 17476
		public bool fadingOut;

		// Token: 0x04004445 RID: 17477
		public double fadeStarted;

		// Token: 0x04004446 RID: 17478
		public bool needsSync;
	}
}
