using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000885 RID: 2181
public class KeyBindUI : MonoBehaviour
{
	// Token: 0x17000454 RID: 1108
	// (get) Token: 0x06003689 RID: 13961 RVA: 0x001494E8 File Offset: 0x001476E8
	// (set) Token: 0x0600368A RID: 13962 RVA: 0x001494EF File Offset: 0x001476EF
	public static bool IsBinding { get; private set; }

	// Token: 0x04003151 RID: 12625
	public GameObject blockingCanvas;

	// Token: 0x04003152 RID: 12626
	public Button btnA;

	// Token: 0x04003153 RID: 12627
	public Button btnB;

	// Token: 0x04003154 RID: 12628
	public string bindString;
}
