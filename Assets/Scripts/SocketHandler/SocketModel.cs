using System.Collections.Generic;
using System;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

internal class SocketModel
{
    // public Root root;
    // public GameData gameData;
    public PlayerData PlayerData;
    public UIData UIdata;

    public InitGameData initGameData;

    public ResultGameData resultGameData;

    internal SocketModel(){

        this.PlayerData= new PlayerData();
        this.UIdata= new UIData();
        this.initGameData= new InitGameData();
        this.resultGameData= new ResultGameData();
    }

}



[Serializable]
public class InitGameData
{
    public List<List<string>> Reel { get; set; }
    public List<List<int>> Lines { get; set; }
    public List<double> Bets { get; set; }
    public bool canSwitchLines { get; set; }
    public List<int> LinesCount { get; set; }
    public List<int> autoSpin { get; set; }
}

[Serializable]
public class ResultGameData
{
    public List<List<string>> ResultReel { get; set; }
    public List<int> linesToEmit { get; set; }
    public List<List<string>> symbolsToEmit { get; set; }
    public double WinAmout { get; set; }
    public double freeSpins { get; set; }
    public double jackpot { get; set; }
    public bool isBonus { get; set; }
    public double BonusStopIndex { get; set; }
}

[Serializable]
public class PlayerData
{
    public double Balance { get; set; }
    public int HaveWon { get; set; }
    public int CurrentWining { get; set; }
}

[Serializable]
public class UIData
{
    public Paylines paylines { get; set; }
    public List<string> spclSymbolTxt { get; set; }
    public string ToULink { get; set; }
    public string PopLink { get; set; }
}

[Serializable]
public class Paylines
{
    public List<Symbol> symbols { get; set; }
}

[Serializable]
public class Symbol
{
    public int ID { get; set; }
    public string Name { get; set; }
    [JsonProperty("multiplier")]
    public object MultiplierObject { get; set; }

    // This property will hold the properly deserialized list of lists of integers
    [JsonIgnore]
    public List<List<int>> Multiplier { get; private set; }

    // Custom deserialization method to handle the conversion
    [OnDeserialized]
    internal void OnDeserializedMethod(StreamingContext context)
    {
        // Handle the case where multiplier is an object (empty in JSON)
        if (MultiplierObject is JObject)
        {
            Multiplier = new List<List<int>>();
        }
        else
        {
            // Deserialize normally assuming it's an array of arrays
            Multiplier = JsonConvert.DeserializeObject<List<List<int>>>(MultiplierObject.ToString());
        }
    }
    public object defaultAmount { get; set; }
    public object symbolsCount { get; set; }
    public object increaseValue { get; set; }
    public int freeSpin { get; set; }
}

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

