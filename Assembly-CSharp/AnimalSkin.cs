using System;
using UnityEngine;

// Token: 0x0200028B RID: 651
public class AnimalSkin : MonoBehaviour, IClientComponent
{
	// Token: 0x06001D41 RID: 7489 RVA: 0x000CA0CC File Offset: 0x000C82CC
	private void Start()
	{
		this.model = base.gameObject.GetComponent<Model>();
		if (!this.dontRandomizeOnStart)
		{
			int num = Mathf.FloorToInt((float)UnityEngine.Random.Range(0, this.animalSkins.Length));
			this.ChangeSkin(num);
		}
	}

	// Token: 0x06001D42 RID: 7490 RVA: 0x000CA110 File Offset: 0x000C8310
	public void ChangeSkin(int iSkin)
	{
		if (this.animalSkins.Length == 0)
		{
			return;
		}
		iSkin = Mathf.Clamp(iSkin, 0, this.animalSkins.Length - 1);
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in this.animalMesh)
		{
			Material[] sharedMaterials = skinnedMeshRenderer.sharedMaterials;
			if (sharedMaterials != null)
			{
				for (int j = 0; j < sharedMaterials.Length; j++)
				{
					sharedMaterials[j] = this.animalSkins[iSkin].multiSkin[j];
				}
				skinnedMeshRenderer.sharedMaterials = sharedMaterials;
			}
		}
		if (this.model != null)
		{
			this.model.skin = iSkin;
		}
	}

	// Token: 0x040015C7 RID: 5575
	public SkinnedMeshRenderer[] animalMesh;

	// Token: 0x040015C8 RID: 5576
	public AnimalMultiSkin[] animalSkins;

	// Token: 0x040015C9 RID: 5577
	private Model model;

	// Token: 0x040015CA RID: 5578
	public bool dontRandomizeOnStart;
}
