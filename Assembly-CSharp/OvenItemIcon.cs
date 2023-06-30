using System;
using System.Collections.Generic;
using System.Linq;
using Rust.UI;
using UnityEngine;

// Token: 0x02000848 RID: 2120
public class OvenItemIcon : MonoBehaviour
{
	// Token: 0x0600360A RID: 13834 RVA: 0x00147A18 File Offset: 0x00145C18
	private void Start()
	{
		OvenItemIcon.OvenSlotConfig ovenSlotConfig = this.SlotConfigs.FirstOrDefault((OvenItemIcon.OvenSlotConfig x) => x.Type == this.SlotType);
		if (ovenSlotConfig == null)
		{
			Debug.LogError(string.Format("Can't find slot config for '{0}'", this.SlotType));
			return;
		}
		this.ItemIcon.emptySlotBackgroundSprite = ovenSlotConfig.BackgroundImage;
		this.MaterialLabel.SetPhrase(ovenSlotConfig.SlotPhrase);
		this.UpdateLabels();
	}

	// Token: 0x0600360B RID: 13835 RVA: 0x00147A83 File Offset: 0x00145C83
	private void Update()
	{
		if (this.ItemIcon.item == this._item)
		{
			return;
		}
		this._item = this.ItemIcon.item;
		this.UpdateLabels();
	}

	// Token: 0x0600360C RID: 13836 RVA: 0x00147AB0 File Offset: 0x00145CB0
	private void UpdateLabels()
	{
		this.CanvasGroup.alpha = ((this._item != null) ? 1f : this.DisabledAlphaScale);
		RustText itemLabel = this.ItemLabel;
		if (itemLabel == null)
		{
			return;
		}
		itemLabel.SetPhrase((this._item == null) ? this.EmptyPhrase : this._item.info.displayName);
	}

	// Token: 0x04002FA7 RID: 12199
	public ItemIcon ItemIcon;

	// Token: 0x04002FA8 RID: 12200
	public RustText ItemLabel;

	// Token: 0x04002FA9 RID: 12201
	public RustText MaterialLabel;

	// Token: 0x04002FAA RID: 12202
	public OvenSlotType SlotType;

	// Token: 0x04002FAB RID: 12203
	public Translate.Phrase EmptyPhrase = new Translate.Phrase("empty", "empty");

	// Token: 0x04002FAC RID: 12204
	public List<OvenItemIcon.OvenSlotConfig> SlotConfigs = new List<OvenItemIcon.OvenSlotConfig>();

	// Token: 0x04002FAD RID: 12205
	public float DisabledAlphaScale;

	// Token: 0x04002FAE RID: 12206
	public CanvasGroup CanvasGroup;

	// Token: 0x04002FAF RID: 12207
	private Item _item;

	// Token: 0x02000E99 RID: 3737
	[Serializable]
	public class OvenSlotConfig
	{
		// Token: 0x04004C94 RID: 19604
		public OvenSlotType Type;

		// Token: 0x04004C95 RID: 19605
		public Sprite BackgroundImage;

		// Token: 0x04004C96 RID: 19606
		public Translate.Phrase SlotPhrase;
	}
}
