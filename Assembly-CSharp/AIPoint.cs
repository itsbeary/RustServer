using System;

// Token: 0x020001E3 RID: 483
public class AIPoint : BaseMonoBehaviour
{
	// Token: 0x060019B1 RID: 6577 RVA: 0x000BC41C File Offset: 0x000BA61C
	public bool InUse()
	{
		return this.currentUser != null;
	}

	// Token: 0x060019B2 RID: 6578 RVA: 0x000BC42A File Offset: 0x000BA62A
	public bool IsUsedBy(BaseEntity user)
	{
		return this.InUse() && !(user == null) && user == this.currentUser;
	}

	// Token: 0x060019B3 RID: 6579 RVA: 0x000BC44D File Offset: 0x000BA64D
	public bool CanBeUsedBy(BaseEntity user)
	{
		return (user != null && this.currentUser == user) || !this.InUse();
	}

	// Token: 0x060019B4 RID: 6580 RVA: 0x000BC471 File Offset: 0x000BA671
	public void SetUsedBy(BaseEntity user, float duration = 5f)
	{
		this.currentUser = user;
		base.CancelInvoke(new Action(this.ClearUsed));
		base.Invoke(new Action(this.ClearUsed), duration);
	}

	// Token: 0x060019B5 RID: 6581 RVA: 0x000BC49F File Offset: 0x000BA69F
	public void SetUsedBy(BaseEntity user)
	{
		this.currentUser = user;
	}

	// Token: 0x060019B6 RID: 6582 RVA: 0x000BC4A8 File Offset: 0x000BA6A8
	public void ClearUsed()
	{
		this.currentUser = null;
	}

	// Token: 0x060019B7 RID: 6583 RVA: 0x000BC4B1 File Offset: 0x000BA6B1
	public void ClearIfUsedBy(BaseEntity user)
	{
		if (this.currentUser == user)
		{
			this.ClearUsed();
		}
	}

	// Token: 0x04001267 RID: 4711
	private BaseEntity currentUser;
}
