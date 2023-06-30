using System;
using UnityEngine;

// Token: 0x02000614 RID: 1556
[CreateAssetMenu(menuName = "Rust/MaterialSound")]
public class MaterialSound : ScriptableObject
{
	// Token: 0x040025D9 RID: 9689
	public SoundDefinition DefaultSound;

	// Token: 0x040025DA RID: 9690
	public MaterialSound.Entry[] Entries;

	// Token: 0x02000DA6 RID: 3494
	[Serializable]
	public class Entry
	{
		// Token: 0x040048BF RID: 18623
		public PhysicMaterial Material;

		// Token: 0x040048C0 RID: 18624
		public SoundDefinition Sound;
	}
}
