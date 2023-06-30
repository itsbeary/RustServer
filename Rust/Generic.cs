using System;
using UnityEngine.SceneManagement;

namespace Rust
{
	// Token: 0x02000B14 RID: 2836
	public static class Generic
	{
		// Token: 0x1700063D RID: 1597
		// (get) Token: 0x0600451A RID: 17690 RVA: 0x00194A82 File Offset: 0x00192C82
		public static Scene BatchingScene
		{
			get
			{
				if (!Generic._batchingScene.IsValid())
				{
					Generic._batchingScene = SceneManager.CreateScene("Batching");
				}
				return Generic._batchingScene;
			}
		}

		// Token: 0x04003D92 RID: 15762
		private static Scene _batchingScene;
	}
}
