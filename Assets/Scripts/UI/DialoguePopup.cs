using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialoguePopup : MonoBehaviour {

    [SerializeField] private DialoguePopupBackground _background;



    

    public void Show() {
        gameObject.SetActive(true);
        _background.Show();
    }

    public void Hide() {
        gameObject.SetActive(false);
        _background.Hide();
    }


    void Update() {

    }


}
