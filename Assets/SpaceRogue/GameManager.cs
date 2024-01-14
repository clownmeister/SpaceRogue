using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //public Map map;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}