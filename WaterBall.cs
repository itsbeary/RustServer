using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000430 RID: 1072
public class WaterBall : BaseEntity
{
	// Token: 0x0600244B RID: 9291 RVA: 0x000E7834 File Offset: 0x000E5A34
	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.Extinguish), 10f);
	}

	// Token: 0x0600244C RID: 9292 RVA: 0x000E7853 File Offset: 0x000E5A53
	public void Extinguish()
	{
		base.CancelInvoke(new Action(this.Extinguish));
		if (!base.IsDestroyed)
		{
			base.Kill(BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x0600244D RID: 9293 RVA: 0x000E7876 File Offset: 0x000E5A76
	public void FixedUpdate()
	{
		if (base.isServer)
		{
			base.GetComponent<Rigidbody>().AddForce(Physics.gravity, ForceMode.Acceleration);
		}
	}

	// Token: 0x0600244E RID: 9294 RVA: 0x000E7894 File Offset: 0x000E5A94
	public static bool DoSplash(Vector3 position, float radius, ItemDefinition liquidDef, int amount)
	{
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(position, radius, list, 1220225811, QueryTriggerInteraction.Collide);
		int num = 0;
		int num2 = amount;
		while (amount > 0 && num < 3)
		{
			List<ISplashable> list2 = Pool.GetList<ISplashable>();
			foreach (BaseEntity baseEntity in list)
			{
				if (!baseEntity.isClient)
				{
					ISplashable splashable = baseEntity as ISplashable;
					if (splashable != null && !list2.Contains(splashable) && splashable.WantsSplash(liquidDef, amount))
					{
						list2.Add(splashable);
					}
				}
			}
			if (list2.Count == 0)
			{
				break;
			}
			int num3 = Mathf.CeilToInt((float)(amount / list2.Count));
			foreach (ISplashable splashable2 in list2)
			{
				int num4 = splashable2.DoSplash(liquidDef, Mathf.Min(amount, num3));
				amount -= num4;
				if (amount <= 0)
				{
					break;
				}
			}
			Pool.FreeList<ISplashable>(ref list2);
			num++;
		}
		Pool.FreeList<BaseEntity>(ref list);
		return amount < num2;
	}

	// Token: 0x0600244F RID: 9295 RVA: 0x000E79C0 File Offset: 0x000E5BC0
	private void OnCollisionEnter(Collision collision)
	{
		if (base.isClient)
		{
			return;
		}
		if (this.myRigidBody.isKinematic)
		{
			return;
		}
		float num = 2.5f;
		WaterBall.DoSplash(base.transform.position + new Vector3(0f, num * 0.75f, 0f), num, this.liquidType, this.waterAmount);
		Effect.server.Run(this.waterExplosion.resourcePath, base.transform.position + new Vector3(0f, 0f, 0f), Vector3.up, null, false);
		this.myRigidBody.isKinematic = true;
		base.Invoke(new Action(this.Extinguish), 2f);
	}

	// Token: 0x04001C3E RID: 7230
	public ItemDefinition liquidType;

	// Token: 0x04001C3F RID: 7231
	public int waterAmount;

	// Token: 0x04001C40 RID: 7232
	public GameObjectRef waterExplosion;

	// Token: 0x04001C41 RID: 7233
	public Rigidbody myRigidBody;
}
