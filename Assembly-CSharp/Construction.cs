using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000255 RID: 597
public class Construction : PrefabAttribute
{
	// Token: 0x06001C75 RID: 7285 RVA: 0x000C60D4 File Offset: 0x000C42D4
	public bool UpdatePlacement(Transform transform, Construction common, ref Construction.Target target)
	{
		if (!target.valid)
		{
			return false;
		}
		if (!common.canBypassBuildingPermission && !target.player.CanBuild())
		{
			Construction.lastPlacementError = "You don't have permission to build here";
			return false;
		}
		List<Socket_Base> list = Pool.GetList<Socket_Base>();
		common.FindMaleSockets(target, list);
		foreach (Socket_Base socket_Base in list)
		{
			Construction.Placement placement = null;
			if (!(target.entity != null) || !(target.socket != null) || !target.entity.IsOccupied(target.socket))
			{
				if (placement == null)
				{
					placement = socket_Base.DoPlacement(target);
				}
				if (placement != null)
				{
					if (!socket_Base.CheckSocketMods(placement))
					{
						transform.position = placement.position;
						transform.rotation = placement.rotation;
					}
					else if (!this.TestPlacingThroughRock(ref placement, target))
					{
						transform.position = placement.position;
						transform.rotation = placement.rotation;
						Construction.lastPlacementError = "Placing through rock";
					}
					else if (!Construction.TestPlacingThroughWall(ref placement, transform, common, target))
					{
						transform.position = placement.position;
						transform.rotation = placement.rotation;
						Construction.lastPlacementError = "Placing through wall";
					}
					else if (!this.TestPlacingCloseToRoad(ref placement, target))
					{
						transform.position = placement.position;
						transform.rotation = placement.rotation;
						Construction.lastPlacementError = "Placing too close to road";
					}
					else if (Vector3.Distance(placement.position, target.player.eyes.position) > common.maxplaceDistance + 1f)
					{
						transform.position = placement.position;
						transform.rotation = placement.rotation;
						Construction.lastPlacementError = "Too far away";
					}
					else
					{
						DeployVolume[] array = PrefabAttribute.server.FindAll<DeployVolume>(this.prefabID);
						if (DeployVolume.Check(placement.position, placement.rotation, array, -1))
						{
							transform.position = placement.position;
							transform.rotation = placement.rotation;
							Construction.lastPlacementError = "Not enough space";
						}
						else if (BuildingProximity.Check(target.player, this, placement.position, placement.rotation))
						{
							transform.position = placement.position;
							transform.rotation = placement.rotation;
						}
						else if (common.isBuildingPrivilege && !target.player.CanPlaceBuildingPrivilege(placement.position, placement.rotation, common.bounds))
						{
							transform.position = placement.position;
							transform.rotation = placement.rotation;
							Construction.lastPlacementError = "Cannot stack building privileges";
						}
						else
						{
							bool flag = target.player.IsBuildingBlocked(placement.position, placement.rotation, common.bounds);
							if (common.canBypassBuildingPermission || !flag)
							{
								target.inBuildingPrivilege = flag;
								transform.SetPositionAndRotation(placement.position, placement.rotation);
								Pool.FreeList<Socket_Base>(ref list);
								return true;
							}
							transform.position = placement.position;
							transform.rotation = placement.rotation;
							Construction.lastPlacementError = "You don't have permission to build here";
						}
					}
				}
			}
		}
		Pool.FreeList<Socket_Base>(ref list);
		return false;
	}

	// Token: 0x06001C76 RID: 7286 RVA: 0x000C6424 File Offset: 0x000C4624
	private bool TestPlacingThroughRock(ref Construction.Placement placement, Construction.Target target)
	{
		OBB obb = new OBB(placement.position, Vector3.one, placement.rotation, this.bounds);
		Vector3 center = target.player.GetCenter(true);
		Vector3 origin = target.ray.origin;
		if (Physics.Linecast(center, origin, 65536, QueryTriggerInteraction.Ignore))
		{
			return false;
		}
		RaycastHit raycastHit;
		Vector3 vector = (obb.Trace(target.ray, out raycastHit, float.PositiveInfinity) ? raycastHit.point : obb.ClosestPoint(origin));
		return !Physics.Linecast(origin, vector, 65536, QueryTriggerInteraction.Ignore);
	}

	// Token: 0x06001C77 RID: 7287 RVA: 0x000C64B4 File Offset: 0x000C46B4
	private static bool TestPlacingThroughWall(ref Construction.Placement placement, Transform transform, Construction common, Construction.Target target)
	{
		Vector3 vector = placement.position;
		if (common.deployOffset != null)
		{
			vector += placement.rotation * common.deployOffset.localPosition;
		}
		Vector3 vector2 = vector - target.ray.origin;
		RaycastHit raycastHit;
		if (!Physics.Raycast(target.ray.origin, vector2.normalized, out raycastHit, vector2.magnitude, 2097152))
		{
			return true;
		}
		StabilityEntity stabilityEntity = raycastHit.GetEntity() as StabilityEntity;
		if (stabilityEntity != null && target.entity == stabilityEntity)
		{
			return true;
		}
		if (vector2.magnitude - raycastHit.distance < 0.2f)
		{
			return true;
		}
		Construction.lastPlacementError = "object in placement path";
		transform.SetPositionAndRotation(raycastHit.point, placement.rotation);
		return false;
	}

	// Token: 0x06001C78 RID: 7288 RVA: 0x000C6590 File Offset: 0x000C4790
	private bool TestPlacingCloseToRoad(ref Construction.Placement placement, Construction.Target target)
	{
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		if (heightMap == null)
		{
			return true;
		}
		if (topologyMap == null)
		{
			return true;
		}
		OBB obb = new OBB(placement.position, Vector3.one, placement.rotation, this.bounds);
		float num = Mathf.Abs(heightMap.GetHeight(obb.position) - obb.position.y);
		if (num > 9f)
		{
			return true;
		}
		float num2 = Mathf.Lerp(3f, 0f, num / 9f);
		Vector3 position = obb.position;
		Vector3 point = obb.GetPoint(-1f, 0f, -1f);
		Vector3 point2 = obb.GetPoint(-1f, 0f, 1f);
		Vector3 point3 = obb.GetPoint(1f, 0f, -1f);
		Vector3 point4 = obb.GetPoint(1f, 0f, 1f);
		int topology = topologyMap.GetTopology(position, num2);
		int topology2 = topologyMap.GetTopology(point, num2);
		int topology3 = topologyMap.GetTopology(point2, num2);
		int topology4 = topologyMap.GetTopology(point3, num2);
		int topology5 = topologyMap.GetTopology(point4, num2);
		return ((topology | topology2 | topology3 | topology4 | topology5) & 526336) == 0;
	}

	// Token: 0x06001C79 RID: 7289 RVA: 0x000C66DC File Offset: 0x000C48DC
	public virtual bool ShowAsNeutral(Construction.Target target)
	{
		return target.inBuildingPrivilege;
	}

	// Token: 0x06001C7A RID: 7290 RVA: 0x000C66E4 File Offset: 0x000C48E4
	public BaseEntity CreateConstruction(Construction.Target target, bool bNeedsValidPlacement = false)
	{
		GameObject gameObject = GameManager.server.CreatePrefab(this.fullName, Vector3.zero, Quaternion.identity, false);
		bool flag = this.UpdatePlacement(gameObject.transform, this, ref target);
		BaseEntity baseEntity = gameObject.ToBaseEntity();
		if (bNeedsValidPlacement && !flag)
		{
			if (baseEntity.IsValid())
			{
				baseEntity.Kill(BaseNetworkable.DestroyMode.None);
			}
			else
			{
				GameManager.Destroy(gameObject, 0f);
			}
			return null;
		}
		DecayEntity decayEntity = baseEntity as DecayEntity;
		if (decayEntity)
		{
			decayEntity.AttachToBuilding(target.entity as DecayEntity);
		}
		return baseEntity;
	}

	// Token: 0x06001C7B RID: 7291 RVA: 0x000C676C File Offset: 0x000C496C
	public bool HasMaleSockets(Construction.Target target)
	{
		foreach (Socket_Base socket_Base in this.allSockets)
		{
			if (socket_Base.male && !socket_Base.maleDummy && socket_Base.TestTarget(target))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001C7C RID: 7292 RVA: 0x000C67B0 File Offset: 0x000C49B0
	public void FindMaleSockets(Construction.Target target, List<Socket_Base> sockets)
	{
		foreach (Socket_Base socket_Base in this.allSockets)
		{
			if (socket_Base.male && !socket_Base.maleDummy && socket_Base.TestTarget(target))
			{
				sockets.Add(socket_Base);
			}
		}
	}

	// Token: 0x06001C7D RID: 7293 RVA: 0x000C67F8 File Offset: 0x000C49F8
	public ConstructionGrade GetGrade(BuildingGrade.Enum iGrade, ulong iSkin)
	{
		foreach (ConstructionGrade constructionGrade in this.grades)
		{
			if (constructionGrade.gradeBase.type == iGrade && constructionGrade.gradeBase.skin == iSkin)
			{
				return constructionGrade;
			}
		}
		return this.defaultGrade;
	}

	// Token: 0x06001C7E RID: 7294 RVA: 0x000C6844 File Offset: 0x000C4A44
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		this.isBuildingPrivilege = rootObj.GetComponent<BuildingPrivlidge>();
		this.isSleepingBag = rootObj.GetComponent<SleepingBag>();
		this.bounds = rootObj.GetComponent<BaseEntity>().bounds;
		this.deployable = base.GetComponent<Deployable>();
		this.placeholder = base.GetComponentInChildren<ConstructionPlaceholder>();
		this.allSockets = base.GetComponentsInChildren<Socket_Base>(true);
		this.allProximities = base.GetComponentsInChildren<BuildingProximity>(true);
		this.socketHandle = base.GetComponentsInChildren<SocketHandle>(true).FirstOrDefault<SocketHandle>();
		this.grades = rootObj.GetComponents<ConstructionGrade>();
		foreach (ConstructionGrade constructionGrade in this.grades)
		{
			if (!(constructionGrade == null))
			{
				constructionGrade.construction = this;
				if (!(this.defaultGrade != null))
				{
					this.defaultGrade = constructionGrade;
				}
			}
		}
	}

	// Token: 0x06001C7F RID: 7295 RVA: 0x000C691F File Offset: 0x000C4B1F
	protected override Type GetIndexedType()
	{
		return typeof(Construction);
	}

	// Token: 0x04001506 RID: 5382
	public static string lastPlacementError;

	// Token: 0x04001507 RID: 5383
	public BaseEntity.Menu.Option info;

	// Token: 0x04001508 RID: 5384
	public bool canBypassBuildingPermission;

	// Token: 0x04001509 RID: 5385
	[FormerlySerializedAs("canRotate")]
	public bool canRotateBeforePlacement;

	// Token: 0x0400150A RID: 5386
	[FormerlySerializedAs("canRotate")]
	public bool canRotateAfterPlacement;

	// Token: 0x0400150B RID: 5387
	public bool checkVolumeOnRotate;

	// Token: 0x0400150C RID: 5388
	public bool checkVolumeOnUpgrade;

	// Token: 0x0400150D RID: 5389
	public bool canPlaceAtMaxDistance;

	// Token: 0x0400150E RID: 5390
	public bool placeOnWater;

	// Token: 0x0400150F RID: 5391
	public Vector3 rotationAmount = new Vector3(0f, 90f, 0f);

	// Token: 0x04001510 RID: 5392
	public Vector3 applyStartingRotation = Vector3.zero;

	// Token: 0x04001511 RID: 5393
	public Transform deployOffset;

	// Token: 0x04001512 RID: 5394
	[Range(0f, 10f)]
	public float healthMultiplier = 1f;

	// Token: 0x04001513 RID: 5395
	[Range(0f, 10f)]
	public float costMultiplier = 1f;

	// Token: 0x04001514 RID: 5396
	[Range(1f, 50f)]
	public float maxplaceDistance = 4f;

	// Token: 0x04001515 RID: 5397
	public Mesh guideMesh;

	// Token: 0x04001516 RID: 5398
	[NonSerialized]
	public Socket_Base[] allSockets;

	// Token: 0x04001517 RID: 5399
	[NonSerialized]
	public BuildingProximity[] allProximities;

	// Token: 0x04001518 RID: 5400
	[NonSerialized]
	public ConstructionGrade defaultGrade;

	// Token: 0x04001519 RID: 5401
	[NonSerialized]
	public SocketHandle socketHandle;

	// Token: 0x0400151A RID: 5402
	[NonSerialized]
	public Bounds bounds;

	// Token: 0x0400151B RID: 5403
	[NonSerialized]
	public bool isBuildingPrivilege;

	// Token: 0x0400151C RID: 5404
	[NonSerialized]
	public bool isSleepingBag;

	// Token: 0x0400151D RID: 5405
	[NonSerialized]
	public ConstructionGrade[] grades;

	// Token: 0x0400151E RID: 5406
	[NonSerialized]
	public Deployable deployable;

	// Token: 0x0400151F RID: 5407
	[NonSerialized]
	public ConstructionPlaceholder placeholder;

	// Token: 0x02000C9D RID: 3229
	public struct Target
	{
		// Token: 0x06004F67 RID: 20327 RVA: 0x001A694C File Offset: 0x001A4B4C
		public Quaternion GetWorldRotation(bool female)
		{
			Quaternion quaternion = this.socket.rotation;
			if (this.socket.male && this.socket.female && female)
			{
				quaternion = this.socket.rotation * Quaternion.Euler(180f, 0f, 180f);
			}
			return this.entity.transform.rotation * quaternion;
		}

		// Token: 0x06004F68 RID: 20328 RVA: 0x001A69C0 File Offset: 0x001A4BC0
		public Vector3 GetWorldPosition()
		{
			return this.entity.transform.localToWorldMatrix.MultiplyPoint3x4(this.socket.position);
		}

		// Token: 0x04004476 RID: 17526
		public bool valid;

		// Token: 0x04004477 RID: 17527
		public Ray ray;

		// Token: 0x04004478 RID: 17528
		public BaseEntity entity;

		// Token: 0x04004479 RID: 17529
		public Socket_Base socket;

		// Token: 0x0400447A RID: 17530
		public bool onTerrain;

		// Token: 0x0400447B RID: 17531
		public Vector3 position;

		// Token: 0x0400447C RID: 17532
		public Vector3 normal;

		// Token: 0x0400447D RID: 17533
		public Vector3 rotation;

		// Token: 0x0400447E RID: 17534
		public BasePlayer player;

		// Token: 0x0400447F RID: 17535
		public bool inBuildingPrivilege;
	}

	// Token: 0x02000C9E RID: 3230
	public class Placement
	{
		// Token: 0x04004480 RID: 17536
		public Vector3 position;

		// Token: 0x04004481 RID: 17537
		public Quaternion rotation;
	}

	// Token: 0x02000C9F RID: 3231
	public class Grade
	{
		// Token: 0x1700069C RID: 1692
		// (get) Token: 0x06004F6A RID: 20330 RVA: 0x001A69F0 File Offset: 0x001A4BF0
		public PhysicMaterial physicMaterial
		{
			get
			{
				return this.grade.physicMaterial;
			}
		}

		// Token: 0x1700069D RID: 1693
		// (get) Token: 0x06004F6B RID: 20331 RVA: 0x001A69FD File Offset: 0x001A4BFD
		public ProtectionProperties damageProtecton
		{
			get
			{
				return this.grade.damageProtecton;
			}
		}

		// Token: 0x04004482 RID: 17538
		public BuildingGrade grade;

		// Token: 0x04004483 RID: 17539
		public float maxHealth;

		// Token: 0x04004484 RID: 17540
		public List<ItemAmount> costToBuild;
	}
}
