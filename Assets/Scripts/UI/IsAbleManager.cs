using UnityEngine;

public class IsAbleManager : MonoBehaviour
{
    public static IsAbleManager Instance;
    private GameObject activeWindow;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool CanOpenNewWindow()
    {
        return activeWindow == null;
    }

    public void OpenWindow(GameObject window)
    {
        if (CanOpenNewWindow())
        {
            activeWindow = window;
            window.SetActive(true);
        }
    }

    public void CloseWindow(GameObject window)
    {
        if (activeWindow == window)
        {
            window.SetActive(false);
            activeWindow = null;
        }
    }
}
