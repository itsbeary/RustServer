using System;
using UnityEngine;

// Token: 0x02000152 RID: 338
public class DestroyArcadeEntity : BaseMonoBehaviour
{
	// Token: 0x06001740 RID: 5952 RVA: 0x000B1146 File Offset: 0x000AF346
	private void Start()
	{
		base.Invoke(new Action(this.DestroyAction), this.TimeToDie + UnityEngine.Random.Range(this.TimeToDieVariance * -0.5f, this.TimeToDieVariance * 0.5f));
	}

	// Token: 0x06001741 RID: 5953 RVA: 0x000B117E File Offset: 0x000AF37E
	private void DestroyAction()
	{
		if ((this.ent != null) & this.ent.host)
		{
			UnityEngine.Object.Destroy(this.ent.gameObject);
		}
	}

	// Token: 0x04000FCF RID: 4047
	public ArcadeEntity ent;

	// Token: 0x04000FD0 RID: 4048
	public float TimeToDie = 1f;

	// Token: 0x04000FD1 RID: 4049
	public float TimeToDieVariance;
}
