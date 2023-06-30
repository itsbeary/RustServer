using System;
using UnityEngine;

// Token: 0x02000706 RID: 1798
[ExecuteInEditMode]
public class WaterBody : MonoBehaviour
{
	// Token: 0x17000415 RID: 1045
	// (get) Token: 0x060032A8 RID: 12968 RVA: 0x0013855D File Offset: 0x0013675D
	// (set) Token: 0x060032A7 RID: 12967 RVA: 0x00138554 File Offset: 0x00136754
	public Transform Transform { get; private set; }

	// Token: 0x060032A9 RID: 12969 RVA: 0x00138565 File Offset: 0x00136765
	private void Awake()
	{
		this.Transform = base.transform;
	}

	// Token: 0x060032AA RID: 12970 RVA: 0x00138573 File Offset: 0x00136773
	private void OnEnable()
	{
		WaterSystem.RegisterBody(this);
	}

	// Token: 0x060032AB RID: 12971 RVA: 0x0013857B File Offset: 0x0013677B
	private void OnDisable()
	{
		WaterSystem.UnregisterBody(this);
	}

	// Token: 0x060032AC RID: 12972 RVA: 0x00138584 File Offset: 0x00136784
	public void OnOceanLevelChanged(float newLevel)
	{
		if (!this.IsOcean)
		{
			return;
		}
		foreach (Collider collider in this.Triggers)
		{
			Vector3 position = collider.transform.position;
			position.y = newLevel;
			collider.transform.position = position;
		}
	}

	// Token: 0x04002981 RID: 10625
	public WaterBodyType Type = WaterBodyType.Lake;

	// Token: 0x04002982 RID: 10626
	public Renderer Renderer;

	// Token: 0x04002983 RID: 10627
	public Collider[] Triggers;

	// Token: 0x04002984 RID: 10628
	public bool IsOcean;

	// Token: 0x04002986 RID: 10630
	public WaterBody.FishingTag FishingType;

	// Token: 0x02000E49 RID: 3657
	[Flags]
	public enum FishingTag
	{
		// Token: 0x04004B4F RID: 19279
		MoonPool = 1,
		// Token: 0x04004B50 RID: 19280
		River = 2,
		// Token: 0x04004B51 RID: 19281
		Ocean = 4,
		// Token: 0x04004B52 RID: 19282
		Swamp = 8
	}
}
