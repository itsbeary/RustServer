using System;
using UnityEngine.UI;

namespace UnityEngine
{
	// Token: 0x02000A33 RID: 2611
	public static class UIEx
	{
		// Token: 0x06003DDA RID: 15834 RVA: 0x0016A0A4 File Offset: 0x001682A4
		public static Vector2 Unpivot(this RectTransform rect, Vector2 localPos)
		{
			localPos.x += rect.pivot.x * rect.rect.width;
			localPos.y += rect.pivot.y * rect.rect.height;
			return localPos;
		}

		// Token: 0x06003DDB RID: 15835 RVA: 0x0016A0FC File Offset: 0x001682FC
		public static void CenterOnPosition(this ScrollRect scrollrect, Vector2 pos)
		{
			RectTransform rectTransform = scrollrect.transform as RectTransform;
			Vector2 vector = new Vector2(scrollrect.content.localScale.x, scrollrect.content.localScale.y);
			pos.x *= vector.x;
			pos.y *= vector.y;
			Vector2 vector2 = new Vector2(scrollrect.content.rect.width * vector.x - rectTransform.rect.width, scrollrect.content.rect.height * vector.y - rectTransform.rect.height);
			pos.x = pos.x / vector2.x + scrollrect.content.pivot.x;
			pos.y = pos.y / vector2.y + scrollrect.content.pivot.y;
			if (scrollrect.movementType != ScrollRect.MovementType.Unrestricted)
			{
				pos.x = Mathf.Clamp(pos.x, 0f, 1f);
				pos.y = Mathf.Clamp(pos.y, 0f, 1f);
			}
			scrollrect.normalizedPosition = pos;
		}

		// Token: 0x06003DDC RID: 15836 RVA: 0x0016A248 File Offset: 0x00168448
		public static void RebuildHackUnity2019(this Image image)
		{
			Sprite sprite = image.sprite;
			image.sprite = null;
			image.sprite = sprite;
		}
	}
}
