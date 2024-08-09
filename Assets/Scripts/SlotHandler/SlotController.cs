using System.Collections.Generic;
using UnityEngine;


public class SlotController : MonoBehaviour
{
    [SerializeField] private SocketController socketController;
    [SerializeField] private SlotView slotView;
    [SerializeField] private int betIndex;


    void Start()
    {
        slotView.PopulateReels();
    }

    internal bool PopulateSLot(ResultGameData resultGameData, InitGameData initGameData)
    {
        slotView.PopulateReels(resultGameData.ResultReel);
        if (resultGameData.linesToEmit.Count > 0)
        {
            slotView.GeneratePayLine(initGameData.Lines, socketController.socketModel.resultGameData.linesToEmit);
            slotView.PopulateIconAnimation(resultGameData.symbolsToEmit);
            return true;
        }
        return false;
    }

    internal void DepopulateAnimation()
    {

        for (int i = 0; i < slotView.animatedIcons.Count; i++)
        {
            slotView.animatedIcons[i].StopAnimation();
        }
        slotView.animatedIcons.Clear();
    }
}
