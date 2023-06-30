using System;
using Rust;
using UnityEngine;

// Token: 0x02000146 RID: 326
public class SkyLantern : StorageContainer, IIgniteable
{
	// Token: 0x06001721 RID: 5921 RVA: 0x0000627E File Offset: 0x0000447E
	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	// Token: 0x06001722 RID: 5922 RVA: 0x000B0938 File Offset: 0x000AEB38
	public override void ServerInit()
	{
		base.ServerInit();
		this.randOffset = ((UnityEngine.Random.Range(0.5f, 1f) * (float)UnityEngine.Random.Range(0, 2) == 1f) ? (-1f) : 1f);
		this.travelVec = (Vector3.forward + Vector3.right * this.randOffset).normalized;
		base.Invoke(new Action(this.StartSinking), this.lifeTime - 15f);
		base.Invoke(new Action(this.SelfDestroy), this.lifeTime);
		this.travelSpeed = UnityEngine.Random.Range(1.75f, 2.25f);
		this.gravityScale *= UnityEngine.Random.Range(1f, 1.25f);
		base.InvokeRepeating(new Action(this.UpdateIdealAltitude), 0f, 1f);
	}

	// Token: 0x06001723 RID: 5923 RVA: 0x000B0A28 File Offset: 0x000AEC28
	public void Ignite(Vector3 fromPos)
	{
		base.gameObject.transform.RemoveComponent<GroundWatch>();
		base.gameObject.transform.RemoveComponent<DestroyOnGroundMissing>();
		base.gameObject.layer = 14;
		this.travelVec = Vector3Ex.Direction2D(base.transform.position, fromPos);
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		this.UpdateIdealAltitude();
	}

	// Token: 0x06001724 RID: 5924 RVA: 0x000B0A8C File Offset: 0x000AEC8C
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		if (!base.isServer)
		{
			return;
		}
		if (info.damageTypes.Has(DamageType.Heat) && this.CanIgnite())
		{
			this.Ignite(info.PointStart);
			return;
		}
		if (base.IsOn() && !base.IsBroken())
		{
			this.StartSinking();
		}
	}

	// Token: 0x06001725 RID: 5925 RVA: 0x00003384 File Offset: 0x00001584
	public void SelfDestroy()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06001726 RID: 5926 RVA: 0x000B0AE2 File Offset: 0x000AECE2
	public bool CanIgnite()
	{
		return !base.IsOn() && !base.IsBroken();
	}

	// Token: 0x06001727 RID: 5927 RVA: 0x000B0AF8 File Offset: 0x000AECF8
	public void UpdateIdealAltitude()
	{
		if (!base.IsOn())
		{
			return;
		}
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		float num = ((heightMap != null) ? heightMap.GetHeight(base.transform.position) : 0f);
		TerrainWaterMap waterMap = TerrainMeta.WaterMap;
		float num2 = ((waterMap != null) ? waterMap.GetHeight(base.transform.position) : 0f);
		this.idealAltitude = Mathf.Max(num, num2) + this.hoverHeight;
		if (this.hoverHeight != 0f)
		{
			this.idealAltitude -= 2f * Mathf.Abs(this.randOffset);
		}
	}

	// Token: 0x06001728 RID: 5928 RVA: 0x000B0B8F File Offset: 0x000AED8F
	public void StartSinking()
	{
		if (base.IsBroken())
		{
			return;
		}
		this.hoverHeight = 0f;
		this.travelVec = Vector3.zero;
		this.UpdateIdealAltitude();
		base.SetFlag(BaseEntity.Flags.Broken, true, false, true);
	}

	// Token: 0x06001729 RID: 5929 RVA: 0x000B0BC4 File Offset: 0x000AEDC4
	public void FixedUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		if (!base.IsOn())
		{
			return;
		}
		float num = Mathf.Abs(base.transform.position.y - this.idealAltitude);
		float num2 = ((base.transform.position.y < this.idealAltitude) ? (-1f) : 1f);
		float num3 = Mathf.InverseLerp(0f, 10f, num) * num2;
		if (base.IsBroken())
		{
			this.travelVec = Vector3.Lerp(this.travelVec, Vector3.zero, Time.fixedDeltaTime * 0.5f);
			num3 = 0.7f;
		}
		Vector3 vector = Vector3.zero;
		vector = Vector3.up * this.gravityScale * Physics.gravity.y * num3;
		vector += this.travelVec * this.travelSpeed;
		Vector3 vector2 = base.transform.position + vector * Time.fixedDeltaTime;
		Vector3 vector3 = Vector3Ex.Direction(vector2, base.transform.position);
		float num4 = Vector3.Distance(vector2, base.transform.position);
		RaycastHit raycastHit;
		if (!Physics.SphereCast(this.collisionCheckPoint.position, this.collisionRadius, vector3, out raycastHit, num4, 1218519297))
		{
			base.transform.position = vector2;
			base.transform.Rotate(Vector3.up, this.rotationSpeed * this.randOffset * Time.deltaTime, Space.Self);
			return;
		}
		this.StartSinking();
	}

	// Token: 0x04000F70 RID: 3952
	public float gravityScale = -0.1f;

	// Token: 0x04000F71 RID: 3953
	public float travelSpeed = 2f;

	// Token: 0x04000F72 RID: 3954
	public float collisionRadius = 0.5f;

	// Token: 0x04000F73 RID: 3955
	public float rotationSpeed = 5f;

	// Token: 0x04000F74 RID: 3956
	public float randOffset = 1f;

	// Token: 0x04000F75 RID: 3957
	public float lifeTime = 120f;

	// Token: 0x04000F76 RID: 3958
	public float hoverHeight = 14f;

	// Token: 0x04000F77 RID: 3959
	public Transform collisionCheckPoint;

	// Token: 0x04000F78 RID: 3960
	private float idealAltitude;

	// Token: 0x04000F79 RID: 3961
	private Vector3 travelVec = Vector3.forward;
}
