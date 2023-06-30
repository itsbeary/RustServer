using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200039A RID: 922
public class PetCommandList : PrefabAttribute
{
	// Token: 0x060020A1 RID: 8353 RVA: 0x000D825D File Offset: 0x000D645D
	protected override Type GetIndexedType()
	{
		return typeof(PetCommandList);
	}

	// Token: 0x060020A2 RID: 8354 RVA: 0x000D8269 File Offset: 0x000D6469
	public List<PetCommandList.PetCommandDesc> GetCommandDescriptions()
	{
		return this.Commands;
	}

	// Token: 0x04001982 RID: 6530
	public List<PetCommandList.PetCommandDesc> Commands;

	// Token: 0x02000CCD RID: 3277
	[Serializable]
	public struct PetCommandDesc
	{
		// Token: 0x0400456D RID: 17773
		public PetCommandType CommandType;

		// Token: 0x0400456E RID: 17774
		public Translate.Phrase Title;

		// Token: 0x0400456F RID: 17775
		public Translate.Phrase Description;

		// Token: 0x04004570 RID: 17776
		public Sprite Icon;

		// Token: 0x04004571 RID: 17777
		public int CommandIndex;

		// Token: 0x04004572 RID: 17778
		public bool Raycast;

		// Token: 0x04004573 RID: 17779
		public int CommandWheelOrder;
	}
}
