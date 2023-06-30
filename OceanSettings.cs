using System;
using System.IO;
using Rust.Water5;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

// Token: 0x02000704 RID: 1796
[CreateAssetMenu(fileName = "New Ocean Settings", menuName = "Water5/Ocean Settings")]
public class OceanSettings : ScriptableObject
{
	// Token: 0x060032A5 RID: 12965 RVA: 0x001384AC File Offset: 0x001366AC
	public unsafe OceanDisplacementShort3[,,] LoadSimData()
	{
		OceanDisplacementShort3[,,] array = new OceanDisplacementShort3[this.spectrumSettings.Length, 72, 65536];
		string text = Application.streamingAssetsPath + "/" + base.name + ".physicsdata.dat";
		if (!File.Exists(text))
		{
			Debug.Log("Simulation Data not found");
			return array;
		}
		byte[] array2 = File.ReadAllBytes(text);
		byte[] array3;
		byte* ptr;
		if ((array3 = array2) == null || array3.Length == 0)
		{
			ptr = null;
		}
		else
		{
			ptr = &array3[0];
		}
		OceanDisplacementShort3[,,] array4;
		OceanDisplacementShort3* ptr2;
		if ((array4 = array) == null || array4.Length == 0)
		{
			ptr2 = null;
		}
		else
		{
			ptr2 = &array4[0, 0, 0];
		}
		UnsafeUtility.MemCpy((void*)ptr2, (void*)ptr, (long)array2.Length);
		array4 = null;
		array3 = null;
		return array;
	}

	// Token: 0x04002970 RID: 10608
	[Header("Compute Shaders")]
	public ComputeShader waveSpectrumCompute;

	// Token: 0x04002971 RID: 10609
	public ComputeShader fftCompute;

	// Token: 0x04002972 RID: 10610
	public ComputeShader waveMergeCompute;

	// Token: 0x04002973 RID: 10611
	public ComputeShader waveInitialSpectrum;

	// Token: 0x04002974 RID: 10612
	[Header("Global Ocean Params")]
	public float[] octaveScales;

	// Token: 0x04002975 RID: 10613
	public float lamda;

	// Token: 0x04002976 RID: 10614
	public float windDirection;

	// Token: 0x04002977 RID: 10615
	public float distanceAttenuationFactor;

	// Token: 0x04002978 RID: 10616
	public float depthAttenuationFactor;

	// Token: 0x04002979 RID: 10617
	[Header("Ocean Spectra")]
	public OceanSpectrumSettings[] spectrumSettings;

	// Token: 0x0400297A RID: 10618
	[HideInInspector]
	public float[] spectrumRanges;
}
