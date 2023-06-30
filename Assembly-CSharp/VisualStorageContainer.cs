using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200042E RID: 1070
public class VisualStorageContainer : LootContainer
{
	// Token: 0x06002440 RID: 9280 RVA: 0x000E74A0 File Offset: 0x000E56A0
	public override void ServerInit()
	{
		base.ServerInit();
	}

	// Token: 0x06002441 RID: 9281 RVA: 0x000A4C15 File Offset: 0x000A2E15
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
	}

	// Token: 0x06002442 RID: 9282 RVA: 0x000E74A8 File Offset: 0x000E56A8
	public override void PopulateLoot()
	{
		base.PopulateLoot();
		for (int i = 0; i < this.inventorySlots; i++)
		{
			global::Item slot = base.inventory.GetSlot(i);
			if (slot != null)
			{
				DroppedItem component = slot.Drop(this.displayNodes[i].transform.position + new Vector3(0f, 0.25f, 0f), Vector3.zero, this.displayNodes[i].transform.rotation).GetComponent<DroppedItem>();
				if (component)
				{
					base.ReceiveCollisionMessages(false);
					base.CancelInvoke(new Action(component.IdleDestroy));
					Rigidbody componentInChildren = component.GetComponentInChildren<Rigidbody>();
					if (componentInChildren)
					{
						componentInChildren.constraints = (RigidbodyConstraints)10;
					}
				}
			}
		}
	}

	// Token: 0x06002443 RID: 9283 RVA: 0x000E756C File Offset: 0x000E576C
	public void ClearRigidBodies()
	{
		if (this.displayModels == null)
		{
			return;
		}
		foreach (VisualStorageContainer.DisplayModel displayModel in this.displayModels)
		{
			if (displayModel != null)
			{
				UnityEngine.Object.Destroy(displayModel.displayModel.GetComponentInChildren<Rigidbody>());
			}
		}
	}

	// Token: 0x06002444 RID: 9284 RVA: 0x000E75B0 File Offset: 0x000E57B0
	public void SetItemsVisible(bool vis)
	{
		if (this.displayModels == null)
		{
			return;
		}
		foreach (VisualStorageContainer.DisplayModel displayModel in this.displayModels)
		{
			if (displayModel != null)
			{
				LODGroup componentInChildren = displayModel.displayModel.GetComponentInChildren<LODGroup>();
				if (componentInChildren)
				{
					componentInChildren.localReferencePoint = (vis ? Vector3.zero : new Vector3(10000f, 10000f, 10000f));
				}
				else
				{
					Debug.Log("VisualStorageContainer item missing LODGroup" + displayModel.displayModel.gameObject.name);
				}
			}
		}
	}

	// Token: 0x06002445 RID: 9285 RVA: 0x000E763B File Offset: 0x000E583B
	public void ItemUpdateComplete()
	{
		this.ClearRigidBodies();
		this.SetItemsVisible(true);
	}

	// Token: 0x06002446 RID: 9286 RVA: 0x000E764C File Offset: 0x000E584C
	public void UpdateVisibleItems(ProtoBuf.ItemContainer msg)
	{
		for (int i = 0; i < this.displayModels.Length; i++)
		{
			VisualStorageContainer.DisplayModel displayModel = this.displayModels[i];
			if (displayModel != null)
			{
				UnityEngine.Object.Destroy(displayModel.displayModel);
				this.displayModels[i] = null;
			}
		}
		if (msg == null)
		{
			return;
		}
		foreach (ProtoBuf.Item item in msg.contents)
		{
			ItemDefinition itemDefinition = ItemManager.FindItemDefinition(item.itemid);
			GameObject gameObject;
			if (itemDefinition.worldModelPrefab != null && itemDefinition.worldModelPrefab.isValid)
			{
				gameObject = itemDefinition.worldModelPrefab.Instantiate(null);
			}
			else
			{
				gameObject = UnityEngine.Object.Instantiate<GameObject>(this.defaultDisplayModel);
			}
			if (gameObject)
			{
				gameObject.transform.SetPositionAndRotation(this.displayNodes[item.slot].transform.position + new Vector3(0f, 0.25f, 0f), this.displayNodes[item.slot].transform.rotation);
				Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
				rigidbody.mass = 1f;
				rigidbody.drag = 0.1f;
				rigidbody.angularDrag = 0.1f;
				rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
				rigidbody.constraints = (RigidbodyConstraints)10;
				this.displayModels[item.slot].displayModel = gameObject;
				this.displayModels[item.slot].slot = item.slot;
				this.displayModels[item.slot].def = itemDefinition;
				gameObject.SetActive(true);
			}
		}
		this.SetItemsVisible(false);
		base.CancelInvoke(new Action(this.ItemUpdateComplete));
		base.Invoke(new Action(this.ItemUpdateComplete), 1f);
	}

	// Token: 0x04001C3A RID: 7226
	public VisualStorageContainerNode[] displayNodes;

	// Token: 0x04001C3B RID: 7227
	public VisualStorageContainer.DisplayModel[] displayModels;

	// Token: 0x04001C3C RID: 7228
	public Transform nodeParent;

	// Token: 0x04001C3D RID: 7229
	public GameObject defaultDisplayModel;

	// Token: 0x02000CF8 RID: 3320
	[Serializable]
	public class DisplayModel
	{
		// Token: 0x04004624 RID: 17956
		public GameObject displayModel;

		// Token: 0x04004625 RID: 17957
		public ItemDefinition def;

		// Token: 0x04004626 RID: 17958
		public int slot;
	}
}
