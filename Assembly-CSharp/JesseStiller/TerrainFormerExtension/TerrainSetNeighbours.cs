using System;
using UnityEngine;

namespace JesseStiller.TerrainFormerExtension
{
	// Token: 0x020009E3 RID: 2531
	public class TerrainSetNeighbours : MonoBehaviour
	{
		// Token: 0x06003C61 RID: 15457 RVA: 0x00162DE2 File Offset: 0x00160FE2
		private void Awake()
		{
			base.GetComponent<Terrain>().SetNeighbors(this.leftTerrain, this.topTerrain, this.rightTerrain, this.bottomTerrain);
			UnityEngine.Object.Destroy(this);
		}

		// Token: 0x06003C62 RID: 15458 RVA: 0x00162E0D File Offset: 0x0016100D
		public void SetNeighbours(Terrain leftTerrain, Terrain topTerrain, Terrain rightTerrain, Terrain bottomTerrain)
		{
			this.leftTerrain = leftTerrain;
			this.topTerrain = topTerrain;
			this.rightTerrain = rightTerrain;
			this.bottomTerrain = bottomTerrain;
		}

		// Token: 0x040036E7 RID: 14055
		[SerializeField]
		private Terrain leftTerrain;

		// Token: 0x040036E8 RID: 14056
		[SerializeField]
		private Terrain topTerrain;

		// Token: 0x040036E9 RID: 14057
		[SerializeField]
		private Terrain rightTerrain;

		// Token: 0x040036EA RID: 14058
		[SerializeField]
		private Terrain bottomTerrain;
	}
}
