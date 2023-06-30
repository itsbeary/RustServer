using System;
using Rust;
using UnityEngine;

// Token: 0x02000333 RID: 819
public class DoorAnimEvents : MonoBehaviour, IClientComponent
{
	// Token: 0x17000287 RID: 647
	// (get) Token: 0x06001F3A RID: 7994 RVA: 0x000D408C File Offset: 0x000D228C
	public Animator animator
	{
		get
		{
			return base.GetComponent<Animator>();
		}
	}

	// Token: 0x06001F3B RID: 7995 RVA: 0x000D4094 File Offset: 0x000D2294
	private void DoorOpenStart()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (!this.openStart.isValid)
		{
			return;
		}
		if (this.animator.IsInTransition(0))
		{
			return;
		}
		if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f)
		{
			return;
		}
		if (this.checkAnimSpeed && this.animator.GetCurrentAnimatorStateInfo(0).speed < 0f)
		{
			return;
		}
		Effect.client.Run(this.openStart.resourcePath, (this.soundTarget == null) ? base.gameObject : this.soundTarget);
	}

	// Token: 0x06001F3C RID: 7996 RVA: 0x000D4134 File Offset: 0x000D2334
	private void DoorOpenEnd()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (!this.openEnd.isValid)
		{
			return;
		}
		if (this.animator.IsInTransition(0))
		{
			return;
		}
		if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
		{
			return;
		}
		if (this.checkAnimSpeed && this.animator.GetCurrentAnimatorStateInfo(0).speed < 0f)
		{
			return;
		}
		Effect.client.Run(this.openEnd.resourcePath, (this.soundTarget == null) ? base.gameObject : this.soundTarget);
	}

	// Token: 0x06001F3D RID: 7997 RVA: 0x000D41D4 File Offset: 0x000D23D4
	private void DoorCloseStart()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (!this.closeStart.isValid)
		{
			return;
		}
		if (this.animator.IsInTransition(0))
		{
			return;
		}
		if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f)
		{
			return;
		}
		if (this.checkAnimSpeed && this.animator.GetCurrentAnimatorStateInfo(0).speed > 0f)
		{
			return;
		}
		Effect.client.Run(this.closeStart.resourcePath, (this.soundTarget == null) ? base.gameObject : this.soundTarget);
	}

	// Token: 0x06001F3E RID: 7998 RVA: 0x000D4274 File Offset: 0x000D2474
	private void DoorCloseEnd()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (!this.closeEnd.isValid)
		{
			return;
		}
		if (this.animator.IsInTransition(0))
		{
			return;
		}
		if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
		{
			return;
		}
		if (this.checkAnimSpeed && this.animator.GetCurrentAnimatorStateInfo(0).speed > 0f)
		{
			return;
		}
		Effect.client.Run(this.closeEnd.resourcePath, (this.soundTarget == null) ? base.gameObject : this.soundTarget);
	}

	// Token: 0x04001828 RID: 6184
	public GameObjectRef openStart;

	// Token: 0x04001829 RID: 6185
	public GameObjectRef openEnd;

	// Token: 0x0400182A RID: 6186
	public GameObjectRef closeStart;

	// Token: 0x0400182B RID: 6187
	public GameObjectRef closeEnd;

	// Token: 0x0400182C RID: 6188
	public GameObject soundTarget;

	// Token: 0x0400182D RID: 6189
	public bool checkAnimSpeed;
}
