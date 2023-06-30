using System;
using UnityEngine.EventSystems;

// Token: 0x020007E9 RID: 2025
public class FpStandaloneInputModule : StandaloneInputModule
{
	// Token: 0x1700044E RID: 1102
	// (get) Token: 0x06003568 RID: 13672 RVA: 0x00145F2B File Offset: 0x0014412B
	public PointerEventData CurrentData
	{
		get
		{
			if (!this.m_PointerData.ContainsKey(-1))
			{
				return new PointerEventData(EventSystem.current);
			}
			return this.m_PointerData[-1];
		}
	}
}
