using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Facepunch;
using Facepunch.Extend;
using Facepunch.Math;
using Facepunch.Models;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000882 RID: 2178
public class NewsSource : MonoBehaviour
{
	// Token: 0x06003679 RID: 13945 RVA: 0x00148C04 File Offset: 0x00146E04
	public void Awake()
	{
		GA.DesignEvent("news:view");
	}

	// Token: 0x0600367A RID: 13946 RVA: 0x00148C10 File Offset: 0x00146E10
	public void OnEnable()
	{
		if (SteamNewsSource.Stories == null || SteamNewsSource.Stories.Length == 0)
		{
			return;
		}
		this.SetStory(SteamNewsSource.Stories[0]);
	}

	// Token: 0x0600367B RID: 13947 RVA: 0x00148C34 File Offset: 0x00146E34
	public void SetStory(SteamNewsSource.Story story)
	{
		NewsSource.<>c__DisplayClass12_0 CS$<>8__locals1 = new NewsSource.<>c__DisplayClass12_0();
		CS$<>8__locals1.story = story;
		PlayerPrefs.SetInt("lastNewsDate", CS$<>8__locals1.story.date);
		this.container.DestroyAllChildren(false);
		this.title.text = CS$<>8__locals1.story.name;
		this.authorName.text = "by " + CS$<>8__locals1.story.author;
		string text = ((long)(Epoch.Current - CS$<>8__locals1.story.date)).FormatSecondsLong();
		this.date.text = "Posted " + text + " ago";
		this.button.onClick.RemoveAllListeners();
		this.button.onClick.AddListener(delegate
		{
			Facepunch.Models.Manifest.NewsInfo.BlogInfo blogInfo2 = base.<SetStory>g__GetBlogPost|1();
			string text3 = ((blogInfo2 != null) ? blogInfo2.Url : null) ?? CS$<>8__locals1.story.url;
			Debug.Log("Opening URL: " + text3);
			UnityEngine.Application.OpenURL(text3);
		});
		Facepunch.Models.Manifest.NewsInfo.BlogInfo blogInfo = CS$<>8__locals1.<SetStory>g__GetBlogPost|1();
		string text2 = ((blogInfo != null) ? blogInfo.HeaderImage : null);
		NewsSource.ParagraphBuilder paragraphBuilder = NewsSource.ParagraphBuilder.New();
		this.ParseBbcode(ref paragraphBuilder, CS$<>8__locals1.story.text, ref text2, 0);
		this.AppendParagraph(ref paragraphBuilder);
		if (text2 != null)
		{
			this.coverImage.Load(text2);
		}
		RustText[] componentsInChildren = this.container.GetComponentsInChildren<RustText>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].DoAutoSize();
		}
	}

	// Token: 0x0600367C RID: 13948 RVA: 0x00148D78 File Offset: 0x00146F78
	private void ParseBbcode(ref NewsSource.ParagraphBuilder currentParagraph, string bbcode, ref string firstImage, int depth = 0)
	{
		foreach (object obj in NewsSource.BbcodeParse.Matches(bbcode))
		{
			Match match = (Match)obj;
			string value = match.Groups[1].Value;
			string value2 = match.Groups[2].Value;
			string value3 = match.Groups[3].Value;
			string value4 = match.Groups[4].Value;
			currentParagraph.Append(value);
			string text = value2.ToLowerInvariant();
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num <= 2369466585U)
			{
				if (num <= 706259085U)
				{
					if (num != 217798785U)
					{
						if (num != 632598351U)
						{
							if (num == 706259085U)
							{
								if (text == "noparse")
								{
									currentParagraph.Append(value4);
								}
							}
						}
						else if (text == "strike")
						{
							currentParagraph.Append("<s>");
							this.ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
							currentParagraph.Append("</s>");
						}
					}
					else if (text == "list")
					{
						currentParagraph.AppendLine();
						foreach (string text2 in NewsSource.GetBulletPoints(value4))
						{
							if (!string.IsNullOrWhiteSpace(text2))
							{
								currentParagraph.Append("\t• ");
								currentParagraph.Append(text2.Trim());
								currentParagraph.AppendLine();
							}
						}
					}
				}
				else if (num <= 1624406948U)
				{
					if (num != 848251934U)
					{
						if (num == 1624406948U)
						{
							if (text == "previewyoutube")
							{
								if (depth == 0)
								{
									string[] array2 = value3.Split(new char[] { ';' });
									this.AppendYouTube(ref currentParagraph, array2[0]);
								}
							}
						}
					}
					else if (text == "url")
					{
						if (value4.Contains("[img]", StringComparison.InvariantCultureIgnoreCase))
						{
							this.ParseBbcode(ref currentParagraph, value4, ref firstImage, depth);
						}
						else
						{
							int count = currentParagraph.Links.Count;
							currentParagraph.Links.Add(value3);
							currentParagraph.Append(string.Format("<link={0}><u>", count));
							this.ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
							currentParagraph.Append("</u></link>");
						}
					}
				}
				else if (num != 2229740804U)
				{
					if (num == 2369466585U)
					{
						if (text == "h4")
						{
							currentParagraph.Append("<size=150%>");
							this.ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
							currentParagraph.Append("</size>");
						}
					}
				}
				else if (text == "img")
				{
					if (depth == 0)
					{
						string text3 = value4.Trim();
						if (firstImage == null)
						{
							firstImage = text3;
						}
						this.AppendImage(ref currentParagraph, text3);
					}
				}
			}
			else if (num <= 2419799442U)
			{
				if (num != 2386244204U)
				{
					if (num != 2403021823U)
					{
						if (num != 2419799442U)
						{
							continue;
						}
						if (!(text == "h1"))
						{
							continue;
						}
					}
					else if (!(text == "h2"))
					{
						continue;
					}
					currentParagraph.Append("<size=200%>");
					this.ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
					currentParagraph.Append("</size>");
				}
				else if (text == "h3")
				{
					currentParagraph.Append("<size=175%>");
					this.ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
					currentParagraph.Append("</size>");
				}
			}
			else if (num <= 3876335077U)
			{
				if (num != 2791659946U)
				{
					if (num == 3876335077U)
					{
						if (text == "b")
						{
							currentParagraph.Append("<b>");
							this.ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
							currentParagraph.Append("</b>");
						}
					}
				}
				else if (text == "olist")
				{
					currentParagraph.AppendLine();
					string[] bulletPoints = NewsSource.GetBulletPoints(value4);
					int num2 = 1;
					foreach (string text4 in bulletPoints)
					{
						if (!string.IsNullOrWhiteSpace(text4))
						{
							currentParagraph.Append(string.Format("\t{0} ", num2++));
							currentParagraph.Append(text4.Trim());
							currentParagraph.AppendLine();
						}
					}
				}
			}
			else if (num != 3960223172U)
			{
				if (num == 4027333648U)
				{
					if (text == "u")
					{
						currentParagraph.Append("<u>");
						this.ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
						currentParagraph.Append("</u>");
					}
				}
			}
			else if (text == "i")
			{
				currentParagraph.Append("<i>");
				this.ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
				currentParagraph.Append("</i>");
			}
		}
	}

	// Token: 0x0600367D RID: 13949 RVA: 0x00149304 File Offset: 0x00147504
	private static string[] GetBulletPoints(string listContent)
	{
		return ((listContent != null) ? listContent.Split(NewsSource.BulletSeparators, StringSplitOptions.RemoveEmptyEntries) : null) ?? Array.Empty<string>();
	}

	// Token: 0x0600367E RID: 13950 RVA: 0x00149324 File Offset: 0x00147524
	private void AppendParagraph(ref NewsSource.ParagraphBuilder currentParagraph)
	{
		if (currentParagraph.StringBuilder.Length > 0)
		{
			string text = currentParagraph.StringBuilder.ToString();
			RustText rustText = UnityEngine.Object.Instantiate<RustText>(this.paragraphTemplate, this.container);
			rustText.SetActive(true);
			rustText.SetText(text);
			NewsParagraph newsParagraph;
			if (rustText.TryGetComponent<NewsParagraph>(out newsParagraph))
			{
				newsParagraph.Links = currentParagraph.Links;
			}
		}
		currentParagraph = NewsSource.ParagraphBuilder.New();
	}

	// Token: 0x0600367F RID: 13951 RVA: 0x0014938A File Offset: 0x0014758A
	private void AppendImage(ref NewsSource.ParagraphBuilder currentParagraph, string url)
	{
		this.AppendParagraph(ref currentParagraph);
		HttpImage httpImage = UnityEngine.Object.Instantiate<HttpImage>(this.imageTemplate, this.container);
		httpImage.SetActive(true);
		httpImage.Load(url);
	}

	// Token: 0x06003680 RID: 13952 RVA: 0x001493B4 File Offset: 0x001475B4
	private void AppendYouTube(ref NewsSource.ParagraphBuilder currentParagraph, string videoId)
	{
		this.AppendParagraph(ref currentParagraph);
		HttpImage httpImage = UnityEngine.Object.Instantiate<HttpImage>(this.youtubeTemplate, this.container);
		httpImage.SetActive(true);
		httpImage.Load("https://img.youtube.com/vi/" + videoId + "/maxresdefault.jpg");
		RustButton component = httpImage.GetComponent<RustButton>();
		if (component != null)
		{
			string videoUrl = "https://www.youtube.com/watch?v=" + videoId;
			component.OnReleased.AddListener(delegate
			{
				Debug.Log("Opening URL: " + videoUrl);
				UnityEngine.Application.OpenURL(videoUrl);
			});
		}
	}

	// Token: 0x04003143 RID: 12611
	private static readonly Regex BbcodeParse = new Regex("([^\\[]*)(?:\\[(\\w+)(?:=([^\\]]+))?\\](.*?)\\[\\/\\2\\])?", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);

	// Token: 0x04003144 RID: 12612
	public RustText title;

	// Token: 0x04003145 RID: 12613
	public RustText date;

	// Token: 0x04003146 RID: 12614
	public RustText authorName;

	// Token: 0x04003147 RID: 12615
	public HttpImage coverImage;

	// Token: 0x04003148 RID: 12616
	public RectTransform container;

	// Token: 0x04003149 RID: 12617
	public Button button;

	// Token: 0x0400314A RID: 12618
	public RustText paragraphTemplate;

	// Token: 0x0400314B RID: 12619
	public HttpImage imageTemplate;

	// Token: 0x0400314C RID: 12620
	public HttpImage youtubeTemplate;

	// Token: 0x0400314D RID: 12621
	private static readonly string[] BulletSeparators = new string[] { "[*]" };

	// Token: 0x02000EA5 RID: 3749
	private struct ParagraphBuilder
	{
		// Token: 0x06005315 RID: 21269 RVA: 0x001B1A98 File Offset: 0x001AFC98
		public static NewsSource.ParagraphBuilder New()
		{
			return new NewsSource.ParagraphBuilder
			{
				StringBuilder = new StringBuilder(),
				Links = new List<string>()
			};
		}

		// Token: 0x06005316 RID: 21270 RVA: 0x001B1AC6 File Offset: 0x001AFCC6
		public void AppendLine()
		{
			this.StringBuilder.AppendLine();
		}

		// Token: 0x06005317 RID: 21271 RVA: 0x001B1AD4 File Offset: 0x001AFCD4
		public void Append(string text)
		{
			this.StringBuilder.Append(text);
		}

		// Token: 0x04004CD5 RID: 19669
		public StringBuilder StringBuilder;

		// Token: 0x04004CD6 RID: 19670
		public List<string> Links;
	}
}
