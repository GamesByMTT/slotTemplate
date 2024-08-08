using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SocketController socketController;
    [SerializeField] private SlotController slotController;
    [SerializeField] private UIController uIController;
    [SerializeField] private int autoSpinCount=5;

    
    [SerializeField] private float delayTime = 2f;
    void Awake()
    {
        socketController.InitiateSocket(OnInit, OnSpinEnd);
    }

    void OnInit()
    {
        uIController.OnspinBinder(OnSpinStart);
        uIController.OnlinebetBinder(socketController.ChangeLineBet);
        uIController.OnAutoSpinBinder(()=>{OnSpinStart();});
        uIController.UpdateBetLineInfo(socketController.socketModel.initGameData.Lines.Count, socketController.socketModel.initGameData.Bets[0]);
        uIController.UpdatePlayerData(socketController.socketModel.PlayerData);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnSpinStart();
        }
    }

    void OnSpinStart()
    {
        var spinData = new { Data = new { currentBet = socketController.socketModel.currentBetIndex, currentLines = 20, spins = 1 }, id = "SPIN" };
        string spinJson = JsonConvert.SerializeObject(spinData);
        socketController.SendData(spinJson);
        uIController.ToggleButtons(false);

        if (autoSpinCount > 0)
        autoSpinCount --;
    }

    async void OnSpinEnd()
    {
        bool isMatched = slotController.PopulateSLot(socketController.socketModel.resultGameData, socketController.socketModel.initGameData);
        uIController.UpdatePlayerData(socketController.socketModel.PlayerData);
    
        if (autoSpinCount > 0)
        {
            Debug.Log("AutoSpinCount : "+autoSpinCount);
            if(isMatched)
            delayTime = 3f;

            await Task.Delay(TimeSpan.FromSeconds(delayTime));
            OnSpinStart();
            delayTime = 3f;
            return;
        }
        uIController.ToggleButtons(true);

      

    }

  

}
