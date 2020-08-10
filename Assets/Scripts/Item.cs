[System.Serializable]
public class Item
{
    public string img_path;
    public string item_id;
    public string name;
    public int price_gold;

    public Item(string img_path, string item_id, string name, int price_gold)
    {
        this.img_path = img_path;
        this.item_id = item_id;
        this.name = name;
        this.price_gold = price_gold;
    }
}
