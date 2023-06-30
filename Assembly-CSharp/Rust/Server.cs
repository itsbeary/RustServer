using System;
using UnityEngine.SceneManagement;

namespace Rust
{
	// Token: 0x02000B15 RID: 2837
	public static class Server
	{
		// Token: 0x1700063E RID: 1598
		// (get) Token: 0x0600451B RID: 17691 RVA: 0x00194AA4 File Offset: 0x00192CA4
		public static Scene EntityScene
		{
			get
			{
				if (!Server._entityScene.IsValid())
				{
					Server._entityScene = SceneManager.CreateScene("Server Entities");
				}
				return Server._entityScene;
			}
		}

		// Token: 0x04003D93 RID: 15763
		public const float UseDistance = 3f;

		// Token: 0x04003D94 RID: 15764
		private static Scene _entityScene;
	}
}
