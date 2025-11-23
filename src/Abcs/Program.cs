using Abcs.Utils;

namespace Abcs;

public class Program
{
	public static void Main()
	{
		string text = "Hello World!";
		int boxRows = -1;
		int boxCols = -1;
		int layer = -1;
		ConsoleIO.SetBuffer(25, 80, 2);
		ConsoleIO.SetAutoFlush(false);
		ConsoleIO.Write("This is a text");
		ConsoleIO.Box(text, boxRows, boxCols, layer, ConsoleColor.White, ConsoleColor.Blue, 0.0F, 0.0F, borderParts:"#");
		ConsoleIO.ClearAll();
		ConsoleIO.Flush();
	}
}
