using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x0200051F RID: 1311
public class GroundWatch : BaseMonoBehaviour, IServerComponent
{
	// Token: 0x060029E8 RID: 10728 RVA: 0x00100D45 File Offset: 0x000FEF45
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(this.groundPosition, this.radius);
	}

	// Token: 0x060029E9 RID: 10729 RVA: 0x00100D74 File Offset: 0x000FEF74
	public static void PhysicsChanged(GameObject obj)
	{
		if (obj == null)
		{
			return;
		}
		Collider component = obj.GetComponent<Collider>();
		if (!component)
		{
			return;
		}
		Bounds bounds = component.bounds;
		List<BaseEntity> list = Facepunch.Pool.GetList<BaseEntity>();
		global::Vis.Entities<BaseEntity>(bounds.center, bounds.extents.magnitude + 1f, list, 2263296, QueryTriggerInteraction.Collide);
		foreach (BaseEntity baseEntity in list)
		{
			if (!baseEntity.IsDestroyed && !baseEntity.isClient && !(baseEntity is BuildingBlock))
			{
				baseEntity.BroadcastMessage("OnPhysicsNeighbourChanged", SendMessageOptions.DontRequireReceiver);
			}
		}
		Facepunch.Pool.FreeList<BaseEntity>(ref list);
	}

	// Token: 0x060029EA RID: 10730 RVA: 0x00100E3C File Offset: 0x000FF03C
	public static void PhysicsChanged(Vector3 origin, float radius, int layerMask)
	{
		List<BaseEntity> list = Facepunch.Pool.GetList<BaseEntity>();
		global::Vis.Entities<BaseEntity>(origin, radius, list, layerMask, QueryTriggerInteraction.Collide);
		foreach (BaseEntity baseEntity in list)
		{
			if (!baseEntity.IsDestroyed && !baseEntity.isClient && !(baseEntity is BuildingBlock))
			{
				baseEntity.BroadcastMessage("OnPhysicsNeighbourChanged", SendMessageOptions.DontRequireReceiver);
			}
		}
		Facepunch.Pool.FreeList<BaseEntity>(ref list);
	}

	// Token: 0x060029EB RID: 10731 RVA: 0x00100EC0 File Offset: 0x000FF0C0
	private void OnPhysicsNeighbourChanged()
	{
		if (!this.OnGround())
		{
			this.fails++;
			if (this.fails < ConVar.Physics.groundwatchfails)
			{
				if (ConVar.Physics.groundwatchdebug)
				{
					Debug.Log("GroundWatch retry: " + this.fails);
				}
				base.Invoke(new Action(this.OnPhysicsNeighbourChanged), ConVar.Physics.groundwatchdelay);
				return;
			}
			BaseEntity baseEntity = base.gameObject.ToBaseEntity();
			if (baseEntity)
			{
				baseEntity.transform.BroadcastMessage("OnGroundMissing", SendMessageOptions.DontRequireReceiver);
				return;
			}
		}
		else
		{
			this.fails = 0;
		}
	}

	// Token: 0x060029EC RID: 10732 RVA: 0x00100F58 File Offset: 0x000FF158
	private bool OnGround()
	{
		BaseEntity component = base.GetComponent<BaseEntity>();
		if (component)
		{
			if (component.HasParent())
			{
				return true;
			}
			Construction construction = PrefabAttribute.server.Find<Construction>(component.prefabID);
			if (construction)
			{
				Socket_Base[] allSockets = construction.allSockets;
				for (int i = 0; i < allSockets.Length; i++)
				{
					SocketMod[] socketMods = allSockets[i].socketMods;
					for (int j = 0; j < socketMods.Length; j++)
					{
						SocketMod_AreaCheck socketMod_AreaCheck = socketMods[j] as SocketMod_AreaCheck;
						if (socketMod_AreaCheck && socketMod_AreaCheck.wantsInside && !socketMod_AreaCheck.DoCheck(component.transform.position, component.transform.rotation, component))
						{
							if (ConVar.Physics.groundwatchdebug)
							{
								Debug.Log("GroundWatch failed: " + socketMod_AreaCheck.hierachyName);
							}
							return false;
						}
					}
				}
			}
		}
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		global::Vis.Colliders<Collider>(base.transform.TransformPoint(this.groundPosition), this.radius, list, this.layers, QueryTriggerInteraction.Collide);
		foreach (Collider collider in list)
		{
			BaseEntity baseEntity = collider.gameObject.ToBaseEntity();
			if (!baseEntity || (!(baseEntity == component) && !baseEntity.IsDestroyed && !baseEntity.isClient))
			{
				if (this.whitelist != null && this.whitelist.Length != 0)
				{
					bool flag = false;
					foreach (BaseEntity baseEntity2 in this.whitelist)
					{
						if (baseEntity.prefabID == baseEntity2.prefabID)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						continue;
					}
				}
				DecayEntity decayEntity = component as DecayEntity;
				DecayEntity decayEntity2 = baseEntity as DecayEntity;
				if (!decayEntity || decayEntity.buildingID == 0U || !decayEntity2 || decayEntity2.buildingID == 0U || decayEntity.buildingID == decayEntity2.buildingID)
				{
					Facepunch.Pool.FreeList<Collider>(ref list);
					return true;
				}
			}
		}
		if (ConVar.Physics.groundwatchdebug)
		{
			Debug.Log("GroundWatch failed: Legacy radius check");
		}
		Facepunch.Pool.FreeList<Collider>(ref list);
		return false;
	}

	// Token: 0x04002204 RID: 8708
	public Vector3 groundPosition = Vector3.zero;

	// Token: 0x04002205 RID: 8709
	public LayerMask layers = 27328512;

	// Token: 0x04002206 RID: 8710
	public float radius = 0.1f;

	// Token: 0x04002207 RID: 8711
	[Header("Whitelist")]
	public BaseEntity[] whitelist;

	// Token: 0x04002208 RID: 8712
	private int fails;
}
