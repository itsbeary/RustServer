using System;
using UnityEngine;

// Token: 0x02000764 RID: 1892
[CreateAssetMenu(menuName = "Rust/Skin Set Collection")]
public class SkinSetCollection : ScriptableObject
{
	// Token: 0x06003499 RID: 13465 RVA: 0x00144D62 File Offset: 0x00142F62
	public int GetIndex(float MeshNumber)
	{
		return Mathf.Clamp(Mathf.FloorToInt(MeshNumber * (float)this.Skins.Length), 0, this.Skins.Length - 1);
	}

	// Token: 0x0600349A RID: 13466 RVA: 0x00144D84 File Offset: 0x00142F84
	public SkinSet Get(float MeshNumber)
	{
		return this.Skins[this.GetIndex(MeshNumber)];
	}

	// Token: 0x04002B2A RID: 11050
	public SkinSet[] Skins;
}
