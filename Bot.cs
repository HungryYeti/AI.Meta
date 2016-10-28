using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Modules;
using Discord.Commands;

namespace AI.Meta
{
	class Bot
	{
		static void Main ( string[] args ) { new Bot ().Start ( args ); }

		private DiscordClient m_client;

		private void Start ( string[] args )
		{
			if ( m_client != null )
			{
				throw new Exception ( "Bot is already started" );
			}

			// Load config file.
			Global.LoadConfig ();

			// Create client.
			m_client = new DiscordClient ( ( conf ) =>
			{
				conf.MessageCacheSize = 0;
				conf.UsePermissionsCache = true;
				conf.EnablePreUpdateEvents  = true;
				conf.LogLevel = LogSeverity.Info;
				conf.LogHandler = OnLogHandler;
			} )

			// Setup commands & modules.
			.UsingCommands ( ( conf ) =>
			{
				conf.AllowMentionPrefix = true;
				conf.HelpMode = HelpMode.Public;
				conf.ExecuteHandler = OnCommandExecuted;
				conf.ErrorHandler = OnErrorHandler;
			} )
			.UsingModules ();

			// Add modules.
			m_client.AddModule<Modules.GithubLog> ( "Github Logger", ModuleFilter.None );

			// Start the client.
			m_client.ExecuteAndWait ( async () =>
			{
				while ( true )
				{
					try
					{
						await m_client.Connect ( Global.Config.Token, TokenType.Bot );
						m_client.SetGame ( Global.Config.Game );

						break;
					}
					catch ( Exception ex )
					{
						m_client.Log.Error ( "Login failed", ex );
						await Task.Delay ( m_client.Config.FailedReconnectDelay );
					}
				}
			} );
		}

		private void OnLogHandler ( object sender, LogMessageEventArgs e )
		{
			Console.WriteLine ( "DN[{0}] {1}: {2}", e.Severity, e.Source, e.Message );
			if ( e.Exception != null )
			{
				Console.WriteLine ( "Something Broke:" );
				Console.WriteLine ( e.Exception );
			}
		}

		private void OnCommandExecuted ( object sender, CommandEventArgs e )
		{
			Console.WriteLine ( $"{DateTime.Now.ToShortTimeString ()} {e.Command.Text} called by {e.User.ToString ()}" );
		}

		private void OnErrorHandler ( object sender, CommandErrorEventArgs e )
		{
			string msg = e.Exception?.GetBaseException ().Message;

			// No exception, show generic message.
			if ( msg == null )
			{
				switch ( e.ErrorType )
				{
					case ( CommandErrorType.Exception ):
						msg = $"{e.User.Mention} ???";
						break;
					case ( CommandErrorType.BadPermissions ):
						msg = $"{e.User.Mention} access denied.";
						break;
					case ( CommandErrorType.BadArgCount ):
						msg = $"{e.User.Mention} wrong no# of arguments.";
						break;

					default:
						msg = null;
						break;
				}

				if ( msg != null )
				{
					e.Channel.SendMessage ( msg );
				}
			}
			else
			{
				e.Channel.SendMessage ( $"{e.User.Mention} 500 internal error, check the logs." );
				m_client.Log.Error ( "Command", e.Exception );
			}
		}
	}
}
