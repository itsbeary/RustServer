using System;

// Token: 0x0200065C RID: 1628
[Serializable]
public struct NoiseParameters
{
	// Token: 0x06002F31 RID: 12081 RVA: 0x0011C056 File Offset: 0x0011A256
	public NoiseParameters(int octaves, float frequency, float amplitude, float offset)
	{
		this.Octaves = octaves;
		this.Frequency = frequency;
		this.Amplitude = amplitude;
		this.Offset = offset;
	}

	// Token: 0x040026DF RID: 9951
	public int Octaves;

	// Token: 0x040026E0 RID: 9952
	public float Frequency;

	// Token: 0x040026E1 RID: 9953
	public float Amplitude;

	// Token: 0x040026E2 RID: 9954
	public float Offset;
}
