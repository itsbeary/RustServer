using System;
using UnityEngine;

// Token: 0x02000446 RID: 1094
public class PlatformEntity : BaseEntity
{
	// Token: 0x060024BE RID: 9406 RVA: 0x000E9724 File Offset: 0x000E7924
	protected void FixedUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		if (this.targetPosition == Vector3.zero || Vector3.Distance(base.transform.position, this.targetPosition) < 0.01f)
		{
			Vector2 vector = UnityEngine.Random.insideUnitCircle * 10f;
			this.targetPosition = base.transform.position + new Vector3(vector.x, 0f, vector.y);
			if (TerrainMeta.HeightMap != null && TerrainMeta.WaterMap != null)
			{
				float height = TerrainMeta.HeightMap.GetHeight(this.targetPosition);
				float height2 = TerrainMeta.WaterMap.GetHeight(this.targetPosition);
				this.targetPosition.y = Mathf.Max(height, height2) + 1f;
			}
			this.targetRotation = Quaternion.LookRotation(this.targetPosition - base.transform.position);
		}
		base.transform.SetPositionAndRotation(Vector3.MoveTowards(base.transform.position, this.targetPosition, Time.fixedDeltaTime * 1f), Quaternion.RotateTowards(base.transform.rotation, this.targetRotation, Time.fixedDeltaTime * 10f));
	}

	// Token: 0x060024BF RID: 9407 RVA: 0x0000627E File Offset: 0x0000447E
	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	// Token: 0x04001CC9 RID: 7369
	private const float movementSpeed = 1f;

	// Token: 0x04001CCA RID: 7370
	private const float rotationSpeed = 10f;

	// Token: 0x04001CCB RID: 7371
	private const float radius = 10f;

	// Token: 0x04001CCC RID: 7372
	private Vector3 targetPosition = Vector3.zero;

	// Token: 0x04001CCD RID: 7373
	private Quaternion targetRotation = Quaternion.identity;
}
