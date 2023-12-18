using System;
using System.Threading;
using CookComputing.XmlRpc;
using PaperCut;

public class JPY_Papercut_Lookup_Console
{

    private static ServerCommandProxy _sp;

    public static void Main()
    {
        // Setup environment
        string printServerIp = "papercut.your-cool-server-in-the.cloud";
        int printServerPort = 3000;
        string printServerToken = "enter your web api token here";

        // Setup Server Proxy & connect
        _sp = new PaperCut.ServerCommandProxy(printServerIp, printServerPort, printServerToken);

        // Test api connection
        try
        {
            Console.WriteLine("\r\n█▀█ ▄▀█ █▀█ █▀▀ █▀█ █▀▀ █░█ ▀█▀   █▀█ █ █▄░█   █░░ █▀█ █▀█ █▄▀ █░█ █▀█\r\n█▀▀ █▀█ █▀▀ ██▄ █▀▄ █▄▄ █▄█ ░█░   █▀▀ █ █░▀█   █▄▄ █▄█ █▄█ █░█ █▄█ █▀▀\n");
            Console.WriteLine("Testing API Connection... Please wait a few seconds.");
            _sp.IsUserExists("administrator");
            Thread.Sleep(new TimeSpan(0, 0, 2));
            Console.Clear();
        }
        catch (XmlRpcFaultException)
        {
            PressAnyKeyExitClear("I tried to connect to the API, but it failed.\nPlease contact IT Support for assistance with this issue.\n\nError Code: API-001\nPress any key to exit...");
        }

    // Main menu option selection
    runMenu:
        Console.Clear();
        WriteMainMenu();

        int userOpt;
        try { userOpt = Convert.ToInt32(Console.ReadLine()); }
        catch { goto runMenu; }

        switch (userOpt)
        {
            case 1:
                {
                    caseOne:
                    Console.Clear();
                    Console.WriteLine("\r\n█░█ █▀ █▀▀ █▀█   █ █▄░█ █▀▀ █▀█   █░░ █▀█ █▀█ █▄▀ █░█ █▀█\r\n█▄█ ▄█ ██▄ █▀▄   █ █░▀█ █▀░ █▄█   █▄▄ █▄█ █▄█ █░█ █▄█ █▀▀\n(To go back, type \"goback\".)");
                    Console.Write("\nPlease enter the username to lookup: ");
                    string userToLookup = Console.ReadLine();

                    if (userToLookup == "goback") goto runMenu;

                    bool userExists = DoesUserExist(userToLookup);
                    if (!userExists)
                    {
                        Console.Clear();
                        WriteErrorText();
                        Console.Write("I could not find that user in the given context.\nPlease ensure you typed the username correctly.\n\nPress any key to try again...");
                        Console.ReadKey();
                        goto caseOne;
                    }

                    LookupUserInformation(userToLookup);
                    break;
                }

            case 2:
                {
                caseTwo:
                    Console.Clear();
                    Console.WriteLine("\r\n█░█ █▀ █▀▀ █▀█   █▀█ █ █▄░█   █░░ █▀█ █▀█ █▄▀ █░█ █▀█\r\n█▄█ ▄█ ██▄ █▀▄   █▀▀ █ █░▀█   █▄▄ █▄█ █▄█ █░█ █▄█ █▀▀\n(To go back, type \"goback\".)");
                    Console.Write("\nPlease enter the username to lookup: ");
                    string userToLookup = Console.ReadLine();

                    if (userToLookup == "goback") goto runMenu;

                    bool userExists = DoesUserExist(userToLookup);
                    if (!userExists)
                    {
                        Console.Clear();
                        WriteErrorText();
                        Console.Write("I could not find that user in the given context.\nPlease ensure you typed the username correctly.\n\nPress any key to try again...");
                        Console.ReadKey();
                        goto caseTwo;
                    }

                    GetUserPin(userToLookup);
                    break;
                }

            case 3:
                {
                    Console.Clear();
                    Console.WriteLine("\r\n█▀▄▀█ █▄█   █ █▄░█ █▀▀ █▀█ █▀█ █▀▄▀█ ▄▀█ ▀█▀ █ █▀█ █▄░█\r\n█░▀░█ ░█░   █ █░▀█ █▀░ █▄█ █▀▄ █░▀░█ █▀█ ░█░ █ █▄█ █░▀█");

                    string currentUsername = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1];

                    bool userExists = DoesUserExist(currentUsername);
                    if (!userExists)
                    {
                        Console.Clear();
                        WriteErrorText();
                        Console.Write("You're a ghost! Just kidding.\nBut seriously, you weren't found in papercut, so a person with username \"{0}\" doesn't exist in the printing server.\nPlease contact IT Services if you believe this is an issue\n\nPress any key to return to the main menu...", currentUsername);
                        Console.ReadKey();
                        goto runMenu;
                    }

                    LookupUserInformation(currentUsername, true);
                    break;
                }

            case 9:
                {
                    Console.Clear();
                    Console.Write("Thank you for using the Papercut Pin Lookup app, by Jack Perry\n\n");
                    Console.Write("Check out Carr Hill High School:\n- Website: https://carrhill.school\n- Twitter (X): https://carrhill.school/x\n\n");
                    Console.Write("Press any key to exit...");
                    Console.ReadKey();
                    System.Environment.Exit(1);
                    break;
                }

            default:
                {
                    goto runMenu;
                }

        }

        goto runMenu;
    }

    /**
     * ========================================
     * =          USER DATA FUNCTIONS         =
     * ========================================
     **/

    private static bool DoesUserExist(string username)
    {
        bool doesUserExist = false;

        try
        {
            doesUserExist = _sp.IsUserExists(username);
        } 
        catch (XmlRpcFaultException)
        {
            Console.WriteLine("\n\nThere was an error connecting to the API. Please seek support from IT Services.\nError code: API-002\n\nPress any key to return to the main menu...");
        }

        return doesUserExist;
    }

    private static void LookupUserInformation(string username, bool addSharedAccounts = false)
    {

        // We pre-fill the list with the objects we're looking for, and this list is replaced with the actual values when the API call returns successfully.
        // Saves us having 2 lists and one being redundant after the first call.
        string[] userProps = new string[] { "card-number", "balance", "disabled-print", "email", "full-name", "print-stats.job-count", "print-stats.page-count" };
        string cardNum, email, fullName, amountString, balanceString, printDisabledString, psJobCountString, psPageCountString;
        decimal amount;

        try
        {
            // Fetch user props from the API
            userProps = _sp.GetUserProperties(username, userProps);
        }
        catch (XmlRpcFaultException)
        {
            Console.WriteLine("\n\nThere was an error connecting to the API. Please seek support from IT Services.\nError code: API-002\n\nPress any key to return to the main menu...");
        };



        // "Card" Number (PIN in our case)
        cardNum = userProps[0];
        if (cardNum.Length > 4) { cardNum = "This user uses a physical security key to print"; }

        // Balance
        amountString = userProps[1];
        amount = Decimal.Parse(amountString);
        balanceString = amount.ToString("C");

        // Print Disabled boolean
        bool.TryParse(userProps[2], out bool printDisabled);
        if (!printDisabled) { printDisabledString = "No restrictions in place for this user"; }
        else { printDisabledString = "Printing is disabled for this user"; };

        // E-Mail Address, Full Name
        email = userProps[3];
        fullName = userProps[4];

        // Stats
        psJobCountString = userProps[5];
        psPageCountString = userProps[6];



        Console.WriteLine("\n\n= User Information");
        Console.WriteLine("  * Name: {0} ({1})\n  * E-Mail Address: {2}\n  * Current Balance: {3}\n  * Printing PIN: {4}\n\n= User statistics\n  * Printing is disabled? {5}\n  * Total Job Count (Lifetime): {6}\n  * Total Page Count (Lifetime): {7}\n\n= Shared Accounts\n  * This feature is not currently enabled.\n  * Watch this space!", fullName, username, email, balanceString, cardNum, printDisabledString, psJobCountString, psPageCountString);
        Console.Write("\n\nPress any key to return to the main menu...");
        Console.ReadKey();

        return;
    }

    private static void GetUserPin(string username)
    {
        // We pre-fill the list with the objects we're looking for, and this list is replaced with the actual values when the API call returns successfully.
        // Saves us having 2 lists and one being redundant after the first call.
        string[] userProps = new string[] { "card-number", "full-name" };
        string cardNum, fullName;

        try
        {
            // Fetch user props from the API
            userProps = _sp.GetUserProperties(username, userProps);
        }
        catch (XmlRpcFaultException)
        {
            Console.WriteLine("\n\nThere was an error connecting to the API. Please seek support from IT Services.\nError code: API-002\n\nPress any key to return to the main menu...");
        };

        // "Card" Number (PIN in our case)
        cardNum = userProps[0];
        if (cardNum.Length > 4) { cardNum = "This user uses a physical security key to print"; }

        // E-Mail Address, Full Name
        fullName = userProps[1];


        Console.WriteLine("\n= User Information");
        Console.WriteLine("  * Name: {0}\n  * Printing PIN: {1}", fullName, cardNum);
        Console.Write("\n\nPress any key to return to the main menu...");
        Console.ReadKey();

        return;
    }

    /**
     * ========================================
     * =          TEXT WRITING FUNCTIONS      =
     * ========================================
     **/

    private static void WriteErrorText()
    {
        Console.WriteLine("\r\n█▀▀ █▀▀█ █▀▀█ █▀▀█ █▀▀█ █ \r\n█▀▀ █▄▄▀ █▄▄▀ █──█ █▄▄▀ ▀ \r\n▀▀▀ ▀─▀▀ ▀─▀▀ ▀▀▀▀ ▀─▀▀ ▄");
    }

    public static void WriteMainMenu(bool developerMode = false)
    {
        if (developerMode)
        {
            Console.Clear();
            Console.Write("Developer mode active.\nEnter action PIN: ");
        }
        else
        {
            // Main menu tarty text
            Console.Write("\r\n███╗░░░███╗░█████╗░██╗███╗░░██╗  ███╗░░░███╗███████╗███╗░░██╗██╗░░░██╗\r\n████╗░████║██╔══██╗██║████╗░██║  ████╗░████║██╔════╝████╗░██║██║░░░██║\r\n██╔████╔██║███████║██║██╔██╗██║  ██╔████╔██║█████╗░░██╔██╗██║██║░░░██║\r\n██║╚██╔╝██║██╔══██║██║██║╚████║  ██║╚██╔╝██║██╔══╝░░██║╚████║██║░░░██║\r\n██║░╚═╝░██║██║░░██║██║██║░╚███║  ██║░╚═╝░██║███████╗██║░╚███║╚██████╔╝\r\n╚═╝░░░░░╚═╝╚═╝░░╚═╝╚═╝╚═╝░░╚══╝  ╚═╝░░░░░╚═╝╚══════╝╚═╝░░╚══╝░╚═════╝░\n");
            Console.Write("Please select an option:\n\n[1] Lookup User\n[2] Get printing pin\n[3] See my info\n[9] Exit");
            Console.Write("\n\nEnter your choice: ");
        }
    }

    /**
     * ========================================
     * =          MENU UTILITIES              =
     * ========================================
     **/
    public static void PressAnyKeyExitClear(string msg, string extraPart = null)
    {
        Console.Clear();
        Console.WriteLine(msg);
        Console.ReadKey();
        System.Environment.Exit(1);
    }
}
