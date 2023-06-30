using System;
using ConVar;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x020003AC RID: 940
public class BaseCorpse : BaseCombatEntity
{
	// Token: 0x06002125 RID: 8485 RVA: 0x000D9E98 File Offset: 0x000D8098
	public override void ResetState()
	{
		this.spawnGroup = null;
		base.ResetState();
	}

	// Token: 0x06002126 RID: 8486 RVA: 0x000D9EA7 File Offset: 0x000D80A7
	public override void ServerInit()
	{
		this.SetupRigidBody();
		this.ResetRemovalTime();
		this.resourceDispenser = base.GetComponent<ResourceDispenser>();
		base.ServerInit();
	}

	// Token: 0x06002127 RID: 8487 RVA: 0x000D9EC8 File Offset: 0x000D80C8
	public virtual void InitCorpse(global::BaseEntity pr)
	{
		this.parentEnt = pr;
		base.transform.SetPositionAndRotation(this.parentEnt.CenterPoint(), this.parentEnt.transform.rotation);
		SpawnPointInstance component = base.GetComponent<SpawnPointInstance>();
		if (component != null)
		{
			this.spawnGroup = component.parentSpawnPointUser as SpawnGroup;
		}
	}

	// Token: 0x06002128 RID: 8488 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool CanRemove()
	{
		return true;
	}

	// Token: 0x06002129 RID: 8489 RVA: 0x000D9F23 File Offset: 0x000D8123
	public void RemoveCorpse()
	{
		if (!this.CanRemove())
		{
			this.ResetRemovalTime();
			return;
		}
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x0600212A RID: 8490 RVA: 0x000D9F3C File Offset: 0x000D813C
	public void ResetRemovalTime(float dur)
	{
		using (TimeWarning.New("ResetRemovalTime", 0))
		{
			if (base.IsInvoking(new Action(this.RemoveCorpse)))
			{
				base.CancelInvoke(new Action(this.RemoveCorpse));
			}
			base.Invoke(new Action(this.RemoveCorpse), dur);
		}
	}

	// Token: 0x0600212B RID: 8491 RVA: 0x000D9FAC File Offset: 0x000D81AC
	public virtual float GetRemovalTime()
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode != null)
		{
			return activeGameMode.CorpseRemovalTime(this);
		}
		return Server.corpsedespawn;
	}

	// Token: 0x0600212C RID: 8492 RVA: 0x000D9FD6 File Offset: 0x000D81D6
	public void ResetRemovalTime()
	{
		this.ResetRemovalTime(this.GetRemovalTime());
	}

	// Token: 0x0600212D RID: 8493 RVA: 0x000D9FE4 File Offset: 0x000D81E4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.corpse = Facepunch.Pool.Get<Corpse>();
		if (this.parentEnt.IsValid())
		{
			info.msg.corpse.parentID = this.parentEnt.net.ID;
		}
	}

	// Token: 0x0600212E RID: 8494 RVA: 0x000DA038 File Offset: 0x000D8238
	public void TakeChildren(global::BaseEntity takeChildrenFrom)
	{
		if (takeChildrenFrom.children == null)
		{
			return;
		}
		using (TimeWarning.New("Corpse.TakeChildren", 0))
		{
			global::BaseEntity[] array = takeChildrenFrom.children.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SwitchParent(this);
			}
		}
	}

	// Token: 0x0600212F RID: 8495 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void ApplyInheritedVelocity(Vector3 velocity)
	{
	}

	// Token: 0x06002130 RID: 8496 RVA: 0x000DA09C File Offset: 0x000D829C
	private Rigidbody SetupRigidBody()
	{
		if (base.isServer)
		{
			GameObject gameObject = base.gameManager.FindPrefab(this.prefabRagdoll.resourcePath);
			if (gameObject == null)
			{
				return null;
			}
			Ragdoll component = gameObject.GetComponent<Ragdoll>();
			if (component == null)
			{
				return null;
			}
			if (component.primaryBody == null)
			{
				Debug.LogError("[BaseCorpse] ragdoll.primaryBody isn't set!" + component.gameObject.name);
				return null;
			}
			if (base.gameObject.GetComponent<Collider>() == null)
			{
				BoxCollider component2 = component.primaryBody.GetComponent<BoxCollider>();
				if (component2 == null)
				{
					Debug.LogError("Ragdoll has unsupported primary collider (make it supported) ", component);
					return null;
				}
				BoxCollider boxCollider = base.gameObject.AddComponent<BoxCollider>();
				boxCollider.size = component2.size * 2f;
				boxCollider.center = component2.center;
				boxCollider.sharedMaterial = component2.sharedMaterial;
			}
		}
		Rigidbody rigidbody = base.GetComponent<Rigidbody>();
		if (rigidbody == null)
		{
			rigidbody = base.gameObject.AddComponent<Rigidbody>();
		}
		rigidbody.mass = 10f;
		rigidbody.useGravity = true;
		rigidbody.drag = 0.5f;
		rigidbody.angularDrag = 0.5f;
		rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
		rigidbody.sleepThreshold = 0.05f;
		if (base.isServer)
		{
			Buoyancy component3 = base.GetComponent<Buoyancy>();
			if (component3 != null)
			{
				component3.rigidBody = rigidbody;
			}
			ConVar.Physics.ApplyDropped(rigidbody);
			Vector3 vector = Vector3Ex.Range(-1f, 1f);
			vector.y += 1f;
			rigidbody.velocity = vector;
			rigidbody.angularVelocity = Vector3Ex.Range(-10f, 10f);
		}
		return rigidbody;
	}

	// Token: 0x06002131 RID: 8497 RVA: 0x000DA23C File Offset: 0x000D843C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.corpse != null)
		{
			this.Load(info.msg.corpse);
		}
	}

	// Token: 0x06002132 RID: 8498 RVA: 0x000DA263 File Offset: 0x000D8463
	private void Load(Corpse corpse)
	{
		if (base.isServer)
		{
			this.parentEnt = global::BaseNetworkable.serverEntities.Find(corpse.parentID) as global::BaseEntity;
		}
		bool isClient = base.isClient;
	}

	// Token: 0x06002133 RID: 8499 RVA: 0x000DA28F File Offset: 0x000D848F
	public override void OnAttacked(HitInfo info)
	{
		if (base.isServer)
		{
			this.ResetRemovalTime();
			if (this.resourceDispenser)
			{
				this.resourceDispenser.OnAttacked(info);
			}
			if (!info.DidGather)
			{
				base.OnAttacked(info);
			}
		}
	}

	// Token: 0x06002134 RID: 8500 RVA: 0x000DA2C7 File Offset: 0x000D84C7
	public override string Categorize()
	{
		return "corpse";
	}

	// Token: 0x170002BF RID: 703
	// (get) Token: 0x06002135 RID: 8501 RVA: 0x000DA2CE File Offset: 0x000D84CE
	public override global::BaseEntity.TraitFlag Traits
	{
		get
		{
			return base.Traits | global::BaseEntity.TraitFlag.Food | global::BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x06002136 RID: 8502 RVA: 0x000DA2DC File Offset: 0x000D84DC
	public override void Eat(BaseNpc baseNpc, float timeSpent)
	{
		this.ResetRemovalTime();
		base.Hurt(timeSpent * 5f);
		baseNpc.AddCalories(timeSpent * 2f);
	}

	// Token: 0x06002137 RID: 8503 RVA: 0x00007A44 File Offset: 0x00005C44
	public override bool ShouldInheritNetworkGroup()
	{
		return false;
	}

	// Token: 0x040019F3 RID: 6643
	public GameObjectRef prefabRagdoll;

	// Token: 0x040019F4 RID: 6644
	public global::BaseEntity parentEnt;

	// Token: 0x040019F5 RID: 6645
	[NonSerialized]
	internal ResourceDispenser resourceDispenser;

	// Token: 0x040019F6 RID: 6646
	[NonSerialized]
	public SpawnGroup spawnGroup;
}
