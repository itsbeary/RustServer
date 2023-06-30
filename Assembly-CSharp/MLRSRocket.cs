using System;
using Rust;
using UnityEngine;

// Token: 0x0200048F RID: 1167
public class MLRSRocket : TimedExplosive, SamSite.ISamSiteTarget
{
	// Token: 0x17000321 RID: 801
	// (get) Token: 0x06002684 RID: 9860 RVA: 0x000F2E7B File Offset: 0x000F107B
	public SamSite.SamTargetType SAMTargetType
	{
		get
		{
			return SamSite.targetTypeMissile;
		}
	}

	// Token: 0x06002685 RID: 9861 RVA: 0x000F2E82 File Offset: 0x000F1082
	public override void ServerInit()
	{
		base.ServerInit();
		this.CreateMapMarker();
		Effect.server.Run(this.launchBlastFXPrefab.resourcePath, base.PivotPoint(), base.transform.up, null, true);
	}

	// Token: 0x06002686 RID: 9862 RVA: 0x000F2EB4 File Offset: 0x000F10B4
	public override void ProjectileImpact(RaycastHit info, Vector3 rayOrigin)
	{
		this.Explode(rayOrigin);
		if (Physics.Raycast(info.point + Vector3.up, Vector3.down, 4f, 1218511121, QueryTriggerInteraction.Ignore))
		{
			Effect.server.Run(this.explosionGroundFXPrefab.resourcePath, info.point, Vector3.up, null, true);
		}
	}

	// Token: 0x06002687 RID: 9863 RVA: 0x000F2F10 File Offset: 0x000F1110
	private void CreateMapMarker()
	{
		BaseEntity baseEntity = this.mapMarkerInstanceRef.Get(base.isServer);
		if (baseEntity.IsValid())
		{
			baseEntity.Kill(BaseNetworkable.DestroyMode.None);
		}
		GameManager server = GameManager.server;
		GameObjectRef gameObjectRef = this.mapMarkerPrefab;
		BaseEntity baseEntity2 = server.CreateEntity((gameObjectRef != null) ? gameObjectRef.resourcePath : null, base.transform.position, Quaternion.identity, true);
		baseEntity2.OwnerID = base.OwnerID;
		baseEntity2.Spawn();
		baseEntity2.SetParent(this, true, false);
		this.mapMarkerInstanceRef.Set(baseEntity2);
	}

	// Token: 0x06002688 RID: 9864 RVA: 0x0000441C File Offset: 0x0000261C
	public bool IsValidSAMTarget(bool staticRespawn)
	{
		return true;
	}

	// Token: 0x06002689 RID: 9865 RVA: 0x000F2F93 File Offset: 0x000F1193
	public override Vector3 GetLocalVelocityServer()
	{
		return this.serverProjectile.CurrentVelocity;
	}

	// Token: 0x0600268A RID: 9866 RVA: 0x000F2FA0 File Offset: 0x000F11A0
	private void OnTriggerEnter(Collider other)
	{
		if (!other.IsOnLayer(Layer.Trigger))
		{
			return;
		}
		if (other.CompareTag("MLRSRocketTrigger"))
		{
			this.Explode();
			TimedExplosive componentInParent = other.GetComponentInParent<TimedExplosive>();
			if (componentInParent != null)
			{
				componentInParent.Explode();
				return;
			}
		}
		else if (other.GetComponent<TriggerSafeZone>() != null)
		{
			base.Kill(BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x04001EF6 RID: 7926
	[SerializeField]
	private GameObjectRef mapMarkerPrefab;

	// Token: 0x04001EF7 RID: 7927
	[SerializeField]
	private GameObjectRef launchBlastFXPrefab;

	// Token: 0x04001EF8 RID: 7928
	[SerializeField]
	private GameObjectRef explosionGroundFXPrefab;

	// Token: 0x04001EF9 RID: 7929
	[SerializeField]
	private ServerProjectile serverProjectile;

	// Token: 0x04001EFA RID: 7930
	private EntityRef mapMarkerInstanceRef;
}
