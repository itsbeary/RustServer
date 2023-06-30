using System;
using UnityEngine;

// Token: 0x0200025B RID: 603
public class ConstructionSocket : Socket_Base
{
	// Token: 0x06001C92 RID: 7314 RVA: 0x000C6C98 File Offset: 0x000C4E98
	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(Vector3.zero, Vector3.forward * 0.6f);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(Vector3.zero, Vector3.right * 0.1f);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * 0.1f);
		Gizmos.DrawIcon(base.transform.position, "light_circle_green.png", false);
	}

	// Token: 0x06001C93 RID: 7315 RVA: 0x000C6D34 File Offset: 0x000C4F34
	private void OnDrawGizmosSelected()
	{
		if (this.female)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawWireCube(this.selectCenter, this.selectSize);
		}
	}

	// Token: 0x06001C94 RID: 7316 RVA: 0x000C6D5F File Offset: 0x000C4F5F
	public override bool TestTarget(Construction.Target target)
	{
		return base.TestTarget(target) && this.IsCompatible(target.socket);
	}

	// Token: 0x06001C95 RID: 7317 RVA: 0x000C6D78 File Offset: 0x000C4F78
	public override bool IsCompatible(Socket_Base socket)
	{
		if (!base.IsCompatible(socket))
		{
			return false;
		}
		ConstructionSocket constructionSocket = socket as ConstructionSocket;
		return !(constructionSocket == null) && constructionSocket.socketType != ConstructionSocket.Type.None && this.socketType != ConstructionSocket.Type.None && constructionSocket.socketType == this.socketType;
	}

	// Token: 0x06001C96 RID: 7318 RVA: 0x000C6DC8 File Offset: 0x000C4FC8
	public override bool CanConnect(Vector3 position, Quaternion rotation, Socket_Base socket, Vector3 socketPosition, Quaternion socketRotation)
	{
		if (!base.CanConnect(position, rotation, socket, socketPosition, socketRotation))
		{
			return false;
		}
		Matrix4x4 matrix4x = Matrix4x4.TRS(position, rotation, Vector3.one);
		Matrix4x4 matrix4x2 = Matrix4x4.TRS(socketPosition, socketRotation, Vector3.one);
		Vector3 vector = matrix4x.MultiplyPoint3x4(this.worldPosition);
		Vector3 vector2 = matrix4x2.MultiplyPoint3x4(socket.worldPosition);
		if (Vector3.Distance(vector, vector2) > 0.01f)
		{
			return false;
		}
		Vector3 vector3 = matrix4x.MultiplyVector(this.worldRotation * Vector3.forward);
		Vector3 vector4 = matrix4x2.MultiplyVector(socket.worldRotation * Vector3.forward);
		float num = Vector3.Angle(vector3, vector4);
		if (this.male && this.female)
		{
			num = Mathf.Min(num, Vector3.Angle(-vector3, vector4));
		}
		if (socket.male && socket.female)
		{
			num = Mathf.Min(num, Vector3.Angle(vector3, -vector4));
		}
		return num <= 1f;
	}

	// Token: 0x06001C97 RID: 7319 RVA: 0x000C6EC0 File Offset: 0x000C50C0
	public bool TestRestrictedAngles(Vector3 suggestedPos, Quaternion suggestedAng, Construction.Target target)
	{
		if (this.restrictPlacementAngle)
		{
			Quaternion quaternion = Quaternion.Euler(0f, this.faceAngle, 0f) * suggestedAng;
			float num = target.ray.direction.XZ3D().DotDegrees(quaternion * Vector3.forward);
			if (num > this.angleAllowed * 0.5f)
			{
				return false;
			}
			if (num < this.angleAllowed * -0.5f)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001C98 RID: 7320 RVA: 0x000C6F38 File Offset: 0x000C5138
	public override Construction.Placement DoPlacement(Construction.Target target)
	{
		if (!target.entity || !target.entity.transform)
		{
			return null;
		}
		if (!this.CanConnectToEntity(target))
		{
			return null;
		}
		ConstructionSocket constructionSocket = target.socket as ConstructionSocket;
		Vector3 worldPosition = target.GetWorldPosition();
		Quaternion worldRotation = target.GetWorldRotation(true);
		if (constructionSocket != null && !this.IsCompatible(constructionSocket))
		{
			return null;
		}
		if (this.rotationDegrees > 0 && (constructionSocket == null || !constructionSocket.restrictPlacementRotation))
		{
			Construction.Placement placement = new Construction.Placement();
			float num = float.MaxValue;
			float num2 = 0f;
			for (int i = 0; i < 360; i += this.rotationDegrees)
			{
				Quaternion quaternion = Quaternion.Euler(0f, (float)(this.rotationOffset + i), 0f);
				Vector3 direction = target.ray.direction;
				Vector3 vector = quaternion * worldRotation * Vector3.up;
				float num3 = Vector3.Angle(direction, vector);
				if (num3 < num)
				{
					num = num3;
					num2 = (float)i;
				}
			}
			for (int j = 0; j < 360; j += this.rotationDegrees)
			{
				Quaternion quaternion2 = worldRotation * Quaternion.Inverse(this.rotation);
				Quaternion quaternion3 = Quaternion.Euler(target.rotation);
				Quaternion quaternion4 = Quaternion.Euler(0f, (float)(this.rotationOffset + j) + num2, 0f);
				Quaternion quaternion5 = quaternion3 * quaternion4 * quaternion2;
				Vector3 vector2 = quaternion5 * this.position;
				placement.position = worldPosition - vector2;
				placement.rotation = quaternion5;
				if (this.CheckSocketMods(placement))
				{
					return placement;
				}
			}
		}
		Construction.Placement placement2 = new Construction.Placement();
		Quaternion quaternion6 = worldRotation * Quaternion.Inverse(this.rotation);
		Vector3 vector3 = quaternion6 * this.position;
		placement2.position = worldPosition - vector3;
		placement2.rotation = quaternion6;
		if (!this.TestRestrictedAngles(worldPosition, worldRotation, target))
		{
			return null;
		}
		return placement2;
	}

	// Token: 0x06001C99 RID: 7321 RVA: 0x0000441C File Offset: 0x0000261C
	protected virtual bool CanConnectToEntity(Construction.Target target)
	{
		return true;
	}

	// Token: 0x0400152C RID: 5420
	public ConstructionSocket.Type socketType;

	// Token: 0x0400152D RID: 5421
	public int rotationDegrees;

	// Token: 0x0400152E RID: 5422
	public int rotationOffset;

	// Token: 0x0400152F RID: 5423
	public bool restrictPlacementRotation;

	// Token: 0x04001530 RID: 5424
	public bool restrictPlacementAngle;

	// Token: 0x04001531 RID: 5425
	public float faceAngle;

	// Token: 0x04001532 RID: 5426
	public float angleAllowed = 150f;

	// Token: 0x04001533 RID: 5427
	[Range(0f, 1f)]
	public float support = 1f;

	// Token: 0x02000CA0 RID: 3232
	public enum Type
	{
		// Token: 0x04004486 RID: 17542
		None,
		// Token: 0x04004487 RID: 17543
		Foundation,
		// Token: 0x04004488 RID: 17544
		Floor,
		// Token: 0x04004489 RID: 17545
		Misc,
		// Token: 0x0400448A RID: 17546
		Doorway,
		// Token: 0x0400448B RID: 17547
		Wall,
		// Token: 0x0400448C RID: 17548
		Block,
		// Token: 0x0400448D RID: 17549
		Ramp,
		// Token: 0x0400448E RID: 17550
		StairsTriangle,
		// Token: 0x0400448F RID: 17551
		Stairs,
		// Token: 0x04004490 RID: 17552
		FloorFrameTriangle,
		// Token: 0x04004491 RID: 17553
		Window,
		// Token: 0x04004492 RID: 17554
		Shutters,
		// Token: 0x04004493 RID: 17555
		WallFrame,
		// Token: 0x04004494 RID: 17556
		FloorFrame,
		// Token: 0x04004495 RID: 17557
		WindowDressing,
		// Token: 0x04004496 RID: 17558
		DoorDressing,
		// Token: 0x04004497 RID: 17559
		Elevator,
		// Token: 0x04004498 RID: 17560
		DoubleDoorDressing
	}
}
