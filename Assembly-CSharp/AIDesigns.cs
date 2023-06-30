using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000364 RID: 868
public static class AIDesigns
{
	// Token: 0x06001FC7 RID: 8135 RVA: 0x000D64CC File Offset: 0x000D46CC
	public static ProtoBuf.AIDesign GetByNameOrInstance(string designName, ProtoBuf.AIDesign entityDesign)
	{
		if (entityDesign != null)
		{
			return entityDesign;
		}
		ProtoBuf.AIDesign byName = AIDesigns.GetByName(designName + "_custom");
		if (byName != null)
		{
			return byName;
		}
		return AIDesigns.GetByName(designName);
	}

	// Token: 0x06001FC8 RID: 8136 RVA: 0x000D64FA File Offset: 0x000D46FA
	public static void RefreshCache(string designName, ProtoBuf.AIDesign design)
	{
		if (!AIDesigns.designs.ContainsKey(designName))
		{
			return;
		}
		AIDesigns.designs[designName] = design;
	}

	// Token: 0x06001FC9 RID: 8137 RVA: 0x000D6518 File Offset: 0x000D4718
	private static ProtoBuf.AIDesign GetByName(string designName)
	{
		ProtoBuf.AIDesign aidesign;
		AIDesigns.designs.TryGetValue(designName, out aidesign);
		if (aidesign != null)
		{
			return aidesign;
		}
		string text = "cfg/ai/" + designName;
		if (!File.Exists(text))
		{
			return null;
		}
		try
		{
			using (FileStream fileStream = File.Open(text, FileMode.Open))
			{
				aidesign = ProtoBuf.AIDesign.Deserialize(fileStream);
				if (aidesign == null)
				{
					return null;
				}
				AIDesigns.designs.Add(designName, aidesign);
			}
		}
		catch (Exception)
		{
			Debug.LogWarning("Error trying to find AI design by name: " + text);
			return null;
		}
		return aidesign;
	}

	// Token: 0x0400190D RID: 6413
	public const string DesignFolderPath = "cfg/ai/";

	// Token: 0x0400190E RID: 6414
	private static Dictionary<string, ProtoBuf.AIDesign> designs = new Dictionary<string, ProtoBuf.AIDesign>();
}
