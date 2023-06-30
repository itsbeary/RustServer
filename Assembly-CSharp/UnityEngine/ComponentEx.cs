using System;
using Facepunch;

namespace UnityEngine
{
	// Token: 0x02000A29 RID: 2601
	public static class ComponentEx
	{
		// Token: 0x06003DA6 RID: 15782 RVA: 0x00169564 File Offset: 0x00167764
		public static T Instantiate<T>(this T component) where T : Component
		{
			return Facepunch.Instantiate.GameObject(component.gameObject, null).GetComponent<T>();
		}

		// Token: 0x06003DA7 RID: 15783 RVA: 0x0016957C File Offset: 0x0016777C
		public static bool HasComponent<T>(this Component component) where T : Component
		{
			return component.GetComponent<T>() != null;
		}
	}
}
