using System;
using UnityEngine;

// Token: 0x02000501 RID: 1281
[CreateAssetMenu(fileName = "NewEntityList", menuName = "Rust/EntityList")]
public class EntityListScriptableObject : ScriptableObject
{
	// Token: 0x0600296A RID: 10602 RVA: 0x000FEBB4 File Offset: 0x000FCDB4
	public bool IsInList(uint prefabId)
	{
		if (this.entities == null)
		{
			return false;
		}
		BaseEntity[] array = this.entities;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].prefabID == prefabId)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0400217B RID: 8571
	[SerializeField]
	public BaseEntity[] entities;

	// Token: 0x0400217C RID: 8572
	[SerializeField]
	public bool whitelist;
}
