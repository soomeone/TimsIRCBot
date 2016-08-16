﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ChatSharp;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using Microsoft.Win32;




namespace Tim_s_IRC_bot
{
    class Program
    {

       

        static void Main(string[] args) 
        {


            {


                
                
                if (System.IO.File.Exists(System.IO.Path.Combine(Environment.GetEnvironmentVariable("SystemDrive"), "/TimsBot/settings.ini")))
                {
                   

                }
                else
                {
                    Console.WriteLine("Welcome to Tims IRC bot");
                    Console.ReadLine();
                    Console.WriteLine("WHich channel do you want the bot to join?");
                    string Channel = Console.ReadLine();
                    Console.WriteLine("Which nickname do you want to have?");

                    string Nickname = Console.ReadLine();
                    Console.WriteLine("Which username do you have?(if none just leave blank)");
                    string username = Console.ReadLine();
                    if (username == "") {
                        username = username.Replace("", "TimsBot");

                       
                    }
                    Console.WriteLine("Which password do you have for the account?(leave blank if none)");
                    string password = Console.ReadLine();
                    System.IO.Directory.CreateDirectory("/TimsBot/");
                    System.IO.Directory.CreateDirectory("/TimsBot/data");
                    System.IO.Directory.CreateDirectory("/TimsBot/data/logs/");
                    System.IO.Directory.CreateDirectory("/TimsBot/data/systems/tokensystem/");
                    System.IO.File.WriteAllText(System.IO.Path.Combine(Environment.GetEnvironmentVariable("SystemDrive"), "/TimsBot/settings.ini"),"Channel= " + Channel + Environment.NewLine + "Botname= " + Nickname + Environment.NewLine + "username= " + username + Environment.NewLine + "password= " + password);
                    System.IO.File.Copy(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, "/TimsBot/TimsBot.exe");
                    System.IO.File.Copy(System.IO.Path.Combine(Environment.CurrentDirectory , "ChatSharp.dll"), "/TimsBot/ChatSharp.dll");
                    System.IO.File.WriteAllText("/TimsBot/data/logs/log.txt", "This is the log File" + Environment.NewLine);
                    Console.Clear();

                    Console.WriteLine("Install successfull, start the program.");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
                
                Console.WriteLine("Connecting to Freenode");
              

                var handle = GetConsoleWindow();

                ShowWindow(handle, SW_HIDE);
               
                string NickName = System.IO.File.ReadAllText("/TimsBot/settings.ini");
                string Channel1 = System.IO.File.ReadAllText("/TimsBot/settings.ini");
                int index = NickName.IndexOf(System.Environment.NewLine);
                NickName = NickName.Substring(index + System.Environment.NewLine.Length).Replace("Botname= ", "");
                Channel1 = Channel1.Remove(Channel1.TrimEnd().LastIndexOf(Environment.NewLine)).Replace("Channel= ", "");
                var client = new IrcClient("irc.freenode.net", new IrcUser("Tim241", "Tims241", "*****"));
                client.ConnectionComplete += (s, e) => client.JoinChannel("##B4A");
                Console.Clear();


                
                NotifyIcon trayIcon = new NotifyIcon();
                trayIcon.Text = "TimsIRCBot";
                trayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);

                ContextMenu trayMenu = new ContextMenu();

               


                trayMenu.MenuItems.Add("exit", new EventHandler(item1_Click));

                trayIcon.ContextMenu = trayMenu;
                trayIcon.Visible = true;
              




                client.ChannelMessageRecieved += (s, e) =>
                {

                    var channel = client.Channels[e.PrivateMessage.Source];

                    if (e.PrivateMessage.Message == "!list")
                        channel.SendMessage(string.Join(", ", channel.Users.Select(u => u.Nick)));
                    else if (e.PrivateMessage.Message.StartsWith("!ban "))

                    {

                        client.SendMessage("You got banned from the channel.", e.PrivateMessage.Message.Substring(5));
                        var target = e.PrivateMessage.Message.Substring(5);
                        client.WhoIs(target, whois => channel.ChangeMode("+b *!*@" + whois.User.Hostname));
                        channel.Kick(e.PrivateMessage.Message.Substring(5));
                        Console.WriteLine(e.PrivateMessage.Message.Substring(5) + " " + "is banned");
                       


                    }
                    else if (e.PrivateMessage.Message.StartsWith("!unban "))
                    {



                        var target = e.PrivateMessage.Message.Substring(7);
                        client.WhoIs(target, whois => channel.ChangeMode("-b *!*@" + whois.User.Hostname));
                        Console.WriteLine(e.PrivateMessage.Message.Substring(7) + " " + "is unbanned");
                        client.SendMessage("You got unbanned from the channel.", e.PrivateMessage.Message.Substring(7));



                    }
                    else if (e.PrivateMessage.Message == "!leave")
                    {
                        channel.Part();
                        Console.WriteLine("Bot left channel.");
                    }
                    else if (e.PrivateMessage.Message.StartsWith("!kick "))
                    {


                      
                        channel.Kick(e.PrivateMessage.Message.Substring(6));
                        Console.WriteLine(e.PrivateMessage.Message.Substring(6) + " " + "is kicked");
                        client.SendMessage("You got kicked from the channel.", e.PrivateMessage.Message.Substring(6));



                    }
                    else if (e.PrivateMessage.Message == "!commands")
                    {
                        channel.SendMessage("------commands-list---------");
                        channel.SendMessage("ban, unban, kick, join, tokens, give, tokens, tokens_add.");
                        channel.SendMessage("-----end-of-commands-list----");
                        Console.WriteLine("!commands requested");
                    }
                    else if (e.PrivateMessage.Message.StartsWith("!say "))
                    {

                        channel.SendMessage(e.PrivateMessage.Message.Substring(5));



                    }
                    else if (e.PrivateMessage.Message.StartsWith("!join "))
                    {

                        client.JoinChannel(e.PrivateMessage.Message.Substring(5));



                    }
                    else if (e.PrivateMessage.Message.StartsWith("!give "))
                    {
                        string value = System.IO.File.ReadAllText("/TimsBot/data/systems/tokensystem/" + e.PrivateMessage.User.Nick);
                        string input2 = e.PrivateMessage.Message;
                        string[] words = input2.Split(' ');
                        string value2 = System.IO.File.ReadAllText("/TimsBot/data/systems/tokensystem/" + words[1]);
                        string value3 = System.IO.File.ReadAllText("/TimsBot/data/systems/tokensystem/" + e.PrivateMessage.User.Nick);
                        int n = Int32.Parse(value3);
                        if (n == 0)
                        {
                            channel.SendMessage(e.PrivateMessage.User.Nick + " You have 0 Tokens!");

                        }
                        else
                        {

                            //Create invalid nickname handler
                            if (client.User.Nick == (words[1]))
                            {


                                //string str1 = words[0];
                                //string str2 = words[1];
                                //string str3 = words[2];
                                int x = Int32.Parse(value);
                                int y = Int32.Parse(words[2]);
                                int u = x - y;
                                int a = Int32.Parse(value2);
                                int b = Int32.Parse(words[2]);
                                int i = a + b;
                                System.IO.File.WriteAllText("/TimsBot/data/systems/tokensystem/" + e.PrivateMessage.User.Nick, "" + u);
                                System.IO.File.WriteAllText("/TimsBot/data/systems/tokensystem/" + words[1], "" + i);
                                Console.WriteLine(words);

                                channel.SendMessage("Transferred " + words[2] + " tokens from " + e.PrivateMessage.User.Nick + " to " + words[1] + "!");
                                string value4 = System.IO.File.ReadAllText("/TimsBot/data/systems/tokensystem/" + e.PrivateMessage.User.Nick);
                                channel.SendMessage(e.PrivateMessage.User.Nick + " You have " + value4 + " Tokens left!");
                            }
                            else
                            {
                                channel.SendMessage(e.PrivateMessage.User.Nick + " , NickName is invalid.");
                            }
                        }


                    }
                    else if (e.PrivateMessage.Message.StartsWith("!tokens"))
                    {

                        string tokenvalue = System.IO.File.ReadAllText("/TimsBot/data/systems/tokensystem/" + e.PrivateMessage.User.Nick);
                        if (System.IO.File.Exists("/TimsBot/data/systems/tokensystem/" + (e.PrivateMessage.User.Nick)))
                        {

                            channel.SendMessage(e.PrivateMessage.User.Nick + "," + " You have " + tokenvalue + " Tokens!");
                        }
                        else
                        {

                        }



                    }
                    else if (e.PrivateMessage.Message.Contains("FUCK"))
                    {
                        channel.Kick(e.PrivateMessage.User.Nick);
                        client.SendMessage("You got kicked from the channel.", e.PrivateMessage.User.Nick);
                    }

                        client.UserJoinedChannel += (f, g) =>
                    {
                        if (System.IO.File.Exists("/TimsBot/data/systems/tokensystem//" + g.User.Nick))
                        {
                           
                        }
                        else {
                            System.IO.File.WriteAllText("/TimsBot/data/systems/tokensystem/" + g.User.Nick , "10000");

                        }

                    };


                };

                client.ConnectAsync();

                Application.Run();
                while (true) ;

            }

        }


        public static void HideConsoleWindow()
        {
            var handle = GetConsoleWindow();

            ShowWindow(handle, SW_HIDE);
        }


        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        public static void item1_Click(object sender, EventArgs e )
        {
            
            Environment.Exit(0);

        }
        }
      

    }






