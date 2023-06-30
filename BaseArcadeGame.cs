using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200014B RID: 331
public class BaseArcadeGame : BaseMonoBehaviour
{
	// Token: 0x06001736 RID: 5942 RVA: 0x000B0ED7 File Offset: 0x000AF0D7
	public BasePlayer GetHostPlayer()
	{
		if (this.ownerMachine)
		{
			return this.ownerMachine.GetDriver();
		}
		return null;
	}

	// Token: 0x04000F92 RID: 3986
	public static List<BaseArcadeGame> globalActiveGames = new List<BaseArcadeGame>();

	// Token: 0x04000F93 RID: 3987
	public Camera cameraToRender;

	// Token: 0x04000F94 RID: 3988
	public RenderTexture renderTexture;

	// Token: 0x04000F95 RID: 3989
	public Texture2D distantTexture;

	// Token: 0x04000F96 RID: 3990
	public Transform center;

	// Token: 0x04000F97 RID: 3991
	public int frameRate = 30;

	// Token: 0x04000F98 RID: 3992
	public Dictionary<uint, ArcadeEntity> activeArcadeEntities = new Dictionary<uint, ArcadeEntity>();

	// Token: 0x04000F99 RID: 3993
	public Sprite[] spriteManifest;

	// Token: 0x04000F9A RID: 3994
	public ArcadeEntity[] entityManifest;

	// Token: 0x04000F9B RID: 3995
	public bool clientside;

	// Token: 0x04000F9C RID: 3996
	public bool clientsideInput = true;

	// Token: 0x04000F9D RID: 3997
	public const int spriteIndexInvisible = 1555;

	// Token: 0x04000F9E RID: 3998
	public GameObject arcadeEntityPrefab;

	// Token: 0x04000F9F RID: 3999
	public BaseArcadeMachine ownerMachine;

	// Token: 0x04000FA0 RID: 4000
	public static int gameOffsetIndex = 0;

	// Token: 0x04000FA1 RID: 4001
	private bool isAuthorative;

	// Token: 0x04000FA2 RID: 4002
	public Canvas canvas;
}
