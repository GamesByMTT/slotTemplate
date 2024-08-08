using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class SlotView : MonoBehaviour
{
    [Header("slots")]
    [SerializeField] private Transform[] slots;
    [SerializeField] private GameObject IconPrefab;
    [SerializeField] private Sprite[] spriteList;
    [SerializeField] private List<Reel> resultMatrix = new List<Reel>(3);
    [SerializeField] private SlotController slotController;
    [SerializeField] private Vector2 iconSize;
    [SerializeField] private float spacing = 30;

    [SerializeField] private Vector2 slotMatrix;

    [Header("payline")]
    [SerializeField] private GameObject lineObject;
    [SerializeField] private Transform lineCanvas;
    [SerializeField] private Vector2 InitialLinePosition;
    [SerializeField] private int x_Distance;
    [SerializeField] private int y_Distance;
    [SerializeField] private List<GameObject> wininglines;

    [Header("sprite animation")]
    [SerializeField] private Sprite[] symbol0SpriteList;
    [SerializeField] private Sprite[] symbol1SpriteList;
    [SerializeField] private Sprite[] symbol2SpriteList;
    [SerializeField] private Sprite[] symbol3SpriteList;
    [SerializeField] private Sprite[] symbol4SpriteList;
    [SerializeField] private Sprite[] symbol5SpriteList;
    [SerializeField] private Sprite[] symbol6SpriteList;
    [SerializeField] private Sprite[] symbol7SpriteList;
    [SerializeField] private Sprite[] symbol8SpriteList;
    [SerializeField] private Sprite[] symbol9SpriteList;
    [SerializeField] private Sprite[] symbol10SpriteList;
    [SerializeField] private Sprite[] symbol11SpriteList;
    [SerializeField] private Sprite[] symbol12SpriteList;
    [SerializeField] private Sprite[] symbol13SpriteList;

    internal void PopulateReels(List<List<int>> iconsId = null)
    {//TODO : CHane Naming Conventions
        clearLine();
        for (int i = 0; i < slotMatrix.x; i++)
        {
            if (iconsId != null)
                PopulateReel(i, iconsId[iconsId.Count - i - 1]);
            else
                PopulateReel(i);

        }
    }


    void PopulateReel(int index, List<int> Ids = null)
    {
        for (int i = 0; i < slotMatrix.y; i++)
        {
            if (Ids != null)
            {
                // int id = int.Parse(Ids[i]);
                resultMatrix[((int)slotMatrix.x-1)-index].reelIcons[i].image.sprite = spriteList[Ids[i]];
                resultMatrix[((int)slotMatrix.x-1)-index].reelIcons[i].id = Ids[i];
                Debug.Log("Result :" + resultMatrix[((int)slotMatrix.x-1)-index]);
            }
            else
            {
                Image image = Instantiate(IconPrefab, slots[i]).GetComponent<Image>();
                ImageAnimation imageAnimation = Instantiate(IconPrefab, slots[i]).GetComponent<ImageAnimation>();
                int id=UnityEngine.Random.Range(0, spriteList.Length);
                image.sprite = spriteList[id];
                Icon icon= new Icon(image,id,imageAnimation);
                Debug.Log("ICONS : " + icon);
                Debug.Log("icon id "+icon.id);
                resultMatrix[(int)slotMatrix.x-1-index].reelIcons.Add(icon);
                icon.image.transform.localPosition = new Vector3(0, (iconSize.y + spacing) * index, 0);
            }
        }
    }

    internal void GeneratePayLine(List<List<int>> y_index, List<int> lineIndexs)
    {

        for (int i = 0; i < lineIndexs.Count; i++)
        {
            GameObject lineObj = Instantiate(lineObject, lineCanvas);
            wininglines.Add(lineObj);
            lineObj.transform.localPosition = new Vector2(InitialLinePosition.x, InitialLinePosition.y);
            UILineRenderer line = lineObj.GetComponent<UILineRenderer>();
            for (int j = 0; j < y_index[lineIndexs[i]].Count; j++)
            {
                var points = new Vector2() { x = j * x_Distance, y = y_index[lineIndexs[i]][j] * -y_Distance };
                var pointlist = new List<Vector2>(line.Points);
                pointlist.Add(points);
                line.Points = pointlist.ToArray();
            }//TODO : Make it Seperate
            var newpointlist = new List<Vector2>(line.Points);
            newpointlist.RemoveAt(0);
            line.Points = newpointlist.ToArray();
        }

    }

    void clearLine()
    {
        for (int i = 0; i < wininglines.Count; i++)
        {
            Destroy(wininglines[i]);
        }
    }

    internal void PopulateIconAnimation(List<List<string>> pos, List<List<int>> result=null)
    {

        Debug.Log("pos :" + JsonConvert.SerializeObject(pos));
        List<string> flatList = new List<string>();
        foreach (var innerList in pos)
        {
            foreach (var str in innerList)
            {
                flatList.Add(str);
            }
        }
        Debug.Log("pos :" + JsonConvert.SerializeObject(flatList));

        for (int i = 0; i < flatList.Count; i++)
        {
            string[] numbers = flatList[i].Split(',');
            var symbol =  resultMatrix[int.Parse(numbers[1])].reelIcons[int.Parse(numbers[0])];
            int id = symbol.id;
            Sprite[] animations=GetSpriteList(id);
            symbol.imageAnimation.textureArray=animations.ToList();
            symbol.imageAnimation.StartAnimation();
        }

    }

    private Sprite[] GetSpriteList(int id)
{
    switch (id)
    {
        case 0:  return symbol0SpriteList;
        case 1: return symbol1SpriteList;
        case 2: return symbol2SpriteList;
        case 3: return symbol3SpriteList;
        case 4: return symbol4SpriteList;
        case 5: return symbol5SpriteList;
        case 6: return symbol6SpriteList;
        case 7: return symbol7SpriteList;
        case 8: return symbol8SpriteList;
        case 9: return symbol9SpriteList;
        case 10: return symbol10SpriteList;
        case 11: return symbol11SpriteList;
        case 12: return symbol12SpriteList;
        case 13: return symbol13SpriteList;
        default:
            Debug.LogError("ID out of range.");
            return null;
    }
}

}

