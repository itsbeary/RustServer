using System;
using UnityEngine;

// Token: 0x02000399 RID: 921
public interface IPet
{
	// Token: 0x0600209D RID: 8349
	bool IsPet();

	// Token: 0x0600209E RID: 8350
	void SetPetOwner(BasePlayer player);

	// Token: 0x0600209F RID: 8351
	bool IsOwnedBy(BasePlayer player);

	// Token: 0x060020A0 RID: 8352
	bool IssuePetCommand(PetCommandType cmd, int param, Ray? ray);
}
