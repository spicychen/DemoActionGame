using System.Collections.Generic;
using PlayFab.ClientModels;
using UnityEngine;
using System;
[System.Serializable]
public class Character
{
    public string img_path;
    public string character_id;
    public string character_type;
    public int attack;
    public int defence;
    public int max_health;
    public int current_health;

    public Character(string char_id, string char_type, int att, int defen, int max_h, int cur_h)
    {
        character_id = char_id;
        character_type = char_type;
        attack = att;
        defence = defen;
        max_health = max_h;
        current_health = cur_h;
    }

    public Character(string img_path, string char_id, string char_type)
    {
        this.img_path = img_path;
        character_id = char_id;
        character_type = char_type;

    }
    public static List<Character> ConvertToCharacters(List<CharacterResult> Characters)
    {
        List<Character> converted_characters = new List<Character>();
        if (Characters.Count > 0)
        {
            foreach (CharacterResult character in Characters)
            {
                var img_path = String.Format("CharacterSprites/{0}", character.CharacterType);
                //Debug.Log(img_path);
                converted_characters.Add(new Character(img_path, character.CharacterId, character.CharacterType));
            }
        }

        return converted_characters;
    }
}
