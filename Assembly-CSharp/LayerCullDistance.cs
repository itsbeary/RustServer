using System;
using UnityEngine;

// Token: 0x02000903 RID: 2307
public class LayerCullDistance : MonoBehaviour
{
	// Token: 0x060037DC RID: 14300 RVA: 0x0014D988 File Offset: 0x0014BB88
	protected void OnEnable()
	{
		Camera component = base.GetComponent<Camera>();
		float[] layerCullDistances = component.layerCullDistances;
		layerCullDistances[LayerMask.NameToLayer(this.Layer)] = this.Distance;
		component.layerCullDistances = layerCullDistances;
	}

	// Token: 0x04003335 RID: 13109
	public string Layer = "Default";

	// Token: 0x04003336 RID: 13110
	public float Distance = 1000f;
}
