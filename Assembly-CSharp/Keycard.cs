using System;

// Token: 0x020003B8 RID: 952
public class Keycard : AttackEntity
{
	// Token: 0x170002C2 RID: 706
	// (get) Token: 0x0600215F RID: 8543 RVA: 0x000DADE8 File Offset: 0x000D8FE8
	public int accessLevel
	{
		get
		{
			Item item = this.GetItem();
			if (item == null)
			{
				return 0;
			}
			ItemModKeycard component = item.info.GetComponent<ItemModKeycard>();
			if (component == null)
			{
				return 0;
			}
			return component.accessLevel;
		}
	}
}
