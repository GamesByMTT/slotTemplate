using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using Newtonsoft.Json;
using Best.SocketIO;
using Best.SocketIO.Events;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
public class SocketControllerv3 : MonoBehaviour
{

    // [SerializeField] internal JSHandler _jsManager;

    // [SerializeField] private string SocketURI = "https://dev.casinoparadize.com";
    [DllImport("__Internal")]
    private static extern IntPtr GetAuthToken(string cookieName);

    [SerializeField] private JSHandler jsHandler;
    [SerializeField] private string SocketURI = "https://jztxjn23-5000.inc1.devtunnels.ms/";
    internal SocketModel socketModel = new SocketModel();
    internal Action onInit;
    internal Action onSpin;
    private Helper helper = new Helper();
    private SocketManager manager;

    [SerializeField] private string authToken;

    protected string gameID = "SL-VIK";

    internal bool isLoaded = false;

    private const int maxReconnectionAttempts = 6;
    private readonly TimeSpan reconnectionDelay = TimeSpan.FromSeconds(2);
    string myAuth = null;

    private void Awake()
    {
        isLoaded = false;
    }

    private void Start()
    {
        //OpenWebsocket();
        // OpenSocket();
    }

    void ReceiveAuthToken(string authToken)
    {
        Debug.Log("Received authToken: " + authToken);
        // Do something with the authToken
        myAuth = authToken;
    }


    internal void InitiateSocket()
    {


        SocketOptions options = new SocketOptions();
        options.ReconnectionAttempts = maxReconnectionAttempts;
        options.ReconnectionDelay = reconnectionDelay;
        options.Reconnection = true;
        Application.ExternalCall("window.parent.postMessage", "authToken", "*");

        // SetupSocketManager(options);

#if UNITY_WEBGL && !UNITY_EDITOR
        jsHandler.RetrieveAuthToken("token", authToken =>
        {
            if (!string.IsNullOrEmpty(authToken))
            {
                Debug.Log("Auth token is " + authToken);
                Func<SocketManager, Socket, object> authFunction = (manager, socket) =>
                {
                    return new
                    {
                        token = authToken
                    };
                };
                options.Auth = authFunction;
                // Proceed with connecting to the server
                SetupSocketManager(options);
            }
            else
            {
                Application.ExternalEval(@"
                window.addEventListener('message', function(event) {
                    if (event.data.type === 'authToken') {
                        // Send the message to Unity
                        SendMessage('SocketControllerV3', 'ReceiveAuthToken', event.data.cookie);
                    }});");

                // Start coroutine to wait for the auth token
                StartCoroutine(WaitForAuthToken(options));
            }
        });
#else
        Func<SocketManager, Socket, object> authFunction = (manager, socket) =>
        {
            return new
            {
                token = authToken
            };
        };
        options.Auth = authFunction;
        // Proceed with connecting to the server
        SetupSocketManager(options);
#endif
    }

    private IEnumerator WaitForAuthToken(SocketOptions options)
    {
        // while (myAuth == null)
        // {
        //     Debug.Log("My Auth is null");
        //     yield return null;
        // }

        yield return new WaitUntil(()=>myAuth!=null);
        Debug.Log("My Auth is not null");
        Func<SocketManager, Socket, object> authFunction = (manager, socket) =>
        {
            return new
            {
                token = myAuth
            };
        };
        options.Auth = authFunction;

        Debug.Log("Auth function configured with token: " + myAuth);

        // Proceed with connecting to the server
        SetupSocketManager(options);
    }

    private void SetupSocketManager(SocketOptions options)
    {
        this.manager = new SocketManager(new Uri(SocketURI), options);
        this.manager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
        this.manager.Socket.On<string>(SocketIOEventTypes.Disconnect, OnDisconnected);
        this.manager.Socket.On<string>(SocketIOEventTypes.Error, OnError);
        this.manager.Socket.On<string>("message", ReceivedMessage);
        this.manager.Socket.On<bool>("socketState", OnSocketState);
        this.manager.Socket.On<string>("internalError", OnSocketError);
        this.manager.Socket.On<string>("alert", OnSocketAlert);
        this.manager.Socket.On<string>("AnotherDevice", OnSocketOtherDevice);
    }

    // Connected event handler implementation
    void OnConnected(ConnectResponse resp)
    {
        Debug.Log("Connected!");
        SendPing();
    }

    private void OnDisconnected(string response)
    {
        Debug.Log("Disconnected from the server");

    }
    private void OnError(string response)
    {
        Debug.LogError("Error: " + response);
    }
    private void ReceivedMessage(string data)
    {
        Debug.Log("Received some_event with data: " + data);

        RecievedData(data);

    }

    private void OnSocketState(bool state)
    {
        if (state)
        {
            Debug.Log("my state is " + state);
            InitRequest("AUTH");
        }

    }
    private void OnSocketError(string data)
    {
        Debug.Log("Received error with data: " + data);
    }
    private void OnSocketAlert(string data)
    {
        Debug.Log("Received alert with data: " + data);
    }

    private void OnSocketOtherDevice(string data)
    {
        Debug.Log("Received Device Error with data: " + data);
    }

    private void SendPing()
    {
        InvokeRepeating("AliveRequest", 0f, 3f);
    }

    private void AliveRequest()
    {
        SendMessage("YES I AM ALIVE");
    }

    internal void SendMessage(string eventName, string json = null)
    {
        // Send the message
        if (this.manager.Socket == null || !this.manager.Socket.IsOpen)
        {
            Debug.LogWarning("Socket is not connected.");
            return;
        }
        if (json == null)
        {
            this.manager.Socket.Emit(eventName);
            return;
        }
        this.manager.Socket.Emit(eventName, json);
        Debug.Log("JSON data sent: " + json);



    }

    private void InitRequest(string eventName)
    {
        var message = new { Data = new { GameID = "SL-VIK" }, id = "Auth" };
        string json = JsonConvert.SerializeObject(message);
        SendMessage(eventName, json);
    }

    internal void CloseSocket()
    {
        SendMessage("EXIT");
        DOVirtual.DelayedCall(0.1f, () =>   
        {
            if (this.manager != null)
            {
                Debug.Log("Dispose my Socket");
                this.manager.Close();
            }
        });
    }


    private void RecievedData(string modifiedResponse)
    {
        Debug.Log("in received data :" + modifiedResponse);
        JObject jsonObject = JObject.Parse(modifiedResponse);
        string messageId = jsonObject["id"].ToString();

        Debug.Log("message id " + messageId);


        var message = jsonObject["message"];
        var gameData = message["GameData"];
        socketModel.PlayerData = message["PlayerData"].ToObject<PlayerData>();
        if (messageId == "InitData")
        {
            socketModel.UIdata = message["UIData"].ToObject<UIData>();

            socketModel.initGameData.Reel = gameData["Reel"].ToObject<List<List<string>>>();
            socketModel.initGameData.Lines = gameData["Lines"].ToObject<List<List<int>>>();
            socketModel.initGameData.Bets = gameData["Bets"].ToObject<List<double>>();
            socketModel.initGameData.canSwitchLines = gameData["canSwitchLines"].ToObject<bool>();
            socketModel.initGameData.LinesCount = gameData["LinesCount"].ToObject<List<int>>();
            socketModel.initGameData.autoSpin = gameData["autoSpin"].ToObject<List<int>>();
            onInit?.Invoke();
        }
        else if (messageId == "ResultData")
        {
            socketModel.resultGameData.ResultReel = helper.ConvertStringListsToIntLists(gameData["ResultReel"].ToObject<List<List<string>>>());
            socketModel.resultGameData.linesToEmit = gameData["linesToEmit"].ToObject<List<int>>();
            socketModel.resultGameData.symbolsToEmit = gameData["symbolsToEmit"].ToObject<List<List<string>>>();
            socketModel.resultGameData.WinAmout = gameData["WinAmout"].ToObject<double>();
            socketModel.resultGameData.freeSpins = gameData["freeSpins"].ToObject<double>();
            socketModel.resultGameData.jackpot = gameData["jackpot"].ToObject<double>();
            socketModel.resultGameData.isBonus = gameData["isBonus"].ToObject<bool>();
            socketModel.resultGameData.BonusStopIndex = gameData["BonusStopIndex"].ToObject<double>();
            print("UI datasdsd: " + JsonConvert.SerializeObject(socketModel.resultGameData.linesToEmit));
            onSpin?.Invoke();

        }

        print("player data: " + JsonConvert.SerializeObject(socketModel.PlayerData));


    }

    internal double ChangeLineBet()
    {
        socketModel.currentBetIndex++;
        if (socketModel.currentBetIndex >= socketModel.initGameData.Bets.Count)
        {
            socketModel.currentBetIndex = 0;
        }

        return socketModel.initGameData.Bets[socketModel.currentBetIndex];
    }

    internal void AccumulateResult(double currBet)
    {
        var spinData = new { data = new { currentBet = 0, currentLines = 20, spins = 1 }, id = "SPIN" };
        // Serialize message data to JSON
        string json = JsonUtility.ToJson(spinData);
        SendMessage("message", json);
    }






}
