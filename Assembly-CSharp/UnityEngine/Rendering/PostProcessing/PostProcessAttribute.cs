using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A4C RID: 2636
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class PostProcessAttribute : Attribute
	{
		// Token: 0x06003F5A RID: 16218 RVA: 0x001732FC File Offset: 0x001714FC
		public PostProcessAttribute(Type renderer, PostProcessEvent eventType, string menuItem, bool allowInSceneView = true)
		{
			this.renderer = renderer;
			this.eventType = eventType;
			this.menuItem = menuItem;
			this.allowInSceneView = allowInSceneView;
			this.builtinEffect = false;
		}

		// Token: 0x06003F5B RID: 16219 RVA: 0x00173328 File Offset: 0x00171528
		internal PostProcessAttribute(Type renderer, string menuItem, bool allowInSceneView = true)
		{
			this.renderer = renderer;
			this.menuItem = menuItem;
			this.allowInSceneView = allowInSceneView;
			this.builtinEffect = true;
		}

		// Token: 0x040038BF RID: 14527
		public readonly Type renderer;

		// Token: 0x040038C0 RID: 14528
		public readonly PostProcessEvent eventType;

		// Token: 0x040038C1 RID: 14529
		public readonly string menuItem;

		// Token: 0x040038C2 RID: 14530
		public readonly bool allowInSceneView;

		// Token: 0x040038C3 RID: 14531
		internal readonly bool builtinEffect;
	}
}
