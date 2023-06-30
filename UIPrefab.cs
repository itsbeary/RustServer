using System;
using Facepunch;
using UnityEngine;

// Token: 0x020008D5 RID: 2261
public class UIPrefab : MonoBehaviour
{
	// Token: 0x0600376C RID: 14188 RVA: 0x0014CA20 File Offset: 0x0014AC20
	private void Awake()
	{
		if (this.prefabSource == null)
		{
			return;
		}
		if (this.createdGameObject != null)
		{
			return;
		}
		this.createdGameObject = Facepunch.Instantiate.GameObject(this.prefabSource, null);
		this.createdGameObject.name = this.prefabSource.name;
		this.createdGameObject.transform.SetParent(base.transform, false);
		this.createdGameObject.Identity();
	}

	// Token: 0x0600376D RID: 14189 RVA: 0x0014CA95 File Offset: 0x0014AC95
	public void SetVisible(bool visible)
	{
		if (this.createdGameObject == null)
		{
			return;
		}
		if (this.createdGameObject.activeSelf == visible)
		{
			return;
		}
		this.createdGameObject.SetActive(visible);
	}

	// Token: 0x040032DB RID: 13019
	public GameObject prefabSource;

	// Token: 0x040032DC RID: 13020
	internal GameObject createdGameObject;
}
