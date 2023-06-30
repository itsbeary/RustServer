using System;
using UnityEngine;

// Token: 0x020002DD RID: 733
public class PlayerModelSkin : MonoBehaviour, IPrefabPreProcess
{
	// Token: 0x06001E01 RID: 7681 RVA: 0x000CD1D0 File Offset: 0x000CB3D0
	public void Setup(SkinSetCollection skin, float hairNum, float meshNum)
	{
		if (!this.SkinRenderer)
		{
			return;
		}
		if (!skin)
		{
			return;
		}
		switch (this.MaterialType)
		{
		case PlayerModelSkin.SkinMaterialType.HEAD:
			this.SkinRenderer.sharedMaterial = skin.Get(meshNum).HeadMaterial;
			return;
		case PlayerModelSkin.SkinMaterialType.EYE:
			this.SkinRenderer.sharedMaterial = skin.Get(meshNum).EyeMaterial;
			return;
		case PlayerModelSkin.SkinMaterialType.BODY:
			this.SkinRenderer.sharedMaterial = skin.Get(meshNum).BodyMaterial;
			return;
		default:
			this.SkinRenderer.sharedMaterial = skin.Get(meshNum).BodyMaterial;
			return;
		}
	}

	// Token: 0x06001E02 RID: 7682 RVA: 0x000CD26E File Offset: 0x000CB46E
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (!clientside)
		{
			return;
		}
		this.SkinRenderer = base.GetComponent<Renderer>();
	}

	// Token: 0x040016FC RID: 5884
	public PlayerModelSkin.SkinMaterialType MaterialType;

	// Token: 0x040016FD RID: 5885
	public Renderer SkinRenderer;

	// Token: 0x02000CB2 RID: 3250
	public enum SkinMaterialType
	{
		// Token: 0x040044E7 RID: 17639
		HEAD,
		// Token: 0x040044E8 RID: 17640
		EYE,
		// Token: 0x040044E9 RID: 17641
		BODY
	}
}
