using System;
using UnityEngine;

// Token: 0x0200072A RID: 1834
[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Impostor : MonoBehaviour, IClientComponent, IPrefabPreProcess
{
	// Token: 0x06003325 RID: 13093 RVA: 0x000063A5 File Offset: 0x000045A5
	private void OnEnable()
	{
	}

	// Token: 0x06003326 RID: 13094 RVA: 0x000063A5 File Offset: 0x000045A5
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
	}

	// Token: 0x040029F5 RID: 10741
	public ImpostorAsset asset;

	// Token: 0x040029F6 RID: 10742
	[Header("Baking")]
	public GameObject reference;

	// Token: 0x040029F7 RID: 10743
	public float angle;

	// Token: 0x040029F8 RID: 10744
	public int resolution = 1024;

	// Token: 0x040029F9 RID: 10745
	public int padding = 32;

	// Token: 0x040029FA RID: 10746
	public bool spriteOutlineAsMesh;
}
