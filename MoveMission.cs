using System;
using UnityEngine;

// Token: 0x0200061C RID: 1564
[CreateAssetMenu(menuName = "Rust/Missions/MoveMission")]
public class MoveMission : BaseMission
{
	// Token: 0x06002E51 RID: 11857 RVA: 0x00116444 File Offset: 0x00114644
	public override void MissionStart(BaseMission.MissionInstance instance, BasePlayer assignee)
	{
		Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
		onUnitSphere.y = 0f;
		onUnitSphere.Normalize();
		Vector3 vector = assignee.transform.position + onUnitSphere * UnityEngine.Random.Range(this.minDistForMovePoint, this.maxDistForMovePoint);
		float num = vector.y;
		float num2 = vector.y;
		if (TerrainMeta.WaterMap != null)
		{
			num2 = TerrainMeta.WaterMap.GetHeight(vector);
		}
		if (TerrainMeta.HeightMap != null)
		{
			num = TerrainMeta.HeightMap.GetHeight(vector);
		}
		vector.y = Mathf.Max(num2, num);
		instance.missionLocation = vector;
		base.MissionStart(instance, assignee);
	}

	// Token: 0x06002E52 RID: 11858 RVA: 0x001164EF File Offset: 0x001146EF
	public override void MissionEnded(BaseMission.MissionInstance instance, BasePlayer assignee)
	{
		base.MissionEnded(instance, assignee);
	}

	// Token: 0x06002E53 RID: 11859 RVA: 0x001164F9 File Offset: 0x001146F9
	public override Sprite GetIcon(BaseMission.MissionInstance instance)
	{
		if (instance.status != BaseMission.MissionStatus.Accomplished)
		{
			return this.icon;
		}
		return this.providerIcon;
	}

	// Token: 0x06002E54 RID: 11860 RVA: 0x00116514 File Offset: 0x00114714
	public override void Think(BaseMission.MissionInstance instance, BasePlayer assignee, float delta)
	{
		float num = Vector3.Distance(instance.missionLocation, assignee.transform.position);
		if (instance.status == BaseMission.MissionStatus.Active && num <= this.minDistFromLocation)
		{
			this.MissionSuccess(instance, assignee);
			BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(instance.providerID);
			if (baseNetworkable)
			{
				instance.missionLocation = baseNetworkable.transform.position;
			}
			return;
		}
		if (instance.status == BaseMission.MissionStatus.Accomplished)
		{
			float num2 = this.minDistFromLocation;
		}
		base.Think(instance, assignee, delta);
	}

	// Token: 0x04002609 RID: 9737
	public float minDistForMovePoint = 20f;

	// Token: 0x0400260A RID: 9738
	public float maxDistForMovePoint = 25f;

	// Token: 0x0400260B RID: 9739
	private float minDistFromLocation = 3f;
}
