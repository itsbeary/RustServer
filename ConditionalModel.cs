using System;
using UnityEngine;

// Token: 0x02000254 RID: 596
public class ConditionalModel : PrefabAttribute
{
	// Token: 0x06001C70 RID: 7280 RVA: 0x000C5FF3 File Offset: 0x000C41F3
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		this.conditions = base.GetComponentsInChildren<ModelConditionTest>(true);
	}

	// Token: 0x06001C71 RID: 7281 RVA: 0x000C6010 File Offset: 0x000C4210
	public bool RunTests(BaseEntity parent)
	{
		for (int i = 0; i < this.conditions.Length; i++)
		{
			if (!this.conditions[i].DoTest(parent))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001C72 RID: 7282 RVA: 0x000C6044 File Offset: 0x000C4244
	public GameObject InstantiateSkin(BaseEntity parent)
	{
		if (!this.onServer && this.isServer)
		{
			return null;
		}
		GameObject gameObject = this.gameManager.CreatePrefab(this.prefab.resourcePath, parent.transform, false);
		if (gameObject)
		{
			gameObject.transform.localPosition = this.worldPosition;
			gameObject.transform.localRotation = this.worldRotation;
			gameObject.AwakeFromInstantiate();
		}
		return gameObject;
	}

	// Token: 0x06001C73 RID: 7283 RVA: 0x000C60B2 File Offset: 0x000C42B2
	protected override Type GetIndexedType()
	{
		return typeof(ConditionalModel);
	}

	// Token: 0x04001502 RID: 5378
	public GameObjectRef prefab;

	// Token: 0x04001503 RID: 5379
	public bool onClient = true;

	// Token: 0x04001504 RID: 5380
	public bool onServer = true;

	// Token: 0x04001505 RID: 5381
	[NonSerialized]
	public ModelConditionTest[] conditions;
}
