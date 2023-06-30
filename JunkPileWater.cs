using System;
using UnityEngine;

// Token: 0x0200016B RID: 363
public class JunkPileWater : JunkPile
{
	// Token: 0x06001781 RID: 6017 RVA: 0x000B2560 File Offset: 0x000B0760
	public override void Spawn()
	{
		Vector3 position = base.transform.position;
		position.y = TerrainMeta.WaterMap.GetHeight(base.transform.position);
		base.transform.position = position;
		base.Spawn();
		this.baseRotation = Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y, 0f);
	}

	// Token: 0x06001782 RID: 6018 RVA: 0x000B25D4 File Offset: 0x000B07D4
	public void FixedUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		this.UpdateMovement();
	}

	// Token: 0x06001783 RID: 6019 RVA: 0x000B25E8 File Offset: 0x000B07E8
	public void UpdateMovement()
	{
		if (this.nextPlayerCheck <= 0f)
		{
			this.nextPlayerCheck = UnityEngine.Random.Range(0.5f, 1f);
			JunkPileWater.junkpileWaterWorkQueue.Add(this);
		}
		if (!this.isSinking && this.hasPlayersNearby)
		{
			float height = WaterSystem.GetHeight(base.transform.position);
			base.transform.position = new Vector3(base.transform.position.x, height, base.transform.position.z);
			if (this.buoyancyPoints != null && this.buoyancyPoints.Length >= 3)
			{
				Vector3 position = base.transform.position;
				Vector3 localPosition = this.buoyancyPoints[0].localPosition;
				Vector3 localPosition2 = this.buoyancyPoints[1].localPosition;
				Vector3 localPosition3 = this.buoyancyPoints[2].localPosition;
				Vector3 vector = localPosition + position;
				Vector3 vector2 = localPosition2 + position;
				Vector3 vector3 = localPosition3 + position;
				vector.y = WaterSystem.GetHeight(vector);
				vector2.y = WaterSystem.GetHeight(vector2);
				vector3.y = WaterSystem.GetHeight(vector3);
				Vector3 vector4 = new Vector3(position.x, vector.y - localPosition.y, position.z);
				Vector3 vector5 = vector2 - vector;
				Vector3 vector6 = Vector3.Cross(vector3 - vector, vector5);
				Vector3 eulerAngles = Quaternion.LookRotation(new Vector3(vector6.x, vector6.z, vector6.y)).eulerAngles;
				Quaternion quaternion = Quaternion.Euler(-eulerAngles.x, 0f, -eulerAngles.y);
				if (this.first)
				{
					this.baseRotation = Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y, 0f);
					this.first = false;
				}
				base.transform.SetPositionAndRotation(vector4, quaternion * this.baseRotation);
			}
		}
	}

	// Token: 0x06001784 RID: 6020 RVA: 0x000B27F1 File Offset: 0x000B09F1
	public void UpdateNearbyPlayers()
	{
		this.hasPlayersNearby = BaseNetworkable.HasCloseConnections(base.transform.position, this.updateCullRange);
	}

	// Token: 0x04001022 RID: 4130
	public static JunkPileWater.JunkpileWaterWorkQueue junkpileWaterWorkQueue = new JunkPileWater.JunkpileWaterWorkQueue();

	// Token: 0x04001023 RID: 4131
	[ServerVar]
	[Help("How many milliseconds to budget for processing life story updates per frame")]
	public static float framebudgetms = 0.25f;

	// Token: 0x04001024 RID: 4132
	public Transform[] buoyancyPoints;

	// Token: 0x04001025 RID: 4133
	public bool debugDraw;

	// Token: 0x04001026 RID: 4134
	public float updateCullRange = 16f;

	// Token: 0x04001027 RID: 4135
	private Quaternion baseRotation = Quaternion.identity;

	// Token: 0x04001028 RID: 4136
	private bool first = true;

	// Token: 0x04001029 RID: 4137
	private TimeUntil nextPlayerCheck;

	// Token: 0x0400102A RID: 4138
	private bool hasPlayersNearby;

	// Token: 0x02000C3E RID: 3134
	public class JunkpileWaterWorkQueue : ObjectWorkQueue<JunkPileWater>
	{
		// Token: 0x06004E5E RID: 20062 RVA: 0x001A2835 File Offset: 0x001A0A35
		protected override void RunJob(JunkPileWater entity)
		{
			if (this.ShouldAdd(entity))
			{
				entity.UpdateNearbyPlayers();
			}
		}

		// Token: 0x06004E5F RID: 20063 RVA: 0x001A2846 File Offset: 0x001A0A46
		protected override bool ShouldAdd(JunkPileWater entity)
		{
			return base.ShouldAdd(entity) && entity.IsValid();
		}
	}
}
