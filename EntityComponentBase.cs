using System;
using Network;

// Token: 0x020003C7 RID: 967
public class EntityComponentBase : BaseMonoBehaviour
{
	// Token: 0x060021CD RID: 8653 RVA: 0x0002CFBB File Offset: 0x0002B1BB
	protected virtual BaseEntity GetBaseEntity()
	{
		return null;
	}

	// Token: 0x060021CE RID: 8654 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void SaveComponent(BaseNetworkable.SaveInfo info)
	{
	}

	// Token: 0x060021CF RID: 8655 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void LoadComponent(BaseNetworkable.LoadInfo info)
	{
	}

	// Token: 0x060021D0 RID: 8656 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		return false;
	}
}
