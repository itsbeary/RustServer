using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200040A RID: 1034
public class SurveyCharge : TimedExplosive
{
	// Token: 0x06002350 RID: 9040 RVA: 0x000E204C File Offset: 0x000E024C
	public override void Explode()
	{
		base.Explode();
		if (WaterLevel.Test(base.transform.position, true, true, this))
		{
			return;
		}
		ResourceDepositManager.ResourceDeposit orCreate = ResourceDepositManager.GetOrCreate(base.transform.position);
		if (orCreate == null)
		{
			return;
		}
		if (Time.realtimeSinceStartup - orCreate.lastSurveyTime < 10f)
		{
			return;
		}
		orCreate.lastSurveyTime = Time.realtimeSinceStartup;
		RaycastHit raycastHit;
		if (!TransformUtil.GetGroundInfo(base.transform.position, out raycastHit, 0.3f, 8388608, null))
		{
			return;
		}
		Vector3 point = raycastHit.point;
		Vector3 normal = raycastHit.normal;
		List<SurveyCrater> list = Pool.GetList<SurveyCrater>();
		Vis.Entities<SurveyCrater>(base.transform.position, 10f, list, 1, QueryTriggerInteraction.Collide);
		bool flag = list.Count > 0;
		Pool.FreeList<SurveyCrater>(ref list);
		if (flag)
		{
			return;
		}
		bool flag2 = false;
		bool flag3 = false;
		foreach (ResourceDepositManager.ResourceDeposit.ResourceDepositEntry resourceDepositEntry in orCreate._resources)
		{
			if (resourceDepositEntry.spawnType == ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM && !resourceDepositEntry.isLiquid && resourceDepositEntry.amount >= 1000)
			{
				int num = Mathf.Clamp(Mathf.CeilToInt(2.5f / resourceDepositEntry.workNeeded * 10f), 0, 5);
				int num2 = 1;
				flag2 = true;
				if (resourceDepositEntry.isLiquid)
				{
					flag3 = true;
				}
				for (int i = 0; i < num; i++)
				{
					Item item = ItemManager.Create(resourceDepositEntry.type, num2, 0UL);
					Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(20f, Vector3.up, true);
					item.Drop(base.transform.position + Vector3.up * 1f, this.GetInheritedDropVelocity() + modifiedAimConeDirection * UnityEngine.Random.Range(5f, 10f), UnityEngine.Random.rotation).SetAngularVelocity(UnityEngine.Random.rotation.eulerAngles * 5f);
				}
			}
		}
		if (flag2)
		{
			string text = (flag3 ? this.craterPrefab_Oil.resourcePath : this.craterPrefab.resourcePath);
			BaseEntity baseEntity = GameManager.server.CreateEntity(text, point, Quaternion.identity, true);
			if (baseEntity)
			{
				baseEntity.Spawn();
			}
		}
	}

	// Token: 0x04001B2E RID: 6958
	public GameObjectRef craterPrefab;

	// Token: 0x04001B2F RID: 6959
	public GameObjectRef craterPrefab_Oil;
}
