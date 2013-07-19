using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This script is attached to the MultiplayerManager and it 
/// is the foundation for our multiplayer system.
/// 
/// This script accesses the ScoreTable script to inform it of
/// the winning score criteria.
/// 
/// This script accesses the GameSettings script.
/// 
/// This script is accessed by the CursorControl script.
/// </summary>

public class MultiplayerManager : MonoBehaviour
{

    //Variables Start___________________________________

    private string titleMessage = "GTGD Series 1 Prototype";

    private string connectToIP = "127.0.0.1";

    private int connectionPort = 26500;

    private bool useNAT = false;

    private string ipAddress;

    private string port;

    private int numberOfPlayers = 10;

    public string playerName;

    public string serverName;

    public string serverNameForClient;

    private bool iWantToSetupAServer = false;

    private bool iWantToConnectToAServer = false;

    private bool iWantToSetupAPublicServer = false;

    private bool iWantToSetupAPrivateServer = false;

    private bool iWantToConnectToAPublicServer = false;

    private bool iWantToConnectToAPrivateServer = false;



    //These variables are used to define the main
    //window.

    private Rect connectionWindowRect;

    private int connectionWindowWidth = 400;

    private int connectionWindowHeight = 280;

    private int btnHeightCntWindow = 60;

    private int leftIndent;

    private int topIndent;


    //These variables are used to define the server
    //shutdown window.

    private Rect serverDisWindowRect;

    private int serverDisWindowWidth = 300;

    private int serverDisWindowHeight = 150;

    private int serverDisWindowLeftIndent = 10;

    private int serverDisWindowTopIndent = 10;


    //These variables are used to define the client
    //disconnect window.

    private Rect clientDisWindowRect;

    private int clientDisWindowWidth = 300;

    private int clientDisWIndowHeight = 170;

    public bool showDisconnectWindow = false;

    private float btnHeightSmaller = 25;


    //These are used in setting the winning score.

    public int winningScore = 20;

    private int scoreButtonWdith = 40;

    private GUIStyle plainStyle = new GUIStyle();


    //Used in MasterServer implementation

    private string gameNameType = "GTGD S2 Test";

    private Ping masterServerPing;

    private Vector2 scrollPosition = Vector2.zero;

    private GUIStyle boldStyleCentered = new GUIStyle();

    private HostData[] hostData;

    private string ipString;

    private List<Ping> serverPingList = new List<Ping>();

    private bool noPublicServers = false;

    private int pbWidth = 70;

    private int sbWidth = 250;

    private int defCntWindowWidth;

    private int defCntWindowHeight;

    private int adjCntWindowWidth = 550;

    private int adjCntWindowHeight = 400;

    //Variables End_____________________________________


    // Use this for initialization
    void Start()
    {
        //Load the last used serverName from registry.
        //If the serverName is blank then use "Server"
        //as a default name.

        serverName = PlayerPrefs.GetString("serverName");

        if (serverName == "")
        {
            serverName = "Server";
        }


        //Load the last used playerName from registry.
        //If the playerName is blank then use "Player"
        //as a default name.

        playerName = PlayerPrefs.GetString("playerName");

        if (playerName == "")
        {
            playerName = "Player";
        }


        //Set GUIStyles.

        plainStyle.alignment = TextAnchor.MiddleLeft;

        plainStyle.normal.textColor = Color.white;

        plainStyle.wordWrap = true;

        plainStyle.fontStyle = FontStyle.Bold;

        boldStyleCentered.alignment = TextAnchor.MiddleCenter;

        boldStyleCentered.normal.textColor = Color.white;

        boldStyleCentered.wordWrap = true;

        boldStyleCentered.fontStyle = FontStyle.Bold;


        //Ping the master server to find out how long
        //it takes to communicate to it. I have to 
        //RequestHostList otherwise the IP address
        //of the default Unity Master Server won't be
        //available.

        MasterServer.RequestHostList(gameNameType);

        masterServerPing = new Ping(MasterServer.ipAddress);


        //Capture the default window size. The window size
        //will be changed when looking at what public servers
        //are available for connecting to.

        defCntWindowHeight = connectionWindowHeight;

        defCntWindowWidth = connectionWindowWidth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            showDisconnectWindow = !showDisconnectWindow;
        }
    }


    void ConnectWindow(int windowID)
    {
        //Leave a gap from the header.

        GUILayout.Space(15);


        //When the player launches the game they have the option
        //to create a server or join a server. The variables
        //iWantToSetupAServer and iWantToConnectToAServer start as
        //false so the player is presented with two buttons
        //"Setup my server" and "Connect to a server". 

        if (iWantToSetupAServer == false &&
            iWantToConnectToAServer == false &&
            iWantToSetupAPrivateServer == false &&
            iWantToSetupAPublicServer == false &&
            iWantToConnectToAPrivateServer == false &&
            iWantToConnectToAPublicServer == false)
        {
            if (GUILayout.Button("Setup a server", GUILayout.Height(btnHeightCntWindow)))
            {
                iWantToSetupAServer = true;
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Connect to a server", GUILayout.Height(btnHeightCntWindow)))
            {
                iWantToConnectToAServer = true;
            }

            GUILayout.Space(10);

            if (Application.isWebPlayer == false && Application.isEditor == false)
            {
                if (GUILayout.Button("Exit Prototype", GUILayout.Height(btnHeightCntWindow)))
                {
                    Application.Quit();
                }
            }
        }

        //If the player clicks on the Setup A Server button then they are given two
        //server options. They can setup a server that's public and registered with 
        //the master server or they can setup a private game where port forwarding or
        //Hamachi or LAN must be used for establishing a connection.

        if (iWantToSetupAServer == true)
        {
            if (GUILayout.Button("Setup a public server", GUILayout.Height(btnHeightCntWindow)))
            {
                iWantToSetupAPublicServer = true;

                iWantToSetupAServer = false;
            }

            if (GUILayout.Button("Setup a private server", GUILayout.Height(btnHeightCntWindow)))
            {
                iWantToSetupAPrivateServer = true;

                iWantToSetupAServer = false;
            }

            if (GUILayout.Button("Go Back", GUILayout.Height(btnHeightCntWindow)))
            {
                iWantToSetupAServer = false;

                iWantToSetupAPrivateServer = false;

                iWantToSetupAPublicServer = false;
            }
        }


        //If the player has chosen to setup a public server then initialize the server
        //and register it with the Master Server.

        if (iWantToSetupAPublicServer == true)
        {
            //The user can type a name for their server into
            //the textfield.

            GUILayout.Label("Enter a name for your server");

            serverName = GUILayout.TextField(serverName);


            GUILayout.Space(5);


            if (GUILayout.Button("Launch and register public server", GUILayout.Height(btnHeightCntWindow)))
            {
                //Save the serverName using PlayerPrefs.

                PlayerPrefs.SetString("serverName", serverName);


                //Tell the ScoreTable script the winning criteria.




                //If this computer doesn't have a public address then use NAT.

                Network.InitializeServer(numberOfPlayers, connectionPort, !Network.HavePublicAddress());

                MasterServer.RegisterHost(gameNameType, serverName, "");

                iWantToSetupAPublicServer = false;
            }


            GUILayout.Space(10);


            if (GUILayout.Button("Go Back", GUILayout.Height(btnHeightCntWindow)))
            {
                iWantToSetupAPublicServer = false;

                iWantToSetupAServer = true;
            }
        }


        if (iWantToSetupAPrivateServer == true)
        {
            //The user can type a name for their server into
            //the textfield.

            GUILayout.Label("Enter a name for your server");

            serverName = GUILayout.TextField(serverName);


            GUILayout.Space(5);


            //The user can type in the Port number for their server
            //into textfield. We defined a default value above in the 
            //variables as 26500.

            GUILayout.Label("Server Port");

            connectionPort = int.Parse(GUILayout.TextField(connectionPort.ToString()));


            GUILayout.Space(10);

            if (GUILayout.Button("Start my own server", GUILayout.Height(btnHeightCntWindow / 2)))
            {
                //Create the server

                Network.InitializeServer(numberOfPlayers, connectionPort, useNAT);


                //Save the serverName using PlayerPrefs.

                PlayerPrefs.SetString("serverName", serverName);


                //Tell the ScoreTable script the winning criteria.



                iWantToSetupAPrivateServer = false;
            }

            if (GUILayout.Button("Go Back", GUILayout.Height(30)))
            {
                iWantToSetupAPrivateServer = false;

                iWantToSetupAServer = true;
            }
        }


        //If the player has chosen to connect to a server then give
        //the player the option to connect to private server that will
        //require Hamachi, or port forwarding, or LAN to connect to, or
        //the option to connect to a server from a list of servers.

        if (iWantToConnectToAServer == true)
        {
            if (GUILayout.Button("Connect to a public server", GUILayout.Height(btnHeightCntWindow)))
            {
                iWantToConnectToAPublicServer = true;

                iWantToConnectToAServer = false;

                MakeConnectionWindowBigger();

                StartCoroutine(TalkToMasterServer());
            }

            if (GUILayout.Button("Connect to a private server", GUILayout.Height(btnHeightCntWindow)))
            {
                iWantToConnectToAPrivateServer = true;

                iWantToConnectToAServer = false;
            }

            if (GUILayout.Button("Go Back", GUILayout.Height(btnHeightCntWindow)))
            {
                iWantToConnectToAServer = false;

                iWantToConnectToAPrivateServer = false;

                iWantToConnectToAPublicServer = false;
            }
        }



        if (iWantToConnectToAPublicServer == true)
        {
            //The user can type their player name into the
            //textfield.

            GUILayout.Label("Enter your player name", plainStyle);

            playerName = GUILayout.TextField(playerName);


            GUILayout.Box("", GUILayout.Height(5));

            GUILayout.Space(15);


            //If hostData is empty and and no public servers were
            //found then display that to the user.

            if (hostData.Length == 0 && noPublicServers == false)
            {
                GUILayout.Space(50);

                GUILayout.Label("Searching for public servers...", boldStyleCentered);

                GUILayout.Space(50);
            }

            //If hostData isn't empty then display the list of public
            //servers it has.

            else if (hostData.Length != 0)
            {
                //Header row

                GUILayout.BeginHorizontal();

                GUILayout.Label("Public servers", plainStyle, GUILayout.Height(btnHeightCntWindow / 2), GUILayout.Width(sbWidth));

                GUILayout.Label("Players", boldStyleCentered, GUILayout.Height(btnHeightCntWindow / 2), GUILayout.Width(pbWidth));

                //GUILayout.Label("IP Address", boldStyleCentered, GUILayout.Height(btnHeightCntWindow/2));

                GUILayout.Label("Latency", boldStyleCentered, GUILayout.Height(btnHeightCntWindow / 2), GUILayout.Width(pbWidth));

                GUILayout.EndHorizontal();


                scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);


                for (int i = 0; i < hostData.Length; i++)
                {
                    GUILayout.BeginHorizontal();

                    //Each of the available public servers are listed as buttons and the player
                    //clicks on the relevant button to connect to a public server.

                    if (GUILayout.Button(hostData[i].gameName,
                        GUILayout.Height(btnHeightCntWindow / 2), GUILayout.Width(sbWidth)))
                    {
                        //Ensure that the player can't join a game with an empty name

                        if (playerName == "")
                        {
                            playerName = "Player";
                        }

                        //If the player has a name that isn't empty then attempt to join 
                        //the server.

                        if (playerName != "")
                        {
                            //Connect to the selected public server and save the player's name
                            //to player prefs.

                            Network.Connect(hostData[i]);

                            PlayerPrefs.SetString("playerName", playerName);
                        }
                    }

                    //Dispaly the number of players currently in the server and the max number of players.

                    GUILayout.Label((hostData[i].connectedPlayers - 1) + "/" + (hostData[i].playerLimit - 1), boldStyleCentered,
                        GUILayout.Height(btnHeightCntWindow / 2), GUILayout.Width(pbWidth));

                    //GUILayout.Label(hostData[i].ip[0].ToString(), boldStyleCentered, GUILayout.Height(btnHeightCntWindow/2));


                    //List the latency of each of the public servers. If the ping isn't complete or a latency couldn't be retreived
                    //then output N/A meaning Not Available. I think we can't ping computers within our own network that don't have 
                    //a public IP address. The ping should work on servers that are not part of our network.

                    if (serverPingList[i].isDone)
                    {
                        if (serverPingList[i].time <= 0)
                        {
                            GUILayout.Label("N/A", boldStyleCentered, GUILayout.Height(btnHeightCntWindow / 2), GUILayout.Width(pbWidth));
                        }

                        else
                        {
                            GUILayout.Label(serverPingList[i].time.ToString(), boldStyleCentered, GUILayout.Width(pbWidth));
                        }
                    }

                    else
                    {
                        GUILayout.Label("N/A", boldStyleCentered, GUILayout.Height(btnHeightCntWindow / 2), GUILayout.Width(pbWidth));
                    }


                    GUILayout.EndHorizontal();

                    GUILayout.Space(10);


                }

                GUILayout.EndScrollView();
            }

            else
            {
                GUILayout.Space(50);

                GUILayout.Label("No public servers found.", boldStyleCentered);

                GUILayout.Space(50);
            }

            GUILayout.Space(15);

            GUILayout.Box("", GUILayout.Height(5));


            //A refresh button that allows the user to refresh the list of
            //public servers.

            if (GUILayout.Button("Refresh", GUILayout.Height(btnHeightCntWindow / 2)))
            {
                noPublicServers = false;

                StartCoroutine(TalkToMasterServer());
            }


            GUILayout.Space(10);


            if (GUILayout.Button("Go Back", GUILayout.Height(btnHeightCntWindow / 2)))
            {
                MakeConnectionWindowDefaultSize();

                iWantToConnectToAPublicServer = false;

                iWantToConnectToAServer = true;

                noPublicServers = false;
            }
        }


        if (iWantToConnectToAPrivateServer == true)
        {
            //The user can type their player name into the
            //textfield.

            GUILayout.Label("Enter your player name");

            playerName = GUILayout.TextField(playerName);


            GUILayout.Space(5);


            //The player can type the IP address for the server
            //that they want to connect to into the textfield.

            GUILayout.Label("Type in Server IP");

            connectToIP = GUILayout.TextField(connectToIP);

            GUILayout.Space(5);


            //The player can type in the Port number for the server
            //they want to connect to into the textfield.

            GUILayout.Label("Server Port");

            connectionPort = int.Parse(GUILayout.TextField(connectionPort.ToString()));

            GUILayout.Space(5);


            //The player clicks on this button to establish a connection.

            if (GUILayout.Button("Connect", GUILayout.Height(btnHeightSmaller)))
            {
                //Ensure that the player can't join a game with an empty name

                if (playerName == "")
                {
                    playerName = "Player";
                }


                //If the player has a name that isn't empty then attempt to join 
                //the server.

                if (playerName != "")
                {
                    //Connect to a server with the IP address contained in
                    //connectToIP and with the port number contained
                    //in connectionPort.

                    Network.Connect(connectToIP, connectionPort);

                    PlayerPrefs.SetString("playerName", playerName);
                }

            }

            GUILayout.Space(5);

            if (GUILayout.Button("Go Back", GUILayout.Height(btnHeightSmaller)))
            {
                iWantToConnectToAPrivateServer = false;

                iWantToConnectToAServer = true;
            }

        }

    }



    IEnumerator TalkToMasterServer()
    {
        hostData = new HostData[0];

        //Clear the list of servers available so that only the most 
        //uptodate list will be put together.

        MasterServer.ClearHostList();

        MasterServer.RequestHostList(gameNameType);


        //Wait a bit as it takes time for the host list to be retrieved
        //fromt the Master Server.

        yield return new WaitForSeconds(masterServerPing.time / 100 + 0.1f);


        //The list of public servers has been retrieved so put this into
        //the hostData array.

        hostData = MasterServer.PollHostList();


        //If no public servers are found then change the bool below to true.
        //This will affect what message is displayed when searching for servers.

        if (hostData.Length == 0)
        {
            noPublicServers = true;
        }


        //Clear the serverPingList and Trim of all the indexes
        //This allows the list to be resused and prevents GUI draw errors.

        serverPingList.Clear();

        serverPingList.TrimExcess();


        //For each public server create an entry in the serverPingList
        //so that the ping of that server can be recorded and the latency
        //then displayed.

        if (hostData.Length != 0)
        {
            for (int i = 0; i < hostData.Length; i++)
            {
                serverPingList.Add(new Ping(hostData[i].ip[0]));
            }
        }
    }


    void MakeConnectionWindowBigger()
    {
        connectionWindowHeight = adjCntWindowHeight;

        connectionWindowWidth = adjCntWindowWidth;
    }


    void MakeConnectionWindowDefaultSize()
    {
        connectionWindowHeight = defCntWindowHeight;

        connectionWindowWidth = defCntWindowWidth;
    }


    void ServerDisconnectWindow(int windowID)
    {
        GUILayout.Label("Server name: " + serverName);


        //Show the number of players connected.

        GUILayout.Label("Number of players connected: " + Network.connections.Length);


        //If there is at least one connection then show the average ping.

        if (Network.connections.Length >= 1)
        {
            GUILayout.Label("Ping: " + Network.GetAveragePing(Network.connections[0]));
        }


        //Shutdown the server if the user clicks on the Shutdown server button.

        if (GUILayout.Button("Shutdown server"))
        {
            Network.Disconnect();
        }
    }



    void ClientDisconnectWindow(int windowID)
    {
        //Show the player the server they are connected to and the
        //average ping of their connection.

        GUILayout.Label("Connected to server: " + serverName);

        GUILayout.Label("Ping; " + Network.GetAveragePing(Network.connections[0]));


        GUILayout.Space(7);


        //The player disconnects from the server when they press the 
        //Disconnect button.

        if (GUILayout.Button("Disconnect", GUILayout.Height(btnHeightSmaller)))
        {
            Network.Disconnect();
        }


        GUILayout.Space(5);


        //Allow the player to access the Settings menu from the Client's disconnect window.




        GUILayout.Space(5);


        //This button allows the player using a webplayer who has can gone 
        //fullscreen to be able to return to the game. Pressing escape in
        //fullscreen doesn't help as that just exits fullscreen.

        if (GUILayout.Button("Return To Game", GUILayout.Height(btnHeightSmaller)))
        {
            showDisconnectWindow = false;
        }
    }


    //Unity function (for client and server)
    void OnDisconnectedFromServer()
    {
        //If a player loses the connection or leaves the scene then
        //the level is restarted on their computer.

        Application.LoadLevel(Application.loadedLevel);
    }

    //Unity function (for server)
    void OnPlayerDisconnected(NetworkPlayer networkPlayer)
    {
        //When the player leaves the server delete them across the network
        //along with their RPCs so that other players no longer see them.

        Network.RemoveRPCs(networkPlayer);

        Network.DestroyPlayerObjects(networkPlayer);
    }

    //Unity function (for server)
    void OnPlayerConnected(NetworkPlayer networkPlayer)
    {
        networkView.RPC("TellPlayerServerName", networkPlayer, serverName);

        networkView.RPC("TellEveryoneWinningCriteria", networkPlayer, winningScore);
    }

    //Unity function (for client)
    void OnConnectedToServer()
    {
        iWantToConnectToAPrivateServer = false;

        iWantToConnectToAPublicServer = false;

        MakeConnectionWindowDefaultSize();
    }

    void OnGUI()
    {
        //If the player is disconnected then run the ConnectWindow function.

        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            //Determine the position of the window based on the width and 
            //height of the screen. The window will be placed in the middle
            //of the screen.

            leftIndent = Screen.width / 2 - connectionWindowWidth / 2;

            topIndent = Screen.height / 2 - connectionWindowHeight / 2;

            connectionWindowRect = new Rect(leftIndent, topIndent, connectionWindowWidth,
                                            connectionWindowHeight);

            connectionWindowRect = GUILayout.Window(0, connectionWindowRect, ConnectWindow,
                                                    titleMessage);

        }

        //If the game is running as a server then run the ServerDisconnectWindow
        //function.

        if (Network.peerType == NetworkPeerType.Server)
        {
            //Defining the Rect for the server's disconnect window.

            serverDisWindowRect = new Rect(serverDisWindowLeftIndent, serverDisWindowTopIndent,
                                           serverDisWindowWidth, serverDisWindowHeight);

            serverDisWindowRect = GUILayout.Window(1, serverDisWindowRect, ServerDisconnectWindow, "");


            //Allows the server to set the score required for a team to win. The winning 
            //criteria can be adjusted by clicking on the + or - button.

            GUI.Box(new Rect(10, 190, 300, 70), "");

            GUILayout.BeginArea(new Rect(20, 200, 280, 60));

            GUILayout.BeginHorizontal();

            GUILayout.Label("Winning Score", plainStyle, GUILayout.Width(100), GUILayout.Height(scoreButtonWdith));

            GUILayout.Label(winningScore.ToString(), plainStyle, GUILayout.Width(40), GUILayout.Height(scoreButtonWdith));

            if (GUILayout.Button("+", GUILayout.Width(scoreButtonWdith), GUILayout.Height(scoreButtonWdith)))
            {
                if (winningScore >= 10)
                {
                    winningScore = winningScore + 10;
                }

                if (winningScore < 10)
                {
                    winningScore = winningScore + 9;
                }

                networkView.RPC("TellEveryoneWinningCriteria", RPCMode.All, winningScore);
            }

            if (GUILayout.Button("-", GUILayout.Width(scoreButtonWdith), GUILayout.Height(scoreButtonWdith)))
            {
                winningScore = winningScore - 10;

                if (winningScore <= 0)
                {
                    winningScore = 1;
                }

                networkView.RPC("TellEveryoneWinningCriteria", RPCMode.All, winningScore);
            }

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }


        //If the connection type is a client (a player) then show a window that allows
        //them to disconnect from the server.

        if (Network.peerType == NetworkPeerType.Client && showDisconnectWindow == true)
        {
            clientDisWindowRect = new Rect(Screen.width / 2 - clientDisWindowWidth / 2,
                                           Screen.height / 2 - clientDisWIndowHeight / 2,
                                           clientDisWindowWidth, clientDisWIndowHeight);

            clientDisWindowRect = GUILayout.Window(1, clientDisWindowRect, ClientDisconnectWindow, "");
        }
    }


    //Used to tell the MultiplayerScript in connected players the serverName. Otherwise
    //players connecting wouldn't be able to see the name of the server.

    [RPC]
    void TellPlayerServerName(string servername)
    {
        serverName = servername;
    }


    //This RPC is sent to all players from the server to tell them of the new winning
    //score criteria.

}
