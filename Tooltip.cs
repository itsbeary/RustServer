using System;
using UnityEngine;

// Token: 0x020008C6 RID: 2246
public class Tooltip : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x17000461 RID: 1121
	// (get) Token: 0x06003749 RID: 14153 RVA: 0x0014C2D4 File Offset: 0x0014A4D4
	public string english
	{
		get
		{
			return this.Text;
		}
	}

	// Token: 0x040032A4 RID: 12964
	public static TooltipContainer Current;

	// Token: 0x040032A5 RID: 12965
	[TextArea]
	public string Text;

	// Token: 0x040032A6 RID: 12966
	public GameObject TooltipObject;

	// Token: 0x040032A7 RID: 12967
	public string token = "";
}
