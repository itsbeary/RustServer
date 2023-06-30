using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200082A RID: 2090
public class ItemIcon : BaseMonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler, IDraggable, IInventoryChanged, IItemAmountChanged, IItemIconChanged
{
	// Token: 0x060035E2 RID: 13794 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnPointerClick(PointerEventData eventData)
	{
	}

	// Token: 0x060035E3 RID: 13795 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnPointerEnter(PointerEventData eventData)
	{
	}

	// Token: 0x060035E4 RID: 13796 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnPointerExit(PointerEventData eventData)
	{
	}

	// Token: 0x04002F1A RID: 12058
	private Color backgroundColor;

	// Token: 0x04002F1B RID: 12059
	public Color selectedBackgroundColor = new Color(0.12156863f, 0.41960785f, 0.627451f, 0.78431374f);

	// Token: 0x04002F1C RID: 12060
	public float unoccupiedAlpha = 1f;

	// Token: 0x04002F1D RID: 12061
	public Color unoccupiedColor;

	// Token: 0x04002F1E RID: 12062
	public ItemContainerSource containerSource;

	// Token: 0x04002F1F RID: 12063
	public int slotOffset;

	// Token: 0x04002F20 RID: 12064
	[Range(0f, 64f)]
	public int slot;

	// Token: 0x04002F21 RID: 12065
	public bool setSlotFromSiblingIndex = true;

	// Token: 0x04002F22 RID: 12066
	public GameObject slots;

	// Token: 0x04002F23 RID: 12067
	public CanvasGroup iconContents;

	// Token: 0x04002F24 RID: 12068
	public CanvasGroup canvasGroup;

	// Token: 0x04002F25 RID: 12069
	public Image iconImage;

	// Token: 0x04002F26 RID: 12070
	public Image underlayImage;

	// Token: 0x04002F27 RID: 12071
	public Text amountText;

	// Token: 0x04002F28 RID: 12072
	public Text hoverText;

	// Token: 0x04002F29 RID: 12073
	public Image hoverOutline;

	// Token: 0x04002F2A RID: 12074
	public Image cornerIcon;

	// Token: 0x04002F2B RID: 12075
	public Image lockedImage;

	// Token: 0x04002F2C RID: 12076
	public Image progressImage;

	// Token: 0x04002F2D RID: 12077
	public Image backgroundImage;

	// Token: 0x04002F2E RID: 12078
	public Image backgroundUnderlayImage;

	// Token: 0x04002F2F RID: 12079
	public Image progressPanel;

	// Token: 0x04002F30 RID: 12080
	public Sprite emptySlotBackgroundSprite;

	// Token: 0x04002F31 RID: 12081
	public CanvasGroup conditionObject;

	// Token: 0x04002F32 RID: 12082
	public Image conditionFill;

	// Token: 0x04002F33 RID: 12083
	public Image maxConditionFill;

	// Token: 0x04002F34 RID: 12084
	public GameObject lightEnabled;

	// Token: 0x04002F35 RID: 12085
	public bool allowSelection = true;

	// Token: 0x04002F36 RID: 12086
	public bool allowDropping = true;

	// Token: 0x04002F37 RID: 12087
	public bool allowMove = true;

	// Token: 0x04002F38 RID: 12088
	public bool showCountDropShadow;

	// Token: 0x04002F39 RID: 12089
	[NonSerialized]
	public Item item;

	// Token: 0x04002F3A RID: 12090
	[NonSerialized]
	public bool invalidSlot;

	// Token: 0x04002F3B RID: 12091
	public SoundDefinition hoverSound;
}
