using System;

// Token: 0x020004B5 RID: 1205
public class MountableParentCombatEntity : BaseCombatEntity
{
	// Token: 0x1700034C RID: 844
	// (get) Token: 0x0600277C RID: 10108 RVA: 0x000F6E91 File Offset: 0x000F5091
	private BaseMountable Mountable
	{
		get
		{
			if (this.mountable == null)
			{
				this.mountable = base.GetComponentInParent<BaseMountable>();
			}
			return this.mountable;
		}
	}

	// Token: 0x04002004 RID: 8196
	private BaseMountable mountable;
}
