using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIController : MonoBehaviour
{
    [SerializeField] internal Button spinButton;
    [SerializeField] internal Button lineBetButton;
    [SerializeField] internal Button autoSpinButton;
    [SerializeField] internal TMP_Text autoSpinInput;
    [SerializeField] internal Button autoSpinStop;

    [SerializeField] internal Button closeButton;
    [SerializeField] internal TMP_InputField autoSpinInputField;

    [Header("BetInfo")]
    [SerializeField] private TMP_Text linesText;
    [SerializeField] private TMP_Text betPerLineText;
    [SerializeField] private TMP_Text totalBetText;

    [Header("player data")]
    [SerializeField] private TMP_Text playerBalance;
    [SerializeField] private TMP_Text playerCurrentWining;



    internal void UpdateBetLineInfo(int linetextDefalut, double betTextDefault)
    {
        linesText.text = linetextDefalut.ToString();
        betPerLineText.text = betTextDefault.ToString();
        totalBetText.text = (linetextDefalut * betTextDefault).ToString();
    }


    internal void UpdatePlayerData(PlayerData playerData)
    {

        playerBalance.text = playerData.Balance.ToString();
        playerCurrentWining.text = playerData.CurrentWining.ToString();

    }

    internal void RemoveButtonListeners(){
        lineBetButton.onClick.RemoveAllListeners();
        spinButton.onClick.RemoveAllListeners();
        autoSpinButton.onClick.RemoveAllListeners();

    }


 

    internal void ToggleButtons(bool toggle)
    {

        spinButton.interactable = toggle;
        lineBetButton.interactable = toggle;
        autoSpinButton.interactable = toggle;
        autoSpinInputField.interactable = toggle;
    }
    internal void UpdateLines(string text)
    {

        linesText.text = text;
    }

    internal void UpdateBetLineInfo(int v, Func<double> changeLineBet)
    {
        throw new NotImplementedException();
    }
}
