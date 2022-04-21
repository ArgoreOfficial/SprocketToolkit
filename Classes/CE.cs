using System;

namespace SprocketToolkit.Classes
{
    public static class CE
    {
        public static void Write(string text, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
        }

        public static void Alert(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(text);
            Console.Beep(800, 100);
            Console.Beep(700, 100);
        }

        public static void Line()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("------------------------------------\n");
        }

    }
}
