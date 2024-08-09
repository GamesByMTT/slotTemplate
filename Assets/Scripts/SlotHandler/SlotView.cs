using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class SlotView : MonoBehaviour
{
    [Header("slots")]
    [SerializeField] internal Transform[] slots;
    [SerializeField] private GameObject IconPrefab;
    [SerializeField] private SpriteAtlas spriteAtlas;
    [SerializeField] private List<Reel> resultMatrix = new List<Reel>(3);
    [SerializeField] internal List<Icon> animatedIcons = new List<Icon>();
    [SerializeField] private SlotController slotController;
    [SerializeField] internal Vector2 iconSize;
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
    [SerializeField] private SpriteAtlas symbol0Atlas;
    [SerializeField] private SpriteAtlas symbol1Atlas;
    [SerializeField] private SpriteAtlas symbol2Atlas;
    [SerializeField] private SpriteAtlas symbol3Atlas;
    [SerializeField] private SpriteAtlas symbol4Atlas;
    [SerializeField] private SpriteAtlas symbol5Atlas;
    [SerializeField] private SpriteAtlas symbol6Atlas;
    [SerializeField] private SpriteAtlas symbol7Atlas;
    [SerializeField] private SpriteAtlas symbol8Atlas;
    [SerializeField] private SpriteAtlas symbol9Atlas;
    [SerializeField] private SpriteAtlas symbol10Atlas;
    [SerializeField] private SpriteAtlas symbol11Atlas;
    [SerializeField] private SpriteAtlas symbol12Atlas;
    [SerializeField] private SpriteAtlas symbol13Atlas;
    internal void PopulateReels()
    {
        clearLine();


        for (int i = 0; i < 15; i++)
        {
            PopulateRow(i);
        }

    }

    internal void PopulateReels(List<List<int>> iconsId)
    {
        clearLine();


        for (int i = 0; i < slotMatrix.x; i++)
        {
            PopulateRow(i, iconsId[iconsId.Count - i - 1]);

        }


    }


    void PopulateRow(int index)
    {
        for (int i = 0; i < slotMatrix.y; i++)
        {
            Image image = Instantiate(IconPrefab, slots[i]).GetComponent<Image>();
            ImageAnimation imageAnimation = image.GetComponent<ImageAnimation>();
            int id = UnityEngine.Random.Range(0, spriteAtlas.spriteCount);
            image.sprite = spriteAtlas.GetSprite(id.ToString());
            Icon icon = new Icon(image, id, imageAnimation);
            Debug.Log("ICONS : " + i + "," + index);
            if (index < (int)slotMatrix.x)
                resultMatrix[(int)slotMatrix.x - 1 - index].reelIcons.Add(icon);
            icon.image.transform.localPosition = new Vector3(0, (iconSize.y + spacing) * index, 0);

        }
    }

    void PopulateRow(int index, List<int> Ids)
    {
        for (int i = 0; i < slotMatrix.y; i++)
        {
            resultMatrix[(int)slotMatrix.x - 1 - index].reelIcons[i].image.sprite = spriteAtlas.GetSprite(Ids[i].ToString());
            resultMatrix[(int)slotMatrix.x - 1 - index].reelIcons[i].id = Ids[i];
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
            var pointlist = new List<Vector2>();
            for (int j = 0; j < y_index[lineIndexs[i]].Count; j++)
            {
                var points = new Vector2() { x = j * x_Distance, y = y_index[lineIndexs[i]][j] * -y_Distance };
                pointlist.Add(points);
            }
            line.Points = pointlist.ToArray();
        }

    }


    void clearLine()
    {
        for (int i = 0; i < wininglines.Count; i++)
        {
            Destroy(wininglines[i]);
        }
    }

    internal void PopulateIconAnimation(List<List<string>> pos)
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
            var symbol = resultMatrix[int.Parse(numbers[1])].reelIcons[int.Parse(numbers[0])];
            int id = symbol.id;
            symbol.StartAnimation(GetSpriteList(id));
            animatedIcons.Add(symbol);
        }

    }

private Sprite[] GetSpriteList(int id)
{
    SpriteAtlas atlas= new SpriteAtlas();

    switch (id)
    {
        case 0: atlas = symbol0Atlas; break;
        case 1: atlas = symbol1Atlas; break;
        case 2: atlas = symbol2Atlas; break;
        case 3: atlas = symbol3Atlas; break;
        case 4: atlas = symbol4Atlas; break;
        case 5: atlas = symbol5Atlas; break;
        case 6: atlas = symbol6Atlas; break;
        case 7: atlas = symbol7Atlas; break;
        case 8: atlas = symbol8Atlas; break;
        case 9: atlas = symbol9Atlas; break;
        case 10: atlas = symbol10Atlas; break;
        case 11: atlas = symbol11Atlas; break;
        case 12: atlas = symbol12Atlas; break;
        case 13: atlas = symbol13Atlas; break;
    }

    if (atlas == null)
    {
        return new Sprite[0];
    }

    Sprite[] sprites = new Sprite[atlas.spriteCount];
    atlas.GetSprites(sprites);
    Array.Sort(sprites, (sprite1, sprite2) => string.Compare(sprite1.name, sprite2.name));
    return sprites;
}

}

