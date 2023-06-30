using System;
using UnityEngine;

// Token: 0x0200023F RID: 575
public class Sound : MonoBehaviour, IClientComponent
{
	// Token: 0x1700025E RID: 606
	// (get) Token: 0x06001C4C RID: 7244 RVA: 0x000C5893 File Offset: 0x000C3A93
	public SoundFade fade
	{
		get
		{
			return this._fade;
		}
	}

	// Token: 0x1700025F RID: 607
	// (get) Token: 0x06001C4D RID: 7245 RVA: 0x000C589B File Offset: 0x000C3A9B
	public SoundModulation modulation
	{
		get
		{
			return this._modulation;
		}
	}

	// Token: 0x17000260 RID: 608
	// (get) Token: 0x06001C4E RID: 7246 RVA: 0x000C58A3 File Offset: 0x000C3AA3
	public SoundOcclusion occlusion
	{
		get
		{
			return this._occlusion;
		}
	}

	// Token: 0x040014A6 RID: 5286
	public static float volumeExponent = Mathf.Log(Mathf.Sqrt(10f), 2f);

	// Token: 0x040014A7 RID: 5287
	public SoundDefinition definition;

	// Token: 0x040014A8 RID: 5288
	public SoundModifier[] modifiers;

	// Token: 0x040014A9 RID: 5289
	public SoundSource soundSource;

	// Token: 0x040014AA RID: 5290
	public AudioSource[] audioSources = new AudioSource[2];

	// Token: 0x040014AB RID: 5291
	[SerializeField]
	private SoundFade _fade;

	// Token: 0x040014AC RID: 5292
	[SerializeField]
	private SoundModulation _modulation;

	// Token: 0x040014AD RID: 5293
	[SerializeField]
	private SoundOcclusion _occlusion;
}
