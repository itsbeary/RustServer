using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000329 RID: 809
public class GameContentList : MonoBehaviour
{
	// Token: 0x04001817 RID: 6167
	public GameContentList.ResourceType resourceType;

	// Token: 0x04001818 RID: 6168
	public List<UnityEngine.Object> foundObjects;

	// Token: 0x02000CC2 RID: 3266
	public enum ResourceType
	{
		// Token: 0x0400453B RID: 17723
		Audio,
		// Token: 0x0400453C RID: 17724
		Textures,
		// Token: 0x0400453D RID: 17725
		Models
	}
}
