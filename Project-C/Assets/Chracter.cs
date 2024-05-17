using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "character")]
public class Chracter : ScriptableObject
{
    TextAsset moveset;
    public GameObject player;
    public Image icon;
    public string characterName;
}
