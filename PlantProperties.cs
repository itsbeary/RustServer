using System;
using UnityEngine;

// Token: 0x02000444 RID: 1092
[CreateAssetMenu(menuName = "Rust/Plant Properties")]
public class PlantProperties : ScriptableObject
{
	// Token: 0x04001CA9 RID: 7337
	public Translate.Phrase Description;

	// Token: 0x04001CAA RID: 7338
	public GrowableGeneProperties Genes;

	// Token: 0x04001CAB RID: 7339
	[ArrayIndexIsEnum(enumType = typeof(PlantProperties.State))]
	public PlantProperties.Stage[] stages = new PlantProperties.Stage[8];

	// Token: 0x04001CAC RID: 7340
	[Header("Metabolism")]
	public AnimationCurve timeOfDayHappiness = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(12f, 1f),
		new Keyframe(24f, 0f)
	});

	// Token: 0x04001CAD RID: 7341
	public AnimationCurve temperatureHappiness = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(-10f, -1f),
		new Keyframe(1f, 0f),
		new Keyframe(30f, 1f),
		new Keyframe(50f, 0f),
		new Keyframe(80f, -1f)
	});

	// Token: 0x04001CAE RID: 7342
	public AnimationCurve temperatureWaterRequirementMultiplier = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(-10f, 1f),
		new Keyframe(0f, 1f),
		new Keyframe(30f, 1f),
		new Keyframe(50f, 1f),
		new Keyframe(80f, 1f)
	});

	// Token: 0x04001CAF RID: 7343
	public AnimationCurve fruitVisualScaleCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.75f, 1f),
		new Keyframe(1f, 0f)
	});

	// Token: 0x04001CB0 RID: 7344
	public int MaxSeasons = 1;

	// Token: 0x04001CB1 RID: 7345
	public float WaterIntake = 20f;

	// Token: 0x04001CB2 RID: 7346
	public float OptimalLightQuality = 1f;

	// Token: 0x04001CB3 RID: 7347
	public float OptimalWaterQuality = 1f;

	// Token: 0x04001CB4 RID: 7348
	public float OptimalGroundQuality = 1f;

	// Token: 0x04001CB5 RID: 7349
	public float OptimalTemperatureQuality = 1f;

	// Token: 0x04001CB6 RID: 7350
	[Header("Harvesting")]
	public BaseEntity.Menu.Option pickOption;

	// Token: 0x04001CB7 RID: 7351
	public BaseEntity.Menu.Option pickAllOption;

	// Token: 0x04001CB8 RID: 7352
	public BaseEntity.Menu.Option eatOption;

	// Token: 0x04001CB9 RID: 7353
	public ItemDefinition pickupItem;

	// Token: 0x04001CBA RID: 7354
	public BaseEntity.Menu.Option cloneOption;

	// Token: 0x04001CBB RID: 7355
	public BaseEntity.Menu.Option cloneAllOption;

	// Token: 0x04001CBC RID: 7356
	public BaseEntity.Menu.Option removeDyingOption;

	// Token: 0x04001CBD RID: 7357
	public BaseEntity.Menu.Option removeDyingAllOption;

	// Token: 0x04001CBE RID: 7358
	public ItemDefinition removeDyingItem;

	// Token: 0x04001CBF RID: 7359
	public GameObjectRef removeDyingEffect;

	// Token: 0x04001CC0 RID: 7360
	public int pickupMultiplier = 1;

	// Token: 0x04001CC1 RID: 7361
	public GameObjectRef pickEffect;

	// Token: 0x04001CC2 RID: 7362
	public int maxHarvests = 1;

	// Token: 0x04001CC3 RID: 7363
	public bool disappearAfterHarvest;

	// Token: 0x04001CC4 RID: 7364
	[Header("Seeds")]
	public GameObjectRef CrossBreedEffect;

	// Token: 0x04001CC5 RID: 7365
	public ItemDefinition SeedItem;

	// Token: 0x04001CC6 RID: 7366
	public ItemDefinition CloneItem;

	// Token: 0x04001CC7 RID: 7367
	public int BaseCloneCount = 1;

	// Token: 0x04001CC8 RID: 7368
	[Header("Market")]
	public int BaseMarketValue = 10;

	// Token: 0x02000CFF RID: 3327
	public enum State
	{
		// Token: 0x0400463E RID: 17982
		Seed,
		// Token: 0x0400463F RID: 17983
		Seedling,
		// Token: 0x04004640 RID: 17984
		Sapling,
		// Token: 0x04004641 RID: 17985
		Crossbreed,
		// Token: 0x04004642 RID: 17986
		Mature,
		// Token: 0x04004643 RID: 17987
		Fruiting,
		// Token: 0x04004644 RID: 17988
		Ripe,
		// Token: 0x04004645 RID: 17989
		Dying
	}

	// Token: 0x02000D00 RID: 3328
	[Serializable]
	public struct Stage
	{
		// Token: 0x170006AD RID: 1709
		// (get) Token: 0x0600502A RID: 20522 RVA: 0x001A8314 File Offset: 0x001A6514
		public float lifeLengthSeconds
		{
			get
			{
				return this.lifeLength * 60f;
			}
		}

		// Token: 0x04004646 RID: 17990
		public PlantProperties.State nextState;

		// Token: 0x04004647 RID: 17991
		public float lifeLength;

		// Token: 0x04004648 RID: 17992
		public float health;

		// Token: 0x04004649 RID: 17993
		public float resources;

		// Token: 0x0400464A RID: 17994
		public float yield;

		// Token: 0x0400464B RID: 17995
		public GameObjectRef skinObject;

		// Token: 0x0400464C RID: 17996
		public bool IgnoreConditions;
	}
}
