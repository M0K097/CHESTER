using System.Collections.Concurrent;
//------>>Logger<<-----//
// a simple class for logging in your code anywhere with timestamp
static public class Logger
{
		static int window_size = 10;
		static ConcurrentDictionary<string, ConcurrentQueue<string>> logs = new ConcurrentDictionary<string, ConcurrentQueue<string>>();


		public static void log(int information, string channel = "CLI")
		{
				if (!logs.ContainsKey(channel))
				{
						logs.TryAdd(channel, new ConcurrentQueue<string>());
				}

				var q1 = logs.GetOrAdd(channel, _ => new ConcurrentQueue<string>());
				q1.Enqueue($"log [{DateTime.UtcNow}]: {information}");

				var q2 = logs.GetOrAdd("MASTER", _ => new ConcurrentQueue<string>());
				q2.Enqueue($"log [{DateTime.UtcNow}]:[{channel}] > {information}");
		}

		public static void initiate()
		{
				logs.TryAdd("MASTER", new ConcurrentQueue<string>());
				log("logger is initiated");
		}

		public static void log(string information, string channel = "CLI")
		{

				if (!logs.ContainsKey(channel))
				{
						logs.TryAdd(channel, new ConcurrentQueue<string>());
				}
				var q1 = logs.GetOrAdd(channel, _ => new ConcurrentQueue<string>());
				q1.Enqueue($"log [{DateTime.UtcNow}]: {information}");

				var q2 = logs.GetOrAdd("MASTER", _ => new ConcurrentQueue<string>());
				q2.Enqueue($"log [{DateTime.UtcNow}]:[{channel}] > {information}");

		}

		public static List<string> list_channels()
		{
				List<string> channels = new List<string>();
				foreach (KeyValuePair<string, ConcurrentQueue<string>> entry in logs)
				{
						channels.Add(entry.Key);

				}
				return channels;
		}

		public static List<string> get_logs(string channel)
		{
				List<string> entries = logs[channel].ToList(); ;
				return entries;
		}


		public static void list_logs(string channel = "CLI")
		{
				int counter = 0;
				if (!logs.ContainsKey(channel))
				{
						Console.WriteLine($"channel:{channel} is empty");
				}
				else
				{
						List<string> entries = logs[channel].ToList();
						foreach (string log in entries.ToList())
						{

								if (counter > entries.Count() - window_size)
								{
										Console.WriteLine(log);
								}
								counter++;
						}
				}
		}
}

