using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Modules;

namespace AI.Meta.Modules
{
	class GithubLog : IModule
	{
		void IModule.Install ( ModuleManager manager )
		{
			manager.MessageReceived += ( s, e ) =>
			{
				if ( !e.User.IsBot )
				{
					if ( e.Channel.Id == Global.Config.GithubChannelId )
					{
						e.Channel.SendMessage ( "Test" );
					}
				}
			};
		}
	}
}
