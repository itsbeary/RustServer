using System;
using UnityEngine;

// Token: 0x020003F8 RID: 1016
[CreateAssetMenu(menuName = "Rust/Growable Gene Properties")]
public class GrowableGeneProperties : ScriptableObject
{
	// Token: 0x04001AC4 RID: 6852
	[ArrayIndexIsEnum(enumType = typeof(GrowableGenetics.GeneType))]
	public GrowableGeneProperties.GeneWeight[] Weights = new GrowableGeneProperties.GeneWeight[5];

	// Token: 0x02000CE0 RID: 3296
	[Serializable]
	public struct GeneWeight
	{
		// Token: 0x040045AE RID: 17838
		public float BaseWeight;

		// Token: 0x040045AF RID: 17839
		public float[] SlotWeights;

		// Token: 0x040045B0 RID: 17840
		public float CrossBreedingWeight;
	}
}
