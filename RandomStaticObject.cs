using System;
using UnityEngine;

// Token: 0x02000684 RID: 1668
public class RandomStaticObject : MonoBehaviour
{
	// Token: 0x06003001 RID: 12289 RVA: 0x00120BE8 File Offset: 0x0011EDE8
	protected void Start()
	{
		uint num = base.transform.position.Seed(World.Seed + this.Seed);
		if (SeedRandom.Value(ref num) > this.Probability)
		{
			for (int i = 0; i < this.Candidates.Length; i++)
			{
				GameManager.Destroy(this.Candidates[i], 0f);
			}
			GameManager.Destroy(this, 0f);
			return;
		}
		int num2 = SeedRandom.Range(num, 0, base.transform.childCount);
		for (int j = 0; j < this.Candidates.Length; j++)
		{
			GameObject gameObject = this.Candidates[j];
			if (j == num2)
			{
				gameObject.SetActive(true);
			}
			else
			{
				GameManager.Destroy(gameObject, 0f);
			}
		}
		GameManager.Destroy(this, 0f);
	}

	// Token: 0x04002797 RID: 10135
	public uint Seed;

	// Token: 0x04002798 RID: 10136
	public float Probability = 0.5f;

	// Token: 0x04002799 RID: 10137
	public GameObject[] Candidates;
}
