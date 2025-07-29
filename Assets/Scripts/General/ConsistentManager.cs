using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsistentManager : MonoBehaviour
{
    public static ConsistentManager Instance { get; private set; }

    [SerializeField]
    private GameObject popupPrefab;

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void spawnPopup()
    {
        
    }
}
