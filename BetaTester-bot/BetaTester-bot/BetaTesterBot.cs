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
            //  comandoEliminar();
            ComandoHola();
            //comandoinfo();
            comandohelp();
            comandoclear();
            MensajeComParametro();
            //RegisterPurgeCommand();

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
                    await discord.Connect(Config.Token, TokenType.Bot);
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

        //private void comandoinfo()
        //{
        //    comando.CreateCommand("info").Do(async (e) =>
        //    {
        //        var user = e.User;

        //        await e.Channel.SendMessage(String.Format("Lo sentimos, por ahora no hay información del servidor"));
        //    });
        //}

        private void comandohelp()
        {
            comando.CreateCommand("help").Do(async (e) =>
            {
                var user = e.User;
               
                await e.Channel.SendMessage(String.Format("Ok, {0} Te enviare un mensaje privado", user.Mention));

                await e.User.SendMessage("[**LISTA DE COMANDOS**]"+ "\n\n" +
                                         "```• hola  : Saludo del Bot " + "\n" +
                                         "• help  : Informacion del bot" + "\n" +
                                         "• nota  : Enviar una nota a travez del bot" + "\n"+
                                         "• clear : Limpiar mensajes del canal" + "\n" +
                                         "• bienvenida y despedida del usuario al server activados ```" + "\n\n" +
                                         //"**Soporte Bot**" + "\n" +
                                         //"Github: github.com/antonventuro/BetaTester-Bot-Discord" + "\n\n" +
                                         "**Social**:" +"\n"+
                                         "Trello: https://trello.com/b/j4dy8b9v/comunidad-gamedevs-hispanos" + "\n" +
                                         "Twitter: -" + "\n" +
                                         "Youtube: -" + "\n\n" +
                                         "**Discord Oficial**" + "\n" +
                                         "Comunidad dedicada a los desarrolladores hispanohablantes, únase al servidor: https://discord.gg/mazXpvp");



            });
        }

        private void comandoclear()
        {
            comando.CreateCommand("clear")
            .Parameter("cantidad", ParameterType.Required)
            .Do(async (e) =>
            {

                var userRoles = e.User.Roles;
                if (userRoles.Any(input => (input.Name.ToUpper() == "ADMINS")))
                {
                    int cantidad = Int32.Parse(e.Args[0]);
                    Message[] mensajeAeliminar;
                    while (cantidad > 0)
                    {
                        if (cantidad < 500)
                        {
                            mensajeAeliminar = await e.Channel.DownloadMessages(cantidad);
                            cantidad = 0;
                        }
                        else
                        {
                            mensajeAeliminar = await e.Channel.DownloadMessages(100);
                            cantidad -= 500;
                        }
                        await e.Channel.DeleteMessages(mensajeAeliminar);
                        await Task.Delay(5000);
                    }
                }
                else
                {
                    
                    await e.Channel.SendMessage("No tienes permiso para usar el comando `clear`");
                   
                }



            });
        }

        private void Bienvenida()
        {
            discord.UserJoined += async (s, e) =>
            {
                var canal = e.Server.FindChannels("general", ChannelType.Text).FirstOrDefault();
                var user = e.User;
                await canal.SendMessage(String.Format(":inbox_tray: {0}, se ha unido a **GameDevs** :smile: :wave: ", user.Mention));
            };
        }

        private void Despedida()
        {
            discord.UserLeft += async (s, e) =>
            {
                var canal = e.Server.FindChannels("general", ChannelType.Text).FirstOrDefault();
                var user = e.User;
                await canal.SendMessage(String.Format(":outbox_tray: {0}, se ha ido de **GameDevs** :slight_frown: :wave:", user.Mention));
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
