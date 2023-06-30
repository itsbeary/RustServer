using System;
using UnityEngine;

// Token: 0x0200099C RID: 2460
public class ExplosionsShaderColorGradient : MonoBehaviour
{
	// Token: 0x06003A5E RID: 14942 RVA: 0x0015879C File Offset: 0x0015699C
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
		this.oldColor = this.matInstance.GetColor(this.propertyID);
	}

	// Token: 0x06003A5F RID: 14943 RVA: 0x0015882D File Offset: 0x00156A2D
	private void OnEnable()
	{
		this.startTime = Time.time;
		this.canUpdate = true;
	}

	// Token: 0x06003A60 RID: 14944 RVA: 0x00158844 File Offset: 0x00156A44
	private void Update()
	{
		float num = Time.time - this.startTime;
		if (this.canUpdate)
		{
			Color color = this.Color.Evaluate(num / this.TimeMultiplier);
			this.matInstance.SetColor(this.propertyID, color * this.oldColor);
		}
		if (num >= this.TimeMultiplier)
		{
			this.canUpdate = false;
		}
	}

	// Token: 0x0400350F RID: 13583
	public string ShaderProperty = "_TintColor";

	// Token: 0x04003510 RID: 13584
	public int MaterialID;

	// Token: 0x04003511 RID: 13585
	public Gradient Color = new Gradient();

	// Token: 0x04003512 RID: 13586
	public float TimeMultiplier = 1f;

	// Token: 0x04003513 RID: 13587
	private bool canUpdate;

	// Token: 0x04003514 RID: 13588
	private Material matInstance;

	// Token: 0x04003515 RID: 13589
	private int propertyID;

	// Token: 0x04003516 RID: 13590
	private float startTime;

	// Token: 0x04003517 RID: 13591
	private Color oldColor;
}
