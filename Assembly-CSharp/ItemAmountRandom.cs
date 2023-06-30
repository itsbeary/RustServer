using System;
using UnityEngine;

// Token: 0x0200060F RID: 1551
[Serializable]
public class ItemAmountRandom
{
	// Token: 0x06002DFF RID: 11775 RVA: 0x00114BDA File Offset: 0x00112DDA
	public int RandomAmount()
	{
		return Mathf.RoundToInt(this.amount.Evaluate(UnityEngine.Random.Range(0f, 1f)));
	}

	// Token: 0x040025C1 RID: 9665
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemDef;

	// Token: 0x040025C2 RID: 9666
	public AnimationCurve amount = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});
}
