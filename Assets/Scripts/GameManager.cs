using System;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using System.Text.RegularExpressions;
public class GameManager : MonoBehaviour
{
    // [SerializeField] private SocketController socketController;
    // [SerializeField] private SocketControllerv2 socketControllerv2;
    [SerializeField] private SocketControllerv3 socketControllerv3;
    [SerializeField] private SlotController slotController;
    [SerializeField] private UIController uIController;
    [SerializeField] private int autoSpinCount = 0;
    [SerializeField] private int freeSpinsCount=0;
    [SerializeField] private float delayTime = 2f;



    private void Start()
    {

        #if UNITY_EDITOR
        StartGame(null);
        #endif
    }
    internal void StartGame(string token)
    {
        Debug.Log("called to satrt game with token"+token);
        // Start the coroutine to manage the initiation
        // StartCoroutine(InitializeSocketCoroutine());
        // socketControllerv2.InitiateConnection();
        socketControllerv3.InitiateSocket(token);
        socketControllerv3.onInit = OnInit;
        socketControllerv3.onSpin = OnSpinEnd;
        uIController.closeButton.onClick.AddListener(socketControllerv3.CloseSocket);
        // socketControllerv2.onInit=OnInit;
        // socketControllerv2.onSpin=OnSpinEnd;




    }



    internal void OnInit()
    {
        // uIController.RemoveButtonListeners();
        uIController.spinButton.onClick.AddListener(delegate { OnSpinStart(); });


        uIController.lineBetButton.onClick.AddListener(delegate
        {
            uIController.UpdateBetLineInfo(socketControllerv3.socketModel.initGameData.Lines.Count, socketControllerv3.ChangeLineBet());

        });

        uIController.autoSpinStop.onClick.AddListener(StopAutoSpin);

        uIController.autoSpinButton.onClick.AddListener(delegate
        {
            autoSpinCount = int.Parse(uIController.autoSpinInputField.text);
            if (autoSpinCount > 0)
            {
                OnSpinStart();

                uIController.autoSpinButton.gameObject.SetActive(false);
            }
        });

        uIController.UpdateBetLineInfo(socketControllerv3.socketModel.initGameData.Lines.Count, socketControllerv3.socketModel.initGameData.Bets[0]);
        uIController.UpdatePlayerData(socketControllerv3.socketModel.PlayerData);
    }

    void StopAutoSpin()
    {
        autoSpinCount = 0;
        uIController.autoSpinButton.gameObject.SetActive(true);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && uIController.spinButton.interactable)
        {
            OnSpinStart();
        }
    }

    void OnSpinStart()
    {
        var spinData = new { data = new { currentBet = socketControllerv3.socketModel.currentBetIndex , currentLines = 20, spins = 1 }, id = "SPIN" };
        string spinJson = JsonConvert.SerializeObject(spinData);
        Debug.Log("spin pressed");
        socketControllerv3.SendMessage("message", spinJson);
        uIController.ToggleButtons(false);
        slotController.StartSpinAnimation();
        if (autoSpinCount > 0)
        {
            autoSpinCount--;
            uIController.autoSpinInputField.text = autoSpinCount.ToString();
        }
        if (freeSpinsCount > 0)
        {
            freeSpinsCount--;
        }
    }

    internal void OnSpinEnd()
    {
        StartCoroutine(SpinEndRoutine());
    }

    IEnumerator SpinEndRoutine(){
        Debug.Log("spin end enter.....");
        bool isMatched=false;

        yield return slotController.PopulateSlotAndCheckResult(socketControllerv3.socketModel.resultGameData, 
        socketControllerv3.socketModel.initGameData, 
        (spins) => freeSpinsCount += spins,
        (match)=>isMatched=match
        );

        uIController.UpdatePlayerData(socketControllerv3.socketModel.PlayerData);

        if (autoSpinCount > 0)
        {

            Debug.Log("AutoSpinCount : " + autoSpinCount);
            if (isMatched)
                delayTime = 3f;
            yield return new WaitForSeconds(delayTime);
            OnSpinStart();
            delayTime = 2f;

            yield break;
        }

        uIController.ToggleButtons(true);
        Debug.Log("spin end exit.....");

    }

    // IEnumerator OnSpinEndRoutine(){
    //     yield return new WaitForSeconds(1f);
    //     bool isMatched = slotController.PopulateSLot(socketControllerv3.socketModel.resultGameData, socketControllerv3.socketModel.initGameData);
    //     uIController.UpdatePlayerData(socketControllerv3.socketModel.PlayerData);

    //     if (autoSpinCount > 0)
    //     {
    //         Debug.Log("AutoSpinCount : " + autoSpinCount);
    //         if (isMatched)
    //             delayTime = 3f;
    //         yield return new WaitForSeconds(delayTime);
    //         OnSpinStart();
    //         delayTime = 3f;

    //         yield break;
    //     }

    //     uIController.ToggleButtons(true);

    // }


}
