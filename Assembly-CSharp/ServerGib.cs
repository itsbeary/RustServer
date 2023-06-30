using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000424 RID: 1060
public class ServerGib : BaseCombatEntity
{
	// Token: 0x0600240D RID: 9229 RVA: 0x000ACE07 File Offset: 0x000AB007
	public override float BoundsPadding()
	{
		return 3f;
	}

	// Token: 0x0600240E RID: 9230 RVA: 0x000E65E8 File Offset: 0x000E47E8
	public static List<global::ServerGib> CreateGibs(string entityToCreatePath, GameObject creator, GameObject gibSource, Vector3 inheritVelocity, float spreadVelocity)
	{
		List<global::ServerGib> list = new List<global::ServerGib>();
		foreach (MeshRenderer meshRenderer in gibSource.GetComponentsInChildren<MeshRenderer>(true))
		{
			MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
			Vector3 normalized = meshRenderer.transform.localPosition.normalized;
			Vector3 vector = creator.transform.localToWorldMatrix.MultiplyPoint(meshRenderer.transform.localPosition) + normalized * 0.5f;
			Quaternion quaternion = creator.transform.rotation * meshRenderer.transform.localRotation;
			global::BaseEntity baseEntity = GameManager.server.CreateEntity(entityToCreatePath, vector, quaternion, true);
			if (baseEntity)
			{
				global::ServerGib component2 = baseEntity.GetComponent<global::ServerGib>();
				component2.transform.SetPositionAndRotation(vector, quaternion);
				component2._gibName = meshRenderer.name;
				MeshCollider component3 = meshRenderer.GetComponent<MeshCollider>();
				Mesh mesh = ((component3 != null) ? component3.sharedMesh : component.sharedMesh);
				component2.PhysicsInit(mesh);
				Vector3 vector2 = meshRenderer.transform.localPosition.normalized * spreadVelocity;
				component2.rigidBody.velocity = inheritVelocity + vector2;
				component2.rigidBody.angularVelocity = Vector3Ex.Range(-1f, 1f).normalized * 1f;
				component2.rigidBody.WakeUp();
				component2.Spawn();
				list.Add(component2);
			}
		}
		foreach (global::ServerGib serverGib in list)
		{
			foreach (global::ServerGib serverGib2 in list)
			{
				if (!(serverGib == serverGib2))
				{
					Physics.IgnoreCollision(serverGib2.GetCollider(), serverGib.GetCollider(), true);
				}
			}
		}
		return list;
	}

	// Token: 0x0600240F RID: 9231 RVA: 0x000E6808 File Offset: 0x000E4A08
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk && this._gibName != "")
		{
			info.msg.servergib = Pool.Get<ProtoBuf.ServerGib>();
			info.msg.servergib.gibName = this._gibName;
		}
	}

	// Token: 0x06002410 RID: 9232 RVA: 0x000E685C File Offset: 0x000E4A5C
	public MeshCollider GetCollider()
	{
		return this.meshCollider;
	}

	// Token: 0x06002411 RID: 9233 RVA: 0x000E6864 File Offset: 0x000E4A64
	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.RemoveMe), 1800f);
	}

	// Token: 0x06002412 RID: 9234 RVA: 0x00003384 File Offset: 0x00001584
	public void RemoveMe()
	{
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06002413 RID: 9235 RVA: 0x000E6884 File Offset: 0x000E4A84
	public virtual void PhysicsInit(Mesh physicsMesh)
	{
		Mesh mesh = null;
		MeshFilter component = base.gameObject.GetComponent<MeshFilter>();
		if (component != null)
		{
			mesh = component.sharedMesh;
			component.sharedMesh = physicsMesh;
		}
		this.meshCollider = base.gameObject.AddComponent<MeshCollider>();
		this.meshCollider.sharedMesh = physicsMesh;
		this.meshCollider.convex = true;
		this.meshCollider.material = this.physicsMaterial;
		if (component != null)
		{
			component.sharedMesh = mesh;
		}
		Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
		rigidbody.useGravity = true;
		rigidbody.mass = Mathf.Clamp(this.meshCollider.bounds.size.magnitude * this.meshCollider.bounds.size.magnitude * 20f, 10f, 2000f);
		rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		rigidbody.collisionDetectionMode = (this.useContinuousCollision ? CollisionDetectionMode.Continuous : CollisionDetectionMode.Discrete);
		if (base.isServer)
		{
			rigidbody.drag = 0.1f;
			rigidbody.angularDrag = 0.1f;
		}
		this.rigidBody = rigidbody;
		base.gameObject.layer = LayerMask.NameToLayer("Default");
		if (base.isClient)
		{
			rigidbody.isKinematic = true;
		}
	}

	// Token: 0x04001C15 RID: 7189
	public GameObject _gibSource;

	// Token: 0x04001C16 RID: 7190
	public string _gibName;

	// Token: 0x04001C17 RID: 7191
	public PhysicMaterial physicsMaterial;

	// Token: 0x04001C18 RID: 7192
	public bool useContinuousCollision;

	// Token: 0x04001C19 RID: 7193
	private MeshCollider meshCollider;

	// Token: 0x04001C1A RID: 7194
	private Rigidbody rigidBody;
}
