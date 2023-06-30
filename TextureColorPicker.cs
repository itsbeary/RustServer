using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Token: 0x020002EE RID: 750
public class TextureColorPicker : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IDragHandler
{
	// Token: 0x06001E46 RID: 7750 RVA: 0x000CE364 File Offset: 0x000CC564
	public virtual void OnPointerDown(PointerEventData eventData)
	{
		this.OnDrag(eventData);
	}

	// Token: 0x06001E47 RID: 7751 RVA: 0x000CE370 File Offset: 0x000CC570
	public virtual void OnDrag(PointerEventData eventData)
	{
		RectTransform rectTransform = base.transform as RectTransform;
		Vector2 vector = default(Vector2);
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out vector))
		{
			vector.x += rectTransform.rect.width * 0.5f;
			vector.y += rectTransform.rect.height * 0.5f;
			vector.x /= rectTransform.rect.width;
			vector.y /= rectTransform.rect.height;
			Color pixel = this.texture.GetPixel((int)(vector.x * (float)this.texture.width), (int)(vector.y * (float)this.texture.height));
			this.onColorSelected.Invoke(pixel);
		}
	}

	// Token: 0x04001780 RID: 6016
	public Texture2D texture;

	// Token: 0x04001781 RID: 6017
	public TextureColorPicker.onColorSelectedEvent onColorSelected = new TextureColorPicker.onColorSelectedEvent();

	// Token: 0x02000CB5 RID: 3253
	[Serializable]
	public class onColorSelectedEvent : UnityEvent<Color>
	{
	}
}
