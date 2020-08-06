[System.Serializable]
public class Charater
{
    public string character_id;
    public string character_type;
    public int attack;
    public int defence;
    public int max_health;
    public int current_health;

    public Charater(string char_id, string char_type, int att, int defen, int max_h, int cur_h)
    {
        character_id = char_id;
        character_type = char_type;
        attack = att;
        defence = defen;
        max_health = max_h;
        current_health = cur_h;
    }
}
