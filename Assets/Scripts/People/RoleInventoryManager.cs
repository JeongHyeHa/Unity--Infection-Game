using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class RoleInventoryManager
{
    // Role에 따른 공통 Inventory를 관리하는 딕셔너리
    private static Dictionary<Role, Dictionary<string, Item>> roleInventories = new Dictionary<Role, Dictionary<string, Item>>();

    // 초기화 시 모든 Role에 대한 Inventory 생성
    static RoleInventoryManager()
    {
        InitializeRoleInventories();
    }

    // 각 Role에 대해 아이템을 초기화
    private static void InitializeRoleInventories()
    {
        foreach (Role role in System.Enum.GetValues(typeof(Role)))
        {
            roleInventories[role] = new Dictionary<string, Item>();
            foreach (var item in ItemManager.InitItems())
            {
                roleInventories[role].Add(item.itemName, item.Clone());
            }
        }
    }

    // Role에 따른 Inventory 반환
    public static Dictionary<string, Item> GetInventoryByRole(Role role)
    {
        return roleInventories[role];
    }
}
