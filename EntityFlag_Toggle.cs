using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020003CB RID: 971
public class EntityFlag_Toggle : EntityComponent<BaseEntity>, IOnPostNetworkUpdate, IOnSendNetworkUpdate, IPrefabPreProcess
{
	// Token: 0x060021DA RID: 8666 RVA: 0x000DC6AF File Offset: 0x000DA8AF
	protected void OnDisable()
	{
		this.hasRunOnce = false;
		this.lastToggleOn = false;
	}

	// Token: 0x060021DB RID: 8667 RVA: 0x000DC6C0 File Offset: 0x000DA8C0
	public void DoUpdate(BaseEntity entity)
	{
		bool flag = ((this.flagCheck == EntityFlag_Toggle.FlagCheck.All) ? entity.HasFlag(this.flag) : entity.HasAny(this.flag));
		if (entity.HasAny(this.notFlag))
		{
			flag = false;
		}
		if (this.hasRunOnce && flag == this.lastToggleOn)
		{
			return;
		}
		this.hasRunOnce = true;
		this.lastToggleOn = flag;
		if (flag)
		{
			this.onFlagEnabled.Invoke();
		}
		else
		{
			this.onFlagDisabled.Invoke();
		}
		this.OnStateToggled(flag);
	}

	// Token: 0x060021DC RID: 8668 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OnStateToggled(bool state)
	{
	}

	// Token: 0x060021DD RID: 8669 RVA: 0x000DC742 File Offset: 0x000DA942
	public void OnPostNetworkUpdate(BaseEntity entity)
	{
		if (base.baseEntity != entity)
		{
			return;
		}
		if (!this.runClientside)
		{
			return;
		}
		this.DoUpdate(entity);
	}

	// Token: 0x060021DE RID: 8670 RVA: 0x000DC763 File Offset: 0x000DA963
	public void OnSendNetworkUpdate(BaseEntity entity)
	{
		if (!this.runServerside)
		{
			return;
		}
		this.DoUpdate(entity);
	}

	// Token: 0x060021DF RID: 8671 RVA: 0x000DC775 File Offset: 0x000DA975
	public void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if ((!clientside || !this.runClientside) && (!serverside || !this.runServerside))
		{
			process.RemoveComponent(this);
		}
	}

	// Token: 0x04001A3B RID: 6715
	public bool runClientside = true;

	// Token: 0x04001A3C RID: 6716
	public bool runServerside = true;

	// Token: 0x04001A3D RID: 6717
	public BaseEntity.Flags flag;

	// Token: 0x04001A3E RID: 6718
	[SerializeField]
	[Tooltip("If multiple flags are defined in 'flag', should they all be set, or any?")]
	private EntityFlag_Toggle.FlagCheck flagCheck;

	// Token: 0x04001A3F RID: 6719
	[SerializeField]
	[Tooltip("Specify any flags that must NOT be on for this toggle to be on")]
	private BaseEntity.Flags notFlag;

	// Token: 0x04001A40 RID: 6720
	[SerializeField]
	private UnityEvent onFlagEnabled = new UnityEvent();

	// Token: 0x04001A41 RID: 6721
	[SerializeField]
	private UnityEvent onFlagDisabled = new UnityEvent();

	// Token: 0x04001A42 RID: 6722
	internal bool hasRunOnce;

	// Token: 0x04001A43 RID: 6723
	internal bool lastToggleOn;

	// Token: 0x02000CD8 RID: 3288
	private enum FlagCheck
	{
		// Token: 0x0400458F RID: 17807
		All,
		// Token: 0x04004590 RID: 17808
		Any
	}
}
