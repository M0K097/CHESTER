using static Logger;
//------->>>Commandline_User_Interface<<--------------//
static public class CLU_UI
{
		static List<string> options = new List<string>();
		static List<string> channels = new List<string>();
		static int selected = 0;
		static int window_size = 5;
		static int selected_log = 0;

		static void show_options(List<string> options)
		{
				int window_start = (selected - window_size);
				int window_end = (selected + window_size);
				channels = list_channels();
				var copy_options = new List<string>(options);
				string? max_string = copy_options.Max();

				if (window_start < 0)
				{
						window_end = window_end + Math.Abs(window_start);
						window_start = 0;
				}
				if (window_end > options.Count())
				{
						if (window_start - (window_end - options.Count()) > 0)
						{
								window_start = window_start - (window_end - options.Count());

						}
						window_end = options.Count();
				}

				for (int i = window_start; i < window_end; i++)
				{
						if (i == selected && max_string != null)
						{
								Console.ForegroundColor = ConsoleColor.Green;
								Console.Write("  > ");
								Console.WriteLine($"{options[i]}");
								Console.ResetColor();
						}
						else
						{
								Console.Write("       ");
								Console.WriteLine(options[i]);
						}

				}
				string selected_channel = (channels[selected_log]);
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine();
				Console.WriteLine("-----------------< LOGS >------------------");
				Console.ResetColor();
				foreach (string channel in channels)
				{
						if (channel == selected_channel)
						{
								Console.ForegroundColor = ConsoleColor.Green;
								Console.Write($"[ {channel} ]");
								Console.ResetColor();
						}
						else
						{
								Console.Write(channel);
						}
						Console.Write("  |  ");
				}
				Console.WriteLine();
				Console.ResetColor();
				list_logs(selected_channel);
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("________________[ CONSOLE ]________________");
				Console.ResetColor();
		}

		static public string run(List<string> options, string menu_name)
		{
				selected = 0;
				bool selecting = true;
				string choosen = "";
				do
				{
						Console.Clear();

						Console.ForegroundColor = ConsoleColor.Green;
						Console.WriteLine($"            <[ {menu_name}: {selected + 1}/{options.Count()} ]>");
						Console.WriteLine("-------------------------------------------");
						Console.WriteLine();
						Console.ResetColor();

						show_options(options);
						ConsoleKeyInfo input = Console.ReadKey(true);
						if (input.Key == ConsoleKey.UpArrow)
						{
								selected--;
						}
						else if (input.Key == ConsoleKey.DownArrow)
						{
								selected++;
						}
						else if (input.Key == ConsoleKey.Enter)
						{
								choosen = options[selected];
								selecting = false;
						}
						else if (input.Key == ConsoleKey.LeftArrow)
						{
								selected_log--;
								if (selected_log < 0)
								{
										selected_log = channels.Count() - 1;
								}
						}
						else if (input.Key == ConsoleKey.RightArrow)
						{
								selected_log++;
								if (selected_log > channels.Count() - 1)
								{
										selected_log = 0;
								}
						}
						//end menu
						if (input.Key == ConsoleKey.Backspace)
						{
								choosen = "return";
								break;
						}

						//keep marker on list
						if (selected < 0)
						{
								selected = options.Count() - 1;
						}
						else if (selected > options.Count() - 1)
						{
								selected = 0;
						}

				}
				while (selecting);

				return choosen;
		}

}
