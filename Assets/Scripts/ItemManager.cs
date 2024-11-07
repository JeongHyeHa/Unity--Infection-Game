using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public string itemName;
    public bool isEquipped;
    public float protectionRate;

    public Item(string itemName, bool isEquipped, float protectionRate)
    {
        this.itemName = itemName;
        this.isEquipped = isEquipped;
        this.protectionRate = protectionRate;
    }

    // Clone 메서드로 개별 인스턴스를 생성
    public Item Clone()
    {
        return new Item(itemName, isEquipped, protectionRate);
    }
}

public class ItemManager
{
    public List<Item> items = InitItems();

    public static List<Item> InitItems()
    {
        return new List<Item>
        {
            new Item("Dental 마스크",false, 2f),
            new Item("N95 마스크",false, 3f),
            new Item("일회용 장갑",false, 2f),
            new Item("라텍스 장갑",false, 5f),
            new Item("의료용 헤어캡",false, 7f),
            new Item("의료용 고글",false, 7f),
            new Item("AP 가운",false, 7f),
            new Item("Level C",false, 7f)
        };
    }


}

