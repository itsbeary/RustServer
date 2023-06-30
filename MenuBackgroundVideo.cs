using System;
using System.Collections;
using System.IO;
using System.Linq;
using Rust;
using UnityEngine;
using UnityEngine.Video;

// Token: 0x0200087F RID: 2175
public class MenuBackgroundVideo : SingletonComponent<MenuBackgroundVideo>
{
	// Token: 0x0600366D RID: 13933 RVA: 0x001489BB File Offset: 0x00146BBB
	protected override void Awake()
	{
		base.Awake();
		this.LoadVideoList();
		this.NextVideo();
		base.GetComponent<VideoPlayer>().errorReceived += this.OnVideoError;
	}

	// Token: 0x0600366E RID: 13934 RVA: 0x001489E6 File Offset: 0x00146BE6
	private void OnVideoError(VideoPlayer source, string message)
	{
		this.errored = true;
	}

	// Token: 0x0600366F RID: 13935 RVA: 0x001489F0 File Offset: 0x00146BF0
	public void LoadVideoList()
	{
		this.videos = (from x in Directory.EnumerateFiles(UnityEngine.Application.streamingAssetsPath + "/MenuVideo/")
			where x.EndsWith(".mp4") || x.EndsWith(".webm")
			orderby Guid.NewGuid()
			select x).ToArray<string>();
	}

	// Token: 0x06003670 RID: 13936 RVA: 0x00148A64 File Offset: 0x00146C64
	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Keypad2))
		{
			this.LoadVideoList();
		}
		if (Input.GetKeyDown(KeyCode.Keypad1))
		{
			this.NextVideo();
		}
	}

	// Token: 0x06003671 RID: 13937 RVA: 0x00148A8C File Offset: 0x00146C8C
	private void NextVideo()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		string[] array = this.videos;
		int num = this.index;
		this.index = num + 1;
		string text = array[num % this.videos.Length];
		this.errored = false;
		if (Global.LaunchCountThisVersion <= 3)
		{
			string text2 = this.videos.Where((string x) => x.EndsWith("whatsnew.mp4")).FirstOrDefault<string>();
			if (!string.IsNullOrEmpty(text2))
			{
				text = text2;
			}
		}
		Debug.Log("Playing Video " + text);
		VideoPlayer component = base.GetComponent<VideoPlayer>();
		component.url = text;
		component.Play();
	}

	// Token: 0x06003672 RID: 13938 RVA: 0x00148B2E File Offset: 0x00146D2E
	internal IEnumerator ReadyVideo()
	{
		if (this.errored)
		{
			yield break;
		}
		VideoPlayer player = base.GetComponent<VideoPlayer>();
		while (!player.isPrepared)
		{
			if (this.errored)
			{
				yield break;
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x0400313E RID: 12606
	private string[] videos;

	// Token: 0x0400313F RID: 12607
	private int index;

	// Token: 0x04003140 RID: 12608
	private bool errored;
}
