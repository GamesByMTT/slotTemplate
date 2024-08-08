using System;
using System.Collections.Generic;
using UnityEngine.UI;

[Serializable]
public class Reel
{
    public List<Icon> reelIcons = new List<Icon>();
    public Reel(List<Icon> ReelIcons){
        this.reelIcons=ReelIcons;
    }

}

[Serializable]
public class Icon{

    public int id;
    public Image image;

    public ImageAnimation imageAnimation;
    public Icon(Image Image, int Id, ImageAnimation ImageAnimation){
        this.image=Image;
        this.id=Id;
        this.imageAnimation=ImageAnimation;
    }
     public void stopAnimation(){
        this.imageAnimation.StopAnimation();
        this.imageAnimation.textureArray.Clear();

     }
    
}