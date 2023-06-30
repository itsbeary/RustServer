using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000235 RID: 565
public class MusicClipLoader
{
	// Token: 0x06001C21 RID: 7201 RVA: 0x000C4F84 File Offset: 0x000C3184
	public void Update()
	{
		for (int i = this.clipsToLoad.Count - 1; i >= 0; i--)
		{
			AudioClip audioClip = this.clipsToLoad[i];
			if (audioClip.loadState != AudioDataLoadState.Loaded && audioClip.loadState != AudioDataLoadState.Loading)
			{
				audioClip.LoadAudioData();
				this.clipsToLoad.RemoveAt(i);
				return;
			}
		}
		for (int j = this.clipsToUnload.Count - 1; j >= 0; j--)
		{
			AudioClip audioClip2 = this.clipsToUnload[j];
			if (audioClip2.loadState == AudioDataLoadState.Loaded)
			{
				audioClip2.UnloadAudioData();
				this.clipsToUnload.RemoveAt(j);
				return;
			}
		}
	}

	// Token: 0x06001C22 RID: 7202 RVA: 0x000C5020 File Offset: 0x000C3220
	public void Refresh()
	{
		for (int i = 0; i < SingletonComponent<MusicManager>.Instance.activeMusicClips.Count; i++)
		{
			MusicTheme.PositionedClip positionedClip = SingletonComponent<MusicManager>.Instance.activeMusicClips[i];
			MusicClipLoader.LoadedAudioClip loadedAudioClip = this.FindLoadedClip(positionedClip.musicClip.audioClip);
			if (loadedAudioClip == null)
			{
				loadedAudioClip = Pool.Get<MusicClipLoader.LoadedAudioClip>();
				loadedAudioClip.clip = positionedClip.musicClip.audioClip;
				loadedAudioClip.unloadTime = (float)UnityEngine.AudioSettings.dspTime + loadedAudioClip.clip.length + 1f;
				this.loadedClips.Add(loadedAudioClip);
				this.loadedClipDict.Add(loadedAudioClip.clip, loadedAudioClip);
				this.clipsToLoad.Add(loadedAudioClip.clip);
			}
			else
			{
				loadedAudioClip.unloadTime = (float)UnityEngine.AudioSettings.dspTime + loadedAudioClip.clip.length + 1f;
				this.clipsToUnload.Remove(loadedAudioClip.clip);
			}
		}
		for (int j = this.loadedClips.Count - 1; j >= 0; j--)
		{
			MusicClipLoader.LoadedAudioClip loadedAudioClip2 = this.loadedClips[j];
			if (UnityEngine.AudioSettings.dspTime > (double)loadedAudioClip2.unloadTime)
			{
				this.clipsToUnload.Add(loadedAudioClip2.clip);
				this.loadedClips.Remove(loadedAudioClip2);
				this.loadedClipDict.Remove(loadedAudioClip2.clip);
				Pool.Free<MusicClipLoader.LoadedAudioClip>(ref loadedAudioClip2);
			}
		}
	}

	// Token: 0x06001C23 RID: 7203 RVA: 0x000C5178 File Offset: 0x000C3378
	private MusicClipLoader.LoadedAudioClip FindLoadedClip(AudioClip clip)
	{
		if (this.loadedClipDict.ContainsKey(clip))
		{
			return this.loadedClipDict[clip];
		}
		return null;
	}

	// Token: 0x04001451 RID: 5201
	public List<MusicClipLoader.LoadedAudioClip> loadedClips = new List<MusicClipLoader.LoadedAudioClip>();

	// Token: 0x04001452 RID: 5202
	public Dictionary<AudioClip, MusicClipLoader.LoadedAudioClip> loadedClipDict = new Dictionary<AudioClip, MusicClipLoader.LoadedAudioClip>();

	// Token: 0x04001453 RID: 5203
	public List<AudioClip> clipsToLoad = new List<AudioClip>();

	// Token: 0x04001454 RID: 5204
	public List<AudioClip> clipsToUnload = new List<AudioClip>();

	// Token: 0x02000C91 RID: 3217
	public class LoadedAudioClip
	{
		// Token: 0x0400443E RID: 17470
		public AudioClip clip;

		// Token: 0x0400443F RID: 17471
		public float unloadTime;
	}
}
