using System;

namespace UnityEngine
{
	// Token: 0x02000A25 RID: 2597
	public static class AniamtorEx
	{
		// Token: 0x06003D98 RID: 15768 RVA: 0x001692EC File Offset: 0x001674EC
		public static void SetFloatFixed(this Animator animator, int id, float value, float dampTime, float deltaTime)
		{
			if (value == 0f)
			{
				float @float = animator.GetFloat(id);
				if (@float == 0f)
				{
					return;
				}
				if (@float < 1E-45f)
				{
					animator.SetFloat(id, 0f);
					return;
				}
			}
			animator.SetFloat(id, value, dampTime, deltaTime);
		}

		// Token: 0x06003D99 RID: 15769 RVA: 0x00169332 File Offset: 0x00167532
		public static void SetBoolChecked(this Animator animator, int id, bool value)
		{
			if (animator.GetBool(id) != value)
			{
				animator.SetBool(id, value);
			}
		}
	}
}
