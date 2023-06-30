using System;
using UnityEngine;

// Token: 0x02000756 RID: 1878
[CreateAssetMenu(menuName = "Rust/Hair Set Collection")]
public class HairSetCollection : ScriptableObject
{
	// Token: 0x06003463 RID: 13411 RVA: 0x00143B34 File Offset: 0x00141D34
	public HairSetCollection.HairSetEntry[] GetListByType(HairType hairType)
	{
		switch (hairType)
		{
		case HairType.Head:
			return this.Head;
		case HairType.Eyebrow:
			return this.Eyebrow;
		case HairType.Facial:
			return this.Facial;
		case HairType.Armpit:
			return this.Armpit;
		case HairType.Pubic:
			return this.Pubic;
		default:
			return null;
		}
	}

	// Token: 0x06003464 RID: 13412 RVA: 0x00143B81 File Offset: 0x00141D81
	public int GetIndex(HairSetCollection.HairSetEntry[] list, float typeNum)
	{
		return Mathf.Clamp(Mathf.FloorToInt(typeNum * (float)list.Length), 0, list.Length - 1);
	}

	// Token: 0x06003465 RID: 13413 RVA: 0x00143B9C File Offset: 0x00141D9C
	public int GetIndex(HairType hairType, float typeNum)
	{
		HairSetCollection.HairSetEntry[] listByType = this.GetListByType(hairType);
		return this.GetIndex(listByType, typeNum);
	}

	// Token: 0x06003466 RID: 13414 RVA: 0x00143BBC File Offset: 0x00141DBC
	public HairSetCollection.HairSetEntry Get(HairType hairType, float typeNum)
	{
		HairSetCollection.HairSetEntry[] listByType = this.GetListByType(hairType);
		return listByType[this.GetIndex(listByType, typeNum)];
	}

	// Token: 0x04002AD3 RID: 10963
	public HairSetCollection.HairSetEntry[] Head;

	// Token: 0x04002AD4 RID: 10964
	public HairSetCollection.HairSetEntry[] Eyebrow;

	// Token: 0x04002AD5 RID: 10965
	public HairSetCollection.HairSetEntry[] Facial;

	// Token: 0x04002AD6 RID: 10966
	public HairSetCollection.HairSetEntry[] Armpit;

	// Token: 0x04002AD7 RID: 10967
	public HairSetCollection.HairSetEntry[] Pubic;

	// Token: 0x02000E74 RID: 3700
	[Serializable]
	public struct HairSetEntry
	{
		// Token: 0x04004BF1 RID: 19441
		public HairSet HairSet;

		// Token: 0x04004BF2 RID: 19442
		public GameObjectRef HairPrefab;

		// Token: 0x04004BF3 RID: 19443
		public HairDyeCollection HairDyeCollection;
	}
}
