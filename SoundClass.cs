using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000240 RID: 576
[CreateAssetMenu(menuName = "Rust/Sound Class")]
public class SoundClass : ScriptableObject
{
	// Token: 0x040014AE RID: 5294
	[Header("Mixer Settings")]
	public AudioMixerGroup output;

	// Token: 0x040014AF RID: 5295
	public AudioMixerGroup firstPersonOutput;

	// Token: 0x040014B0 RID: 5296
	[Header("Occlusion Settings")]
	public bool enableOcclusion;

	// Token: 0x040014B1 RID: 5297
	public bool playIfOccluded = true;

	// Token: 0x040014B2 RID: 5298
	public float occlusionGain = 1f;

	// Token: 0x040014B3 RID: 5299
	[Tooltip("Use this mixer group when the sound is occluded to save DSP CPU usage. Only works for non-looping sounds.")]
	public AudioMixerGroup occludedOutput;

	// Token: 0x040014B4 RID: 5300
	[Header("Voice Limiting")]
	public int globalVoiceMaxCount = 100;

	// Token: 0x040014B5 RID: 5301
	public int priority = 128;

	// Token: 0x040014B6 RID: 5302
	public List<SoundDefinition> definitions = new List<SoundDefinition>();
}
