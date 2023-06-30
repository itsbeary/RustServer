using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000754 RID: 1876
[CreateAssetMenu(menuName = "Rust/Hair Set")]
public class HairSet : ScriptableObject
{
	// Token: 0x06003460 RID: 13408 RVA: 0x00143A40 File Offset: 0x00141C40
	public void Process(PlayerModelHair playerModelHair, HairDyeCollection dyeCollection, HairDye dye, MaterialPropertyBlock block)
	{
		List<SkinnedMeshRenderer> list = Pool.GetList<SkinnedMeshRenderer>();
		playerModelHair.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true, list);
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in list)
		{
			if (!(skinnedMeshRenderer.sharedMesh == null) && !(skinnedMeshRenderer.sharedMaterial == null))
			{
				string name = skinnedMeshRenderer.sharedMesh.name;
				string name2 = skinnedMeshRenderer.sharedMaterial.name;
				if (!skinnedMeshRenderer.gameObject.activeSelf)
				{
					skinnedMeshRenderer.gameObject.SetActive(true);
				}
				for (int i = 0; i < this.MeshReplacements.Length; i++)
				{
					this.MeshReplacements[i].Test(name);
				}
				if (dye != null && skinnedMeshRenderer.gameObject.activeSelf)
				{
					dye.Apply(dyeCollection, block);
				}
			}
		}
		Pool.FreeList<SkinnedMeshRenderer>(ref list);
	}

	// Token: 0x06003461 RID: 13409 RVA: 0x000063A5 File Offset: 0x000045A5
	public void ProcessMorphs(GameObject obj, int blendShapeIndex = -1)
	{
	}

	// Token: 0x04002ACB RID: 10955
	public HairSet.MeshReplace[] MeshReplacements;

	// Token: 0x02000E73 RID: 3699
	[Serializable]
	public class MeshReplace
	{
		// Token: 0x060052CA RID: 21194 RVA: 0x001B115A File Offset: 0x001AF35A
		public bool Test(string materialName)
		{
			return this.FindName == materialName;
		}

		// Token: 0x04004BEE RID: 19438
		[HideInInspector]
		public string FindName;

		// Token: 0x04004BEF RID: 19439
		public Mesh Find;

		// Token: 0x04004BF0 RID: 19440
		public Mesh[] ReplaceShapes;
	}
}
