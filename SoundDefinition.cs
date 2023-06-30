using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000242 RID: 578
public class SoundDefinition : ScriptableObject
{
	// Token: 0x17000261 RID: 609
	// (get) Token: 0x06001C53 RID: 7251 RVA: 0x000C5928 File Offset: 0x000C3B28
	public float maxDistance
	{
		get
		{
			if (this.template == null)
			{
				return 0f;
			}
			AudioSource component = this.template.Get().GetComponent<AudioSource>();
			if (component == null)
			{
				return 0f;
			}
			return component.maxDistance;
		}
	}

	// Token: 0x06001C54 RID: 7252 RVA: 0x000C596C File Offset: 0x000C3B6C
	public float GetLength()
	{
		float num = 0f;
		for (int i = 0; i < this.weightedAudioClips.Count; i++)
		{
			AudioClip audioClip = this.weightedAudioClips[i].audioClip;
			if (audioClip)
			{
				num = Mathf.Max(audioClip.length, num);
			}
		}
		for (int j = 0; j < this.distanceAudioClips.Count; j++)
		{
			List<WeightedAudioClip> audioClips = this.distanceAudioClips[j].audioClips;
			for (int k = 0; k < audioClips.Count; k++)
			{
				AudioClip audioClip2 = audioClips[k].audioClip;
				if (audioClip2)
				{
					num = Mathf.Max(audioClip2.length, num);
				}
			}
		}
		float num2 = 1f / (this.pitch - this.pitchVariation);
		return num * num2;
	}

	// Token: 0x06001C55 RID: 7253 RVA: 0x0002CFBB File Offset: 0x0002B1BB
	public Sound Play()
	{
		return null;
	}

	// Token: 0x06001C56 RID: 7254 RVA: 0x0002CFBB File Offset: 0x0002B1BB
	public Sound Play(GameObject forGameObject)
	{
		return null;
	}

	// Token: 0x040014B9 RID: 5305
	public GameObjectRef template;

	// Token: 0x040014BA RID: 5306
	[Horizontal(2, -1)]
	public List<WeightedAudioClip> weightedAudioClips = new List<WeightedAudioClip>
	{
		new WeightedAudioClip()
	};

	// Token: 0x040014BB RID: 5307
	public List<SoundDefinition.DistanceAudioClipList> distanceAudioClips;

	// Token: 0x040014BC RID: 5308
	public SoundClass soundClass;

	// Token: 0x040014BD RID: 5309
	public bool defaultToFirstPerson;

	// Token: 0x040014BE RID: 5310
	public bool loop;

	// Token: 0x040014BF RID: 5311
	public bool randomizeStartPosition;

	// Token: 0x040014C0 RID: 5312
	public bool useHighQualityFades;

	// Token: 0x040014C1 RID: 5313
	[Range(0f, 1f)]
	public float volume = 1f;

	// Token: 0x040014C2 RID: 5314
	[Range(0f, 1f)]
	public float volumeVariation;

	// Token: 0x040014C3 RID: 5315
	[Range(-3f, 3f)]
	public float pitch = 1f;

	// Token: 0x040014C4 RID: 5316
	[Range(0f, 1f)]
	public float pitchVariation;

	// Token: 0x040014C5 RID: 5317
	[Header("Voice limiting")]
	public bool dontVoiceLimit;

	// Token: 0x040014C6 RID: 5318
	public int globalVoiceMaxCount = 100;

	// Token: 0x040014C7 RID: 5319
	public int localVoiceMaxCount = 100;

	// Token: 0x040014C8 RID: 5320
	public float localVoiceRange = 10f;

	// Token: 0x040014C9 RID: 5321
	public float voiceLimitFadeOutTime = 0.05f;

	// Token: 0x040014CA RID: 5322
	public float localVoiceDebounceTime = 0.1f;

	// Token: 0x040014CB RID: 5323
	[Header("Occlusion Settings")]
	public bool forceOccludedPlayback;

	// Token: 0x040014CC RID: 5324
	[Header("Doppler")]
	public bool enableDoppler;

	// Token: 0x040014CD RID: 5325
	public float dopplerAmount = 0.18f;

	// Token: 0x040014CE RID: 5326
	public float dopplerScale = 1f;

	// Token: 0x040014CF RID: 5327
	public float dopplerAdjustmentRate = 1f;

	// Token: 0x040014D0 RID: 5328
	[Header("Custom curves")]
	public AnimationCurve falloffCurve;

	// Token: 0x040014D1 RID: 5329
	public bool useCustomFalloffCurve;

	// Token: 0x040014D2 RID: 5330
	public AnimationCurve spatialBlendCurve;

	// Token: 0x040014D3 RID: 5331
	public bool useCustomSpatialBlendCurve;

	// Token: 0x040014D4 RID: 5332
	public AnimationCurve spreadCurve;

	// Token: 0x040014D5 RID: 5333
	public bool useCustomSpreadCurve;

	// Token: 0x02000C97 RID: 3223
	[Serializable]
	public class DistanceAudioClipList
	{
		// Token: 0x04004464 RID: 17508
		public int distance;

		// Token: 0x04004465 RID: 17509
		[Horizontal(2, -1)]
		public List<WeightedAudioClip> audioClips;
	}
}
