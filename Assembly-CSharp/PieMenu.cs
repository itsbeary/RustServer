using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020008A6 RID: 2214
[ExecuteInEditMode]
public class PieMenu : UIBehaviour
{
	// Token: 0x17000457 RID: 1111
	// (get) Token: 0x060036F1 RID: 14065 RVA: 0x0014A6B8 File Offset: 0x001488B8
	// (set) Token: 0x060036F2 RID: 14066 RVA: 0x0014A6C0 File Offset: 0x001488C0
	public bool IsOpen { get; private set; }

	// Token: 0x060036F3 RID: 14067 RVA: 0x0014A6CC File Offset: 0x001488CC
	protected override void Start()
	{
		base.Start();
		PieMenu.Instance = this;
		this.canvasGroup = base.GetComponentInChildren<CanvasGroup>();
		this.canvasGroup.alpha = 0f;
		this.canvasGroup.interactable = false;
		this.canvasGroup.blocksRaycasts = false;
		this.IsOpen = false;
		this.isClosing = true;
		base.gameObject.SetChildComponentsEnabled(false);
	}

	// Token: 0x060036F4 RID: 14068 RVA: 0x0014A733 File Offset: 0x00148933
	public void Clear()
	{
		this.options = new PieMenu.MenuOption[0];
	}

	// Token: 0x060036F5 RID: 14069 RVA: 0x0014A744 File Offset: 0x00148944
	public void AddOption(PieMenu.MenuOption option)
	{
		List<PieMenu.MenuOption> list = this.options.ToList<PieMenu.MenuOption>();
		list.Add(option);
		this.options = list.ToArray();
	}

	// Token: 0x060036F6 RID: 14070 RVA: 0x0014A770 File Offset: 0x00148970
	public void FinishAndOpen()
	{
		this.IsOpen = true;
		this.isClosing = false;
		this.SetDefaultOption();
		this.Rebuild();
		this.UpdateInteraction(false);
		this.PlayOpenSound();
		LeanTween.cancel(base.gameObject);
		LeanTween.cancel(this.scaleTarget);
		base.GetComponent<CanvasGroup>().alpha = 0f;
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 1f, 0.1f).setEase(LeanTweenType.easeOutCirc);
		this.scaleTarget.transform.localScale = Vector3.one * 1.5f;
		LeanTween.scale(this.scaleTarget, Vector3.one, 0.1f).setEase(LeanTweenType.easeOutBounce);
		PieMenu.Instance.gameObject.SetChildComponentsEnabled(true);
	}

	// Token: 0x060036F7 RID: 14071 RVA: 0x0014A833 File Offset: 0x00148A33
	protected override void OnEnable()
	{
		base.OnEnable();
		this.Rebuild();
	}

	// Token: 0x060036F8 RID: 14072 RVA: 0x0014A844 File Offset: 0x00148A44
	public void SetDefaultOption()
	{
		this.defaultOption = null;
		for (int i = 0; i < this.options.Length; i++)
		{
			if (!this.options[i].disabled)
			{
				if (this.defaultOption == null)
				{
					this.defaultOption = this.options[i];
				}
				if (this.options[i].selected)
				{
					this.defaultOption = this.options[i];
					return;
				}
			}
		}
	}

	// Token: 0x060036F9 RID: 14073 RVA: 0x000063A5 File Offset: 0x000045A5
	public void PlayOpenSound()
	{
	}

	// Token: 0x060036FA RID: 14074 RVA: 0x000063A5 File Offset: 0x000045A5
	public void PlayCancelSound()
	{
	}

	// Token: 0x060036FB RID: 14075 RVA: 0x0014A8B0 File Offset: 0x00148AB0
	public void Close(bool success = false)
	{
		if (this.isClosing)
		{
			return;
		}
		this.isClosing = true;
		NeedsCursor component = base.GetComponent<NeedsCursor>();
		if (component != null)
		{
			component.enabled = false;
		}
		LeanTween.cancel(base.gameObject);
		LeanTween.cancel(this.scaleTarget);
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 0f, 0.2f).setEase(LeanTweenType.easeOutCirc);
		LeanTween.scale(this.scaleTarget, Vector3.one * (success ? 1.5f : 0.5f), 0.2f).setEase(LeanTweenType.easeOutCirc);
		PieMenu.Instance.gameObject.SetChildComponentsEnabled(false);
		this.IsOpen = false;
		this.selectedOption = null;
	}

	// Token: 0x060036FC RID: 14076 RVA: 0x0014A968 File Offset: 0x00148B68
	private void Update()
	{
		if (this.pieBackground.innerSize != this.innerSize || this.pieBackground.outerSize != this.outerSize || this.pieBackground.startRadius != this.startRadius || this.pieBackground.endRadius != this.startRadius + this.radiusSize)
		{
			this.pieBackground.startRadius = this.startRadius;
			this.pieBackground.endRadius = this.startRadius + this.radiusSize;
			this.pieBackground.innerSize = this.innerSize;
			this.pieBackground.outerSize = this.outerSize;
			this.pieBackground.SetVerticesDirty();
		}
		this.UpdateInteraction(true);
		if (this.IsOpen)
		{
			CursorManager.HoldOpen(false);
			IngameMenuBackground.Enabled = true;
		}
	}

	// Token: 0x060036FD RID: 14077 RVA: 0x0014AA3C File Offset: 0x00148C3C
	public void Rebuild()
	{
		this.options = this.options.OrderBy((PieMenu.MenuOption x) => x.order).ToArray<PieMenu.MenuOption>();
		while (this.optionsCanvas.transform.childCount > 0)
		{
			if (UnityEngine.Application.isPlaying)
			{
				GameManager.DestroyImmediate(this.optionsCanvas.transform.GetChild(0).gameObject, true);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(this.optionsCanvas.transform.GetChild(0).gameObject);
			}
		}
		if (this.options.Length != 0)
		{
			for (int i = 0; i < this.options.Length; i++)
			{
				bool flag = false;
				if (this.options[i].allowMerge)
				{
					if (i > 0)
					{
						flag |= this.options[i].order == this.options[i - 1].order;
					}
					if (i < this.options.Length - 1)
					{
						flag |= this.options[i].order == this.options[i + 1].order;
					}
				}
				this.options[i].wantsMerge = flag;
			}
			int num = this.options.Length;
			int num2 = this.options.Where((PieMenu.MenuOption x) => x.wantsMerge).Count<PieMenu.MenuOption>();
			int num3 = num - num2;
			int num4 = num3 + num2 / 2;
			float num5 = this.radiusSize / (float)num * 0.75f;
			float num6 = (this.radiusSize - num5 * (float)num2) / (float)num3;
			float num7 = this.startRadius - this.radiusSize / (float)num4 * 0.25f;
			for (int j = 0; j < this.options.Length; j++)
			{
				float num8 = (this.options[j].wantsMerge ? 0.8f : 1f);
				float num9 = (this.options[j].wantsMerge ? num5 : num6);
				GameObject gameObject = Facepunch.Instantiate.GameObject(this.pieOptionPrefab, null);
				gameObject.transform.SetParent(this.optionsCanvas.transform, false);
				this.options[j].option = gameObject.GetComponent<PieOption>();
				this.options[j].option.UpdateOption(num7, num9, this.sliceGaps, this.options[j].name, this.outerSize, this.innerSize, num8 * this.iconSize, this.options[j].sprite, this.options[j].showOverlay);
				this.options[j].option.imageIcon.material = ((this.options[j].overrideColorMode != null && this.options[j].overrideColorMode.Value.Mode == PieMenu.MenuOption.ColorMode.PieMenuSpriteColorOption.SpriteColor) ? null : this.IconMaterial);
				num7 += num9;
			}
		}
		this.selectedOption = null;
	}

	// Token: 0x060036FE RID: 14078 RVA: 0x0014AD44 File Offset: 0x00148F44
	public void UpdateInteraction(bool allowLerp = true)
	{
		if (this.isClosing)
		{
			return;
		}
		Vector3 vector = UnityEngine.Input.mousePosition - new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f);
		float num = Mathf.Atan2(vector.x, vector.y) * 57.29578f;
		if (num < 0f)
		{
			num += 360f;
		}
		for (int i = 0; i < this.options.Length; i++)
		{
			float midRadius = this.options[i].option.midRadius;
			float sliceSize = this.options[i].option.sliceSize;
			if ((vector.magnitude < 32f && this.options[i] == this.defaultOption) || (vector.magnitude >= 32f && Mathf.Abs(Mathf.DeltaAngle(num, midRadius)) < sliceSize * 0.5f))
			{
				if (allowLerp)
				{
					this.pieSelection.startRadius = Mathf.MoveTowardsAngle(this.pieSelection.startRadius, this.options[i].option.background.startRadius, Time.deltaTime * Mathf.Abs(Mathf.DeltaAngle(this.pieSelection.startRadius, this.options[i].option.background.startRadius) * 30f + 10f));
					this.pieSelection.endRadius = Mathf.MoveTowardsAngle(this.pieSelection.endRadius, this.options[i].option.background.endRadius, Time.deltaTime * Mathf.Abs(Mathf.DeltaAngle(this.pieSelection.endRadius, this.options[i].option.background.endRadius) * 30f + 10f));
				}
				else
				{
					this.pieSelection.startRadius = this.options[i].option.background.startRadius;
					this.pieSelection.endRadius = this.options[i].option.background.endRadius;
				}
				this.middleImage.material = this.IconMaterial;
				if (this.options[i].overrideColorMode != null)
				{
					if (this.options[i].overrideColorMode.Value.Mode == PieMenu.MenuOption.ColorMode.PieMenuSpriteColorOption.CustomColor)
					{
						Color customColor = this.options[i].overrideColorMode.Value.CustomColor;
						this.pieSelection.color = customColor;
						customColor.a = PieMenu.middleImageColor.a;
						this.middleImage.color = customColor;
					}
					else if (this.options[i].overrideColorMode.Value.Mode == PieMenu.MenuOption.ColorMode.PieMenuSpriteColorOption.SpriteColor)
					{
						this.pieSelection.color = PieMenu.pieSelectionColor;
						this.middleImage.color = Color.white;
						this.middleImage.material = null;
					}
				}
				else
				{
					this.pieSelection.color = PieMenu.pieSelectionColor;
					this.middleImage.color = PieMenu.middleImageColor;
				}
				this.pieSelection.SetVerticesDirty();
				this.middleImage.sprite = this.options[i].sprite;
				this.middleTitle.text = this.options[i].name;
				this.middleDesc.text = this.options[i].desc;
				this.middleRequired.text = "";
				Facepunch.Input.Button buttonObjectWithBind = Facepunch.Input.GetButtonObjectWithBind("+prevskin");
				if (this.options[i].actionPrev != null && buttonObjectWithBind != null && buttonObjectWithBind.Code != KeyCode.None)
				{
					this.arrowLeft.SetActive(true);
					this.arrowLeft.GetComponentInChildren<TextMeshProUGUI>().text = buttonObjectWithBind.Code.ToShortname(true);
				}
				else
				{
					this.arrowLeft.SetActive(false);
				}
				Facepunch.Input.Button buttonObjectWithBind2 = Facepunch.Input.GetButtonObjectWithBind("+nextskin");
				if (this.options[i].actionNext != null && buttonObjectWithBind2 != null && buttonObjectWithBind2.Code != KeyCode.None)
				{
					this.arrowRight.SetActive(true);
					this.arrowRight.GetComponentInChildren<TextMeshProUGUI>().text = buttonObjectWithBind2.Code.ToShortname(true);
				}
				else
				{
					this.arrowRight.SetActive(false);
				}
				string text = this.options[i].requirements;
				if (text != null)
				{
					text = text.Replace("[e]", "<color=#CD412B>");
					text = text.Replace("[/e]", "</color>");
					this.middleRequired.text = text;
				}
				if (!this.options[i].showOverlay)
				{
					this.options[i].option.imageIcon.color = this.colorIconHovered;
				}
				if (this.selectedOption != this.options[i])
				{
					if (this.selectedOption != null && !this.options[i].disabled)
					{
						this.scaleTarget.transform.localScale = Vector3.one;
						LeanTween.scale(this.scaleTarget, Vector3.one * 1.03f, 0.2f).setEase(PieMenu.easePunch);
					}
					if (this.selectedOption != null)
					{
						this.selectedOption.option.imageIcon.RebuildHackUnity2019();
					}
					this.selectedOption = this.options[i];
					if (this.selectedOption != null)
					{
						this.selectedOption.option.imageIcon.RebuildHackUnity2019();
					}
				}
			}
			else
			{
				this.options[i].option.imageIcon.material = this.IconMaterial;
				if (this.options[i].overrideColorMode != null)
				{
					if (this.options[i].overrideColorMode.Value.Mode == PieMenu.MenuOption.ColorMode.PieMenuSpriteColorOption.CustomColor)
					{
						this.options[i].option.imageIcon.color = this.options[i].overrideColorMode.Value.CustomColor;
					}
					else if (this.options[i].overrideColorMode.Value.Mode == PieMenu.MenuOption.ColorMode.PieMenuSpriteColorOption.SpriteColor)
					{
						this.options[i].option.imageIcon.color = Color.white;
						this.options[i].option.imageIcon.material = null;
					}
				}
				else
				{
					this.options[i].option.imageIcon.color = this.colorIconActive;
				}
			}
			if (this.options[i].disabled)
			{
				this.options[i].option.imageIcon.color = this.colorIconDisabled;
				this.options[i].option.background.color = this.colorBackgroundDisabled;
			}
		}
	}

	// Token: 0x060036FF RID: 14079 RVA: 0x0000441C File Offset: 0x0000261C
	public bool DoSelect()
	{
		return true;
	}

	// Token: 0x06003700 RID: 14080 RVA: 0x000063A5 File Offset: 0x000045A5
	public void DoPrev()
	{
	}

	// Token: 0x06003701 RID: 14081 RVA: 0x000063A5 File Offset: 0x000045A5
	public void DoNext()
	{
	}

	// Token: 0x040031D9 RID: 12761
	public static PieMenu Instance;

	// Token: 0x040031DA RID: 12762
	public Image middleBox;

	// Token: 0x040031DB RID: 12763
	public PieShape pieBackgroundBlur;

	// Token: 0x040031DC RID: 12764
	public PieShape pieBackground;

	// Token: 0x040031DD RID: 12765
	public PieShape pieSelection;

	// Token: 0x040031DE RID: 12766
	public GameObject pieOptionPrefab;

	// Token: 0x040031DF RID: 12767
	public GameObject optionsCanvas;

	// Token: 0x040031E0 RID: 12768
	public PieMenu.MenuOption[] options;

	// Token: 0x040031E1 RID: 12769
	public GameObject scaleTarget;

	// Token: 0x040031E2 RID: 12770
	public GameObject arrowLeft;

	// Token: 0x040031E3 RID: 12771
	public GameObject arrowRight;

	// Token: 0x040031E4 RID: 12772
	public float sliceGaps = 10f;

	// Token: 0x040031E5 RID: 12773
	[Range(0f, 1f)]
	public float outerSize = 1f;

	// Token: 0x040031E6 RID: 12774
	[Range(0f, 1f)]
	public float innerSize = 0.5f;

	// Token: 0x040031E7 RID: 12775
	[Range(0f, 1f)]
	public float iconSize = 0.8f;

	// Token: 0x040031E8 RID: 12776
	[Range(0f, 360f)]
	public float startRadius;

	// Token: 0x040031E9 RID: 12777
	[Range(0f, 360f)]
	public float radiusSize = 360f;

	// Token: 0x040031EA RID: 12778
	public Image middleImage;

	// Token: 0x040031EB RID: 12779
	public TextMeshProUGUI middleTitle;

	// Token: 0x040031EC RID: 12780
	public TextMeshProUGUI middleDesc;

	// Token: 0x040031ED RID: 12781
	public TextMeshProUGUI middleRequired;

	// Token: 0x040031EE RID: 12782
	public Color colorIconActive;

	// Token: 0x040031EF RID: 12783
	public Color colorIconHovered;

	// Token: 0x040031F0 RID: 12784
	public Color colorIconDisabled;

	// Token: 0x040031F1 RID: 12785
	public Color colorBackgroundDisabled;

	// Token: 0x040031F2 RID: 12786
	public SoundDefinition clipOpen;

	// Token: 0x040031F3 RID: 12787
	public SoundDefinition clipCancel;

	// Token: 0x040031F4 RID: 12788
	public SoundDefinition clipChanged;

	// Token: 0x040031F5 RID: 12789
	public SoundDefinition clipSelected;

	// Token: 0x040031F6 RID: 12790
	public PieMenu.MenuOption defaultOption;

	// Token: 0x040031F7 RID: 12791
	private bool isClosing;

	// Token: 0x040031F8 RID: 12792
	private CanvasGroup canvasGroup;

	// Token: 0x040031FA RID: 12794
	public Material IconMaterial;

	// Token: 0x040031FB RID: 12795
	internal PieMenu.MenuOption selectedOption;

	// Token: 0x040031FC RID: 12796
	private static Color pieSelectionColor = new Color(0.804f, 0.255f, 0.169f, 1f);

	// Token: 0x040031FD RID: 12797
	private static Color middleImageColor = new Color(0.804f, 0.255f, 0.169f, 0.784f);

	// Token: 0x040031FE RID: 12798
	private static AnimationCurve easePunch = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.112586f, 0.9976035f),
		new Keyframe(0.3120486f, 0.01720615f),
		new Keyframe(0.4316337f, 0.17030682f),
		new Keyframe(0.5524869f, 0.03141804f),
		new Keyframe(0.6549395f, 0.002909959f),
		new Keyframe(0.770987f, 0.009817753f),
		new Keyframe(0.8838775f, 0.001939224f),
		new Keyframe(1f, 0f)
	});

	// Token: 0x02000EB3 RID: 3763
	[Serializable]
	public class MenuOption
	{
		// Token: 0x04004CFB RID: 19707
		public string name;

		// Token: 0x04004CFC RID: 19708
		public string desc;

		// Token: 0x04004CFD RID: 19709
		public string requirements;

		// Token: 0x04004CFE RID: 19710
		public Sprite sprite;

		// Token: 0x04004CFF RID: 19711
		public bool disabled;

		// Token: 0x04004D00 RID: 19712
		public int order;

		// Token: 0x04004D01 RID: 19713
		public PieMenu.MenuOption.ColorMode? overrideColorMode;

		// Token: 0x04004D02 RID: 19714
		public bool showOverlay;

		// Token: 0x04004D03 RID: 19715
		[NonSerialized]
		public Action<BasePlayer> action;

		// Token: 0x04004D04 RID: 19716
		[NonSerialized]
		public Action<BasePlayer> actionPrev;

		// Token: 0x04004D05 RID: 19717
		[NonSerialized]
		public Action<BasePlayer> actionNext;

		// Token: 0x04004D06 RID: 19718
		[NonSerialized]
		public PieOption option;

		// Token: 0x04004D07 RID: 19719
		[NonSerialized]
		public bool selected;

		// Token: 0x04004D08 RID: 19720
		[NonSerialized]
		public bool allowMerge;

		// Token: 0x04004D09 RID: 19721
		[NonSerialized]
		public bool wantsMerge;

		// Token: 0x02000FE7 RID: 4071
		public struct ColorMode
		{
			// Token: 0x040051A3 RID: 20899
			public PieMenu.MenuOption.ColorMode.PieMenuSpriteColorOption Mode;

			// Token: 0x040051A4 RID: 20900
			public Color CustomColor;

			// Token: 0x02001006 RID: 4102
			public enum PieMenuSpriteColorOption
			{
				// Token: 0x04005268 RID: 21096
				CustomColor,
				// Token: 0x04005269 RID: 21097
				SpriteColor
			}
		}
	}
}
