﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;

namespace NovetusLauncher
{
    public partial class NovetusConsole : Form
    {
        static LauncherFormShared ConsoleForm;
        static ScriptType loadMode = ScriptType.Server;
        bool helpMode = false;
        bool disableCommands = false;
        string[] argList;
        FileFormat.Config savedConfig;

        public NovetusConsole(string[] args)
        {
            ConsoleForm = new LauncherFormShared();
            argList = args;
            InitializeComponent();
        }

        private void NovetusConsole_Load(object sender, EventArgs e)
        {
            Util.ConsolePrint("Novetus version " + GlobalVars.ProgramInformation.Version + " loaded. Initializing config.", 4);
            Util.ConsolePrint("Novetus path: " + GlobalPaths.BasePath, 4);
            if (!File.Exists(GlobalPaths.ConfigDir + "\\" + GlobalPaths.ConfigName))
            {
                Util.ConsolePrint("WARNING - " + GlobalPaths.ConfigDir + "\\" + GlobalPaths.ConfigName + " not found. Creating one with default values.", 5);
                ConsoleForm.WriteConfigValues();
            }
            if (!File.Exists(GlobalPaths.ConfigDir + "\\" + GlobalPaths.ConfigNameCustomization))
            {
                Util.ConsolePrint("WARNING - " + GlobalPaths.ConfigDir + "\\" + GlobalPaths.ConfigNameCustomization + " not found. Creating one with default values.", 5);
                ConsoleForm.WriteCustomizationValues();
            }
            if (!File.Exists(GlobalPaths.ConfigDir + "\\servers.txt"))
            {
                Util.ConsolePrint("WARNING - " + GlobalPaths.ConfigDir + "\\servers.txt not found. Creating empty file.", 5);
                File.Create(GlobalPaths.ConfigDir + "\\servers.txt").Dispose();
            }
            if (!File.Exists(GlobalPaths.ConfigDir + "\\ports.txt"))
            {
                Util.ConsolePrint("WARNING - " + GlobalPaths.ConfigDir + "\\ports.txt not found. Creating empty file.", 5);
                File.Create(GlobalPaths.ConfigDir + "\\ports.txt").Dispose();
            }
            NovetusFuncs.SetupAdminPassword();

            if (argList.Length > 0)
            {
                //DO ARGS HERE
                ConsoleProcessArguments();
            }
        }

        private void ConsoleProcessArguments()
        {
            CommandLineArguments.Arguments ConsoleArgs = new CommandLineArguments.Arguments(argList);

            if (ConsoleArgs["help"] != null)
            {
                helpMode = true;
                ConsoleHelp();
            }

            if (ConsoleArgs["cmdonly"] != null && ConsoleArgs["cmdmode"] != null && !helpMode)
            {
                //cmd mode
                savedConfig = GlobalVars.UserConfiguration;
                disableCommands = true;
                bool no3d = false;
                bool nomap = false;

                if (ConsoleArgs["headless"] != null)
                {
                    Visible = false;
                    ShowInTaskbar = false;
                    Opacity = 0;
                }

                if (ConsoleArgs["load"] != null)
                {
                    string error = "Load Mode '" + ConsoleArgs["load"] + "' is not available. Loading " + loadMode;

                    try
                    {
                        object check = Enum.Parse(typeof(ScriptType), ConsoleArgs["load"], true);

                        if (check == null || (ScriptType)check == ScriptType.None)
                        {
                            Util.ConsolePrint(error, 2);
                        }
                        else
                        {
                            loadMode = (ScriptType)check;
                            Util.ConsolePrint("Load Mode set to '" + loadMode + "'.", 3);
                        }
                    }
                    catch (Exception)
                    {
                        Util.ConsolePrint(error, 2);
                    }

                    if (ConsoleArgs["map"] != null)
                    {
                        GlobalVars.UserConfiguration.Map = ConsoleArgs["map"];
                        GlobalVars.UserConfiguration.MapPath = ConsoleArgs["map"];
                        Util.ConsolePrint("Novetus will now launch the client with the map " + GlobalVars.UserConfiguration.MapPath, 4);
                    }
                    else
                    {
                        Util.ConsolePrint("Novetus will launch the sclient with the map defined in the INI file.", 4);
                    }

                    if (ConsoleArgs["client"] != null)
                    {
                        GlobalVars.UserConfiguration.SelectedClient = ConsoleArgs["client"];
                    }
                    else
                    {
                        Util.ConsolePrint("Novetus will launch the client defined in the INI file.", 4);
                    }

                    switch (loadMode)
                    {
                        case ScriptType.Client:
                            {

                            }
                            break;
                        case ScriptType.Server:
                            {
                                if (ConsoleArgs["no3d"] != null)
                                {
                                    no3d = true;
                                    Util.ConsolePrint("Novetus will now launch the server in No3D mode.", 4);
                                    Util.ConsolePrint("Launching the server without graphics enables better performance. " +
                                        "However, launching the server with no graphics may cause some elements in later clients may be disabled, such as Dialog boxes." +
                                        "This feature may also make your server unstable.", 5);
                                }

                                if (ConsoleArgs["hostport"] != null)
                                {
                                    GlobalVars.UserConfiguration.RobloxPort = Convert.ToInt32(ConsoleArgs["hostport"]);
                                }

                                if (ConsoleArgs["upnp"] != null)
                                {
                                    GlobalVars.UserConfiguration.UPnP = Convert.ToBoolean(ConsoleArgs["upnp"]);

                                    if (GlobalVars.UserConfiguration.UPnP)
                                    {
                                        Util.ConsolePrint("Novetus will now use UPnP for port forwarding.", 4);
                                    }
                                    else
                                    {
                                        Util.ConsolePrint("Novetus will not use UPnP for port forwarding. Make sure the port " + GlobalVars.UserConfiguration.RobloxPort + " is properly forwarded or you are running a LAN redirection tool.", 4);
                                    }
                                }

                                if (ConsoleArgs["notifications"] != null)
                                {
                                    GlobalVars.UserConfiguration.ShowServerNotifications = Convert.ToBoolean(ConsoleArgs["notifications"]);

                                    if (GlobalVars.UserConfiguration.ShowServerNotifications)
                                    {
                                        Util.ConsolePrint("Novetus will show notifications on player join/leave.", 4);
                                    }
                                    else
                                    {
                                        Util.ConsolePrint("Novetus will not show notifications on player join/leave.", 4);
                                    }
                                }

                                if (ConsoleArgs["maxplayers"] != null)
                                {
                                    GlobalVars.UserConfiguration.PlayerLimit = Convert.ToInt32(ConsoleArgs["maxplayers"]);
                                }

                                if (ConsoleArgs["serverbrowsername"] != null)
                                {
                                    GlobalVars.UserConfiguration.ServerBrowserServerName = ConsoleArgs["serverbrowsername"];
                                }

                                if (ConsoleArgs["serverbrowseraddress"] != null)
                                {
                                    GlobalVars.UserConfiguration.ServerBrowserServerAddress = ConsoleArgs["serverbrowseraddress"];
                                }
                            }
                            break;
                        case ScriptType.Studio:
                            {
                                if (ConsoleArgs["nomap"] != null)
                                {
                                    nomap = true;
                                    Util.ConsolePrint("Novetus will now launch Studio with no map.", 4);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }

                ConsoleForm.StartGame(loadMode, no3d, nomap, true);
            }
        }

        public void ConsoleProcessCommands(string cmd)
        {
            if (disableCommands)
                return;

            switch (cmd)
            {
                case string server when server.Contains("server", StringComparison.InvariantCultureIgnoreCase) == true:
                    try
                    {
                        string[] vals = server.Split(' ');

                        if (vals[1].Equals("3d", StringComparison.InvariantCultureIgnoreCase))
                        {
                            ConsoleForm.StartGame(ScriptType.Server, false, false, true);
                        }
                        else if (vals[1].Equals("no3d", StringComparison.InvariantCultureIgnoreCase))
                        {
                            ConsoleForm.StartGame(ScriptType.Server, true, false, true);
                        }
                        else
                        {
                            ConsoleForm.StartGame(ScriptType.Server, false, false, true);
                        }
                    }
                    catch (Exception)
                    {
                        ConsoleForm.StartGame(ScriptType.Server, false, false, true);
                    }
                    break;
                case string client when string.Compare(client, "client", true, CultureInfo.InvariantCulture) == 0:
                    ConsoleForm.StartGame(ScriptType.Client);
                    break;
                case string solo when string.Compare(solo, "solo", true, CultureInfo.InvariantCulture) == 0:
                    ConsoleForm.StartGame(ScriptType.Solo);
                    break;
                case string studio when studio.Contains("studio", StringComparison.InvariantCultureIgnoreCase) == true:
                    try
                    {
                        string[] vals = studio.Split(' ');

                        if (vals[1].Equals("map", StringComparison.InvariantCultureIgnoreCase))
                        {
                            ConsoleForm.StartGame(ScriptType.Studio, false, false, true);
                        }
                        else if (vals[1].Equals("nomap", StringComparison.InvariantCultureIgnoreCase))
                        {
                            ConsoleForm.StartGame(ScriptType.Studio, false, true, true);
                        }
                        else
                        {
                            ConsoleForm.StartGame(ScriptType.Studio, false, false, true);
                        }
                    }
                    catch (Exception)
                    {
                        ConsoleForm.StartGame(ScriptType.Studio, false, false, true);
                    }
                    break;
                case string config when config.Contains("config", StringComparison.InvariantCultureIgnoreCase) == true:
                    try
                    {
                        string[] vals = config.Split(' ');

                        if (vals[1].Equals("save", StringComparison.InvariantCultureIgnoreCase))
                        {
                            ConsoleForm.WriteConfigValues();
                        }
                        else if (vals[1].Equals("load", StringComparison.InvariantCultureIgnoreCase))
                        {
                            ConsoleForm.ReadConfigValues();
                        }
                        else if (vals[1].Equals("reset", StringComparison.InvariantCultureIgnoreCase))
                        {
                            ConsoleForm.ResetConfigValues();
                        }
                        else
                        {
                            Util.ConsolePrint("Please specify 'save', 'load', or 'reset'.", 2);
                        }
                    }
                    catch (Exception)
                    {
                        Util.ConsolePrint("Please specify 'save', 'load', or 'reset'.", 2);
                    }
                    break;
                case string help when string.Compare(help, "help", true, CultureInfo.InvariantCulture) == 0:
                    ConsoleHelp();
                    break;
                case string sdk when string.Compare(sdk, "sdk", true, CultureInfo.InvariantCulture) == 0:
                    ConsoleForm.LoadLauncher();
                    break;
                case string dlldelete when string.Compare(dlldelete, "dlldelete", true, CultureInfo.InvariantCulture) == 0:
                    if (GlobalVars.UserConfiguration.DisableReshadeDelete == true)
                    {
                        GlobalVars.UserConfiguration.DisableReshadeDelete = false;
                        Util.ConsolePrint("ReShade DLL deletion enabled.", 4);
                    }
                    else
                    {
                        GlobalVars.UserConfiguration.DisableReshadeDelete = true;
                        Util.ConsolePrint("ReShade DLL deletion disabled.", 4);
                    }
                    break;
                case string altserverip when altserverip.Contains("altserverip", StringComparison.InvariantCultureIgnoreCase) == true:
                    try
                    {
                        string[] vals = altserverip.Split(' ');

                        if (vals[1].Equals("none", StringComparison.InvariantCultureIgnoreCase))
                        {
                            GlobalVars.UserConfiguration.AlternateServerIP = "";
                            Util.ConsolePrint("Alternate Server IP removed.", 4);
                        }
                        else
                        {
                            GlobalVars.UserConfiguration.AlternateServerIP = vals[1];
                            Util.ConsolePrint("Alternate Server IP set to " + vals[1], 4);
                        }
                    }
                    catch (Exception)
                    {
                        Util.ConsolePrint("Please specify the IP address you would like to set Novetus to.", 2);
                    }
                    break;
                case string clear when clear.Contains("clear", StringComparison.InvariantCultureIgnoreCase) == true:
                    ClearConsole();
                    break;
                case string important when string.Compare(important, GlobalVars.Important, true, CultureInfo.InvariantCulture) == 0:
                    GlobalVars.AdminMode = true;
                    Util.ConsolePrint("ADMIN MODE ENABLED.", 4);
                    Util.ConsolePrint("YOU ARE GOD.", 2);
                    break;
                case string decode when (string.Compare(decode, "decode", true, CultureInfo.InvariantCulture) == 0 || string.Compare(decode, "decrypt", true, CultureInfo.InvariantCulture) == 0):
                    Decoder de = new Decoder();
                    de.Show();
                    Util.ConsolePrint("???", 2);
                    break;
                default:
                    Util.ConsolePrint("Command is either not registered or valid", 2);
                    break;
            }
        }

        public void ConsoleHelp()
        {
            ClearConsole();
            Util.ConsolePrint("Help:", 3, true);
            Util.ConsolePrint("---------", 1, true);
            Util.ConsolePrint("Commands:", 3, true);
            Util.ConsolePrint("---------", 1, true);
            Util.ConsolePrint("+ client | Launches client with launcher settings", 4, true);
            Util.ConsolePrint("+ solo | Launches client in Play Solo mode with launcher settings", 4, true);
            Util.ConsolePrint("+ server 3d | Launches server with launcher settings", 4, true);
            Util.ConsolePrint("+ server no3d | Launches server in NoGraphics mode with launcher settings", 4, true);
            Util.ConsolePrint("+ studio map | Launches Roblox Studio with the selected map", 4, true);
            Util.ConsolePrint("+ studio nomap | Launches Roblox Studio without the selected map", 4, true);
            Util.ConsolePrint("+ sdk | Launches the Novetus SDK Launcher", 4, true);
            Util.ConsolePrint("+ dlldelete | Toggle the deletion of opengl32.dll when ReShade is off.", 4, true);
            Util.ConsolePrint("+ altserverip <IP> | Sets the alternate server IP for server info. Replace <IP> with your specified IP or specify 'none' to remove the current alternate server IP", 4, true);
            Util.ConsolePrint("+ clear | Clears all text in this window.", 4, true);
            Util.ConsolePrint("+ help | Clears all text and shows this list.", 4, true);
            Util.ConsolePrint("+ config save | Saves the config file", 4, true);
            Util.ConsolePrint("+ config load | Reloads the config file", 4, true);
            Util.ConsolePrint("+ config reset | Resets the config file", 4, true);
            Util.ConsolePrint("---------", 1, true);
            Util.ConsolePrint("Command-Line Parameters:", 3, true);
            Util.ConsolePrint("---------", 1, true);
            Util.ConsolePrint("GLOBAL - Affects launcher session.", 5, true);
            Util.ConsolePrint("---------", 1, true);
            Util.ConsolePrint("- sdk | Launches the Novetus SDK Launcher", 4, true);
            Util.ConsolePrint("- cmdonly | Launches the Novetus Console only.", 4, true);
            Util.ConsolePrint("- nofilelist | Disables file list generation", 4, true);
            Util.ConsolePrint("- nocmd | Don't launch the Novetus Console", 4, true);
            Util.ConsolePrint("---------", 1, true);
            Util.ConsolePrint("CONSOLE - Affects console only.", 5, true);
            Util.ConsolePrint("---------", 1, true);
            Util.ConsolePrint("- help | Clears all text and shows this list.", 4, true);
            Util.ConsolePrint("- cmdmode | Puts the console into NovetusCMD mode.", 4, true);
            Util.ConsolePrint("---------", 1, true);
            Util.ConsolePrint("CMDMODE - Affects console in NovetusCMD mode.", 5, true);
            Util.ConsolePrint("---------", 1, true);
            Util.ConsolePrint("- load <Client, Server, Solo, Studio, EasterEgg> | The type of client script to load. ", 4, true);
            Util.ConsolePrint("- headless | Hides the console window upon launch.", 4, true);
            Util.ConsolePrint("---------", 1, true);
            Util.ConsolePrint("LOAD - Parameters for loading clients in NovetusCMD mode.", 5, true);
            Util.ConsolePrint("---------", 1, true);
            Util.ConsolePrint("- map <Map Path in Quotation Marks> | Specifies the path to a map.", 4, true);
            Util.ConsolePrint("- client <Client Name in Quotation Marks> | Specifies the client for Novetus to load.", 4, true);
            Util.ConsolePrint("- no3d | Server Only. Puts the server into No Graphics mode.", 4, true);
            Util.ConsolePrint("- hostport <Port> | Server Only. Specifies the port the server should host on.", 4, true);
            Util.ConsolePrint("- upnp <True/False> | Server Only. Toggles UPnP (Universal Plug and Play).", 4, true);
            Util.ConsolePrint("- notifications <True/False> | Server Only. Toggle player join/leave notifications.", 4, true);
            Util.ConsolePrint("- maxplayers <Player Count> | Server Only. Specifies the server's player count.", 4, true);
            Util.ConsolePrint("- serverbrowsername <Name in Quotation Marks> | Server Only. Specifies the name the server should use on the Server Browser.", 4, true);
            Util.ConsolePrint("- serverbrowseraddress <Address in Quotation Marks> | Server Only. Specifies the Master Server the server should use.", 4, true);
            Util.ConsolePrint("- nomap | Studio Only. Loads Studio without a map.", 4, true);
            Util.ConsolePrint(GlobalVars.Important2, 0, true, true);
            ScrollToTop();
        }

        private void ProcessConsole(object sender, KeyEventArgs e)
        {
            if (helpMode)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    ConsoleForm.CloseEventInternal();
                }
                return;
            }

            //Command proxy

            int totalLines = ConsoleBox.Lines.Length;
            if (totalLines > 0)
            {
                string lastLine = ConsoleBox.Lines[totalLines - 1];

                if (e.KeyCode == Keys.Enter)
                {
                    ConsoleBox.AppendText(Environment.NewLine, Color.White);
                    ConsoleProcessCommands(lastLine);
                    e.Handled = true;
                }
            }

            if (e.Modifiers == Keys.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.X:
                    case Keys.Z:
                        e.Handled = true;
                        break;
                    default:
                        break;
                }
            }
        }

        private void ClearConsole()
        {
            ConsoleBox.Text = "";
            ConsoleBox.SelectionStart = 0;
            ConsoleBox.ScrollToCaret();
        }

        private void ScrollToTop()
        {
            ConsoleBox.SelectionStart = 0;
            ConsoleBox.ScrollToCaret();
        }

        private void ConsoleClose(object sender, FormClosingEventArgs e)
        {
            CommandLineArguments.Arguments ConsoleArgs = new CommandLineArguments.Arguments(argList);
            if (ConsoleArgs["cmdonly"] != null && ConsoleArgs["cmdmode"] != null && !helpMode)
            {
                GlobalVars.UserConfiguration = savedConfig;
            }
            ConsoleForm.CloseEventInternal();
        }
    }
}
