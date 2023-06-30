using System;
using UnityEngine;

// Token: 0x020003A8 RID: 936
public class PreloadedCassetteContent : ScriptableObject
{
	// Token: 0x06002115 RID: 8469 RVA: 0x000D993C File Offset: 0x000D7B3C
	public SoundDefinition GetSoundContent(int index, PreloadedCassetteContent.PreloadType type)
	{
		switch (type)
		{
		case PreloadedCassetteContent.PreloadType.Short:
			return this.GetDefinition(index, this.ShortTapeContent);
		case PreloadedCassetteContent.PreloadType.Medium:
			return this.GetDefinition(index, this.MediumTapeContent);
		case PreloadedCassetteContent.PreloadType.Long:
			return this.GetDefinition(index, this.LongTapeContent);
		default:
			return null;
		}
	}

	// Token: 0x06002116 RID: 8470 RVA: 0x000D9988 File Offset: 0x000D7B88
	private SoundDefinition GetDefinition(int index, SoundDefinition[] array)
	{
		index = Mathf.Clamp(index, 0, array.Length);
		return array[index];
	}

	// Token: 0x06002117 RID: 8471 RVA: 0x000D999C File Offset: 0x000D7B9C
	public uint GetSoundContentId(SoundDefinition def)
	{
		uint num = 0U;
		SoundDefinition[] array = this.ShortTapeContent;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == def)
			{
				return num;
			}
			num += 1U;
		}
		array = this.MediumTapeContent;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == def)
			{
				return num;
			}
			num += 1U;
		}
		array = this.LongTapeContent;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == def)
			{
				return num;
			}
			num += 1U;
		}
		return num;
	}

	// Token: 0x06002118 RID: 8472 RVA: 0x000D9A20 File Offset: 0x000D7C20
	public SoundDefinition GetSoundContent(uint id)
	{
		int num = 0;
		foreach (SoundDefinition soundDefinition in this.ShortTapeContent)
		{
			if ((long)num++ == (long)((ulong)id))
			{
				return soundDefinition;
			}
		}
		foreach (SoundDefinition soundDefinition2 in this.MediumTapeContent)
		{
			if ((long)num++ == (long)((ulong)id))
			{
				return soundDefinition2;
			}
		}
		foreach (SoundDefinition soundDefinition3 in this.LongTapeContent)
		{
			if ((long)num++ == (long)((ulong)id))
			{
				return soundDefinition3;
			}
		}
		return null;
	}

	// Token: 0x040019E7 RID: 6631
	public SoundDefinition[] ShortTapeContent;

	// Token: 0x040019E8 RID: 6632
	public SoundDefinition[] MediumTapeContent;

	// Token: 0x040019E9 RID: 6633
	public SoundDefinition[] LongTapeContent;

	// Token: 0x02000CD1 RID: 3281
	public enum PreloadType
	{
		// Token: 0x0400457D RID: 17789
		Short,
		// Token: 0x0400457E RID: 17790
		Medium,
		// Token: 0x0400457F RID: 17791
		Long
	}
}
