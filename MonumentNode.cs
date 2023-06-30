using System;
using UnityEngine;

// Token: 0x020006BC RID: 1724
public class MonumentNode : MonoBehaviour
{
	// Token: 0x060031B6 RID: 12726 RVA: 0x001299AC File Offset: 0x00127BAC
	protected void Awake()
	{
		if (!(SingletonComponent<WorldSetup>.Instance == null))
		{
			if (SingletonComponent<WorldSetup>.Instance.MonumentNodes == null)
			{
				Debug.LogError("WorldSetup.Instance.MonumentNodes is null.", this);
				return;
			}
			SingletonComponent<WorldSetup>.Instance.MonumentNodes.Add(this);
		}
	}

	// Token: 0x060031B7 RID: 12727 RVA: 0x001299E4 File Offset: 0x00127BE4
	public void Process(ref uint seed)
	{
		if (World.Networked)
		{
			World.Spawn("Monument", "assets/bundled/prefabs/autospawn/" + this.ResourceFolder + "/");
			return;
		}
		Prefab<MonumentInfo>[] array = Prefab.Load<MonumentInfo>("assets/bundled/prefabs/autospawn/" + this.ResourceFolder, null, null, true);
		if (array == null || array.Length == 0)
		{
			return;
		}
		Prefab<MonumentInfo> random = array.GetRandom(ref seed);
		float height = TerrainMeta.HeightMap.GetHeight(base.transform.position);
		Vector3 vector = new Vector3(base.transform.position.x, height, base.transform.position.z);
		Quaternion localRotation = random.Object.transform.localRotation;
		Vector3 localScale = random.Object.transform.localScale;
		random.ApplyDecorComponents(ref vector, ref localRotation, ref localScale);
		World.AddPrefab("Monument", random, vector, localRotation, localScale);
	}

	// Token: 0x04002862 RID: 10338
	public string ResourceFolder = string.Empty;
}
