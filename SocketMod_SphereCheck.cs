using System;
using UnityEngine;

// Token: 0x02000280 RID: 640
public class SocketMod_SphereCheck : SocketMod
{
	// Token: 0x06001D14 RID: 7444 RVA: 0x000C92E4 File Offset: 0x000C74E4
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = (this.wantsCollide ? new Color(0f, 1f, 0f, 0.7f) : new Color(1f, 0f, 0f, 0.7f));
		Gizmos.DrawSphere(Vector3.zero, this.sphereRadius);
	}

	// Token: 0x06001D15 RID: 7445 RVA: 0x000C9354 File Offset: 0x000C7554
	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 vector = place.position + place.rotation * this.worldPosition;
		if (this.wantsCollide == GamePhysics.CheckSphere(vector, this.sphereRadius, this.layerMask.value, QueryTriggerInteraction.UseGlobal))
		{
			return true;
		}
		bool flag = false;
		Construction.lastPlacementError = "Failed Check: Sphere Test (" + this.hierachyName + ")";
		if (this.layerMask == 2097152 && this.wantsCollide)
		{
			Construction.lastPlacementError = SocketMod_SphereCheck.Error_WantsCollideConstruction.translated;
			if (flag)
			{
				Construction.lastPlacementError = Construction.lastPlacementError + " (" + this.hierachyName + ")";
			}
		}
		else if (!this.wantsCollide && (this.layerMask & 2097152) == 2097152)
		{
			Construction.lastPlacementError = SocketMod_SphereCheck.Error_DoesNotWantCollideConstruction.translated;
			if (flag)
			{
				Construction.lastPlacementError = Construction.lastPlacementError + " (" + this.hierachyName + ")";
			}
		}
		else
		{
			Construction.lastPlacementError = "Failed Check: Sphere Test (" + this.hierachyName + ")";
		}
		return false;
	}

	// Token: 0x040015A1 RID: 5537
	public float sphereRadius = 1f;

	// Token: 0x040015A2 RID: 5538
	public LayerMask layerMask;

	// Token: 0x040015A3 RID: 5539
	public bool wantsCollide;

	// Token: 0x040015A4 RID: 5540
	public static Translate.Phrase Error_WantsCollideConstruction = new Translate.Phrase("error_wantsconstruction", "Must be placed on construction");

	// Token: 0x040015A5 RID: 5541
	public static Translate.Phrase Error_DoesNotWantCollideConstruction = new Translate.Phrase("error_doesnotwantconstruction", "Cannot be placed on construction");
}
