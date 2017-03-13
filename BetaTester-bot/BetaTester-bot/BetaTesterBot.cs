using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Modules;


namespace BetaTester_bot
{
    class BetaTesterBot
    {

        DiscordClient discord;
        CommandService comando;
       

        public BetaTesterBot()
        {



            ConexionBotClient();
            PrefixBot();
            comando = discord.GetService<CommandService>();

            //--------------------------------------------
            ComandoHola();
            comandoinfo();
            MensajeComParametro();
           

            //--------------------------------------------
            Bienvenida();
            Despedida();
            ConexionBotToken();

        }

        public void ConexionBotToken()
        {
            try
            {
                discord.ExecuteAndWait(async () =>
                {
                    await discord.Connect("MjkwODk5Nzc2Njg0NDI1MjE3.C6hpHw.NbAbt2v0m24HU79tFMffm0fmpCs", TokenType.Bot);
                });
            }
            catch
            {

            }
        }

        public void ConexionBotClient()
        {
            discord = new DiscordClient(x =>
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });
        }


        public void PrefixBot()
        {
            discord.UsingCommands(x =>
            {
                x.PrefixChar = '!';
                x.AllowMentionPrefix = true;
            });
        }


        //---------------------------------------------------

        private void ComandoHola()
        {
            comando.CreateCommand("Hola").Do(async (e) =>
            {
                var user = e.User;

                await e.Channel.SendMessage(String.Format("Hola! {0} ¿Cómo estás?, Espero que bien. :slight_smile::v:", user.Mention));
            });
        }

        private void comandoinfo()
        {
            comando.CreateCommand("info").Do(async (e) =>
            {
                var user = e.User;

                await e.Channel.SendMessage(String.Format("Lo sentimos, por ahora no hay información del servidor"));
            });
        }

        private void Bienvenida()
        {
            discord.UserJoined += async (s, e) =>
            {
                var canal = e.Server.FindChannels("general", ChannelType.Text).FirstOrDefault();
                var user = e.User;
                await canal.SendMessage(String.Format("{0} se ha unido al servidor", user.Mention));
            };
        }

        private void Despedida()
        {
            discord.UserLeft += async (s, e) =>
            {
                var canal = e.Server.FindChannels("general", ChannelType.Text).FirstOrDefault();
                var user = e.User;
                await canal.SendMessage(String.Format("{0} ha salido del servidor", user.Mention));
            };
        }
        private void MensajeComParametro()
        {

            comando.CreateCommand("nota").Parameter("mensaje", ParameterType.Multiple).Do(async (e) =>
            {
                await Donota(e);
            });

        }


        private async Task Donota(CommandEventArgs e)
        {
            var canal = e.Server.FindChannels(e.Args[0], ChannelType.Text).FirstOrDefault();

            var mensaje = ConstructorMensaje(e, canal != null);

            if (canal != null)
            {
                await canal.SendMessage(mensaje);
            }
            else
            {
                await e.Channel.SendMessage(mensaje);
            }
        }

        private string ConstructorMensaje(CommandEventArgs e, bool primerArg)
        {
            string mensaje = "";
            var nombre = e.User.Nickname != null ? e.User.Nickname : e.User.Name;
            var startIndex = primerArg ? 1 : 0;

            for (int i = startIndex; i < e.Args.Length; i++)
            {
                mensaje += e.Args[i].ToString() + " ";
            }

            var result = nombre + " Dice: " + mensaje;

            return result;
        }

        //-------------------------------------------------------

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);

        }
    }
}
