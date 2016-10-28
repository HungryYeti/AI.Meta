using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AI.Meta
{
	public static class Global
	{
		public static Config Config { get; set; }

		public static void LoadConfig ()
		{
			try
			{
				var file = File.ReadAllText ( "config.json" );
				Config = JsonConvert.DeserializeObject<Config> ( file );
			}
			catch ( Exception ex )
			{
				Console.WriteLine ( $"Error loading conf.json: {ex.Message}" );
			}

			if ( Config == null )
			{
				Config = new Config ();
			}
		}
	}
}
