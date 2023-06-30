using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020002D4 RID: 724
public class MeshPaintableSource : MonoBehaviour, IClientComponent
{
	// Token: 0x06001DDA RID: 7642 RVA: 0x000CC8A8 File Offset: 0x000CAAA8
	public void Init()
	{
		if (this.texture == null)
		{
			this.texture = new Texture2D(this.texWidth, this.texHeight, TextureFormat.ARGB32, false);
			this.texture.name = "MeshPaintableSource_" + base.gameObject.name;
			this.texture.wrapMode = TextureWrapMode.Clamp;
			this.texture.Clear(Color.clear);
		}
		if (MeshPaintableSource.block == null)
		{
			MeshPaintableSource.block = new MaterialPropertyBlock();
		}
		else
		{
			MeshPaintableSource.block.Clear();
		}
		this.UpdateMaterials(MeshPaintableSource.block, null, false, this.isSelected);
		List<Renderer> list = Pool.GetList<Renderer>();
		(this.applyToAllRenderers ? base.transform.root : base.transform).GetComponentsInChildren<Renderer>(true, list);
		foreach (Renderer renderer in list)
		{
			PlayerModelSkin playerModelSkin;
			if (this.applyToSkinRenderers || !renderer.TryGetComponent<PlayerModelSkin>(out playerModelSkin))
			{
				renderer.SetPropertyBlock(MeshPaintableSource.block);
			}
		}
		if (this.extraRenderers != null)
		{
			foreach (Renderer renderer2 in this.extraRenderers)
			{
				if (renderer2 != null)
				{
					renderer2.SetPropertyBlock(MeshPaintableSource.block);
				}
			}
		}
		if (this.applyToFirstPersonLegs && this.legRenderer != null)
		{
			this.legRenderer.SetPropertyBlock(MeshPaintableSource.block);
		}
		Pool.FreeList<Renderer>(ref list);
	}

	// Token: 0x06001DDB RID: 7643 RVA: 0x000CCA3C File Offset: 0x000CAC3C
	public void Free()
	{
		if (this.texture)
		{
			UnityEngine.Object.Destroy(this.texture);
			this.texture = null;
		}
	}

	// Token: 0x06001DDC RID: 7644 RVA: 0x000CCA5D File Offset: 0x000CAC5D
	public void OnDestroy()
	{
		this.Free();
	}

	// Token: 0x06001DDD RID: 7645 RVA: 0x000CCA65 File Offset: 0x000CAC65
	public virtual void UpdateMaterials(MaterialPropertyBlock block, Texture2D textureOverride = null, bool forEditing = false, bool isSelected = false)
	{
		block.SetTexture(this.replacementTextureName, textureOverride ?? this.texture);
	}

	// Token: 0x06001DDE RID: 7646 RVA: 0x000CCA80 File Offset: 0x000CAC80
	public virtual Color32[] UpdateFrom(Texture2D input)
	{
		this.Init();
		Color32[] pixels = input.GetPixels32();
		this.texture.SetPixels32(pixels);
		this.texture.Apply(true, false);
		return pixels;
	}

	// Token: 0x06001DDF RID: 7647 RVA: 0x000CCAB4 File Offset: 0x000CACB4
	public void Load(byte[] data)
	{
		this.Init();
		if (data != null)
		{
			this.texture.LoadImage(data);
			this.texture.Apply(true, false);
		}
	}

	// Token: 0x06001DE0 RID: 7648 RVA: 0x000CCADC File Offset: 0x000CACDC
	public void Clear()
	{
		if (this.texture == null)
		{
			return;
		}
		this.texture.Clear(new Color(0f, 0f, 0f, 0f));
		this.texture.Apply(true, false);
	}

	// Token: 0x040016CE RID: 5838
	public Vector4 uvRange = new Vector4(0f, 0f, 1f, 1f);

	// Token: 0x040016CF RID: 5839
	public int texWidth = 256;

	// Token: 0x040016D0 RID: 5840
	public int texHeight = 128;

	// Token: 0x040016D1 RID: 5841
	public string replacementTextureName = "_DecalTexture";

	// Token: 0x040016D2 RID: 5842
	public float cameraFOV = 60f;

	// Token: 0x040016D3 RID: 5843
	public float cameraDistance = 2f;

	// Token: 0x040016D4 RID: 5844
	[NonSerialized]
	public Texture2D texture;

	// Token: 0x040016D5 RID: 5845
	public GameObject sourceObject;

	// Token: 0x040016D6 RID: 5846
	public Mesh collisionMesh;

	// Token: 0x040016D7 RID: 5847
	public Vector3 localPosition;

	// Token: 0x040016D8 RID: 5848
	public Vector3 localRotation;

	// Token: 0x040016D9 RID: 5849
	public bool applyToAllRenderers = true;

	// Token: 0x040016DA RID: 5850
	public Renderer[] extraRenderers;

	// Token: 0x040016DB RID: 5851
	public bool paint3D;

	// Token: 0x040016DC RID: 5852
	public bool applyToSkinRenderers = true;

	// Token: 0x040016DD RID: 5853
	public bool applyToFirstPersonLegs = true;

	// Token: 0x040016DE RID: 5854
	[NonSerialized]
	public bool isSelected;

	// Token: 0x040016DF RID: 5855
	[NonSerialized]
	public Renderer legRenderer;

	// Token: 0x040016E0 RID: 5856
	private static MaterialPropertyBlock block;
}
