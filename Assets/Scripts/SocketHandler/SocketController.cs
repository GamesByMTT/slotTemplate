using Newtonsoft.Json.Linq;
using SocketIOClient.Newtonsoft.Json;
using SocketIOClient;
using System;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
public class SocketController : MonoBehaviour
{
    [SerializeField] private SocketIOUnity socket;

    internal SocketModel socketModel = new SocketModel();

    [SerializeField] private string authToken;
    [SerializeField] private string url;

    private Helper helper= new Helper();

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start(){

        

    }

    internal void InitiateSocket(Action OnInit, Action OnSpin)
    {

        var TokenObj = new { token = authToken };
        var uri = new Uri(url);

        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Auth = TokenObj,
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket,
            // ReconnectionAttempts = 5

        });

        socket.JsonSerializer = new NewtonsoftJsonSerializer();

        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("socket.OnConnected");
        };

        socket.OnPing += (sender, e) =>
        {
            Debug.Log("Ping");

        };

        socket.OnPong += (sender, e) =>
        {
            Debug.Log("Pong: " + e.TotalMilliseconds);
        };

        socket.OnDisconnected += (sender, e) =>
        {
            Debug.Log("disconnect: " + e);
            Cleanup();

        };

        socket.OnReconnectAttempt += (sender, e) =>
        {
            Debug.Log($"{DateTime.Now} Reconnecting: attempt = {e}");
        };

        // Handler for "message" event
        socket.OnAnyInUnityThread((name, response) =>
        {
            string modifiedResponse = response.GetValue<string>();


            if (IsJSON(modifiedResponse))
            {
                try
                {
                    RecievedData(modifiedResponse, OnInit, OnSpin);

                }
                catch (Exception ex)
                {

                    Debug.Log(ex.Message);
                }


            }


        });


        socket.On("socketState", initReq);
        socket.Connect();

        Debug.Log("Connecting...");
    }

    private void OnDisable()
    {
        Cleanup();

    }
    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     EmitSpin();
        // }
    }

    private void RecievedData(string modifiedResponse, Action OnInit, Action OnSpin)
    {
        JObject jsonObject = JObject.Parse(modifiedResponse);
        string messageId = jsonObject["id"].ToString();
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
            OnInit();
        }
        else if(messageId == "ResultData")
        {

            socketModel.resultGameData.ResultReel = helper.ConvertStringListsToIntLists(gameData["ResultReel"].ToObject<List<List<string>>>());
            socketModel.resultGameData.linesToEmit = gameData["linesToEmit"].ToObject<List<int>>();
            socketModel.resultGameData.symbolsToEmit = gameData["symbolsToEmit"].ToObject<List<List<string>>>();
            socketModel.resultGameData.WinAmout = gameData["WinAmout"].ToObject<double>();
            socketModel.resultGameData.freeSpins = gameData["freeSpins"].ToObject<double>();
            socketModel.resultGameData.jackpot = gameData["jackpot"].ToObject<double>();
            socketModel.resultGameData.isBonus = gameData["isBonus"].ToObject<bool>();
            socketModel.resultGameData.BonusStopIndex = gameData["BonusStopIndex"].ToObject<double>();
            print("UI data: " + JsonConvert.SerializeObject(socketModel.resultGameData));
            OnSpin();

        }

        print("player data: " + JsonConvert.SerializeObject(socketModel.PlayerData));


    }

    private void initReq(SocketIOResponse iOResponse)
    {
        Debug.Log("called in init req");
        var obj = new { Data = new { GameID = "SL-VIK" }, id = "Auth" };
        string json = JsonConvert.SerializeObject(obj);
        socket.Emit("AUTH", json);

    }
    bool IsJSON(string str)
    {
        if (string.IsNullOrWhiteSpace(str)) { return false; }
        str = str.Trim();
        if ((str.StartsWith("{") && str.EndsWith("}")) || //For object
            (str.StartsWith("[") && str.EndsWith("]"))) //For array
        {

            return true;
        }
        else
        {
            return false;
        }
    }

   
    internal void SendData(string Data)
    {
        Debug.Log("sending to player :"+Data);
        socket.Emit("message", Data);

    }

    internal double ChangeLineBet(){
        socketModel.currentBetIndex++;
        if(socketModel.currentBetIndex>=socketModel.initGameData.Bets.Count){
            socketModel.currentBetIndex=0;
        }

        return socketModel.initGameData.Bets[socketModel.currentBetIndex];
    }
    private void Cleanup()
    {
        if (socket != null)
        {
            socket.OnConnected -= (sender, e) => { Debug.Log("socket.OnConnected"); };
            socket.OnPing -= (sender, e) => { Debug.Log("Ping"); };
            socket.OnPong -= (sender, e) => { Debug.Log("Pong: " + e.TotalMilliseconds); };
            socket.OnDisconnected -= (sender, e) => { Debug.Log("disconnect: " + e); Cleanup(); };
            socket.OnReconnectAttempt -= (sender, e) => { Debug.Log($"{DateTime.Now} Reconnecting: attempt = {e}"); };
            // Unsubscribe from any other custom events here if needed
            socket.Dispose();
        }
    }



}
