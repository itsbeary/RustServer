using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000221 RID: 545
[CreateAssetMenu(menuName = "Rust/Ambience Definition")]
public class AmbienceDefinition : ScriptableObject
{
	// Token: 0x040013BE RID: 5054
	[Header("Sound")]
	public List<SoundDefinition> sounds;

	// Token: 0x040013BF RID: 5055
	[Horizontal(2, -1)]
	public AmbienceDefinition.ValueRange stingFrequency = new AmbienceDefinition.ValueRange(15f, 30f);

	// Token: 0x040013C0 RID: 5056
	[Header("Environment")]
	[InspectorFlags]
	public TerrainBiome.Enum biomes = (TerrainBiome.Enum)(-1);

	// Token: 0x040013C1 RID: 5057
	[InspectorFlags]
	public TerrainTopology.Enum topologies = (TerrainTopology.Enum)(-1);

	// Token: 0x040013C2 RID: 5058
	public EnvironmentType environmentType = EnvironmentType.Underground;

	// Token: 0x040013C3 RID: 5059
	public bool useEnvironmentType;

	// Token: 0x040013C4 RID: 5060
	public AnimationCurve time = AnimationCurve.Linear(0f, 0f, 24f, 0f);

	// Token: 0x040013C5 RID: 5061
	[Horizontal(2, -1)]
	public AmbienceDefinition.ValueRange rain = new AmbienceDefinition.ValueRange(0f, 1f);

	// Token: 0x040013C6 RID: 5062
	[Horizontal(2, -1)]
	public AmbienceDefinition.ValueRange wind = new AmbienceDefinition.ValueRange(0f, 1f);

	// Token: 0x040013C7 RID: 5063
	[Horizontal(2, -1)]
	public AmbienceDefinition.ValueRange snow = new AmbienceDefinition.ValueRange(0f, 1f);

	// Token: 0x02000C85 RID: 3205
	[Serializable]
	public class ValueRange
	{
		// Token: 0x06004F41 RID: 20289 RVA: 0x001A61E6 File Offset: 0x001A43E6
		public ValueRange(float min, float max)
		{
			this.min = min;
			this.max = max;
		}

		// Token: 0x040043FE RID: 17406
		public float min;

		// Token: 0x040043FF RID: 17407
		public float max;
	}
}
