using System;
using UnityEngine;

namespace Painting
{
	// Token: 0x020009E4 RID: 2532
	[Serializable]
	public class Brush
	{
		// Token: 0x040036EB RID: 14059
		public float spacing;

		// Token: 0x040036EC RID: 14060
		public Vector2 brushSize;

		// Token: 0x040036ED RID: 14061
		public Texture2D texture;

		// Token: 0x040036EE RID: 14062
		public Color color;

		// Token: 0x040036EF RID: 14063
		public bool erase;
	}
}
