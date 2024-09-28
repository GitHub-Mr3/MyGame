using Mr3;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGMain : MonoBehaviour
{
    private GameObject Player;
    private void Awake()
    {
        Player = GameObject.Find("Player");
    }
  
    // Start is called before the first frame update
    void Start()
    {
        AddComponent();
    }
  
    void AddComponent()
    {
        Player.AddComponent<InputMgr>();
        Player.AddComponent<PlayerCtrl>();
    }
}
