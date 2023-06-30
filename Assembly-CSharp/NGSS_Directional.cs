using System;
using ConVar;
using UnityEngine;

// Token: 0x020009A0 RID: 2464
[RequireComponent(typeof(Light))]
[ExecuteInEditMode]
public class NGSS_Directional : MonoBehaviour
{
	// Token: 0x06003A75 RID: 14965 RVA: 0x00158EE4 File Offset: 0x001570E4
	private void Update()
	{
		bool flag = ConVar.Graphics.shadowquality >= 2;
		this.SetGlobalSettings(flag);
	}

	// Token: 0x06003A76 RID: 14966 RVA: 0x00158F04 File Offset: 0x00157104
	private void SetGlobalSettings(bool enabled)
	{
		if (enabled)
		{
			Shader.SetGlobalFloat("NGSS_PCSS_GLOBAL_SOFTNESS", this.PCSS_GLOBAL_SOFTNESS);
			Shader.SetGlobalFloat("NGSS_PCSS_FILTER_DIR_MIN", (this.PCSS_FILTER_DIR_MIN > this.PCSS_FILTER_DIR_MAX) ? this.PCSS_FILTER_DIR_MAX : this.PCSS_FILTER_DIR_MIN);
			Shader.SetGlobalFloat("NGSS_PCSS_FILTER_DIR_MAX", (this.PCSS_FILTER_DIR_MAX < this.PCSS_FILTER_DIR_MIN) ? this.PCSS_FILTER_DIR_MIN : this.PCSS_FILTER_DIR_MAX);
			Shader.SetGlobalFloat("NGSS_POISSON_SAMPLING_NOISE_DIR", this.BANDING_NOISE_AMOUNT);
		}
	}

	// Token: 0x04003537 RID: 13623
	[Tooltip("Overall softness for both PCF and PCSS shadows.\nRecommended value: 0.01.")]
	[Range(0f, 0.02f)]
	public float PCSS_GLOBAL_SOFTNESS = 0.01f;

	// Token: 0x04003538 RID: 13624
	[Tooltip("PCSS softness when shadows is close to caster.\nRecommended value: 0.05.")]
	[Range(0f, 1f)]
	public float PCSS_FILTER_DIR_MIN = 0.05f;

	// Token: 0x04003539 RID: 13625
	[Tooltip("PCSS softness when shadows is far from caster.\nRecommended value: 0.25.\nIf too high can lead to visible artifacts when early bailout is enabled.")]
	[Range(0f, 0.5f)]
	public float PCSS_FILTER_DIR_MAX = 0.25f;

	// Token: 0x0400353A RID: 13626
	[Tooltip("Amount of banding or noise. Example: 0.0 gives 100 % Banding and 10.0 gives 100 % Noise.")]
	[Range(0f, 10f)]
	public float BANDING_NOISE_AMOUNT = 1f;

	// Token: 0x0400353B RID: 13627
	[Tooltip("Recommended values: Mobile = 16, Consoles = 25, Desktop Low = 32, Desktop High = 64")]
	public NGSS_Directional.SAMPLER_COUNT SAMPLERS_COUNT;

	// Token: 0x02000EE5 RID: 3813
	public enum SAMPLER_COUNT
	{
		// Token: 0x04004DC3 RID: 19907
		SAMPLERS_16,
		// Token: 0x04004DC4 RID: 19908
		SAMPLERS_25,
		// Token: 0x04004DC5 RID: 19909
		SAMPLERS_32,
		// Token: 0x04004DC6 RID: 19910
		SAMPLERS_64
	}
}
