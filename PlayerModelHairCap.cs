using System;
using UnityEngine;

// Token: 0x020002DC RID: 732
public class PlayerModelHairCap : MonoBehaviour
{
	// Token: 0x06001DFF RID: 7679 RVA: 0x000CD130 File Offset: 0x000CB330
	public void SetupHairCap(SkinSetCollection skin, float hairNum, float meshNum, MaterialPropertyBlock block)
	{
		int index = skin.GetIndex(meshNum);
		SkinSet skinSet = skin.Skins[index];
		if (skinSet == null)
		{
			return;
		}
		for (int i = 0; i < 5; i++)
		{
			if ((this.hairCapMask & (HairCapMask)(1 << i)) != (HairCapMask)0)
			{
				float num;
				float num2;
				PlayerModelHair.GetRandomVariation(hairNum, i, index, out num, out num2);
				HairType hairType = (HairType)i;
				HairSetCollection.HairSetEntry hairSetEntry = skinSet.HairCollection.Get(hairType, num);
				if (!(hairSetEntry.HairSet == null))
				{
					HairDyeCollection hairDyeCollection = hairSetEntry.HairDyeCollection;
					if (!(hairDyeCollection == null))
					{
						HairDye hairDye = hairDyeCollection.Get(num2);
						if (hairDye != null)
						{
							hairDye.ApplyCap(hairDyeCollection, hairType, block);
						}
					}
				}
			}
		}
	}

	// Token: 0x040016FB RID: 5883
	[InspectorFlags]
	public HairCapMask hairCapMask;
}
