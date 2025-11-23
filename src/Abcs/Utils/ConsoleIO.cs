using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Abcs.Utils;

public static class ConsoleIO
{
	public const string SINGLE_BORDER_PARTS = "─│┌┐└┘";
	public const string DOUBLE_BORDER_PARTS = "═║╔╗╚╝";

	public const string SINGLE_BORDER_PARTS_WITH_CONNECTORS = "─│┌┐└┘┴┬┤├";
	public const string DOUBLE_BORDER_PARTS_WITH_CONNECTORS = "═║╔╗╚╝╩╦╣╠";

	private static int rows = 25;
	private static int cols = 80;
	private static int layers = 1;

	private static int cursorRow = 0;
	private static int cursorCol = 0;
	private static int cursorLayer = 0;

	private static string[,,] buffer = new string[layers, rows, cols];

	private static bool autoFlush = true;

	public static void SetAutoFlush(bool b) { autoFlush = b; }

	public static void SetBuffer(int rows = -1, int cols = -1, int layers = -1)
	{
		ConsoleIO.cols = (cols < 0) ? ConsoleIO.cols : cols;
		ConsoleIO.rows = (rows < 0) ? ConsoleIO.rows : rows;
		ConsoleIO.layers = (layers < 0) ? ConsoleIO.layers : layers;
		buffer = new string[ConsoleIO.layers, ConsoleIO.rows, ConsoleIO.cols];
	}

	public static void SetCols(int cols)
	{
		ConsoleIO.cols = (cols < 0) ? ConsoleIO.cols : cols;
		buffer = new string[ConsoleIO.layers, ConsoleIO.rows, ConsoleIO.cols];
	}

	public static void SetRows(int rows)
	{
		ConsoleIO.rows = (rows < 0) ? ConsoleIO.rows : rows;
		buffer = new string[ConsoleIO.layers, ConsoleIO.rows, ConsoleIO.cols];
	}

	public static void SetLayers(int layers)
	{
		ConsoleIO.layers = (layers < 0) ? ConsoleIO.layers : layers;
		buffer = new string[ConsoleIO.layers, ConsoleIO.rows, ConsoleIO.cols];
	}

	public static void SetCursor(int row = -1, int col = -1, int layer = -1)
	{
		cursorRow = (row < 0) ? cursorRow : row;
		cursorCol = (col < 0) ? cursorCol : col;
		cursorLayer = (layer < 0) ? cursorLayer : layer;
	}

	public static void SetCursorRow(int row)
	{
		cursorRow = row;
	}

	public static void SetCursorCol(int col)
	{
		cursorCol = col;
	}

	public static void SetCursorLayer(int layer)
	{
		cursorLayer = layer;
	}

	public static string[] GetBorder(string borderParts, ConsoleColor? fg, ConsoleColor? bg)
	{
		string[] border = new string[10];

		if(string.IsNullOrEmpty(borderParts))
		{
			Array.Fill(border, Color(string.Empty, fg, bg));
		}
		else if(borderParts.Length == 1)
		{
			Array.Fill(border, Color(borderParts, fg, bg));
		}
		else //if(borderParts.Length >= 2)
		{
			string hl = Color(borderParts[0], fg, bg);
			string vl = Color(borderParts[1], fg, bg);

			Array.Fill(border, hl);
			border[1] = vl;
			border[8] = vl;
			border[9] = vl;

			if(borderParts.Length >= 6)
			{
				border[2] = Color(borderParts[2], fg, bg);
				border[3] = Color(borderParts[3], fg, bg);
				border[4] = Color(borderParts[4], fg, bg);
				border[5] = Color(borderParts[5], fg, bg);
			}

			if(borderParts.Length >= 10)
			{
				border[6] = Color(borderParts[6], fg, bg);
				border[7] = Color(borderParts[7], fg, bg);
				border[8] = Color(borderParts[8], fg, bg);
				border[9] = Color(borderParts[9], fg, bg);
			}
		}

		return border;
	}

	private static void FillBox(int t, int l, int b, int r, int layer, ConsoleColor? fg, ConsoleColor? bg, string f = " ")
	{
		for(int i = t; i <= b; i++)
		{
			for(int j = l; j <= r; j++)
			{
				buffer[layer, i, j] = Color(f, fg, bg);
			}
		}
	}

	public static void Clear(int layer = -1)
	{
		if(layer < 0) { layer = cursorLayer; }

		FillBox(0, 0, rows - 1, cols - 1, layer, null, null, (layer == 0) ? " " : "");

		if(autoFlush) { Flush(); }
	}

	public static void ClearAll()
	{
		for(int layer = 0; layer < layers; layer++) { Clear(layer); }
	}

	private static void DrawBorder(int t, int l, int b, int r, int c, int m, int layer, ConsoleColor? fg, ConsoleColor? bg, string borderParts)
	{
		string[] border = GetBorder(borderParts, fg, bg);

		for(int i = l + 1; i < r; i++)
		{
			buffer[layer, t, i] = border[0];
			buffer[layer, b, i] = border[0];
		}

		for(int i = t + 1; i < b; i++)
		{
			buffer[layer, i, l] = border[1];
			buffer[layer, i, r] = border[1];
		}

		buffer[layer, t, l] = border[2];
		buffer[layer, t, r] = border[3];
		buffer[layer, b, l] = border[4];
		buffer[layer, b, r] = border[5];
		buffer[layer, t, c] = border[6];
		buffer[layer, b, c] = border[7];
		buffer[layer, m, l] = border[8];
		buffer[layer, m, r] = border[9];
	}

	public static List<string> Wrap(string text, int boxCols)
	{
		int margin = (boxCols < 0) ? cols - 2 : Math.Min(boxCols, cols - 2);
		int lastIndex = 0;
		int colCount = 0;
		var lines = new List<string>();

		for(int i = 0; i < text.Length; i++)
		{
			if(text[i] == '\n') { lines.Add(text.Substring(lastIndex, i - lastIndex)); lastIndex = i + 1; colCount = 0; continue; }
			colCount++;
			if(colCount >= margin) { lines.Add(text.Substring(lastIndex, i + 1 - lastIndex)); lastIndex = i + 1; colCount = 0; }
		}

		if (lastIndex < text.Length) { lines.Add(text.Substring(lastIndex)); }

		return lines;
	}

	public static string ConsoleColorToAnsiForeground(ConsoleColor? color)
	{
		switch(color)
		{
			case ConsoleColor.Black: return "\x1b[30m";
			case ConsoleColor.DarkRed: return "\x1b[31m";
			case ConsoleColor.DarkGreen: return "\x1b[32m";
			case ConsoleColor.DarkYellow: return "\x1b[33m";
			case ConsoleColor.DarkBlue: return "\x1b[34m";
			case ConsoleColor.DarkMagenta: return "\x1b[35m";
			case ConsoleColor.DarkCyan: return "\x1b[36m";
			case ConsoleColor.Gray: return "\x1b[37m";

			case ConsoleColor.DarkGray: return "\x1b[90m";
			case ConsoleColor.Red: return "\x1b[91m";
			case ConsoleColor.Green: return "\x1b[92m";
			case ConsoleColor.Yellow: return "\x1b[93m";
			case ConsoleColor.Blue: return "\x1b[94m";
			case ConsoleColor.Magenta: return "\x1b[95m";
			case ConsoleColor.Cyan: return "\x1b[96m";
			case ConsoleColor.White: return "\x1b[97m";

			default:
				return "\x1b[0m";  // RESET on any non-match
		}
	}

	public static string ConsoleColorToAnsiBackground(ConsoleColor? color)
	{
		switch(color)
		{
			case ConsoleColor.Black: return "\x1b[40m";
			case ConsoleColor.DarkRed: return "\x1b[41m";
			case ConsoleColor.DarkGreen: return "\x1b[42m";
			case ConsoleColor.DarkYellow: return "\x1b[43m";
			case ConsoleColor.DarkBlue: return "\x1b[44m";
			case ConsoleColor.DarkMagenta: return "\x1b[45m";
			case ConsoleColor.DarkCyan: return "\x1b[46m";
			case ConsoleColor.Gray: return "\x1b[47m";

			case ConsoleColor.DarkGray: return "\x1b[100m";
			case ConsoleColor.Red: return "\x1b[101m";
			case ConsoleColor.Green: return "\x1b[102m";
			case ConsoleColor.Yellow: return "\x1b[103m";
			case ConsoleColor.Blue: return "\x1b[104m";
			case ConsoleColor.Magenta: return "\x1b[105m";
			case ConsoleColor.Cyan: return "\x1b[106m";
			case ConsoleColor.White: return "\x1b[107m";

			default:
				return "\x1b[0m";  // RESET on any non-match
		}
	}

	public static string Color(char ch, ConsoleColor? fg = null, ConsoleColor? bg = null)
	{
		string fgc = ConsoleColorToAnsiForeground(fg);
		string bgc = ConsoleColorToAnsiBackground(bg);

		return fgc + bgc + ch + "\x1b[0m";
	}

	public static string Color(string text, ConsoleColor? fg = null, ConsoleColor? bg = null)
	{
		string fgc = ConsoleColorToAnsiForeground(fg);
		string bgc = ConsoleColorToAnsiBackground(bg);

		return fgc + bgc + text + "\x1b[0m";
	}

	public static void Write(string text, int row = -1, int col = -1, int layer = -1, ConsoleColor? fg = default, ConsoleColor? bg = default)
	{
		text = text.Replace("\r", "");

		if(row < 0) { row = cursorRow; }
		if(col < 0) { col = cursorCol; }
		if(layer < 0) { layer = cursorLayer; }

		cursorRow = row;
		cursorCol = col;
		cursorLayer = layer;

		for(int i = 0; i < text.Length; i++)
		{
			char c = text[i];

			if(c == '\n') { cursorRow++; cursorCol = 0; continue; }
			if(cursorCol >= cols) { cursorRow++; cursorCol = 0; }

			buffer[layer, cursorRow, cursorCol++] = Color(c, fg, bg);
		}

		if(autoFlush) { Flush(); }
	}

	public static void Box(string text, int boxRows = -1, int boxCols = -1, int layer = -1, ConsoleColor? fg = default, ConsoleColor? bg = default, float hAlign = 0.5F, float vAlign = 0.5F, string borderParts = SINGLE_BORDER_PARTS)
	{
		text = text.Replace("\r", "");

		var lines = Wrap(text, boxCols);
		int maxRowCount = (boxRows > 0) ? boxRows : lines.Count;
		int maxColCount = (boxCols > 0) ? boxCols : lines.Max(l => l.Length);

		if(maxRowCount > rows) { SetRows(maxRowCount); }
		if(layer < 0) { layer = layers - 1; }

		cursorLayer = layer;

		int t = (int) ((rows - maxRowCount - 2) * vAlign);
		int l = (int) ((cols - maxColCount - 2) * hAlign);
		int b = t + maxRowCount + 1;
		int r = l + maxColCount + 1;
		int c = (l + r) / 2;
		int m = (t + b) / 2;

		FillBox(t, l, b, r, layer, fg, bg, " ");
		DrawBorder(t, l, b, r, c, m, layer, fg, bg, borderParts);

		for(int i = 0; i < lines.Count; i++)
		{
			Write(lines[i], t + 1 + i, l + 1, layer, fg, bg);
		}

		if(autoFlush) { Flush(); }
	}

	public static T Read<T>(string prompt, int row = -1, int col = -1, int layer = -1, ConsoleColor? fg = default, ConsoleColor? bg = default)
	{
		while(true)
		{
			try
			{
				Write(prompt, row, col, layer, fg, bg);
				Flush();

				Console.SetCursorPosition(cursorCol, cursorRow);
				string input = Console.ReadLine()!;
				Console.SetCursorPosition(0, rows);

				Type t = typeof(T);

				if(t.IsEnum)
				{
					return (T) Enum.Parse(t, input, true);
				}
				else if(typeof(IConvertible).IsAssignableFrom(t))
				{
					return (T) Convert.ChangeType(input, t);
				}
				else
				{
					break;
				}
			}
			catch
			{

			}
		}

		throw new InvalidCastException($"Type {typeof(T).FullName} cannot be parsed from a string.");
	}

	public static T Prompt<T>(string prompt, int boxRows = -1, int boxCols = -1, int layer = -1, ConsoleColor? fg = default, ConsoleColor? bg = default, float hAlign = 0.5F, float vAlign = 0.5F, string borderParts = SINGLE_BORDER_PARTS)
	{
		Box(prompt, boxRows, boxCols, layer, fg, bg, hAlign, vAlign, borderParts);
		Flush();
		return Read<T>(string.Empty, -1, -1, layer, fg, bg);
	}

	public static void Flush()
	{
		// These operations are only supported on Windows; guard the call.

		if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			Console.SetWindowSize(cols + 1, rows + 1);
			Console.SetBufferSize(cols + 1, rows + 1);
		}

		Console.Clear();
		Flush(buffer);
	}

	private static void Flush(string[,,] buffer)
	{
		for(int l = 0; l < layers; l++)
		{
			Console.SetCursorPosition(0, 0);

			for(int r = 0; r < rows; r++)
			{
				for(int c = 0; c < cols; c++)
				{
					string s = buffer[l, r, c];

					if(IsAnsiNullOrEmpty(s))
					{
						Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);
					}
					else
					{
						Console.Write(s);
					}
				}

				Console.WriteLine();
			}
		}
	}

	private static readonly Regex _ansiRegex =
			new Regex(@"\x1b\[[0-9;]*[A-Za-z]", RegexOptions.Compiled);

	public static bool IsAnsiNullOrEmpty(string? s)
	{
		if(string.IsNullOrEmpty(s))
			return true;

		string stripped = _ansiRegex.Replace(s, "");

		foreach(char c in stripped) { if(c != '\0') { return false; } }

		return true;
	}
}
