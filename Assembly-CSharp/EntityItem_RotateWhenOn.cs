using System;
using UnityEngine;

// Token: 0x020003CE RID: 974
public class EntityItem_RotateWhenOn : EntityComponent<BaseEntity>
{
	// Token: 0x04001A45 RID: 6725
	public EntityItem_RotateWhenOn.State on;

	// Token: 0x04001A46 RID: 6726
	public EntityItem_RotateWhenOn.State off;

	// Token: 0x04001A47 RID: 6727
	internal bool currentlyOn;

	// Token: 0x04001A48 RID: 6728
	internal bool stateInitialized;

	// Token: 0x04001A49 RID: 6729
	public BaseEntity.Flags targetFlag = BaseEntity.Flags.On;

	// Token: 0x02000CD9 RID: 3289
	[Serializable]
	public class State
	{
		// Token: 0x04004591 RID: 17809
		public Vector3 rotation;

		// Token: 0x04004592 RID: 17810
		public float initialDelay;

		// Token: 0x04004593 RID: 17811
		public float timeToTake = 2f;

		// Token: 0x04004594 RID: 17812
		public AnimationCurve animationCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(1f, 1f)
		});

		// Token: 0x04004595 RID: 17813
		public string effectOnStart = "";

		// Token: 0x04004596 RID: 17814
		public string effectOnFinish = "";

		// Token: 0x04004597 RID: 17815
		public SoundDefinition movementLoop;

		// Token: 0x04004598 RID: 17816
		public float movementLoopFadeOutTime = 0.1f;

		// Token: 0x04004599 RID: 17817
		public SoundDefinition startSound;

		// Token: 0x0400459A RID: 17818
		public SoundDefinition stopSound;
	}
}
