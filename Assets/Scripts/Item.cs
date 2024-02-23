using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName ="ScriptableObject/Item")]
public class Item : ScriptableObject
{
    new public string name = "New Item";
    public Sprite icon = null;
    public Color iconColor;
    public bool stackable = false;
    //    public ItemType type;
    //    public ActionType actionType;

    //}

    //public enum ItemType
    //{
    //    Tool,
    //    Collectible,
    //    Artifact
    //}

    //public enum ActionType
    //{

}
