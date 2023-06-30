using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000769 RID: 1897
public class StringPool
{
	// Token: 0x060034A8 RID: 13480 RVA: 0x00144F70 File Offset: 0x00143170
	private static void Init()
	{
		if (StringPool.initialized)
		{
			return;
		}
		StringPool.toString = new Dictionary<uint, string>();
		StringPool.toNumber = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);
		GameManifest gameManifest = FileSystem.Load<GameManifest>("Assets/manifest.asset", true);
		uint num = 0U;
		while ((ulong)num < (ulong)((long)gameManifest.pooledStrings.Length))
		{
			string str = gameManifest.pooledStrings[(int)num].str;
			uint hash = gameManifest.pooledStrings[(int)num].hash;
			string text;
			if (StringPool.toString.TryGetValue(hash, out text))
			{
				if (str != text)
				{
					Debug.LogWarning(string.Format("Hash collision: {0} already exists in string pool. `{1}` and `{2}` have the same hash.", hash, str, text));
				}
			}
			else
			{
				StringPool.toString.Add(hash, str);
				StringPool.toNumber.Add(str, hash);
			}
			num += 1U;
		}
		StringPool.initialized = true;
		StringPool.closest = StringPool.Get("closest");
	}

	// Token: 0x060034A9 RID: 13481 RVA: 0x00145044 File Offset: 0x00143244
	public static string Get(uint i)
	{
		if (i == 0U)
		{
			return string.Empty;
		}
		StringPool.Init();
		string text;
		if (StringPool.toString.TryGetValue(i, out text))
		{
			return text;
		}
		Debug.LogWarning("StringPool.GetString - no string for ID" + i);
		return "";
	}

	// Token: 0x060034AA RID: 13482 RVA: 0x0014508C File Offset: 0x0014328C
	public static uint Get(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return 0U;
		}
		StringPool.Init();
		uint num;
		if (StringPool.toNumber.TryGetValue(str, out num))
		{
			return num;
		}
		Debug.LogWarning("StringPool.GetNumber - no number for string " + str);
		return 0U;
	}

	// Token: 0x060034AB RID: 13483 RVA: 0x001450CC File Offset: 0x001432CC
	public static uint Add(string str)
	{
		uint num = 0U;
		if (!StringPool.toNumber.TryGetValue(str, out num))
		{
			num = str.ManifestHash();
			StringPool.toString.Add(num, str);
			StringPool.toNumber.Add(str, num);
		}
		return num;
	}

	// Token: 0x04002B46 RID: 11078
	private static Dictionary<uint, string> toString;

	// Token: 0x04002B47 RID: 11079
	private static Dictionary<string, uint> toNumber;

	// Token: 0x04002B48 RID: 11080
	private static bool initialized;

	// Token: 0x04002B49 RID: 11081
	public static uint closest;
}
