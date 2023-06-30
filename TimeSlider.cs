using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008C3 RID: 2243
public class TimeSlider : MonoBehaviour
{
	// Token: 0x06003739 RID: 14137 RVA: 0x0014BF04 File Offset: 0x0014A104
	private void Start()
	{
		this.slider = base.GetComponent<Slider>();
	}

	// Token: 0x0600373A RID: 14138 RVA: 0x0014BF12 File Offset: 0x0014A112
	private void Update()
	{
		if (TOD_Sky.Instance == null)
		{
			return;
		}
		this.slider.value = TOD_Sky.Instance.Cycle.Hour;
	}

	// Token: 0x0600373B RID: 14139 RVA: 0x0014BF3C File Offset: 0x0014A13C
	public void OnValue(float f)
	{
		if (TOD_Sky.Instance == null)
		{
			return;
		}
		TOD_Sky.Instance.Cycle.Hour = f;
		TOD_Sky.Instance.UpdateAmbient();
		TOD_Sky.Instance.UpdateReflection();
		TOD_Sky.Instance.UpdateFog();
	}

	// Token: 0x040032A0 RID: 12960
	private Slider slider;
}
