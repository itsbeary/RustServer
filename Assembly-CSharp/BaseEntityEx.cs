using System;

// Token: 0x020003AE RID: 942
public static class BaseEntityEx
{
	// Token: 0x06002139 RID: 8505 RVA: 0x000DA2FE File Offset: 0x000D84FE
	public static bool IsValidEntityReference<T>(this T obj) where T : class
	{
		return obj as BaseEntity != null;
	}

	// Token: 0x0600213A RID: 8506 RVA: 0x000DA314 File Offset: 0x000D8514
	public static bool HasEntityInParents(this BaseEntity ent, BaseEntity toFind)
	{
		if (ent == null || toFind == null)
		{
			return false;
		}
		if (ent == toFind || ent.EqualNetID(toFind))
		{
			return true;
		}
		BaseEntity baseEntity = ent.GetParentEntity();
		while (baseEntity != null)
		{
			if (baseEntity == toFind || baseEntity.EqualNetID(toFind))
			{
				return true;
			}
			baseEntity = baseEntity.GetParentEntity();
		}
		return false;
	}
}
