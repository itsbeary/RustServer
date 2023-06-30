using System;
using UnityEngine;

// Token: 0x02000485 RID: 1157
public class CH47Helicopter : BaseHelicopterVehicle
{
	// Token: 0x06002645 RID: 9797 RVA: 0x000F19FF File Offset: 0x000EFBFF
	public override void ServerInit()
	{
		this.rigidBody.isKinematic = false;
		base.ServerInit();
		this.CreateMapMarker();
	}

	// Token: 0x06002646 RID: 9798 RVA: 0x000F1A19 File Offset: 0x000EFC19
	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
	}

	// Token: 0x06002647 RID: 9799 RVA: 0x000F1A24 File Offset: 0x000EFC24
	public void CreateMapMarker()
	{
		if (this.mapMarkerInstance)
		{
			this.mapMarkerInstance.Kill(BaseNetworkable.DestroyMode.None);
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.mapMarkerEntityPrefab.resourcePath, Vector3.zero, Quaternion.identity, true);
		baseEntity.Spawn();
		baseEntity.SetParent(this, false, false);
		this.mapMarkerInstance = baseEntity;
	}

	// Token: 0x06002648 RID: 9800 RVA: 0x00007A44 File Offset: 0x00005C44
	protected override bool CanPushNow(BasePlayer pusher)
	{
		return false;
	}

	// Token: 0x04001EAE RID: 7854
	public GameObjectRef mapMarkerEntityPrefab;

	// Token: 0x04001EAF RID: 7855
	private BaseEntity mapMarkerInstance;
}
