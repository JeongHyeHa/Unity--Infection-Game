using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance { get; private set; }

    [SerializeField] private Material highlightMaterial;
    private Dictionary<GameObject, NPCData> npcDataDict = new Dictionary<GameObject, NPCData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy(gameObject);
        }
    }

    public void RegisterNPC(GameObject npcObject, Person person, Renderer objectRenderer)
    {
        if (npcDataDict.ContainsKey(npcObject))
        {
            npcDataDict[npcObject] = new NPCData(person, objectRenderer, objectRenderer.materials);
        }
        else
        {
            npcDataDict.Add(npcObject, new NPCData(person, objectRenderer, objectRenderer.materials));
        }
    }

    public void HighlightNPC(GameObject npcObject)
    {
        if (npcDataDict.TryGetValue(npcObject, out NPCData npcData))
        {
            if (npcData.ObjectRenderer != null)
            {
                Material[] mats = new Material[npcData.OriginalMaterials.Length];
                for (int i = 0; i < npcData.OriginalMaterials.Length; i++)
                {
                    mats[i] = highlightMaterial;
                }
                npcData.ObjectRenderer.materials = mats;
            }
        }
    }

    public void UnhighlightNPC(GameObject npcObject)
    {
        if (npcDataDict.TryGetValue(npcObject, out NPCData npcData))
        {
            if (npcData.ObjectRenderer != null)
            {
                npcData.ObjectRenderer.materials = npcData.OriginalMaterials;
            }
        }
    }
}

public class NPCData
{
    public Person Person { get; }
    public Renderer ObjectRenderer { get; }
    public Material[] OriginalMaterials { get; }

    public NPCData(Person person, Renderer objectRenderer, Material[] originalMaterials)
    {
        Person = person;
        ObjectRenderer = objectRenderer;
        OriginalMaterials = originalMaterials;
    }
}
