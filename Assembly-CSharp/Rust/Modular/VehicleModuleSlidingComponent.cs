using System;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000B3A RID: 2874
	[Serializable]
	public class VehicleModuleSlidingComponent
	{
		// Token: 0x0600459E RID: 17822 RVA: 0x001967A9 File Offset: 0x001949A9
		public bool WantsOpenPos(BaseEntity parentEntity)
		{
			return parentEntity.HasFlag(this.flag_SliderOpen);
		}

		// Token: 0x0600459F RID: 17823 RVA: 0x001967B7 File Offset: 0x001949B7
		public void Use(BaseVehicleModule parentModule)
		{
			parentModule.SetFlag(this.flag_SliderOpen, !this.WantsOpenPos(parentModule), false, true);
		}

		// Token: 0x060045A0 RID: 17824 RVA: 0x001967D1 File Offset: 0x001949D1
		public void ServerUpdateTick(BaseVehicleModule parentModule)
		{
			this.CheckPosition(parentModule, Time.fixedDeltaTime);
		}

		// Token: 0x060045A1 RID: 17825 RVA: 0x001967E0 File Offset: 0x001949E0
		private void CheckPosition(BaseEntity parentEntity, float dt)
		{
			bool flag = this.WantsOpenPos(parentEntity);
			if (flag && this.positionPercent == 1f)
			{
				return;
			}
			if (!flag && this.positionPercent == 0f)
			{
				return;
			}
			float num = (flag ? (dt / this.moveTime) : (-(dt / this.moveTime)));
			this.positionPercent = Mathf.Clamp01(this.positionPercent + num);
			foreach (VehicleModuleSlidingComponent.SlidingPart slidingPart in this.slidingParts)
			{
				if (!(slidingPart.transform == null))
				{
					slidingPart.transform.localPosition = Vector3.Lerp(slidingPart.closedPosition, slidingPart.openPosition, this.positionPercent);
				}
			}
		}

		// Token: 0x04003E93 RID: 16019
		public string interactionColliderName = "MyCollider";

		// Token: 0x04003E94 RID: 16020
		public BaseEntity.Flags flag_SliderOpen = BaseEntity.Flags.Reserved3;

		// Token: 0x04003E95 RID: 16021
		public float moveTime = 1f;

		// Token: 0x04003E96 RID: 16022
		public VehicleModuleSlidingComponent.SlidingPart[] slidingParts;

		// Token: 0x04003E97 RID: 16023
		public SoundDefinition openSoundDef;

		// Token: 0x04003E98 RID: 16024
		public SoundDefinition closeSoundDef;

		// Token: 0x04003E99 RID: 16025
		private float positionPercent;

		// Token: 0x02000FAB RID: 4011
		[Serializable]
		public class SlidingPart
		{
			// Token: 0x040050FF RID: 20735
			public Transform transform;

			// Token: 0x04005100 RID: 20736
			public Vector3 openPosition;

			// Token: 0x04005101 RID: 20737
			public Vector3 closedPosition;
		}
	}
}
