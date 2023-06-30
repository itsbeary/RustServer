using System;
using UnityEngine;

// Token: 0x0200025C RID: 604
public class ConstructionSocket_Elevator : ConstructionSocket
{
	// Token: 0x06001C9B RID: 7323 RVA: 0x000C714C File Offset: 0x000C534C
	protected override bool CanConnectToEntity(Construction.Target target)
	{
		Elevator elevator;
		if ((elevator = target.entity as Elevator) != null && elevator.Floor >= this.MaxFloor)
		{
			return false;
		}
		Vector3 worldPosition = target.GetWorldPosition();
		Quaternion worldRotation = target.GetWorldRotation(true);
		return !GamePhysics.CheckOBB(new OBB(worldPosition, new Vector3(2f, 0.5f, 2f), worldRotation), 2097152, QueryTriggerInteraction.UseGlobal) && base.CanConnectToEntity(target);
	}

	// Token: 0x06001C9C RID: 7324 RVA: 0x000C71BC File Offset: 0x000C53BC
	public override bool CanConnect(Vector3 position, Quaternion rotation, Socket_Base socket, Vector3 socketPosition, Quaternion socketRotation)
	{
		if (!base.CanConnect(position, rotation, socket, socketPosition, socketRotation))
		{
			return false;
		}
		Vector3 vector = Matrix4x4.TRS(position, rotation, Vector3.one).MultiplyPoint3x4(this.worldPosition);
		return !GamePhysics.CheckOBB(new OBB(vector, new Vector3(2f, 0.5f, 2f), rotation), 2097152, QueryTriggerInteraction.UseGlobal);
	}

	// Token: 0x04001534 RID: 5428
	public int MaxFloor = 5;
}
