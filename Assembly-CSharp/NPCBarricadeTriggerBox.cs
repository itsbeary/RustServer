using System;
using ConVar;
using UnityEngine;

// Token: 0x0200020E RID: 526
public class NPCBarricadeTriggerBox : MonoBehaviour
{
	// Token: 0x06001B9B RID: 7067 RVA: 0x000C35B0 File Offset: 0x000C17B0
	public void Setup(Barricade t)
	{
		this.target = t;
		base.transform.SetParent(this.target.transform, false);
		base.gameObject.layer = 18;
		BoxCollider boxCollider = base.gameObject.AddComponent<BoxCollider>();
		boxCollider.isTrigger = true;
		boxCollider.center = Vector3.zero;
		boxCollider.size = Vector3.one * AI.npc_door_trigger_size + Vector3.right * this.target.bounds.size.x;
	}

	// Token: 0x06001B9C RID: 7068 RVA: 0x000C3640 File Offset: 0x000C1840
	private void OnTriggerEnter(Collider other)
	{
		if (this.target == null || this.target.isClient)
		{
			return;
		}
		if (NPCBarricadeTriggerBox.playerServerLayer < 0)
		{
			NPCBarricadeTriggerBox.playerServerLayer = LayerMask.NameToLayer("Player (Server)");
		}
		if ((other.gameObject.layer & NPCBarricadeTriggerBox.playerServerLayer) > 0)
		{
			BasePlayer component = other.gameObject.GetComponent<BasePlayer>();
			if (component != null && component.IsNpc && !(component is BasePet))
			{
				this.target.Kill(BaseNetworkable.DestroyMode.Gib);
			}
		}
	}

	// Token: 0x0400137E RID: 4990
	private Barricade target;

	// Token: 0x0400137F RID: 4991
	private static int playerServerLayer = -1;
}
