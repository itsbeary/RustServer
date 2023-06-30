using System;
using ConVar;
using Facepunch.Rust;
using UnityEngine;

// Token: 0x020004C7 RID: 1223
public class DroppedItem : WorldItem
{
	// Token: 0x060027FA RID: 10234 RVA: 0x0000627E File Offset: 0x0000447E
	public override float GetNetworkTime()
	{
		return UnityEngine.Time.fixedTime;
	}

	// Token: 0x060027FB RID: 10235 RVA: 0x000F9287 File Offset: 0x000F7487
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.GetDespawnDuration() < float.PositiveInfinity)
		{
			base.Invoke(new Action(this.IdleDestroy), this.GetDespawnDuration());
		}
		base.ReceiveCollisionMessages(true);
	}

	// Token: 0x060027FC RID: 10236 RVA: 0x000F92BB File Offset: 0x000F74BB
	public virtual float GetDespawnDuration()
	{
		Item item = this.item;
		if (item == null)
		{
			return Server.itemdespawn;
		}
		return item.GetDespawnDuration();
	}

	// Token: 0x060027FD RID: 10237 RVA: 0x000F92D2 File Offset: 0x000F74D2
	public void IdleDestroy()
	{
		Analytics.Azure.OnItemDespawn(this, this.item, (int)this.DropReason, this.DroppedBy);
		base.DestroyItem();
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x060027FE RID: 10238 RVA: 0x000F92FC File Offset: 0x000F74FC
	public override void OnCollision(Collision collision, BaseEntity hitEntity)
	{
		if (this.item == null)
		{
			return;
		}
		if (this.item.MaxStackable() <= 1)
		{
			return;
		}
		DroppedItem droppedItem = hitEntity as DroppedItem;
		if (droppedItem == null)
		{
			return;
		}
		if (droppedItem.item == null)
		{
			return;
		}
		if (droppedItem.item.info != this.item.info)
		{
			return;
		}
		droppedItem.OnDroppedOn(this);
	}

	// Token: 0x060027FF RID: 10239 RVA: 0x000F9360 File Offset: 0x000F7560
	public void OnDroppedOn(DroppedItem di)
	{
		if (this.item == null)
		{
			return;
		}
		if (di.item == null)
		{
			return;
		}
		if (di.item.info != this.item.info)
		{
			return;
		}
		if (di.item.IsBlueprint() && di.item.blueprintTarget != this.item.blueprintTarget)
		{
			return;
		}
		if ((di.item.hasCondition && di.item.condition != di.item.maxCondition) || (this.item.hasCondition && this.item.condition != this.item.maxCondition))
		{
			return;
		}
		if (di.item.info != null)
		{
			if (di.item.info.amountType == ItemDefinition.AmountType.Genetics)
			{
				int num = ((di.item.instanceData != null) ? di.item.instanceData.dataInt : (-1));
				int num2 = ((this.item.instanceData != null) ? this.item.instanceData.dataInt : (-1));
				if (num != num2)
				{
					return;
				}
			}
			if (di.item.info.GetComponent<ItemModSign>() != null && ItemModAssociatedEntity<SignContent>.GetAssociatedEntity(di.item, true) != null)
			{
				return;
			}
			if (this.item.info != null && this.item.info.GetComponent<ItemModSign>() != null && ItemModAssociatedEntity<SignContent>.GetAssociatedEntity(this.item, true) != null)
			{
				return;
			}
		}
		int num3 = di.item.amount + this.item.amount;
		if (num3 > this.item.MaxStackable())
		{
			return;
		}
		if (num3 == 0)
		{
			return;
		}
		if (di.DropReason == DroppedItem.DropReasonEnum.Player)
		{
			this.DropReason = DroppedItem.DropReasonEnum.Player;
		}
		di.DestroyItem();
		di.Kill(BaseNetworkable.DestroyMode.None);
		this.item.amount = num3;
		this.item.MarkDirty();
		if (this.GetDespawnDuration() < float.PositiveInfinity)
		{
			base.Invoke(new Action(this.IdleDestroy), this.GetDespawnDuration());
		}
		Effect.server.Run("assets/bundled/prefabs/fx/notice/stack.world.fx.prefab", this, 0U, Vector3.zero, Vector3.zero, null, false);
	}

	// Token: 0x06002800 RID: 10240 RVA: 0x000F9585 File Offset: 0x000F7785
	public override void OnParentChanging(BaseEntity oldParent, BaseEntity newParent)
	{
		base.OnParentChanging(oldParent, newParent);
		if (newParent != null)
		{
			this.OnParented();
		}
	}

	// Token: 0x06002801 RID: 10241 RVA: 0x000F95A0 File Offset: 0x000F77A0
	internal override void OnParentRemoved()
	{
		if (this.rB == null)
		{
			base.OnParentRemoved();
			return;
		}
		Vector3 vector = base.transform.position;
		Quaternion rotation = base.transform.rotation;
		base.SetParent(null, false, false);
		RaycastHit raycastHit;
		if (UnityEngine.Physics.Raycast(vector + Vector3.up * 2f, Vector3.down, out raycastHit, 2f, 27328512) && vector.y < raycastHit.point.y)
		{
			vector += Vector3.up * 1.5f;
		}
		base.transform.position = vector;
		base.transform.rotation = rotation;
		ConVar.Physics.ApplyDropped(this.rB);
		this.childCollider.gameObject.layer = base.gameObject.layer;
		this.rB.isKinematic = false;
		this.rB.useGravity = true;
		this.rB.WakeUp();
		if (this.GetDespawnDuration() < float.PositiveInfinity)
		{
			base.Invoke(new Action(this.IdleDestroy), this.GetDespawnDuration());
		}
	}

	// Token: 0x06002802 RID: 10242 RVA: 0x000F96C0 File Offset: 0x000F78C0
	public void GoKinematic()
	{
		this.rB.isKinematic = true;
		this.childCollider.gameObject.layer = 19;
	}

	// Token: 0x06002803 RID: 10243 RVA: 0x000F96E0 File Offset: 0x000F78E0
	public override void PostInitShared()
	{
		base.PostInitShared();
		GameObject gameObject;
		if (this.item != null && this.item.info.worldModelPrefab.isValid)
		{
			gameObject = this.item.info.worldModelPrefab.Instantiate(null);
		}
		else
		{
			gameObject = UnityEngine.Object.Instantiate<GameObject>(this.itemModel);
		}
		gameObject.transform.SetParent(base.transform, false);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.SetLayerRecursive(base.gameObject.layer);
		this.childCollider = gameObject.GetComponent<Collider>();
		if (this.childCollider)
		{
			this.childCollider.enabled = false;
			if (base.HasParent())
			{
				this.OnParented();
			}
			else
			{
				this.childCollider.enabled = true;
			}
		}
		if (base.isServer)
		{
			WorldModel component = gameObject.GetComponent<WorldModel>();
			float num = (component ? component.mass : 1f);
			float num2 = 0.1f;
			float num3 = 0.1f;
			this.rB = base.gameObject.AddComponent<Rigidbody>();
			this.rB.mass = num;
			this.rB.drag = num2;
			this.rB.angularDrag = num3;
			this.rB.interpolation = RigidbodyInterpolation.None;
			ConVar.Physics.ApplyDropped(this.rB);
			Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
		}
		if (this.item != null)
		{
			PhysicsEffects component2 = base.gameObject.GetComponent<PhysicsEffects>();
			if (component2 != null)
			{
				component2.entity = this;
				if (this.item.info.physImpactSoundDef != null)
				{
					component2.physImpactSoundDef = this.item.info.physImpactSoundDef;
				}
			}
		}
		gameObject.SetActive(true);
	}

	// Token: 0x06002804 RID: 10244 RVA: 0x000F98C4 File Offset: 0x000F7AC4
	private void OnParented()
	{
		if (this.childCollider == null)
		{
			return;
		}
		if (this.childCollider)
		{
			this.childCollider.enabled = false;
			base.Invoke(new Action(this.EnableCollider), 0.1f);
		}
	}

	// Token: 0x06002805 RID: 10245 RVA: 0x000F9910 File Offset: 0x000F7B10
	private void EnableCollider()
	{
		if (this.childCollider)
		{
			this.childCollider.enabled = true;
		}
	}

	// Token: 0x06002806 RID: 10246 RVA: 0x00007A44 File Offset: 0x00005C44
	public override bool ShouldInheritNetworkGroup()
	{
		return false;
	}

	// Token: 0x04002092 RID: 8338
	[Header("DroppedItem")]
	public GameObject itemModel;

	// Token: 0x04002093 RID: 8339
	private Collider childCollider;

	// Token: 0x04002094 RID: 8340
	private Rigidbody rB;

	// Token: 0x04002095 RID: 8341
	private const int interactionOnlyLayer = 19;

	// Token: 0x04002096 RID: 8342
	[NonSerialized]
	public DroppedItem.DropReasonEnum DropReason;

	// Token: 0x04002097 RID: 8343
	[NonSerialized]
	public ulong DroppedBy;

	// Token: 0x02000D35 RID: 3381
	public enum DropReasonEnum
	{
		// Token: 0x0400471B RID: 18203
		Unknown,
		// Token: 0x0400471C RID: 18204
		Player,
		// Token: 0x0400471D RID: 18205
		Death,
		// Token: 0x0400471E RID: 18206
		Loot
	}
}
