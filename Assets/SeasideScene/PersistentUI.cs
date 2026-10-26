using UnityEngine;

public class PersistentUI : MonoBehaviour
{
    private static PersistentUI instance;

    void Awake()
    {
        // 确保切换场景时不会被销毁，并且不会重复创建多个
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}