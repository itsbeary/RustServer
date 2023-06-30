using System;
using UnityEngine;

// Token: 0x0200099D RID: 2461
public class ExplosionsShaderFloatCurves : MonoBehaviour
{
	// Token: 0x06003A62 RID: 14946 RVA: 0x001588D0 File Offset: 0x00156AD0
	private void Start()
	{
		Material[] materials = base.GetComponent<Renderer>().materials;
		if (this.MaterialID >= materials.Length)
		{
			Debug.Log("ShaderColorGradient: Material ID more than shader materials count.");
		}
		this.matInstance = materials[this.MaterialID];
		if (!this.matInstance.HasProperty(this.ShaderProperty))
		{
			Debug.Log("ShaderColorGradient: Shader not have \"" + this.ShaderProperty + "\" property");
		}
		this.propertyID = Shader.PropertyToID(this.ShaderProperty);
	}

	// Token: 0x06003A63 RID: 14947 RVA: 0x0015894A File Offset: 0x00156B4A
	private void OnEnable()
	{
		this.startTime = Time.time;
		this.canUpdate = true;
	}

	// Token: 0x06003A64 RID: 14948 RVA: 0x00158960 File Offset: 0x00156B60
	private void Update()
	{
		float num = Time.time - this.startTime;
		if (this.canUpdate)
		{
			float num2 = this.FloatPropertyCurve.Evaluate(num / this.GraphTimeMultiplier) * this.GraphScaleMultiplier;
			this.matInstance.SetFloat(this.propertyID, num2);
		}
		if (num >= this.GraphTimeMultiplier)
		{
			this.canUpdate = false;
		}
	}

	// Token: 0x04003518 RID: 13592
	public string ShaderProperty = "_BumpAmt";

	// Token: 0x04003519 RID: 13593
	public int MaterialID;

	// Token: 0x0400351A RID: 13594
	public AnimationCurve FloatPropertyCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x0400351B RID: 13595
	public float GraphTimeMultiplier = 1f;

	// Token: 0x0400351C RID: 13596
	public float GraphScaleMultiplier = 1f;

	// Token: 0x0400351D RID: 13597
	private bool canUpdate;

	// Token: 0x0400351E RID: 13598
	private Material matInstance;

	// Token: 0x0400351F RID: 13599
	private int propertyID;

	// Token: 0x04003520 RID: 13600
	private float startTime;
}
