using System;
using Rust;
using UnityEngine;

// Token: 0x020003C0 RID: 960
public class AnimatedBuildingBlock : StabilityEntity
{
	// Token: 0x060021BB RID: 8635 RVA: 0x000DC1FC File Offset: 0x000DA3FC
	public override void ServerInit()
	{
		base.ServerInit();
		if (!Rust.Application.isLoadingSave)
		{
			this.UpdateAnimationParameters(true);
		}
	}

	// Token: 0x060021BC RID: 8636 RVA: 0x000DC212 File Offset: 0x000DA412
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.UpdateAnimationParameters(true);
	}

	// Token: 0x060021BD RID: 8637 RVA: 0x000DC221 File Offset: 0x000DA421
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		this.UpdateAnimationParameters(false);
	}

	// Token: 0x060021BE RID: 8638 RVA: 0x000DC234 File Offset: 0x000DA434
	protected void UpdateAnimationParameters(bool init)
	{
		if (!this.model)
		{
			return;
		}
		if (!this.model.animator)
		{
			return;
		}
		if (!this.model.animator.isInitialized)
		{
			return;
		}
		bool flag = this.animatorNeedsInitializing || this.animatorIsOpen != base.IsOpen() || (init && this.isAnimating);
		bool flag2 = this.animatorNeedsInitializing || init;
		if (flag)
		{
			this.isAnimating = true;
			this.model.animator.enabled = true;
			this.model.animator.SetBool("open", this.animatorIsOpen = base.IsOpen());
			if (flag2)
			{
				this.model.animator.fireEvents = false;
				if (this.model.animator.isActiveAndEnabled)
				{
					this.model.animator.Update(0f);
					this.model.animator.Update(20f);
				}
				this.PutAnimatorToSleep();
			}
			else
			{
				this.model.animator.fireEvents = base.isClient;
				if (base.isServer)
				{
					base.SetFlag(BaseEntity.Flags.Busy, true, false, true);
				}
			}
		}
		else if (flag2)
		{
			this.PutAnimatorToSleep();
		}
		this.animatorNeedsInitializing = false;
	}

	// Token: 0x060021BF RID: 8639 RVA: 0x000DC37A File Offset: 0x000DA57A
	protected void OnAnimatorFinished()
	{
		if (!this.isAnimating)
		{
			this.PutAnimatorToSleep();
		}
		this.isAnimating = false;
	}

	// Token: 0x060021C0 RID: 8640 RVA: 0x000DC394 File Offset: 0x000DA594
	private void PutAnimatorToSleep()
	{
		if (!this.model || !this.model.animator)
		{
			Debug.LogWarning(base.transform.GetRecursiveName("") + " has missing model/animator", base.gameObject);
			return;
		}
		this.model.animator.enabled = false;
		if (base.isServer)
		{
			base.SetFlag(BaseEntity.Flags.Busy, false, false, true);
		}
		this.OnAnimatorDisabled();
	}

	// Token: 0x060021C1 RID: 8641 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OnAnimatorDisabled()
	{
	}

	// Token: 0x04001A2C RID: 6700
	private bool animatorNeedsInitializing = true;

	// Token: 0x04001A2D RID: 6701
	private bool animatorIsOpen = true;

	// Token: 0x04001A2E RID: 6702
	private bool isAnimating;
}
