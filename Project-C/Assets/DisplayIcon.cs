using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayIcon : MonoBehaviour
{
    public Chracter character;
    
    public TMPro.TextMeshProUGUI text;
    public RawImage icon;

    private void Start()
    {
        text.text = character.characterName;
        icon.texture = character.icon;
    }
}
