using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020004FB RID: 1275
public abstract class DeployVolume : PrefabAttribute
{
	// Token: 0x1700037F RID: 895
	// (get) Token: 0x06002947 RID: 10567 RVA: 0x000FE41F File Offset: 0x000FC61F
	// (set) Token: 0x06002948 RID: 10568 RVA: 0x000FE427 File Offset: 0x000FC627
	public bool IsBuildingBlock { get; set; }

	// Token: 0x06002949 RID: 10569 RVA: 0x000FE430 File Offset: 0x000FC630
	protected override Type GetIndexedType()
	{
		return typeof(DeployVolume);
	}

	// Token: 0x0600294A RID: 10570 RVA: 0x000FE43C File Offset: 0x000FC63C
	public override void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
		this.IsBuildingBlock = rootObj.GetComponent<BuildingBlock>() != null;
	}

	// Token: 0x0600294B RID: 10571
	protected abstract bool Check(Vector3 position, Quaternion rotation, int mask = -1);

	// Token: 0x0600294C RID: 10572
	protected abstract bool Check(Vector3 position, Quaternion rotation, OBB test, int mask = -1);

	// Token: 0x0600294D RID: 10573 RVA: 0x000FE460 File Offset: 0x000FC660
	public static bool Check(Vector3 position, Quaternion rotation, DeployVolume[] volumes, int mask = -1)
	{
		for (int i = 0; i < volumes.Length; i++)
		{
			if (volumes[i].Check(position, rotation, mask))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600294E RID: 10574 RVA: 0x000FE48C File Offset: 0x000FC68C
	public static bool Check(Vector3 position, Quaternion rotation, DeployVolume[] volumes, OBB test, int mask = -1)
	{
		for (int i = 0; i < volumes.Length; i++)
		{
			if (volumes[i].Check(position, rotation, test, mask))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600294F RID: 10575 RVA: 0x000FE4BC File Offset: 0x000FC6BC
	public static bool CheckSphere(Vector3 pos, float radius, int layerMask, DeployVolume volume)
	{
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(pos, radius, list, layerMask, QueryTriggerInteraction.Collide);
		bool flag = DeployVolume.CheckFlags(list, volume);
		Pool.FreeList<Collider>(ref list);
		return flag;
	}

	// Token: 0x06002950 RID: 10576 RVA: 0x000FE4E8 File Offset: 0x000FC6E8
	public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, int layerMask, DeployVolume volume)
	{
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapCapsule(start, end, radius, list, layerMask, QueryTriggerInteraction.Collide);
		bool flag = DeployVolume.CheckFlags(list, volume);
		Pool.FreeList<Collider>(ref list);
		return flag;
	}

	// Token: 0x06002951 RID: 10577 RVA: 0x000FE518 File Offset: 0x000FC718
	public static bool CheckOBB(OBB obb, int layerMask, DeployVolume volume)
	{
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapOBB(obb, list, layerMask, QueryTriggerInteraction.Collide);
		bool flag = DeployVolume.CheckFlags(list, volume);
		Pool.FreeList<Collider>(ref list);
		return flag;
	}

	// Token: 0x06002952 RID: 10578 RVA: 0x000FE544 File Offset: 0x000FC744
	public static bool CheckBounds(Bounds bounds, int layerMask, DeployVolume volume)
	{
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapBounds(bounds, list, layerMask, QueryTriggerInteraction.Collide);
		bool flag = DeployVolume.CheckFlags(list, volume);
		Pool.FreeList<Collider>(ref list);
		return flag;
	}

	// Token: 0x17000380 RID: 896
	// (get) Token: 0x06002953 RID: 10579 RVA: 0x000FE56E File Offset: 0x000FC76E
	// (set) Token: 0x06002954 RID: 10580 RVA: 0x000FE575 File Offset: 0x000FC775
	public static Collider LastDeployHit { get; private set; }

	// Token: 0x06002955 RID: 10581 RVA: 0x000FE580 File Offset: 0x000FC780
	private static bool CheckFlags(List<Collider> list, DeployVolume volume)
	{
		DeployVolume.LastDeployHit = null;
		for (int i = 0; i < list.Count; i++)
		{
			DeployVolume.LastDeployHit = list[i];
			GameObject gameObject = list[i].gameObject;
			if (!gameObject.CompareTag("DeployVolumeIgnore"))
			{
				ColliderInfo component = gameObject.GetComponent<ColliderInfo>();
				if ((!(component != null) || !component.HasFlag(ColliderInfo.Flags.OnlyBlockBuildingBlock) || volume.IsBuildingBlock) && (component == null || volume.ignore == (ColliderInfo.Flags)0 || !component.HasFlag(volume.ignore)))
				{
					if (volume.entityList.Length == 0 && volume.entityGroups.Length == 0)
					{
						return true;
					}
					BaseEntity baseEntity = list[i].ToBaseEntity();
					if (volume.entityGroups != null)
					{
						foreach (EntityListScriptableObject entityListScriptableObject in volume.entityGroups)
						{
							if (entityListScriptableObject.entities == null || entityListScriptableObject.entities.Length == 0)
							{
								Debug.LogWarning("Skipping entity group '" + entityListScriptableObject.name + "' when checking volume: there are no entities");
							}
							else if (!DeployVolume.CheckEntityList(baseEntity, entityListScriptableObject.entities, entityListScriptableObject.whitelist))
							{
								return false;
							}
						}
					}
					return DeployVolume.CheckEntityList(baseEntity, volume.entityList, volume.entityMode == DeployVolume.EntityMode.IncludeList);
				}
			}
		}
		return false;
	}

	// Token: 0x06002956 RID: 10582 RVA: 0x000FE6D8 File Offset: 0x000FC8D8
	private static bool CheckEntityList(BaseEntity entity, BaseEntity[] entities, bool whitelist)
	{
		if (entities == null || entities.Length == 0)
		{
			return true;
		}
		bool flag = false;
		if (entity != null)
		{
			foreach (BaseEntity baseEntity in entities)
			{
				if (entity.prefabID == baseEntity.prefabID)
				{
					flag = true;
					break;
				}
			}
		}
		if (whitelist)
		{
			return flag;
		}
		return !flag;
	}

	// Token: 0x0400216B RID: 8555
	public LayerMask layers = 537001984;

	// Token: 0x0400216C RID: 8556
	[global::InspectorFlags]
	public ColliderInfo.Flags ignore;

	// Token: 0x0400216D RID: 8557
	public DeployVolume.EntityMode entityMode;

	// Token: 0x0400216E RID: 8558
	[FormerlySerializedAs("entities")]
	public BaseEntity[] entityList;

	// Token: 0x0400216F RID: 8559
	[SerializeField]
	public EntityListScriptableObject[] entityGroups;

	// Token: 0x02000D4B RID: 3403
	public enum EntityMode
	{
		// Token: 0x04004762 RID: 18274
		ExcludeList,
		// Token: 0x04004763 RID: 18275
		IncludeList
	}
}
