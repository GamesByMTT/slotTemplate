using Newtonsoft.Json.Linq;
using SocketIOClient.Newtonsoft.Json;
using SocketIOClient;
using System;
using UnityEngine;
using UnityEditor.VersionControl;
using System.Collections.Generic;
using SocketIOClient.Messages;
using Newtonsoft.Json;
using System.Runtime.Serialization;

public class SocketManger : MonoBehaviour
{

    [SerializeField] private SocketIOUnity socket;

    SocketModel socketModel = new SocketModel();

    [SerializeField] private string authToken;
    [SerializeField] private string url;

    // Start is called before the first frame update
    void Start()
    {

        var TokenObj = new { token = authToken };
        var uri = new Uri(url);

        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Auth = TokenObj,
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket,

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

        };

        socket.OnReconnectAttempt += (sender, e) =>
        {
            Debug.Log($"{DateTime.Now} Reconnecting: attempt = {e}");
        };

        // Handler for "message" event
        socket.OnAnyInUnityThread((name, response) =>
        {
            var ReceivedText = "";
            // Debug.Log(jsonObject);
            ReceivedText += "Received On " + name + " : " + response + "\n";
            string modifiedResponse = response.GetValue<string>();
            Debug.Log("unity thread " + modifiedResponse);


            if (IsJSON(modifiedResponse))
            {
                try
                {
                    PopulateData(modifiedResponse);

                }
                catch (Exception ex)
                {

                    Debug.Log("error while convertion: " + ex.Message);
                }


            }
            else
            {
                print("response boolean" + modifiedResponse);
            }

        });


        socket.On("socketState", initReq);

        Debug.Log("Connecting...");
        socket.Connect();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EmitSpin();
        }
    }

    private void PopulateData(string modifiedResponse)
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
        }

        if (messageId == "ResultData")
        {

            socketModel.resultGameData.ResultReel = gameData["ResultReel"].ToObject<List<List<string>>>();
            socketModel.resultGameData.linesToEmit = gameData["linesToEmit"].ToObject<List<int>>();
            socketModel.resultGameData.symbolsToEmit = gameData["symbolsToEmit"].ToObject<List<List<string>>>();
            socketModel.resultGameData.WinAmout = gameData["WinAmout"].ToObject<double>();
            socketModel.resultGameData.freeSpins = gameData["freeSpins"].ToObject<double>();
            socketModel.resultGameData.jackpot = gameData["jackpot"].ToObject<double>();
            socketModel.resultGameData.isBonus = gameData["isBonus"].ToObject<bool>();
            socketModel.resultGameData.BonusStopIndex = gameData["BonusStopIndex"].ToObject<double>();
        }

    }
    private void initReq(SocketIOResponse iOResponse)
    {
        print("called");

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

    public void EmitSpin()
    {

        var obj = new { Data = new { currentBet = 0, currentLines = 20, spins = 1 }, id = "SPIN" };
        string json = JsonConvert.SerializeObject(obj);
        Debug.Log(json);
        socket.Emit("message", json);
    }
}




// [Serializable]
// public class GameData
// {
//     public List<List<string>> ResultReel { get; set; }
//     public List<object> LinesToEmit { get; set; }
//     public List<object> SymbolsToEmit { get; set; }
//     public int WinAmout { get; set; }
//     public int FreeSpins { get; set; }
//     public int Jackpot { get; set; }
//     public bool IsBonus { get; set; }
//     public int BonusStopIndex { get; set; }
//     public List<object> BonusResult { get; set; }
// }

// [Serializable]
// public class PlayerData
// {
//     public double Balance { get; set; }
//     public int HaveWon { get; set; }
//     public int CurrentWining { get; set; }
// }

// public class Message
// {
//     public GameData GameData { get; set; }
//     public PlayerData PlayerData { get; set; }

//     public UIData UIData { get; set; }
//     public string Username { get; set; }
// }

// [Serializable]
// public class Root
// {
//     public string Id { get; set; }
//     public Message Message { get; set; }
// }

// [Serializable]
// public class UIData
// {
//     public Paylines paylines { get; set; }
//     public List<string> spclSymbolTxt { get; set; }
//     public string ToULink { get; set; }
//     public string PopLink { get; set; }
// }

// [Serializable]
// public class Paylines
// {
//     public List<Symbol> symbols { get; set; }
// }

// [Serializable]
// public class Symbol
// {
//     public int ID { get; set; }
//     public string Name { get; set; }
//     [JsonProperty("multiplier")]
//     public object MultiplierObject { get; set; }

//     // This property will hold the properly deserialized list of lists of integers
//     [JsonIgnore]
//     public List<List<int>> Multiplier { get; private set; }

//     // Custom deserialization method to handle the conversion
//     [OnDeserialized]
//     internal void OnDeserializedMethod(StreamingContext context)
//     {
//         // Handle the case where multiplier is an object (empty in JSON)
//         if (MultiplierObject is JObject)
//         {
//             Multiplier = new List<List<int>>();
//         }
//         else
//         {
//             // Deserialize normally assuming it's an array of arrays
//             Multiplier = JsonConvert.DeserializeObject<List<List<int>>>(MultiplierObject.ToString());
//         }
//     }
//     public object defaultAmount { get; set; }
//     public object symbolsCount { get; set; }
//     public object increaseValue { get; set; }
//     public int freeSpin { get; set; }
// }

