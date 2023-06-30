using System;
using UnityEngine;

// Token: 0x0200028D RID: 653
public class AnimalAnimation : MonoBehaviour, IClientComponent
{
	// Token: 0x040015CC RID: 5580
	public BaseEntity Entity;

	// Token: 0x040015CD RID: 5581
	public BaseNpc Target;

	// Token: 0x040015CE RID: 5582
	public Animator Animator;

	// Token: 0x040015CF RID: 5583
	public MaterialEffect FootstepEffects;

	// Token: 0x040015D0 RID: 5584
	public Transform[] Feet;

	// Token: 0x040015D1 RID: 5585
	public SoundDefinition saddleMovementSoundDef;

	// Token: 0x040015D2 RID: 5586
	public SoundDefinition saddleMovementSoundDefWood;

	// Token: 0x040015D3 RID: 5587
	public SoundDefinition saddleMovementSoundDefRoadsign;

	// Token: 0x040015D4 RID: 5588
	public AnimationCurve saddleMovementGainCurve;

	// Token: 0x040015D5 RID: 5589
	[Tooltip("Ensure there is a float param called idleOffset if this is enabled")]
	public bool hasIdleOffset;

	// Token: 0x040015D6 RID: 5590
	[ReadOnly]
	public string BaseFolder;

	// Token: 0x040015D7 RID: 5591
	public const BaseEntity.Flags Flag_WoodArmor = BaseEntity.Flags.Reserved5;

	// Token: 0x040015D8 RID: 5592
	public const BaseEntity.Flags Flag_RoadsignArmor = BaseEntity.Flags.Reserved6;
}
