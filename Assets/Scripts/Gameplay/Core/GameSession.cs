using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSession : MonoBehaviour {

    public static GameSession Instance;


    private float _hellBucks = 10;

    private float _bankDeposit = 0;



    public float HellBucks { get { return _hellBucks; } set { _hellBucks = value; } }
    public float BankDeposit { get { return _bankDeposit; } set { _bankDeposit = value; } }


    private void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(Instance != this) {
            Destroy(gameObject);
        }
    }



}
