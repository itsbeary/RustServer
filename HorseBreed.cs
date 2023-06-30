using System;
using UnityEngine;

// Token: 0x02000212 RID: 530
[CreateAssetMenu(menuName = "Scriptable Object/Horse Breed", fileName = "newbreed.asset")]
public class HorseBreed : ScriptableObject
{
	// Token: 0x04001383 RID: 4995
	public Translate.Phrase breedName;

	// Token: 0x04001384 RID: 4996
	public Translate.Phrase breedDesc;

	// Token: 0x04001385 RID: 4997
	public Material[] materialOverrides;

	// Token: 0x04001386 RID: 4998
	public float maxHealth = 1f;

	// Token: 0x04001387 RID: 4999
	public float maxSpeed = 1f;

	// Token: 0x04001388 RID: 5000
	public float staminaDrain = 1f;

	// Token: 0x04001389 RID: 5001
	public float maxStamina = 1f;
}
