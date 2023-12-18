using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    //informacion que tendra cada item o plantilla
    public string ItemName;
    public Sprite ItemImage;
    public string ItemDescription;
    //public double price;
    public GameObject Item3DModel;
}
