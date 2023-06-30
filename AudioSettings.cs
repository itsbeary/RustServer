using System;
using ConVar;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000296 RID: 662
public class AudioSettings : MonoBehaviour
{
	// Token: 0x06001D57 RID: 7511 RVA: 0x000CA490 File Offset: 0x000C8690
	private void Update()
	{
		if (this.mixer == null)
		{
			return;
		}
		this.mixer.SetFloat("MasterVol", this.LinearToDecibel(Audio.master * global::AudioSettings.duckingFactor));
		float num;
		this.mixer.GetFloat("MusicVol", out num);
		if (!LevelManager.isLoaded || !MainCamera.isValid)
		{
			this.mixer.SetFloat("MusicVol", Mathf.Lerp(num, this.LinearToDecibel(Audio.musicvolumemenu), UnityEngine.Time.deltaTime));
		}
		else
		{
			this.mixer.SetFloat("MusicVol", Mathf.Lerp(num, this.LinearToDecibel(Audio.musicvolume), UnityEngine.Time.deltaTime));
		}
		float num2 = 1f - ((SingletonComponent<MixerSnapshotManager>.Instance == null) ? 0f : SingletonComponent<MixerSnapshotManager>.Instance.deafness);
		this.mixer.SetFloat("WorldVol", this.LinearToDecibel(Audio.game * num2));
		this.mixer.SetFloat("WorldVolFlashbang", this.LinearToDecibel(Audio.game));
		this.mixer.SetFloat("VoiceVol", this.LinearToDecibel(Audio.voices * num2));
		this.mixer.SetFloat("InstrumentVol", this.LinearToDecibel(Audio.instruments * num2));
		float num3 = this.LinearToDecibel(Audio.voiceProps * num2) - 28.7f;
		this.mixer.SetFloat("VoicePropsVol", num3 * num2);
		this.mixer.SetFloat("SeasonalEventsVol", this.LinearToDecibel(Audio.eventAudio * num2));
	}

	// Token: 0x06001D58 RID: 7512 RVA: 0x000CA620 File Offset: 0x000C8820
	private float LinearToDecibel(float linear)
	{
		float num;
		if (linear > 0f)
		{
			num = 20f * Mathf.Log10(linear);
		}
		else
		{
			num = -144f;
		}
		return num;
	}

	// Token: 0x040015FF RID: 5631
	public static float duckingFactor = 1f;

	// Token: 0x04001600 RID: 5632
	public AudioMixer mixer;
}
