using System;
using System.Collections.Generic;
using System.IO;

// Token: 0x02000951 RID: 2385
public static class RawWriter
{
	// Token: 0x0600392C RID: 14636 RVA: 0x00153944 File Offset: 0x00151B44
	public static void Write(IEnumerable<byte> data, string path)
	{
		using (FileStream fileStream = File.Open(path, FileMode.Create))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
			{
				foreach (byte b in data)
				{
					binaryWriter.Write(b);
				}
			}
		}
	}

	// Token: 0x0600392D RID: 14637 RVA: 0x001539C8 File Offset: 0x00151BC8
	public static void Write(IEnumerable<int> data, string path)
	{
		using (FileStream fileStream = File.Open(path, FileMode.Create))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
			{
				foreach (int num in data)
				{
					binaryWriter.Write(num);
				}
			}
		}
	}

	// Token: 0x0600392E RID: 14638 RVA: 0x00153A4C File Offset: 0x00151C4C
	public static void Write(IEnumerable<short> data, string path)
	{
		using (FileStream fileStream = File.Open(path, FileMode.Create))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
			{
				foreach (short num in data)
				{
					binaryWriter.Write(num);
				}
			}
		}
	}

	// Token: 0x0600392F RID: 14639 RVA: 0x00153AD0 File Offset: 0x00151CD0
	public static void Write(IEnumerable<float> data, string path)
	{
		using (FileStream fileStream = File.Open(path, FileMode.Create))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
			{
				foreach (float num in data)
				{
					binaryWriter.Write(num);
				}
			}
		}
	}
}
