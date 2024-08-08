using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIController : MonoBehaviour
{
    [SerializeField] private Button spinButton;
    [SerializeField] private Button lineBetButton;
    [SerializeField] private Button autoSpinButton;
    [SerializeField] private TMP_Text autoSpinInput;
    [SerializeField] private TMP_InputField autoSpinInputField;

    [Header("BetInfo")]
    [SerializeField] private TMP_Text linesText;
    [SerializeField] private TMP_Text betPerLineText;
    [SerializeField] private TMP_Text totalBetText;

    [Header("player data")]
    [SerializeField] private TMP_Text playerBalance;
    [SerializeField] private TMP_Text playerCurrentWining;



    internal void UpdateBetLineInfo(int linetextDefalut, double betTextDefault){
        linesText.text=linetextDefalut.ToString();
        betPerLineText.text=betTextDefault.ToString();
        totalBetText.text=(linetextDefalut*betTextDefault).ToString();
    }
    internal void OnspinBinder(Action action)
    {
        spinButton.onClick.RemoveAllListeners();
        spinButton.onClick.AddListener(delegate { action(); });
    }

    internal void UpdatePlayerData(PlayerData playerData){

        playerBalance.text=playerData.Balance.ToString();
        playerCurrentWining.text=playerData.CurrentWining.ToString();

    }

    internal void OnlinebetBinder(Func<double> action){
        lineBetButton.onClick.RemoveAllListeners();
        lineBetButton.onClick.AddListener(delegate{   
            
            UpdateBetLineInfo(20,action());
            Debug.Log("OnlinebetBinder");
        
        });

    }

    internal void OnAutoSpinBinder(Action action){

        autoSpinButton.onClick.RemoveAllListeners();
        autoSpinButton.onClick.AddListener(delegate{ 

            action();
        });
    }

    internal void ToggleButtons(bool toggle){

    spinButton.interactable=toggle;
    lineBetButton.interactable=toggle;
    autoSpinButton.interactable=toggle;
    autoSpinInputField.interactable=toggle;
    }
    internal void UpdateLines(string text)
    {

        linesText.text=text;
    }
}
