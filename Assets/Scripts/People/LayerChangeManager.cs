using System.Collections.Generic;
using UnityEngine;

public class LayerChangeManager
{
    public string[] layers = { "Floor 2 L", "Floor 2 R", "Floor 3 L", "Floor 3 R", "Floor 4 L", "Floor 4 R", "Floor 5 L", "Floor 5 R", "Floor 1 L", "Floor 1 R" }; // 추가되는 Layer는 여기에 넣기
    public Dictionary<int, int[]> layerMapping;

    public void Init()
    {
        layerMapping = new Dictionary<int, int[]>()
        {
            { LayerMask.NameToLayer("Floor 1"), new int[] { LayerMask.NameToLayer("Floor 2"), LayerMask.NameToLayer("Floor 1") } },
            { LayerMask.NameToLayer("Floor 2"), new int[] { LayerMask.NameToLayer("Floor 3"), LayerMask.NameToLayer("Floor 2") } },
            { LayerMask.NameToLayer("Floor 3"), new int[] { LayerMask.NameToLayer("Floor 4"), LayerMask.NameToLayer("Floor 3") } },
            { LayerMask.NameToLayer("Floor 4"), new int[] { LayerMask.NameToLayer("Floor 5"), LayerMask.NameToLayer("Floor 4") } },
            { LayerMask.NameToLayer("Floor 1 L"), new int[] { LayerMask.NameToLayer("Floor 1 L"), LayerMask.NameToLayer("Floor 1") } },
            { LayerMask.NameToLayer("Floor 1 R"), new int[] { LayerMask.NameToLayer("Floor 1 R"), LayerMask.NameToLayer("Floor 1") } },
            { LayerMask.NameToLayer("Floor 2 L"), new int[] { LayerMask.NameToLayer("Floor 2 L"), LayerMask.NameToLayer("Floor 2") } },
            { LayerMask.NameToLayer("Floor 2 R"), new int[] { LayerMask.NameToLayer("Floor 2 R"), LayerMask.NameToLayer("Floor 2") } },
            { LayerMask.NameToLayer("Floor 3 L"), new int[] { LayerMask.NameToLayer("Floor 3 L"), LayerMask.NameToLayer("Floor 3") } },
            { LayerMask.NameToLayer("Floor 3 R"), new int[] { LayerMask.NameToLayer("Floor 3 R"), LayerMask.NameToLayer("Floor 3") } },
            { LayerMask.NameToLayer("Floor 4 L"), new int[] { LayerMask.NameToLayer("Floor 4 L"), LayerMask.NameToLayer("Floor 4") } },
            { LayerMask.NameToLayer("Floor 4 R"), new int[] { LayerMask.NameToLayer("Floor 4 R"), LayerMask.NameToLayer("Floor 4") } },
            { LayerMask.NameToLayer("Floor 5 L"), new int[] { LayerMask.NameToLayer("Floor 5 L"), LayerMask.NameToLayer("Floor 5") } },
            { LayerMask.NameToLayer("Floor 5 R"), new int[] { LayerMask.NameToLayer("Floor 5 R"), LayerMask.NameToLayer("Floor 5") } },
        };
    }

    public void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (child != null)
            {
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }
    }
}
