using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000258 RID: 600
public class ConstructionSkin : BasePrefab
{
	// Token: 0x06001C88 RID: 7304 RVA: 0x000C6B30 File Offset: 0x000C4D30
	public int DetermineConditionalModelState(BuildingBlock parent)
	{
		ConditionalModel[] array = PrefabAttribute.server.FindAll<ConditionalModel>(this.prefabID);
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].RunTests(parent))
			{
				num |= 1 << i;
			}
		}
		return num;
	}

	// Token: 0x06001C89 RID: 7305 RVA: 0x000C6B74 File Offset: 0x000C4D74
	private void CreateConditionalModels(BuildingBlock parent)
	{
		ConditionalModel[] array = PrefabAttribute.server.FindAll<ConditionalModel>(this.prefabID);
		for (int i = 0; i < array.Length; i++)
		{
			if (parent.GetConditionalModel(i))
			{
				GameObject gameObject = array[i].InstantiateSkin(parent);
				if (!(gameObject == null))
				{
					if (this.conditionals == null)
					{
						this.conditionals = new List<GameObject>();
					}
					this.conditionals.Add(gameObject);
				}
			}
		}
	}

	// Token: 0x06001C8A RID: 7306 RVA: 0x000C6BDC File Offset: 0x000C4DDC
	private void DestroyConditionalModels(BuildingBlock parent)
	{
		if (this.conditionals == null)
		{
			return;
		}
		for (int i = 0; i < this.conditionals.Count; i++)
		{
			parent.gameManager.Retire(this.conditionals[i]);
		}
		this.conditionals.Clear();
	}

	// Token: 0x06001C8B RID: 7307 RVA: 0x000C6C2A File Offset: 0x000C4E2A
	public virtual void Refresh(BuildingBlock parent)
	{
		this.DestroyConditionalModels(parent);
		this.CreateConditionalModels(parent);
	}

	// Token: 0x06001C8C RID: 7308 RVA: 0x000C6C3A File Offset: 0x000C4E3A
	public void Destroy(BuildingBlock parent)
	{
		this.DestroyConditionalModels(parent);
		parent.gameManager.Retire(base.gameObject);
	}

	// Token: 0x06001C8D RID: 7309 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual uint GetStartingDetailColour(uint playerColourIndex)
	{
		return 0U;
	}

	// Token: 0x04001528 RID: 5416
	public List<GameObject> conditionals;
}
