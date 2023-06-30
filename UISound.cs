using System;
using ConVar;
using UnityEngine;

// Token: 0x020008A7 RID: 2215
public static class UISound
{
	// Token: 0x06003704 RID: 14084 RVA: 0x0014B530 File Offset: 0x00149730
	private static AudioSource GetAudioSource()
	{
		if (UISound.source != null)
		{
			return UISound.source;
		}
		UISound.source = new GameObject("UISound").AddComponent<AudioSource>();
		UISound.source.spatialBlend = 0f;
		UISound.source.volume = 1f;
		return UISound.source;
	}

	// Token: 0x06003705 RID: 14085 RVA: 0x0014B587 File Offset: 0x00149787
	public static void Play(AudioClip clip, float volume = 1f)
	{
		if (clip == null)
		{
			return;
		}
		UISound.GetAudioSource().volume = volume * Audio.master * 0.4f;
		UISound.GetAudioSource().PlayOneShot(clip);
	}

	// Token: 0x040031FF RID: 12799
	private static AudioSource source;
}
