using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008A4 RID: 2212
public class UIParticle : BaseMonoBehaviour
{
	// Token: 0x060036EB RID: 14059 RVA: 0x0014A318 File Offset: 0x00148518
	public static void Add(UIParticle particleSource, RectTransform spawnPosition, RectTransform particleCanvas)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(particleSource.gameObject);
		gameObject.transform.SetParent(spawnPosition, false);
		gameObject.transform.localPosition = new Vector3(UnityEngine.Random.Range(0f, spawnPosition.rect.width) - spawnPosition.rect.width * spawnPosition.pivot.x, UnityEngine.Random.Range(0f, spawnPosition.rect.height) - spawnPosition.rect.height * spawnPosition.pivot.y, 0f);
		gameObject.transform.SetParent(particleCanvas, true);
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x060036EC RID: 14060 RVA: 0x0014A3E4 File Offset: 0x001485E4
	private void Start()
	{
		base.transform.localScale *= UnityEngine.Random.Range(this.InitialScale.x, this.InitialScale.y);
		this.velocity.x = UnityEngine.Random.Range(this.InitialX.x, this.InitialX.y);
		this.velocity.y = UnityEngine.Random.Range(this.InitialY.x, this.InitialY.y);
		this.gravity = UnityEngine.Random.Range(this.Gravity.x, this.Gravity.y);
		this.scaleVelocity = UnityEngine.Random.Range(this.ScaleVelocity.x, this.ScaleVelocity.y);
		Image component = base.GetComponent<Image>();
		if (component)
		{
			component.color = this.InitialColor.Evaluate(UnityEngine.Random.Range(0f, 1f));
		}
		this.lifetime = UnityEngine.Random.Range(this.InitialDelay.x, this.InitialDelay.y) * -1f;
		if (this.lifetime < 0f)
		{
			base.GetComponent<CanvasGroup>().alpha = 0f;
		}
		base.Invoke(new Action(this.Die), UnityEngine.Random.Range(this.LifeTime.x, this.LifeTime.y) + this.lifetime * -1f);
	}

	// Token: 0x060036ED RID: 14061 RVA: 0x0014A560 File Offset: 0x00148760
	private void Update()
	{
		if (this.lifetime < 0f)
		{
			this.lifetime += Time.deltaTime;
			if (this.lifetime < 0f)
			{
				return;
			}
			base.GetComponent<CanvasGroup>().alpha = 1f;
		}
		else
		{
			this.lifetime += Time.deltaTime;
		}
		Vector3 position = base.transform.position;
		Vector3 vector = base.transform.localScale;
		this.velocity.y = this.velocity.y - this.gravity * Time.deltaTime;
		position.x += this.velocity.x * Time.deltaTime;
		position.y += this.velocity.y * Time.deltaTime;
		vector += Vector3.one * this.scaleVelocity * Time.deltaTime;
		if (vector.x <= 0f || vector.y <= 0f)
		{
			this.Die();
			return;
		}
		base.transform.position = position;
		base.transform.localScale = vector;
	}

	// Token: 0x060036EE RID: 14062 RVA: 0x0014A683 File Offset: 0x00148883
	private void Die()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x040031CC RID: 12748
	public Vector2 LifeTime;

	// Token: 0x040031CD RID: 12749
	public Vector2 Gravity = new Vector2(1000f, 1000f);

	// Token: 0x040031CE RID: 12750
	public Vector2 InitialX;

	// Token: 0x040031CF RID: 12751
	public Vector2 InitialY;

	// Token: 0x040031D0 RID: 12752
	public Vector2 InitialScale = Vector2.one;

	// Token: 0x040031D1 RID: 12753
	public Vector2 InitialDelay;

	// Token: 0x040031D2 RID: 12754
	public Vector2 ScaleVelocity;

	// Token: 0x040031D3 RID: 12755
	public Gradient InitialColor;

	// Token: 0x040031D4 RID: 12756
	private float lifetime;

	// Token: 0x040031D5 RID: 12757
	private float gravity;

	// Token: 0x040031D6 RID: 12758
	private Vector2 velocity;

	// Token: 0x040031D7 RID: 12759
	private float scaleVelocity;
}
