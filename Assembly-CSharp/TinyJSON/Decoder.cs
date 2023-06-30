using System;
using System.IO;
using System.Text;

namespace TinyJSON
{
	// Token: 0x020009CE RID: 2510
	public sealed class Decoder : IDisposable
	{
		// Token: 0x06003BCB RID: 15307 RVA: 0x00160C0B File Offset: 0x0015EE0B
		private Decoder(string jsonString)
		{
			this.json = new StringReader(jsonString);
		}

		// Token: 0x06003BCC RID: 15308 RVA: 0x00160C20 File Offset: 0x0015EE20
		public static Variant Decode(string jsonString)
		{
			Variant variant;
			using (Decoder decoder = new Decoder(jsonString))
			{
				variant = decoder.DecodeValue();
			}
			return variant;
		}

		// Token: 0x06003BCD RID: 15309 RVA: 0x00160C58 File Offset: 0x0015EE58
		public void Dispose()
		{
			this.json.Dispose();
			this.json = null;
		}

		// Token: 0x06003BCE RID: 15310 RVA: 0x00160C6C File Offset: 0x0015EE6C
		private ProxyObject DecodeObject()
		{
			ProxyObject proxyObject = new ProxyObject();
			this.json.Read();
			for (;;)
			{
				Decoder.Token nextToken = this.NextToken;
				if (nextToken == Decoder.Token.None)
				{
					break;
				}
				if (nextToken == Decoder.Token.CloseBrace)
				{
					return proxyObject;
				}
				if (nextToken != Decoder.Token.Comma)
				{
					string text = this.DecodeString();
					if (text == null)
					{
						goto Block_4;
					}
					if (this.NextToken != Decoder.Token.Colon)
					{
						goto Block_5;
					}
					this.json.Read();
					proxyObject.Add(text, this.DecodeValue());
				}
			}
			return null;
			Block_4:
			return null;
			Block_5:
			return null;
		}

		// Token: 0x06003BCF RID: 15311 RVA: 0x00160CDC File Offset: 0x0015EEDC
		private ProxyArray DecodeArray()
		{
			ProxyArray proxyArray = new ProxyArray();
			this.json.Read();
			bool flag = true;
			while (flag)
			{
				Decoder.Token nextToken = this.NextToken;
				if (nextToken == Decoder.Token.None)
				{
					return null;
				}
				if (nextToken != Decoder.Token.CloseBracket)
				{
					if (nextToken != Decoder.Token.Comma)
					{
						proxyArray.Add(this.DecodeByToken(nextToken));
					}
				}
				else
				{
					flag = false;
				}
			}
			return proxyArray;
		}

		// Token: 0x06003BD0 RID: 15312 RVA: 0x00160D2C File Offset: 0x0015EF2C
		private Variant DecodeValue()
		{
			Decoder.Token nextToken = this.NextToken;
			return this.DecodeByToken(nextToken);
		}

		// Token: 0x06003BD1 RID: 15313 RVA: 0x00160D48 File Offset: 0x0015EF48
		private Variant DecodeByToken(Decoder.Token token)
		{
			switch (token)
			{
			case Decoder.Token.OpenBrace:
				return this.DecodeObject();
			case Decoder.Token.OpenBracket:
				return this.DecodeArray();
			case Decoder.Token.String:
				return this.DecodeString();
			case Decoder.Token.Number:
				return this.DecodeNumber();
			case Decoder.Token.True:
				return new ProxyBoolean(true);
			case Decoder.Token.False:
				return new ProxyBoolean(false);
			case Decoder.Token.Null:
				return null;
			}
			return null;
		}

		// Token: 0x06003BD2 RID: 15314 RVA: 0x00160DB8 File Offset: 0x0015EFB8
		private Variant DecodeString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.json.Read();
			bool flag = true;
			while (flag)
			{
				if (this.json.Peek() == -1)
				{
					break;
				}
				char c = this.NextChar;
				if (c != '"')
				{
					if (c != '\\')
					{
						stringBuilder.Append(c);
					}
					else if (this.json.Peek() == -1)
					{
						flag = false;
					}
					else
					{
						c = this.NextChar;
						if (c <= '\\')
						{
							if (c == '"' || c == '/' || c == '\\')
							{
								stringBuilder.Append(c);
							}
						}
						else if (c <= 'f')
						{
							if (c != 'b')
							{
								if (c == 'f')
								{
									stringBuilder.Append('\f');
								}
							}
							else
							{
								stringBuilder.Append('\b');
							}
						}
						else if (c != 'n')
						{
							switch (c)
							{
							case 'r':
								stringBuilder.Append('\r');
								break;
							case 't':
								stringBuilder.Append('\t');
								break;
							case 'u':
							{
								StringBuilder stringBuilder2 = new StringBuilder();
								for (int i = 0; i < 4; i++)
								{
									stringBuilder2.Append(this.NextChar);
								}
								stringBuilder.Append((char)Convert.ToInt32(stringBuilder2.ToString(), 16));
								break;
							}
							}
						}
						else
						{
							stringBuilder.Append('\n');
						}
					}
				}
				else
				{
					flag = false;
				}
			}
			return new ProxyString(stringBuilder.ToString());
		}

		// Token: 0x06003BD3 RID: 15315 RVA: 0x00160F0F File Offset: 0x0015F10F
		private Variant DecodeNumber()
		{
			return new ProxyNumber(this.NextWord);
		}

		// Token: 0x06003BD4 RID: 15316 RVA: 0x00160F1C File Offset: 0x0015F11C
		private void ConsumeWhiteSpace()
		{
			while (" \t\n\r".IndexOf(this.PeekChar) != -1)
			{
				this.json.Read();
				if (this.json.Peek() == -1)
				{
					break;
				}
			}
		}

		// Token: 0x170004D3 RID: 1235
		// (get) Token: 0x06003BD5 RID: 15317 RVA: 0x00160F50 File Offset: 0x0015F150
		private char PeekChar
		{
			get
			{
				int num = this.json.Peek();
				if (num != -1)
				{
					return Convert.ToChar(num);
				}
				return '\0';
			}
		}

		// Token: 0x170004D4 RID: 1236
		// (get) Token: 0x06003BD6 RID: 15318 RVA: 0x00160F75 File Offset: 0x0015F175
		private char NextChar
		{
			get
			{
				return Convert.ToChar(this.json.Read());
			}
		}

		// Token: 0x170004D5 RID: 1237
		// (get) Token: 0x06003BD7 RID: 15319 RVA: 0x00160F88 File Offset: 0x0015F188
		private string NextWord
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				while (" \t\n\r{}[],:\"".IndexOf(this.PeekChar) == -1)
				{
					stringBuilder.Append(this.NextChar);
					if (this.json.Peek() == -1)
					{
						break;
					}
				}
				return stringBuilder.ToString();
			}
		}

		// Token: 0x170004D6 RID: 1238
		// (get) Token: 0x06003BD8 RID: 15320 RVA: 0x00160FD4 File Offset: 0x0015F1D4
		private Decoder.Token NextToken
		{
			get
			{
				this.ConsumeWhiteSpace();
				if (this.json.Peek() == -1)
				{
					return Decoder.Token.None;
				}
				char peekChar = this.PeekChar;
				if (peekChar <= '[')
				{
					switch (peekChar)
					{
					case '"':
						return Decoder.Token.String;
					case '#':
					case '$':
					case '%':
					case '&':
					case '\'':
					case '(':
					case ')':
					case '*':
					case '+':
					case '.':
					case '/':
						break;
					case ',':
						this.json.Read();
						return Decoder.Token.Comma;
					case '-':
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
						return Decoder.Token.Number;
					case ':':
						return Decoder.Token.Colon;
					default:
						if (peekChar == '[')
						{
							return Decoder.Token.OpenBracket;
						}
						break;
					}
				}
				else
				{
					if (peekChar == ']')
					{
						this.json.Read();
						return Decoder.Token.CloseBracket;
					}
					if (peekChar == '{')
					{
						return Decoder.Token.OpenBrace;
					}
					if (peekChar == '}')
					{
						this.json.Read();
						return Decoder.Token.CloseBrace;
					}
				}
				string nextWord = this.NextWord;
				if (nextWord == "false")
				{
					return Decoder.Token.False;
				}
				if (nextWord == "true")
				{
					return Decoder.Token.True;
				}
				if (!(nextWord == "null"))
				{
					return Decoder.Token.None;
				}
				return Decoder.Token.Null;
			}
		}

		// Token: 0x040036C3 RID: 14019
		private const string whiteSpace = " \t\n\r";

		// Token: 0x040036C4 RID: 14020
		private const string wordBreak = " \t\n\r{}[],:\"";

		// Token: 0x040036C5 RID: 14021
		private StringReader json;

		// Token: 0x02000EF7 RID: 3831
		private enum Token
		{
			// Token: 0x04004E22 RID: 20002
			None,
			// Token: 0x04004E23 RID: 20003
			OpenBrace,
			// Token: 0x04004E24 RID: 20004
			CloseBrace,
			// Token: 0x04004E25 RID: 20005
			OpenBracket,
			// Token: 0x04004E26 RID: 20006
			CloseBracket,
			// Token: 0x04004E27 RID: 20007
			Colon,
			// Token: 0x04004E28 RID: 20008
			Comma,
			// Token: 0x04004E29 RID: 20009
			String,
			// Token: 0x04004E2A RID: 20010
			Number,
			// Token: 0x04004E2B RID: 20011
			True,
			// Token: 0x04004E2C RID: 20012
			False,
			// Token: 0x04004E2D RID: 20013
			Null
		}
	}
}
