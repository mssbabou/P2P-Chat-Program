using System;

namespace P2PChatConsole
{
    class ConsoleManager
    {
        public static string Start(string[] menuItems)
        {
            int selectedItemIndex = 0;
            Console.CursorVisible = false; // hide the cursor
            var cursorPosition = Console.GetCursorPosition(); // save cursor position
            for (int i = 0; i < menuItems.Length; i++)
            {
                if (i == selectedItemIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                }
                Console.WriteLine(menuItems[i]);
                Console.ResetColor();
            }

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true); // read key without displaying it
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedItemIndex = Math.Max(0, selectedItemIndex - 1);
                        break;
                    case ConsoleKey.DownArrow:
                        selectedItemIndex = Math.Min(menuItems.Length - 1, selectedItemIndex + 1);
                        break;
                    case ConsoleKey.Enter:
                    case ConsoleKey.RightArrow:
                        Console.SetCursorPosition(0, menuItems.Length + 1 + cursorPosition.Top); // set cursor position to bottom left corner of menu
                        Console.CursorVisible = true; // show the cursor again
                        return menuItems[selectedItemIndex];
                }

                Console.SetCursorPosition(cursorPosition.Left, cursorPosition.Top); // set cursor position to top left corner of console window
                for (int i = 0; i < menuItems.Length; i++)
                {
                    if (i == selectedItemIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine(menuItems[i]);
                    Console.ResetColor();
                }
            }

            return "";
        }
    }
}