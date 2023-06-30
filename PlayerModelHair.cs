using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020002DA RID: 730
public class PlayerModelHair : MonoBehaviour
{
	// Token: 0x17000275 RID: 629
	// (get) Token: 0x06001DF8 RID: 7672 RVA: 0x000CCF60 File Offset: 0x000CB160
	public Dictionary<Renderer, PlayerModelHair.RendererMaterials> Materials
	{
		get
		{
			return this.materials;
		}
	}

	// Token: 0x06001DF9 RID: 7673 RVA: 0x000CCF68 File Offset: 0x000CB168
	private void CacheOriginalMaterials()
	{
		if (this.materials != null)
		{
			return;
		}
		List<SkinnedMeshRenderer> list = Pool.GetList<SkinnedMeshRenderer>();
		base.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true, list);
		this.materials = new Dictionary<Renderer, PlayerModelHair.RendererMaterials>();
		this.materials.Clear();
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in list)
		{
			this.materials.Add(skinnedMeshRenderer, new PlayerModelHair.RendererMaterials(skinnedMeshRenderer));
		}
		Pool.FreeList<SkinnedMeshRenderer>(ref list);
	}

	// Token: 0x06001DFA RID: 7674 RVA: 0x000CCFFC File Offset: 0x000CB1FC
	private void Setup(HairType type, HairSetCollection hair, int meshIndex, float typeNum, float dyeNum, MaterialPropertyBlock block)
	{
		this.CacheOriginalMaterials();
		HairSetCollection.HairSetEntry hairSetEntry = hair.Get(type, typeNum);
		if (hairSetEntry.HairSet == null)
		{
			Debug.LogWarning("Hair.Get returned a NULL hair");
			return;
		}
		int num = -1;
		if (type == HairType.Facial || type == HairType.Eyebrow)
		{
			num = meshIndex;
		}
		HairDye hairDye = null;
		HairDyeCollection hairDyeCollection = hairSetEntry.HairDyeCollection;
		if (hairDyeCollection != null)
		{
			hairDye = hairDyeCollection.Get(dyeNum);
		}
		hairSetEntry.HairSet.Process(this, hairDyeCollection, hairDye, block);
		hairSetEntry.HairSet.ProcessMorphs(base.gameObject, num);
	}

	// Token: 0x06001DFB RID: 7675 RVA: 0x000CD07C File Offset: 0x000CB27C
	public void Setup(SkinSetCollection skin, float hairNum, float meshNum, MaterialPropertyBlock block)
	{
		int index = skin.GetIndex(meshNum);
		SkinSet skinSet = skin.Skins[index];
		if (skinSet == null)
		{
			Debug.LogError("Skin.Get returned a NULL skin");
			return;
		}
		int num = (int)this.type;
		float num2;
		float num3;
		PlayerModelHair.GetRandomVariation(hairNum, num, index, out num2, out num3);
		this.Setup(this.type, skinSet.HairCollection, index, num2, num3, block);
	}

	// Token: 0x06001DFC RID: 7676 RVA: 0x000CD0D9 File Offset: 0x000CB2D9
	public static void GetRandomVariation(float hairNum, int typeIndex, int meshIndex, out float typeNum, out float dyeNum)
	{
		int num = Mathf.FloorToInt(hairNum * 100000f);
		typeNum = PlayerModelHair.GetRandomHairType(hairNum, typeIndex);
		UnityEngine.Random.InitState(num + meshIndex);
		dyeNum = UnityEngine.Random.Range(0f, 1f);
	}

	// Token: 0x06001DFD RID: 7677 RVA: 0x000CD109 File Offset: 0x000CB309
	public static float GetRandomHairType(float hairNum, int typeIndex)
	{
		UnityEngine.Random.InitState(Mathf.FloorToInt(hairNum * 100000f) + typeIndex);
		return UnityEngine.Random.Range(0f, 1f);
	}

	// Token: 0x040016F3 RID: 5875
	public HairType type;

	// Token: 0x040016F4 RID: 5876
	private Dictionary<Renderer, PlayerModelHair.RendererMaterials> materials;

	// Token: 0x02000CB1 RID: 3249
	public struct RendererMaterials
	{
		// Token: 0x06004F7F RID: 20351 RVA: 0x001A6D44 File Offset: 0x001A4F44
		public RendererMaterials(Renderer r)
		{
			this.original = r.sharedMaterials;
			this.replacement = this.original.Clone() as Material[];
			this.names = new string[this.original.Length];
			for (int i = 0; i < this.original.Length; i++)
			{
				this.names[i] = this.original[i].name;
			}
		}

		// Token: 0x040044E3 RID: 17635
		public string[] names;

		// Token: 0x040044E4 RID: 17636
		public Material[] original;

		// Token: 0x040044E5 RID: 17637
		public Material[] replacement;
	}
}
