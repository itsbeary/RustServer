using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007D0 RID: 2000
public class PhoneDialler : UIDialog
{
	// Token: 0x04002CE2 RID: 11490
	public GameObject DialingRoot;

	// Token: 0x04002CE3 RID: 11491
	public GameObject CallInProcessRoot;

	// Token: 0x04002CE4 RID: 11492
	public GameObject IncomingCallRoot;

	// Token: 0x04002CE5 RID: 11493
	public RustText ThisPhoneNumber;

	// Token: 0x04002CE6 RID: 11494
	public RustInput PhoneNameInput;

	// Token: 0x04002CE7 RID: 11495
	public RustText textDisplay;

	// Token: 0x04002CE8 RID: 11496
	public RustText CallTimeText;

	// Token: 0x04002CE9 RID: 11497
	public RustButton DefaultDialViewButton;

	// Token: 0x04002CEA RID: 11498
	public RustText[] IncomingCallNumber;

	// Token: 0x04002CEB RID: 11499
	public GameObject NumberDialRoot;

	// Token: 0x04002CEC RID: 11500
	public GameObject PromptVoicemailRoot;

	// Token: 0x04002CED RID: 11501
	public RustButton ContactsButton;

	// Token: 0x04002CEE RID: 11502
	public RustText FailText;

	// Token: 0x04002CEF RID: 11503
	public NeedsCursor CursorController;

	// Token: 0x04002CF0 RID: 11504
	public NeedsKeyboard KeyboardController;

	// Token: 0x04002CF1 RID: 11505
	public Translate.Phrase WrongNumberPhrase;

	// Token: 0x04002CF2 RID: 11506
	public Translate.Phrase NetworkBusy;

	// Token: 0x04002CF3 RID: 11507
	public Translate.Phrase Engaged;

	// Token: 0x04002CF4 RID: 11508
	public GameObjectRef DirectoryEntryPrefab;

	// Token: 0x04002CF5 RID: 11509
	public Transform DirectoryRoot;

	// Token: 0x04002CF6 RID: 11510
	public GameObject NoDirectoryRoot;

	// Token: 0x04002CF7 RID: 11511
	public RustButton DirectoryPageUp;

	// Token: 0x04002CF8 RID: 11512
	public RustButton DirectoryPageDown;

	// Token: 0x04002CF9 RID: 11513
	public Transform ContactsRoot;

	// Token: 0x04002CFA RID: 11514
	public RustInput ContactsNameInput;

	// Token: 0x04002CFB RID: 11515
	public RustInput ContactsNumberInput;

	// Token: 0x04002CFC RID: 11516
	public GameObject NoContactsRoot;

	// Token: 0x04002CFD RID: 11517
	public RustButton AddContactButton;

	// Token: 0x04002CFE RID: 11518
	public SoundDefinition DialToneSfx;

	// Token: 0x04002CFF RID: 11519
	public Button[] NumberButtons;

	// Token: 0x04002D00 RID: 11520
	public Translate.Phrase AnsweringMachine;

	// Token: 0x04002D01 RID: 11521
	public VoicemailDialog Voicemail;

	// Token: 0x04002D02 RID: 11522
	public GameObject VoicemailRoot;
}
