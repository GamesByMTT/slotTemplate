using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using System;
public class SlotController : MonoBehaviour
{
    [SerializeField] private SocketController socketController;
    [SerializeField] private SlotView slotView;
    [SerializeField] private int betIndex;

    private List<Tween> alltweens = new List<Tween>();
    void Start()
    {
        slotView.PopulateReels();



        //grid height=no icons*iconheight + (no icons-1)*space
        //slot pos=slot transformheight/2+space
    }

    internal bool  PopulateSLot(ResultGameData resultGameData, InitGameData initGameData)
    {
        slotView.PopulateReels(resultGameData.ResultReel);
        StopSpinAnimation();
        if (resultGameData.linesToEmit.Count > 0)
        {
            slotView.GeneratePayLine(initGameData.Lines, socketController.socketModel.resultGameData.linesToEmit);
            slotView.PopulateIconAnimation(resultGameData.symbolsToEmit);
            return true;
        }
        // KillAllTween();
        return false;
    }

    internal void checkforWin(){
        
    }
    internal void DepopulateAnimation()
    {

        for (int i = 0; i < slotView.animatedIcons.Count; i++)
        {
            slotView.animatedIcons[i].StopAnimation();
        }
        slotView.animatedIcons.Clear();
    }

    internal void StartSpinAnimation()
    {
        for (int i = 0; i < slotView.slots.Length; i++)
        {
            // slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
            Tweener tweener = slotView.slots[i].DOLocalMoveY(-1200 + 200, 0.075f).SetLoops(-1, LoopType.Restart).SetDelay(0).SetEase(Ease.InOutBounce);
            tweener.Play();
            alltweens.Add(tweener);
            Debug.Log("sasas" + alltweens.Count);
        }

    }

    internal async void StopSpinAnimation()
    {
        // int tweenpos = (reqpos * slotView.iconSize.x) -  slotView.iconSize.x;
        for (int i = 0; i < slotView.slots.Length; i++)
        {
            alltweens[i].Pause();
            alltweens[i] = slotView.slots[i].DOLocalMoveY(-360 + 70 + 70, 0.5f).SetEase(Ease.OutElastic);
            await Task.Delay(TimeSpan.FromSeconds(0.1f));

        }        

    }

    internal void KillAllTween()
    {
        for (int i = 0; i < alltweens.Count; i++)
        {
            alltweens[i].Kill();
        }
        alltweens.Clear();

    }
}
