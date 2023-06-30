using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Token: 0x02000751 RID: 1873
public class GameManifest : ScriptableObject
{
	// Token: 0x17000445 RID: 1093
	// (get) Token: 0x0600344F RID: 13391 RVA: 0x00143301 File Offset: 0x00141501
	public static GameManifest Current
	{
		get
		{
			if (GameManifest.loadedManifest != null)
			{
				return GameManifest.loadedManifest;
			}
			GameManifest.Load();
			return GameManifest.loadedManifest;
		}
	}

	// Token: 0x06003450 RID: 13392 RVA: 0x00143320 File Offset: 0x00141520
	public static void Load()
	{
		if (GameManifest.loadedManifest != null)
		{
			return;
		}
		GameManifest.loadedManifest = FileSystem.Load<GameManifest>("Assets/manifest.asset", true);
		foreach (GameManifest.PrefabProperties prefabProperties in GameManifest.loadedManifest.prefabProperties)
		{
			GameManifest.guidToPath.Add(prefabProperties.guid, prefabProperties.name);
			GameManifest.pathToGuid.Add(prefabProperties.name, prefabProperties.guid);
		}
		foreach (GameManifest.GuidPath guidPath in GameManifest.loadedManifest.guidPaths)
		{
			if (!GameManifest.guidToPath.ContainsKey(guidPath.guid))
			{
				GameManifest.guidToPath.Add(guidPath.guid, guidPath.name);
				GameManifest.pathToGuid.Add(guidPath.name, guidPath.guid);
			}
		}
		DebugEx.Log(GameManifest.GetMetadataStatus(), StackTraceLogType.None);
	}

	// Token: 0x06003451 RID: 13393 RVA: 0x00143400 File Offset: 0x00141600
	public static void LoadAssets()
	{
		if (Skinnable.All != null)
		{
			return;
		}
		Skinnable.All = FileSystem.LoadAllFromBundle<Skinnable>("skinnables.preload.bundle", "t:Skinnable");
		if (Skinnable.All == null || Skinnable.All.Length == 0)
		{
			throw new Exception("Error loading skinnables");
		}
		DebugEx.Log(GameManifest.GetAssetStatus(), StackTraceLogType.None);
	}

	// Token: 0x06003452 RID: 13394 RVA: 0x00143450 File Offset: 0x00141650
	internal static Dictionary<string, string[]> LoadEffectDictionary()
	{
		GameManifest.EffectCategory[] array = GameManifest.loadedManifest.effectCategories;
		Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
		foreach (GameManifest.EffectCategory effectCategory in array)
		{
			dictionary.Add(effectCategory.folder, effectCategory.prefabs.ToArray());
		}
		return dictionary;
	}

	// Token: 0x06003453 RID: 13395 RVA: 0x00143498 File Offset: 0x00141698
	internal static string GUIDToPath(string guid)
	{
		if (string.IsNullOrEmpty(guid))
		{
			Debug.LogError("GUIDToPath: guid is empty");
			return string.Empty;
		}
		GameManifest.Load();
		string text;
		if (GameManifest.guidToPath.TryGetValue(guid, out text))
		{
			return text;
		}
		Debug.LogWarning("GUIDToPath: no path found for guid " + guid);
		return string.Empty;
	}

	// Token: 0x06003454 RID: 13396 RVA: 0x001434E8 File Offset: 0x001416E8
	internal static UnityEngine.Object GUIDToObject(string guid)
	{
		UnityEngine.Object @object = null;
		if (GameManifest.guidToObject.TryGetValue(guid, out @object))
		{
			return @object;
		}
		string text = GameManifest.GUIDToPath(guid);
		if (string.IsNullOrEmpty(text))
		{
			Debug.LogWarning("Missing file for guid " + guid);
			GameManifest.guidToObject.Add(guid, null);
			return null;
		}
		UnityEngine.Object object2 = FileSystem.Load<UnityEngine.Object>(text, true);
		GameManifest.guidToObject.Add(guid, object2);
		return object2;
	}

	// Token: 0x06003455 RID: 13397 RVA: 0x0014354C File Offset: 0x0014174C
	internal static void Invalidate(string path)
	{
		string text;
		if (!GameManifest.pathToGuid.TryGetValue(path, out text))
		{
			return;
		}
		UnityEngine.Object @object;
		if (!GameManifest.guidToObject.TryGetValue(text, out @object))
		{
			return;
		}
		if (@object != null)
		{
			UnityEngine.Object.DestroyImmediate(@object, true);
		}
		GameManifest.guidToObject.Remove(text);
	}

	// Token: 0x06003456 RID: 13398 RVA: 0x00143598 File Offset: 0x00141798
	private static string GetMetadataStatus()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (GameManifest.loadedManifest != null)
		{
			stringBuilder.Append("Manifest Metadata Loaded");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			stringBuilder.Append(GameManifest.loadedManifest.pooledStrings.Length.ToString());
			stringBuilder.Append(" pooled strings");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			stringBuilder.Append(GameManifest.loadedManifest.prefabProperties.Length.ToString());
			stringBuilder.Append(" prefab properties");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			stringBuilder.Append(GameManifest.loadedManifest.effectCategories.Length.ToString());
			stringBuilder.Append(" effect categories");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			stringBuilder.Append(GameManifest.loadedManifest.entities.Length.ToString());
			stringBuilder.Append(" entity names");
			stringBuilder.AppendLine();
		}
		else
		{
			stringBuilder.Append("Manifest Metadata Missing");
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06003457 RID: 13399 RVA: 0x001436CC File Offset: 0x001418CC
	private static string GetAssetStatus()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (GameManifest.loadedManifest != null)
		{
			stringBuilder.Append("Manifest Assets Loaded");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			stringBuilder.Append((Skinnable.All != null) ? Skinnable.All.Length.ToString() : "0");
			stringBuilder.Append(" skinnable objects");
		}
		else
		{
			stringBuilder.Append("Manifest Assets Missing");
		}
		return stringBuilder.ToString();
	}

	// Token: 0x04002AB7 RID: 10935
	internal static GameManifest loadedManifest;

	// Token: 0x04002AB8 RID: 10936
	internal static Dictionary<string, string> guidToPath = new Dictionary<string, string>();

	// Token: 0x04002AB9 RID: 10937
	internal static Dictionary<string, string> pathToGuid = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

	// Token: 0x04002ABA RID: 10938
	internal static Dictionary<string, UnityEngine.Object> guidToObject = new Dictionary<string, UnityEngine.Object>();

	// Token: 0x04002ABB RID: 10939
	public GameManifest.PooledString[] pooledStrings;

	// Token: 0x04002ABC RID: 10940
	public GameManifest.PrefabProperties[] prefabProperties;

	// Token: 0x04002ABD RID: 10941
	public GameManifest.EffectCategory[] effectCategories;

	// Token: 0x04002ABE RID: 10942
	public GameManifest.GuidPath[] guidPaths;

	// Token: 0x04002ABF RID: 10943
	public string[] entities;

	// Token: 0x02000E6D RID: 3693
	[Serializable]
	public struct PooledString
	{
		// Token: 0x04004BD1 RID: 19409
		[HideInInspector]
		public string str;

		// Token: 0x04004BD2 RID: 19410
		public uint hash;
	}

	// Token: 0x02000E6E RID: 3694
	[Serializable]
	public class PrefabProperties
	{
		// Token: 0x04004BD3 RID: 19411
		[HideInInspector]
		public string name;

		// Token: 0x04004BD4 RID: 19412
		public string guid;

		// Token: 0x04004BD5 RID: 19413
		public uint hash;

		// Token: 0x04004BD6 RID: 19414
		public bool pool;
	}

	// Token: 0x02000E6F RID: 3695
	[Serializable]
	public class EffectCategory
	{
		// Token: 0x04004BD7 RID: 19415
		[HideInInspector]
		public string folder;

		// Token: 0x04004BD8 RID: 19416
		public List<string> prefabs;
	}

	// Token: 0x02000E70 RID: 3696
	[Serializable]
	public class GuidPath
	{
		// Token: 0x04004BD9 RID: 19417
		[HideInInspector]
		public string name;

		// Token: 0x04004BDA RID: 19418
		public string guid;
	}
}
