using System;
using System.Collections;
using Rust;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020008C8 RID: 2248
public class TypeThroughButton : Button, IUpdateSelectedHandler, IEventSystemHandler
{
	// Token: 0x0600374C RID: 14156 RVA: 0x0014C2F0 File Offset: 0x0014A4F0
	public void OnUpdateSelected(BaseEventData eventData)
	{
		if (this.typingTarget == null)
		{
			return;
		}
		while (Event.PopEvent(this._processingEvent))
		{
			if (this._processingEvent.rawType == EventType.KeyDown && this._processingEvent.character != '\0')
			{
				Event @event = new Event(this._processingEvent);
				Global.Runner.StartCoroutine(this.DelayedActivateTextField(@event));
				break;
			}
		}
		eventData.Use();
	}

	// Token: 0x0600374D RID: 14157 RVA: 0x0014C35A File Offset: 0x0014A55A
	private IEnumerator DelayedActivateTextField(Event e)
	{
		this.typingTarget.ActivateInputField();
		this.typingTarget.Select();
		if (e.character != ' ')
		{
			InputField inputField = this.typingTarget;
			inputField.text += " ";
		}
		this.typingTarget.MoveTextEnd(false);
		this.typingTarget.ProcessEvent(e);
		yield return null;
		this.typingTarget.caretPosition = this.typingTarget.text.Length;
		this.typingTarget.ForceLabelUpdate();
		yield break;
	}

	// Token: 0x040032AA RID: 12970
	public InputField typingTarget;

	// Token: 0x040032AB RID: 12971
	private Event _processingEvent = new Event();
}
