using System;
using UnityEngine;

namespace Facepunch.GUI
{
	// Token: 0x02000B05 RID: 2821
	public static class Controls
	{
		// Token: 0x060044BE RID: 17598 RVA: 0x00192E44 File Offset: 0x00191044
		public static float FloatSlider(string strLabel, float value, float low, float high, string format = "0.00")
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(strLabel, new GUILayoutOption[] { GUILayout.Width(Controls.labelWidth) });
			float num = float.Parse(GUILayout.TextField(value.ToString(format), new GUILayoutOption[] { GUILayout.ExpandWidth(true) }));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			float num2 = GUILayout.HorizontalSlider(num, low, high, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return num2;
		}

		// Token: 0x060044BF RID: 17599 RVA: 0x00192EB8 File Offset: 0x001910B8
		public static int IntSlider(string strLabel, int value, int low, int high, string format = "0")
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(strLabel, new GUILayoutOption[] { GUILayout.Width(Controls.labelWidth) });
			float num = (float)int.Parse(GUILayout.TextField(value.ToString(format), new GUILayoutOption[] { GUILayout.ExpandWidth(true) }));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			int num2 = (int)GUILayout.HorizontalSlider(num, (float)low, (float)high, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return num2;
		}

		// Token: 0x060044C0 RID: 17600 RVA: 0x00192F2E File Offset: 0x0019112E
		public static string TextArea(string strName, string value)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(strName, new GUILayoutOption[] { GUILayout.Width(Controls.labelWidth) });
			string text = GUILayout.TextArea(value, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return text;
		}

		// Token: 0x060044C1 RID: 17601 RVA: 0x00192F63 File Offset: 0x00191163
		public static bool Checkbox(string strName, bool value)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(strName, new GUILayoutOption[] { GUILayout.Width(Controls.labelWidth) });
			bool flag = GUILayout.Toggle(value, "", Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return flag;
		}

		// Token: 0x060044C2 RID: 17602 RVA: 0x00192F9D File Offset: 0x0019119D
		public static bool Button(string strName)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			bool flag = GUILayout.Button(strName, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return flag;
		}

		// Token: 0x04003D3F RID: 15679
		public static float labelWidth = 100f;
	}
}
