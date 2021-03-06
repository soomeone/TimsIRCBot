using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ChatSharp;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;





namespace Tim_s_IRC_bot
{
    class Program
    {

        static void Main(string[] args)
        {


            {

                Console.Title = "Tim's IRC Bot";

                if (!File.Exists(Path.Combine(combine, "TimsBot/settings.ini")))
                {
                    Console.Title = "Tim's IRC Bot Setup";
                    Console.WriteLine("Welcome to Tims IRC bot, press enter to continue");
                    Console.ReadLine();
                    Console.Clear();
                    Console.WriteLine("Which server do you want the bot to join?");
                    string Server = Console.ReadLine();
                    Console.Clear();
                    Console.WriteLine("Which channel do you want the bot to join?");
                    string Channel = Console.ReadLine();
                    Console.Clear();
                    Console.WriteLine("Which nickname do you want to have?");

                    string Nickname = Console.ReadLine();
                    Console.Clear();
                    Console.WriteLine("Which username do you have?(NEEDED!)");
                    string username = Console.ReadLine();
                    Console.Clear();
                    Console.WriteLine("Which password do you have for the account?(NEEDED!)");
                    string password = Console.ReadLine();
                    Console.WriteLine("What is your username on the irc server? Then you have access to all the admin commands.");
                    string Owner = Console.ReadLine();
                    Console.Clear();
                    CreateDir("TimsBot/");
                    CreateDir("TimsBot/data");
                    CreateDir("TimsBot/data/logs/");
                    CreateDir("TimsBot/data/systems/warnsystem/warning/");
                    CreateDir("TimsBot/data/systems/warnsystem/offtopic/");
                    CreateDir("TimsBot/data/access/");
                    CreateDir("TimsBot/data/access/commands/");
                    CreateDir("TimsBot/data/access/commands/owner/");
                    CreateDir("TimsBot/data/access/commands/users/");
                    WriteAllText("TimsBot/settings.ini", "Server=" + Server + Environment.NewLine + "Channel=" + Channel + Environment.NewLine + "Botname=" + Nickname + Environment.NewLine + "username=" + username + Environment.NewLine + "password=" + password);
                    File.Copy(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, (Path.Combine(combine, "TimsBot/TimsBot.exe")));
                    WriteAllText("TimsBot/data/logs/log.txt", "This is the log File" + Environment.NewLine);
                    WriteAllText("TimsBot/data/access/commands/owner/" + Owner, "Owner");
                    Console.Clear();
                    Console.WriteLine("You can open the ControlPanel by right clicking on the new system tray icon, when you run the program.");
                    Console.ReadLine();

                    Console.WriteLine("Install successfull, you can open the program by going to the desktop and click on the shortcut or navigate to TimsBot/ and run TimsBot.exe.");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
                try
                {



                    string readvalue = File.ReadAllText((Path.Combine(combine, "TimsBot/settings.ini")));

                    string modify1 = readvalue.Replace("Server=", "").Replace("Channel=", "").Replace("Botname=", "").Replace("username=", "").Replace("password=", "");

                    string[] splitvalue = modify1.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);


                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Clear();
                    Console.Title = "Tim's IRC Bot is connecting";
                    Console.WriteLine("Connecting to the server.");
                    var client = new IrcClient(splitvalue[0], new IrcUser(splitvalue[2], splitvalue[3], splitvalue[4]));
                    client.ConnectionComplete += (s, e) =>
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("Connected to the server.");
                        client.JoinChannel(splitvalue[1]);
                        Console.Title = "Tim's IRC Bot is connected";
                        Console.WriteLine("Joining channel.");
                        Console.ResetColor();
                        Console.Clear();

                    };
                    Console.ResetColor();
                    client.PrivateMessageRecieved += (s, f) =>
                    {
                        if (f.PrivateMessage.Message == "!commands")
                        {
                            if (File.Exists(Path.Combine(combine, "TimsBot/data/access/commands/owner/" + f.PrivateMessage.Source)))
                            {
                                client.SendMessage("Owner Commands + syntax: !add_mod (nick), !say (channel/nick) (message) ,", f.PrivateMessage.Source);
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.Write("[" + DateTime.Now.ToString("HH:mm:ss") + "]");
                                Console.ResetColor();
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("!commands requested by ");
                                Console.ResetColor();
                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.Write("[OWNER]" + f.PrivateMessage.Source + Environment.NewLine);
                            }
                            else if (File.Exists(Path.Combine(combine, "TimsBot/data/access/commands/users/" + f.PrivateMessage.Source)))
                            {
                                client.SendMessage("Mod commands are the !commands commands.", f.PrivateMessage.Source);
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.Write("[" + DateTime.Now.ToString("HH:mm:ss") + "]");
                                Console.ResetColor();
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("!commands requested by ");
                                Console.ResetColor();
                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.Write("[MOD]" + f.PrivateMessage.Source + Environment.NewLine);
                            }



                        }
                        if (f.PrivateMessage.Message.StartsWith("!add_mod "))
                        {
                            if (File.Exists(Path.Combine(combine, "TimsBot/data/access/commands/owner/" + f.PrivateMessage.Source)))
                            {
                                File.Create((Path.Combine(combine, "TimsBot/data/access/commands/users/" + f.PrivateMessage.Message.Substring(9))));
                            }
                        }
                    };

                    client.ChannelMessageRecieved += (s, e) =>
                    {

                        var channel = client.Channels[e.PrivateMessage.Source];

                        /*if (e.PrivateMessage.Message == "!list")
                            channel.SendMessage(string.Join(", ", channel.Users.Select(u => u.Nick)));*/

                        String[] Messageargs = e.PrivateMessage.Message.Split(' ');

                        switch (Messageargs[0])
                        {
                            case "!ban":
                                if (File.Exists((Path.Combine(combine, "TimsBot/data/access/commands/users/")) + e.PrivateMessage.User.Nick))
                                {
                                    var target = e.PrivateMessage.Message.Substring(5);
                                    client.WhoIs(target, whois => channel.ChangeMode("+b *!*@" + whois.User.Hostname));
                                    channel.Kick(e.PrivateMessage.Message.Substring(5));
                                    client.SendMessage("You got banned from " + splitvalue[1], e.PrivateMessage.Message.Substring(5));
                                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                                    Console.Write("[" + DateTime.Now.ToString("HH:mm:ss") + "]");
                                    Console.ResetColor();
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write(e.PrivateMessage.Message.Substring(5) + " is banned in " + splitvalue[1] + " by ");
                                    Console.ResetColor();
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    Console.Write(e.PrivateMessage.User.Nick + Environment.NewLine);
                                    channel.Kick(e.PrivateMessage.Message.Substring(5));
                                }


                                break;
                            case "!unban":
                                if (File.Exists((Path.Combine(combine, "TimsBot/data/access/commands/users/")) + e.PrivateMessage.User.Nick))
                                {

                                    var target = e.PrivateMessage.Message.Substring(7);
                                    client.WhoIs(target, whois => channel.ChangeMode("-b *!*@" + whois.User.Hostname));
                                    Console.WriteLine(e.PrivateMessage.Message.Substring(7) + " " + "is unbanned");
                                    client.SendMessage("You got unbanned from " + splitvalue[1], e.PrivateMessage.Message.Substring(7));
                                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                                    Console.Write("[" + DateTime.Now.ToString("HH:mm:ss") + "]");
                                    Console.ResetColor();
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write(e.PrivateMessage.Message.Substring(7) + " is unbanned in " + splitvalue[1] + " by ");
                                    Console.ResetColor();
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    Console.Write(e.PrivateMessage.User.Nick + Environment.NewLine);

                                }

                                break;
                            case "!leave":
                                if (File.Exists((Path.Combine(combine, "TimsBot/data/access/commands/users/")) + e.PrivateMessage.User.Nick))
                                {
                                    channel.Part();
                                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                                    Console.Write("[" + DateTime.Now.ToString("HH:mm:ss") + "]");
                                    Console.ResetColor();
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write("!leave requested by ");
                                    Console.ResetColor();
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    Console.Write(e.PrivateMessage.User.Nick + Environment.NewLine);
                                }
                                break;
                            case "!cycle":
                                if (File.Exists((Path.Combine(combine, "TimsBot/data/access/commands/users/")) + e.PrivateMessage.User.Nick))
                                {

                                    channel.Part();

                                    client.JoinChannel(splitvalue[1]);
                                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                                    Console.Write("[" + DateTime.Now.ToString("HH:mm:ss") + "]");
                                    Console.ResetColor();
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write("!cycle requested by ");
                                    Console.ResetColor();
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    Console.Write(e.PrivateMessage.User.Nick + Environment.NewLine);

                                }
                                break;
                            case "!kick":
                                if (File.Exists((Path.Combine(combine, "TimsBot/data/access/commands/users/")) + e.PrivateMessage.User.Nick))
                                {


                                    channel.Kick(e.PrivateMessage.Message.Substring(6));
                                    client.SendMessage("You got kicked from " + splitvalue[1], e.PrivateMessage.Message.Substring(6));
                                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                                    Console.Write("[" + DateTime.Now.ToString("HH:mm:ss") + "]");
                                    Console.ResetColor();
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write(e.PrivateMessage.Message.Substring(6) + " is kicked from " + splitvalue[1] + " by ");
                                    Console.ResetColor();
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    Console.Write(e.PrivateMessage.User.Nick + Environment.NewLine);
                                }


                                break;
                            case "!commands":
                                if (File.Exists((Path.Combine(combine, "TimsBot/data/access/commands/users/")) + e.PrivateMessage.User.Nick))
                                {

                                    channel.SendMessage(e.PrivateMessage.User.Nick + ", Commands: !ban, !unban, !kick, !join, !warn, !warn_offtopic, !op, !deop");
                                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                                    Console.Write("[" + DateTime.Now.ToString("HH:mm:ss") + "]");
                                    Console.ResetColor();
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write("!commands requested by ");
                                    Console.ResetColor();
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    Console.Write(e.PrivateMessage.User.Nick + Environment.NewLine);
                                }

                                break;
                            case "!say":
                                if (File.Exists((Path.Combine(combine, "TimsBot/data/access/commands/users/")) + e.PrivateMessage.User.Nick))
                                {
                                    string[] splitvalue2 = e.PrivateMessage.Message.Split(' ');

                                    client.SendMessage(splitvalue2[1], splitvalue[1]);
                                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                                    Console.Write("[" + DateTime.Now.ToString("HH:mm:ss") + "]");
                                    Console.ResetColor();
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write("!say requested by ");
                                    Console.ResetColor();
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    Console.Write(e.PrivateMessage.User.Nick + Environment.NewLine);
                                }


                                break;
                            case "!join":

                                if (File.Exists((Path.Combine(combine, "TimsBot/data/access/commands/users/")) + e.PrivateMessage.User.Nick))
                                {
                                    client.JoinChannel(e.PrivateMessage.Message.Substring(5));
                                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                                    Console.Write("[" + DateTime.Now.ToString("HH:mm:ss") + "]");
                                    Console.ResetColor();
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write("Bot joined" + e.PrivateMessage.Message.Substring(5) + " by");
                                    Console.ResetColor();
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    Console.Write(e.PrivateMessage.User.Nick + Environment.NewLine);
                                }


                                break;
                            case "!warn_offtopic":
                                if (File.Exists((Path.Combine(combine, "TimsBot/data/access/commands/users/")) + e.PrivateMessage.User.Nick))
                                {
                                    if (Directory.Exists((Path.Combine(combine, "TimsBot/data/systems/warnsystem/offtopic/")) + e.PrivateMessage.Message.Substring(15)))
                                    {
                                        if (File.Exists((Path.Combine(combine, "TimsBot/data/systems/warnsystem/offtopic/")) + e.PrivateMessage.Message.Substring(15) + "/" + "4"))
                                        {
                                            var target = e.PrivateMessage.Message.Substring(15);
                                            client.WhoIs(target, whois => channel.ChangeMode("+b *!*@" + whois.User.Hostname));

                                            Console.WriteLine(e.PrivateMessage.Message.Substring(15) + " " + "is banned");
                                            client.SendMessage("You got banned from " + splitvalue[1] + " because you keep going off topic", e.PrivateMessage.Message.Substring(15));
                                            channel.Kick(e.PrivateMessage.Message.Substring(15));
                                        }
                                        else
                                        {
                                            if (File.Exists((Path.Combine(combine, "TimsBot/data/systems/warnsystem/offtopic/")) + e.PrivateMessage.Message.Substring(15) + "/" + "3"))
                                            {
                                                WriteAllText("TimsBot/data/systems/warnsystem/offtopic/" + e.PrivateMessage.Message.Substring(15) + "/" + "4", "Warning number 4");
                                                channel.Kick(e.PrivateMessage.Message.Substring(15));
                                                Console.WriteLine(e.PrivateMessage.Message.Substring(15) + " " + "is kicked");
                                                client.SendMessage("You got kicked from " + splitvalue[1] + " because you keep going off topic", e.PrivateMessage.Message.Substring(6));

                                            }
                                            else
                                            {
                                                if (File.Exists((Path.Combine(combine, "TimsBot/data/systems/warnsystem/offtopic/")) + e.PrivateMessage.Message.Substring(15) + "/" + "2"))
                                                {
                                                    WriteAllText("TimsBot/data/systems/warnsystem/offtopic/" + e.PrivateMessage.Message.Substring(15) + "/" + "3", "Warning number 3");
                                                    client.SendMessage("This is your last warning, please stop being off topic, you will be kicked if you go on in " + splitvalue[1], e.PrivateMessage.Message.Substring(15));
                                                }
                                                else
                                                {
                                                    WriteAllText("TimsBot/data/systems/warnsystem/offtopic/" + e.PrivateMessage.Message.Substring(15) + "/" + "2", "Warning number 2");
                                                    client.SendMessage("Please remember, this is a chat for chat with related topics in " + splitvalue[1], e.PrivateMessage.Message.Substring(15));
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        CreateDir("TimsBot/data/systems/warnsystem/offtopic/" + e.PrivateMessage.Message.Substring(15));
                                        client.SendMessage("Please try and stay on topic in " + splitvalue[1], e.PrivateMessage.Message.Substring(15));
                                    }

                                }

                                else

                                {

                                }



                                break;
                            case "!warn":
                                if (File.Exists((Path.Combine(combine, "TimsBot/data/access/commands/users/")) + e.PrivateMessage.User.Nick))
                                {
                                    if (Directory.Exists((Path.Combine(combine, "TimsBot/data/systems/warnsystem/warning/")) + e.PrivateMessage.Message.Substring(6)))
                                    {
                                        if (File.Exists((Path.Combine(combine, "TimsBot/data/systems/warnsystem/warning/")) + e.PrivateMessage.Message.Substring(6) + "/" + "4"))
                                        {
                                            var target = e.PrivateMessage.Message.Substring(6);
                                            client.WhoIs(target, whois => channel.ChangeMode("+b *!*@" + whois.User.Hostname));

                                            Console.WriteLine(e.PrivateMessage.Message.Substring(6) + " " + "is banned");
                                            client.SendMessage("You got banned from " + splitvalue[1] + " because you have 5 warnings", e.PrivateMessage.Message.Substring(6));
                                            channel.Kick(e.PrivateMessage.Message.Substring(6));
                                        }
                                        else
                                        {
                                            if (File.Exists((Path.Combine(combine, "TimsBot/data/systems/warnsystem/warning/")) + e.PrivateMessage.Message.Substring(6) + "/" + "3"))
                                            {
                                                WriteAllText("TimsBot/data/systems/warnsystem/warning/" + e.PrivateMessage.Message.Substring(6) + "/" + "4", "Warning number 4");
                                                channel.Kick(e.PrivateMessage.Message.Substring(6));
                                                Console.WriteLine(e.PrivateMessage.Message.Substring(6) + " " + "is kicked");
                                                client.SendMessage("You got kicked from " + splitvalue[1] + " because you have 4 warnings, if you get one more warning you will get banned.", e.PrivateMessage.Message.Substring(6));

                                            }
                                            else
                                            {
                                                if (File.Exists((Path.Combine(combine, "TimsBot/data/systems/warnsystem/warning/")) + e.PrivateMessage.Message.Substring(6) + "/" + "2"))
                                                {
                                                    WriteAllText("TimsBot/data/systems/warnsystem/warning/" + e.PrivateMessage.Message.Substring(6) + "/" + "3", "Warning number 3");
                                                    client.SendMessage("Please remember, this is your 3th warning in in " + splitvalue[1], e.PrivateMessage.Message.Substring(6));
                                                }
                                                else
                                                {
                                                    WriteAllText("TimsBot/data/systems/warnsystem/warning/" + e.PrivateMessage.Message.Substring(6) + "/" + "2", "Warning number 2");
                                                    client.SendMessage("Please remember, this is your second warning in " + splitvalue[1], e.PrivateMessage.Message.Substring(6));
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        CreateDir("TimsBot/data/systems/warnsystem/warning/" + e.PrivateMessage.Message.Substring(6));
                                        client.SendMessage("Please remember, this is your first warning in " + splitvalue[1], e.PrivateMessage.Message.Substring(6));
                                    }

                                }
                                break;
                            case "!op":

                                if (File.Exists((Path.Combine(combine, "TimsBot/data/access/commands/users/")) + e.PrivateMessage.User.Nick))
                                {
                                    var target = e.PrivateMessage.User.Nick;
                                    client.WhoIs(target, whois => channel.ChangeMode("+o  " + e.PrivateMessage.User.Nick));
                                }
                                break;
                            case "!deop":
                                if (File.Exists((Path.Combine(combine, "TimsBot/data/access/commands/users/")) + e.PrivateMessage.User.Nick))
                                {
                                    var target = e.PrivateMessage.User.Nick;
                                    client.WhoIs(target, whois => channel.ChangeMode("-o " + e.PrivateMessage.User.Nick));
                                }

                                break;
                        }


                    };

                    client.ConnectAsync();


                    while (true) ;

                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Error: '{0}'", e);
                    Console.ReadLine();

                }
            }
        }


        static string combine = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        public static void CreateDir(String dir)
        {
            Directory.CreateDirectory((Path.Combine(combine, dir)));
        }

        public static void WriteAllText(String path, String text)
        {
            File.WriteAllText((Path.Combine(combine, path)), text);
        }
    }

}
