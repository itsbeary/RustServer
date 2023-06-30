using System;
using UnityEngine;

// Token: 0x02000238 RID: 568
public class MusicUtil
{
	// Token: 0x06001C36 RID: 7222 RVA: 0x000C571B File Offset: 0x000C391B
	public static double BeatsToSeconds(float tempo, float beats)
	{
		return 60.0 / (double)tempo * (double)beats;
	}

	// Token: 0x06001C37 RID: 7223 RVA: 0x000C572C File Offset: 0x000C392C
	public static double BarsToSeconds(float tempo, float bars)
	{
		return MusicUtil.BeatsToSeconds(tempo, bars * 4f);
	}

	// Token: 0x06001C38 RID: 7224 RVA: 0x000C573B File Offset: 0x000C393B
	public static int SecondsToSamples(double seconds)
	{
		return MusicUtil.SecondsToSamples(seconds, UnityEngine.AudioSettings.outputSampleRate);
	}

	// Token: 0x06001C39 RID: 7225 RVA: 0x000C5748 File Offset: 0x000C3948
	public static int SecondsToSamples(double seconds, int sampleRate)
	{
		return (int)((double)sampleRate * seconds);
	}

	// Token: 0x06001C3A RID: 7226 RVA: 0x000C574F File Offset: 0x000C394F
	public static int SecondsToSamples(float seconds)
	{
		return MusicUtil.SecondsToSamples(seconds, UnityEngine.AudioSettings.outputSampleRate);
	}

	// Token: 0x06001C3B RID: 7227 RVA: 0x000C575C File Offset: 0x000C395C
	public static int SecondsToSamples(float seconds, int sampleRate)
	{
		return (int)((float)sampleRate * seconds);
	}

	// Token: 0x06001C3C RID: 7228 RVA: 0x000C5763 File Offset: 0x000C3963
	public static int BarsToSamples(float tempo, float bars, int sampleRate)
	{
		return MusicUtil.SecondsToSamples(MusicUtil.BarsToSeconds(tempo, bars), sampleRate);
	}

	// Token: 0x06001C3D RID: 7229 RVA: 0x000C5772 File Offset: 0x000C3972
	public static int BarsToSamples(float tempo, float bars)
	{
		return MusicUtil.SecondsToSamples(MusicUtil.BarsToSeconds(tempo, bars));
	}

	// Token: 0x06001C3E RID: 7230 RVA: 0x000C5780 File Offset: 0x000C3980
	public static int BeatsToSamples(float tempo, float beats)
	{
		return MusicUtil.SecondsToSamples(MusicUtil.BeatsToSeconds(tempo, beats));
	}

	// Token: 0x06001C3F RID: 7231 RVA: 0x000C578E File Offset: 0x000C398E
	public static float SecondsToBeats(float tempo, double seconds)
	{
		return tempo / 60f * (float)seconds;
	}

	// Token: 0x06001C40 RID: 7232 RVA: 0x000C579A File Offset: 0x000C399A
	public static float SecondsToBars(float tempo, double seconds)
	{
		return MusicUtil.SecondsToBeats(tempo, seconds) / 4f;
	}

	// Token: 0x06001C41 RID: 7233 RVA: 0x000C57A9 File Offset: 0x000C39A9
	public static float Quantize(float position, float gridSize)
	{
		return Mathf.Round(position / gridSize) * gridSize;
	}

	// Token: 0x06001C42 RID: 7234 RVA: 0x000C57B5 File Offset: 0x000C39B5
	public static float FlooredQuantize(float position, float gridSize)
	{
		return Mathf.Floor(position / gridSize) * gridSize;
	}

	// Token: 0x0400147E RID: 5246
	public const float OneSixteenth = 0.0625f;
}
