using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x0200055A RID: 1370
public class Prefab : IComparable<Prefab>
{
	// Token: 0x06002A55 RID: 10837 RVA: 0x00102378 File Offset: 0x00100578
	public Prefab(string name, GameObject prefab, GameManager manager, PrefabAttribute.Library attribute)
	{
		this.ID = StringPool.Get(name);
		this.Name = name;
		this.Folder = (string.IsNullOrWhiteSpace(name) ? "" : Path.GetDirectoryName(name));
		this.Object = prefab;
		this.Manager = manager;
		this.Attribute = attribute;
		this.Parameters = (prefab ? prefab.GetComponent<PrefabParameters>() : null);
	}

	// Token: 0x06002A56 RID: 10838 RVA: 0x001023E6 File Offset: 0x001005E6
	public static implicit operator GameObject(Prefab prefab)
	{
		return prefab.Object;
	}

	// Token: 0x06002A57 RID: 10839 RVA: 0x001023F0 File Offset: 0x001005F0
	public int CompareTo(Prefab that)
	{
		if (that == null)
		{
			return 1;
		}
		PrefabPriority prefabPriority = ((this.Parameters != null) ? this.Parameters.Priority : PrefabPriority.Default);
		return ((that.Parameters != null) ? that.Parameters.Priority : PrefabPriority.Default).CompareTo(prefabPriority);
	}

	// Token: 0x06002A58 RID: 10840 RVA: 0x00102450 File Offset: 0x00100650
	public bool ApplyTerrainAnchors(ref Vector3 pos, Quaternion rot, Vector3 scale, TerrainAnchorMode mode, SpawnFilter filter = null)
	{
		TerrainAnchor[] array = this.Attribute.FindAll<TerrainAnchor>(this.ID);
		return this.Object.transform.ApplyTerrainAnchors(array, ref pos, rot, scale, mode, filter);
	}

	// Token: 0x06002A59 RID: 10841 RVA: 0x00102488 File Offset: 0x00100688
	public bool ApplyTerrainAnchors(ref Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter filter = null)
	{
		TerrainAnchor[] array = this.Attribute.FindAll<TerrainAnchor>(this.ID);
		return this.Object.transform.ApplyTerrainAnchors(array, ref pos, rot, scale, filter);
	}

	// Token: 0x06002A5A RID: 10842 RVA: 0x001024C0 File Offset: 0x001006C0
	public bool ApplyTerrainChecks(Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter filter = null)
	{
		TerrainCheck[] array = this.Attribute.FindAll<TerrainCheck>(this.ID);
		return this.Object.transform.ApplyTerrainChecks(array, pos, rot, scale, filter);
	}

	// Token: 0x06002A5B RID: 10843 RVA: 0x001024F8 File Offset: 0x001006F8
	public bool ApplyTerrainFilters(Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter filter = null)
	{
		TerrainFilter[] array = this.Attribute.FindAll<TerrainFilter>(this.ID);
		return this.Object.transform.ApplyTerrainFilters(array, pos, rot, scale, filter);
	}

	// Token: 0x06002A5C RID: 10844 RVA: 0x00102530 File Offset: 0x00100730
	public void ApplyTerrainModifiers(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		TerrainModifier[] array = this.Attribute.FindAll<TerrainModifier>(this.ID);
		this.Object.transform.ApplyTerrainModifiers(array, pos, rot, scale);
	}

	// Token: 0x06002A5D RID: 10845 RVA: 0x00102564 File Offset: 0x00100764
	public void ApplyTerrainPlacements(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		TerrainPlacement[] array = this.Attribute.FindAll<TerrainPlacement>(this.ID);
		this.Object.transform.ApplyTerrainPlacements(array, pos, rot, scale);
	}

	// Token: 0x06002A5E RID: 10846 RVA: 0x00102598 File Offset: 0x00100798
	public bool ApplyWaterChecks(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		WaterCheck[] array = this.Attribute.FindAll<WaterCheck>(this.ID);
		return this.Object.transform.ApplyWaterChecks(array, pos, rot, scale);
	}

	// Token: 0x06002A5F RID: 10847 RVA: 0x001025CC File Offset: 0x001007CC
	public bool ApplyBoundsChecks(Vector3 pos, Quaternion rot, Vector3 scale, LayerMask rejectOnLayer)
	{
		BoundsCheck[] array = this.Attribute.FindAll<BoundsCheck>(this.ID);
		BaseEntity component = this.Object.GetComponent<BaseEntity>();
		return !(component != null) || component.ApplyBoundsChecks(array, pos, rot, scale, rejectOnLayer);
	}

	// Token: 0x06002A60 RID: 10848 RVA: 0x00102610 File Offset: 0x00100810
	public void ApplyDecorComponents(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		DecorComponent[] array = this.Attribute.FindAll<DecorComponent>(this.ID);
		this.Object.transform.ApplyDecorComponents(array, ref pos, ref rot, ref scale);
	}

	// Token: 0x06002A61 RID: 10849 RVA: 0x00102643 File Offset: 0x00100843
	public bool CheckEnvironmentVolumes(Vector3 pos, Quaternion rot, Vector3 scale, EnvironmentType type)
	{
		return this.Object.transform.CheckEnvironmentVolumes(pos, rot, scale, type);
	}

	// Token: 0x06002A62 RID: 10850 RVA: 0x0010265A File Offset: 0x0010085A
	public bool CheckEnvironmentVolumesInsideTerrain(Vector3 pos, Quaternion rot, Vector3 scale, EnvironmentType type, float padding = 0f)
	{
		return this.Object.transform.CheckEnvironmentVolumesInsideTerrain(pos, rot, scale, type, padding);
	}

	// Token: 0x06002A63 RID: 10851 RVA: 0x00102673 File Offset: 0x00100873
	public bool CheckEnvironmentVolumesOutsideTerrain(Vector3 pos, Quaternion rot, Vector3 scale, EnvironmentType type, float padding = 0f)
	{
		return this.Object.transform.CheckEnvironmentVolumesOutsideTerrain(pos, rot, scale, type, padding);
	}

	// Token: 0x06002A64 RID: 10852 RVA: 0x0010268C File Offset: 0x0010088C
	public void ApplySequenceReplacement(List<Prefab> sequence, ref Prefab replacement, Prefab[] possibleReplacements, int pathLength, int pathIndex)
	{
		PathSequence pathSequence = this.Attribute.Find<PathSequence>(this.ID);
		if (pathSequence != null)
		{
			pathSequence.ApplySequenceReplacement(sequence, ref replacement, possibleReplacements, pathLength, pathIndex);
		}
	}

	// Token: 0x06002A65 RID: 10853 RVA: 0x001026C1 File Offset: 0x001008C1
	public GameObject Spawn(Transform transform, bool active = true)
	{
		return this.Manager.CreatePrefab(this.Name, transform, active);
	}

	// Token: 0x06002A66 RID: 10854 RVA: 0x001026D6 File Offset: 0x001008D6
	public GameObject Spawn(Vector3 pos, Quaternion rot, bool active = true)
	{
		return this.Manager.CreatePrefab(this.Name, pos, rot, active);
	}

	// Token: 0x06002A67 RID: 10855 RVA: 0x001026EC File Offset: 0x001008EC
	public GameObject Spawn(Vector3 pos, Quaternion rot, Vector3 scale, bool active = true)
	{
		return this.Manager.CreatePrefab(this.Name, pos, rot, scale, active);
	}

	// Token: 0x06002A68 RID: 10856 RVA: 0x00102704 File Offset: 0x00100904
	public BaseEntity SpawnEntity(Vector3 pos, Quaternion rot, bool active = true)
	{
		return this.Manager.CreateEntity(this.Name, pos, rot, active);
	}

	// Token: 0x06002A69 RID: 10857 RVA: 0x0010271C File Offset: 0x0010091C
	public static Prefab<T> Load<T>(uint id, GameManager manager = null, PrefabAttribute.Library attribute = null) where T : Component
	{
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		string text = StringPool.Get(id);
		if (string.IsNullOrWhiteSpace(text))
		{
			Debug.LogWarning(string.Format("Could not find path for prefab ID {0}", id));
			return null;
		}
		GameObject gameObject = manager.FindPrefab(text);
		T component = gameObject.GetComponent<T>();
		return new Prefab<T>(text, gameObject, component, manager, attribute);
	}

	// Token: 0x06002A6A RID: 10858 RVA: 0x0010277C File Offset: 0x0010097C
	public static Prefab Load(uint id, GameManager manager = null, PrefabAttribute.Library attribute = null)
	{
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		string text = StringPool.Get(id);
		if (string.IsNullOrWhiteSpace(text))
		{
			Debug.LogWarning(string.Format("Could not find path for prefab ID {0}", id));
			return null;
		}
		GameObject gameObject = manager.FindPrefab(text);
		return new Prefab(text, gameObject, manager, attribute);
	}

	// Token: 0x06002A6B RID: 10859 RVA: 0x001027D4 File Offset: 0x001009D4
	public static Prefab[] Load(string folder, GameManager manager = null, PrefabAttribute.Library attribute = null, bool useProbabilities = true)
	{
		if (string.IsNullOrEmpty(folder))
		{
			return null;
		}
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		string[] array = Prefab.FindPrefabNames(folder, useProbabilities);
		Prefab[] array2 = new Prefab[array.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			string text = array[i];
			GameObject gameObject = manager.FindPrefab(text);
			array2[i] = new Prefab(text, gameObject, manager, attribute);
		}
		return array2;
	}

	// Token: 0x06002A6C RID: 10860 RVA: 0x00102839 File Offset: 0x00100A39
	public static Prefab<T>[] Load<T>(string folder, GameManager manager = null, PrefabAttribute.Library attribute = null, bool useProbabilities = true) where T : Component
	{
		if (string.IsNullOrEmpty(folder))
		{
			return null;
		}
		return Prefab.Load<T>(Prefab.FindPrefabNames(folder, useProbabilities), manager, attribute);
	}

	// Token: 0x06002A6D RID: 10861 RVA: 0x00102854 File Offset: 0x00100A54
	public static Prefab<T>[] Load<T>(string[] names, GameManager manager = null, PrefabAttribute.Library attribute = null) where T : Component
	{
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		Prefab<T>[] array = new Prefab<T>[names.Length];
		for (int i = 0; i < array.Length; i++)
		{
			string text = names[i];
			GameObject gameObject = manager.FindPrefab(text);
			T component = gameObject.GetComponent<T>();
			array[i] = new Prefab<T>(text, gameObject, component, manager, attribute);
		}
		return array;
	}

	// Token: 0x06002A6E RID: 10862 RVA: 0x001028B0 File Offset: 0x00100AB0
	public static Prefab LoadRandom(string folder, ref uint seed, GameManager manager = null, PrefabAttribute.Library attribute = null, bool useProbabilities = true)
	{
		if (string.IsNullOrEmpty(folder))
		{
			return null;
		}
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		string[] array = Prefab.FindPrefabNames(folder, useProbabilities);
		if (array.Length == 0)
		{
			return null;
		}
		string text = array[SeedRandom.Range(ref seed, 0, array.Length)];
		GameObject gameObject = manager.FindPrefab(text);
		return new Prefab(text, gameObject, manager, attribute);
	}

	// Token: 0x06002A6F RID: 10863 RVA: 0x00102908 File Offset: 0x00100B08
	public static Prefab<T> LoadRandom<T>(string folder, ref uint seed, GameManager manager = null, PrefabAttribute.Library attribute = null, bool useProbabilities = true) where T : Component
	{
		if (string.IsNullOrEmpty(folder))
		{
			return null;
		}
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		string[] array = Prefab.FindPrefabNames(folder, useProbabilities);
		if (array.Length == 0)
		{
			return null;
		}
		string text = array[SeedRandom.Range(ref seed, 0, array.Length)];
		GameObject gameObject = manager.FindPrefab(text);
		T component = gameObject.GetComponent<T>();
		return new Prefab<T>(text, gameObject, component, manager, attribute);
	}

	// Token: 0x1700038D RID: 909
	// (get) Token: 0x06002A70 RID: 10864 RVA: 0x00102968 File Offset: 0x00100B68
	public static PrefabAttribute.Library DefaultAttribute
	{
		get
		{
			return PrefabAttribute.server;
		}
	}

	// Token: 0x1700038E RID: 910
	// (get) Token: 0x06002A71 RID: 10865 RVA: 0x0010296F File Offset: 0x00100B6F
	public static GameManager DefaultManager
	{
		get
		{
			return GameManager.server;
		}
	}

	// Token: 0x06002A72 RID: 10866 RVA: 0x00102978 File Offset: 0x00100B78
	private static string[] FindPrefabNames(string strPrefab, bool useProbabilities = false)
	{
		strPrefab = strPrefab.TrimEnd(new char[] { '/' }).ToLower();
		GameObject[] array = FileSystem.LoadPrefabs(strPrefab + "/");
		List<string> list = new List<string>(array.Length);
		foreach (GameObject gameObject in array)
		{
			string text = strPrefab + "/" + gameObject.name.ToLower() + ".prefab";
			if (!useProbabilities)
			{
				list.Add(text);
			}
			else
			{
				PrefabParameters component = gameObject.GetComponent<PrefabParameters>();
				int num = (component ? component.Count : 1);
				for (int j = 0; j < num; j++)
				{
					list.Add(text);
				}
			}
		}
		list.Sort();
		return list.ToArray();
	}

	// Token: 0x040022AD RID: 8877
	public uint ID;

	// Token: 0x040022AE RID: 8878
	public string Name;

	// Token: 0x040022AF RID: 8879
	public string Folder;

	// Token: 0x040022B0 RID: 8880
	public GameObject Object;

	// Token: 0x040022B1 RID: 8881
	public GameManager Manager;

	// Token: 0x040022B2 RID: 8882
	public PrefabAttribute.Library Attribute;

	// Token: 0x040022B3 RID: 8883
	public PrefabParameters Parameters;
}
