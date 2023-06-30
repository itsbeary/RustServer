using System;
using UnityEngine;

// Token: 0x0200016E RID: 366
[CreateAssetMenu(menuName = "Rust/Delivery Drone Config")]
public class DeliveryDroneConfig : BaseScriptableObject
{
	// Token: 0x0600179A RID: 6042 RVA: 0x000B3254 File Offset: 0x000B1454
	public void FindDescentPoints(VendingMachine vendingMachine, float currentY, out Vector3 waitPosition, out Vector3 descendPosition)
	{
		float num = this.maxDistanceFromVendingMachine / 4f;
		for (int i = 0; i <= 4; i++)
		{
			Vector3 vector = Vector3.forward * (num * (float)i);
			Vector3 vector2 = vendingMachine.transform.TransformPoint(this.vendingMachineOffset + vector);
			Vector3 vector3 = vector2 + Vector3.up * this.testHeight;
			RaycastHit raycastHit;
			if (!Physics.BoxCast(vector3, this.halfExtents, Vector3.down, out raycastHit, vendingMachine.transform.rotation, this.testHeight, this.layerMask))
			{
				waitPosition = vector2;
				descendPosition = vector3.WithY(currentY);
				return;
			}
			if (i == 4)
			{
				waitPosition = vector3 + Vector3.down * (raycastHit.distance - this.halfExtents.y * 2f);
				descendPosition = vector3.WithY(currentY);
				return;
			}
		}
		throw new Exception("Bug: FindDescentPoint didn't return a fallback value");
	}

	// Token: 0x0600179B RID: 6043 RVA: 0x000B3358 File Offset: 0x000B1558
	public bool IsVendingMachineAccessible(VendingMachine vendingMachine, Vector3 offset, out RaycastHit hitInfo)
	{
		Vector3 vector = vendingMachine.transform.TransformPoint(offset);
		return !Physics.BoxCast(vector + Vector3.up * this.testHeight, this.halfExtents, Vector3.down, out hitInfo, vendingMachine.transform.rotation, this.testHeight, this.layerMask) && vendingMachine.IsVisibleAndCanSee(vector, 2f);
	}

	// Token: 0x04001040 RID: 4160
	public Vector3 vendingMachineOffset = new Vector3(0f, 1f, 1f);

	// Token: 0x04001041 RID: 4161
	public float maxDistanceFromVendingMachine = 1f;

	// Token: 0x04001042 RID: 4162
	public Vector3 halfExtents = new Vector3(0.5f, 0.5f, 0.5f);

	// Token: 0x04001043 RID: 4163
	public float testHeight = 200f;

	// Token: 0x04001044 RID: 4164
	public LayerMask layerMask = 161546496;
}
