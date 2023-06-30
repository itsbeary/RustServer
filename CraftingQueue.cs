using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200081B RID: 2075
public class CraftingQueue : SingletonComponent<CraftingQueue>
{
	// Token: 0x04002EC5 RID: 11973
	public GameObject queueContainer;

	// Token: 0x04002EC6 RID: 11974
	public GameObject queueItemPrefab;

	// Token: 0x04002EC7 RID: 11975
	private ScrollRect scrollRect;
}
