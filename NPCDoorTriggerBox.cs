using System;
using ConVar;
using UnityEngine;

// Token: 0x0200020F RID: 527
public class NPCDoorTriggerBox : MonoBehaviour
{
	// Token: 0x06001B9F RID: 7071 RVA: 0x000C36D0 File Offset: 0x000C18D0
	public void Setup(Door d)
	{
		this.door = d;
		base.transform.SetParent(this.door.transform, false);
		base.gameObject.layer = 18;
		BoxCollider boxCollider = base.gameObject.AddComponent<BoxCollider>();
		boxCollider.isTrigger = true;
		boxCollider.center = Vector3.zero;
		boxCollider.size = Vector3.one * AI.npc_door_trigger_size;
	}

	// Token: 0x06001BA0 RID: 7072 RVA: 0x000C373C File Offset: 0x000C193C
	private void OnTriggerEnter(Collider other)
	{
		if (this.door == null || this.door.isClient || this.door.IsLocked())
		{
			return;
		}
		if (!this.door.isSecurityDoor && this.door.IsOpen())
		{
			return;
		}
		if (this.door.isSecurityDoor && !this.door.IsOpen())
		{
			return;
		}
		if (NPCDoorTriggerBox.playerServerLayer < 0)
		{
			NPCDoorTriggerBox.playerServerLayer = LayerMask.NameToLayer("Player (Server)");
		}
		if ((other.gameObject.layer & NPCDoorTriggerBox.playerServerLayer) > 0)
		{
			BasePlayer component = other.gameObject.GetComponent<BasePlayer>();
			if (component != null && component.IsNpc && !this.door.isSecurityDoor)
			{
				this.door.SetOpen(true, false);
			}
		}
	}

	// Token: 0x04001380 RID: 4992
	private Door door;

	// Token: 0x04001381 RID: 4993
	private static int playerServerLayer = -1;
}
