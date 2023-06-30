using System;
using UnityEngine;

// Token: 0x0200027E RID: 638
public class SocketMod_PhysicMaterial : SocketMod
{
	// Token: 0x06001D0F RID: 7439 RVA: 0x000C90E0 File Offset: 0x000C72E0
	public override bool DoCheck(Construction.Placement place)
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(place.position + place.rotation.eulerAngles.normalized * 0.5f, -place.rotation.eulerAngles.normalized, out raycastHit, 1f, 27328512, QueryTriggerInteraction.Ignore))
		{
			this.foundMaterial = raycastHit.collider.GetMaterialAt(raycastHit.point);
			PhysicMaterial[] validMaterials = this.ValidMaterials;
			for (int i = 0; i < validMaterials.Length; i++)
			{
				if (validMaterials[i] == this.foundMaterial)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0400159B RID: 5531
	public PhysicMaterial[] ValidMaterials;

	// Token: 0x0400159C RID: 5532
	private PhysicMaterial foundMaterial;
}
