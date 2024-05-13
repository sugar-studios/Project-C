using System;
using System.Collections.Generic;
using UnityEngine;

public static class JSONUtilityHelper
{
    [Serializable]
    private class AttackData
    {
        public string type;
        public float damage;
        public float knockback;
        public float knockbackScaling;
        public float startup;
        public float active;
        public float endlag;
        public string id;
        public int priority;
        public float[] hitboxSize;
        public float[] rotation;
    }

    [Serializable]
    private class AttacksContainer
    {
        public List<AttackData> attacks;
    }

    [Serializable]
    private class CharactersContainer
    {
        public Dictionary<string, AttacksContainer> Characters;
    }

    // Embedded JSON string
    private static string jsonString = @"
    {
      ""Characters"": {
        ""Default"": {
          ""attacks"": [
            {
              ""type"": ""Square"",
              ""damage"": 10,
              ""knockback"": 5,
              ""knockbackScaling"": 1.2,
              ""startup"": 0.1,
              ""active"": 0.5,
              ""endlag"": 0.2,
              ""id"": ""light"",
              ""priority"": 1,
              ""hitboxSize"": [1, 1, 1],
              ""rotation"": [0, 0, 0]
            },
            {
              ""type"": ""Square"",
              ""damage"": 20,
              ""knockback"": 10,
              ""knockbackScaling"": 1.5,
              ""startup"": 0.2,
              ""active"": 0.7,
              ""endlag"": 0.3,
              ""id"": ""heavy"",
              ""priority"": 2,
              ""hitboxSize"": [1, 1, 1],
              ""rotation"": [0, 0, 45]
            },
            {
              ""type"": ""Capsule"",
              ""damage"": 30,
              ""knockback"": 15,
              ""knockbackScaling"": 2.0,
              ""startup"": 0.3,
              ""active"": 1.0,
              ""endlag"": 0.4,
              ""id"": ""trademark"",
              ""priority"": 3,
              ""hitboxSize"": [2, 2, 2],
              ""rotation"": [0, 0, 90]
            }
          ]
        }
      }
    }";

    public static Dictionary<string, Attack> LoadAttacksForCharacter(string characterName)
    {
        CharactersContainer characters = JsonUtility.FromJson<CharactersContainer>(jsonString);
        if (characters == null || characters.Characters == null)
        {
            Debug.LogError("Invalid JSON structure in embedded string.");
            return null;
        }

        // Extract base character name from the GameObject's name (removes "(clone)" and any other suffixes)
        string baseCharacterName = characterName.Split('(')[0].Trim();

        if (!characters.Characters.ContainsKey(baseCharacterName))
        {
            Debug.LogError("No moveset found for character: " + baseCharacterName);
            return null;
        }

        AttacksContainer container = characters.Characters[baseCharacterName];
        Dictionary<string, Attack> attacks = new Dictionary<string, Attack>();
        foreach (AttackData data in container.attacks)
        {
            Attack.HitboxType hitboxType = (Attack.HitboxType)Enum.Parse(typeof(Attack.HitboxType), data.type);
            GameObject prefab = hitboxType == Attack.HitboxType.Square ? PlayerAttacker.SquareHitbox : PlayerAttacker.CapsuleHitbox;
            Vector3 size = new Vector3(data.hitboxSize[0], data.hitboxSize[1], data.hitboxSize[2]);
            Vector3 rotation = new Vector3(data.rotation[0], data.rotation[1], data.rotation[2]);

            Attack attack = new Attack(hitboxType, data.damage, data.knockback, data.knockbackScaling, data.startup, data.active, data.endlag, data.id, data.priority, prefab, size, rotation);
            attacks.Add(data.id, attack);
        }

        return attacks;
    }
}
