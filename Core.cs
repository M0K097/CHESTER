//Copyright (c) 2025 Moritz Kolb
//Licensed for non-commercial use only. See LICENSE file for details.
//This project is for educational and personal use only.
//It does not provide financial advice, trading advice, or investment //recommendations.
//All trading involves risk. You are responsible for your own decisions.
using static Core_Utilities;
using static Logger;
//---------------[MAIN MENU]---------------------->>

Console.WriteLine($"booting up  Chester...");
Logger.initiate();
log($"Chester is ready");

//========> PARENT!!!!


while (true)
{
		Console.Clear();
		List<string> main_menu = new List<string> { "DOWNLOAD_DATA", "BACKTEST", "CREATE NEW INSTANCE", "MONITOR INSTANCES", "STOP INSTANCES", "LOGS" };
		string selected_main_option = CLU_UI.run(main_menu, "MAIN MENU");


		switch (selected_main_option)
		{
				case ("DOWNLOAD_DATA"):
						await download_data();
						break;

				case ("BACKTEST"):
						do_backtest();
						break;

				case ("CREATE NEW INSTANCE"):
						Instance_Manager.start_instance();
						break;

				case ("STOP INSTANCES"):
						await Instance_Manager.stop_instance();
						break;
				case ("MONITOR INSTANCES"):
						monitor_instances();
						break;
				case ("LOGS"):
					 	select_log();
						break;
		}

}

