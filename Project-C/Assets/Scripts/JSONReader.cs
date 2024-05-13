using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class JSONReader : MonoBehaviour
{
    [System.Serializable]
    public class Character
    {
        public string name;
        public string description;
        public Moveset moveset;
    }

    [System.Serializable]
    public class Moveset
    {
        public Attack lightAttack;
        public Attack heavyAttack;
        public Attack trademarkAttack;
    }

    [System.Serializable]
    public class CharacterData
    {
        public List<Character> characters;
    }

    public TextAsset jsonFile;
    private CharacterData characterData;

    void Awake()
    {
        characterData = JsonUtility.FromJson<CharacterData>(jsonFile.text);
    }

    public Moveset GetMovesetByName(string characterName)
    {
        foreach (var character in characterData.characters)
        {
            if (character.name == characterName)
            {
                return character.moveset;
            }
        }
        return null;
    }
}
