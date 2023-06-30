using System;
using UnityEngine;

// Token: 0x0200018B RID: 395
public class ItemModOpenWrapped : ItemMod
{
	// Token: 0x060017F5 RID: 6133 RVA: 0x000B4484 File Offset: 0x000B2684
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command == "open")
		{
			if (item.amount <= 0)
			{
				return;
			}
			Item slot = item.contents.GetSlot(0);
			if (slot == null)
			{
				return;
			}
			int position = item.position;
			ItemContainer rootContainer = item.GetRootContainer();
			item.RemoveFromContainer();
			slot.MoveToContainer(rootContainer, position, true, false, null, true);
			item.Remove(0f);
			if (this.successEffect.isValid)
			{
				Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
			}
		}
	}

	// Token: 0x040010B4 RID: 4276
	public GameObjectRef successEffect;

	// Token: 0x040010B5 RID: 4277
	public static Translate.Phrase open_wrapped_gift = new Translate.Phrase("open_wrapped_gift", "Unwrap");

	// Token: 0x040010B6 RID: 4278
	public static Translate.Phrase open_wrapped_gift_desc = new Translate.Phrase("open_wrapped_gift_desc", "Unwrap the gift and reveal its contents");
}
