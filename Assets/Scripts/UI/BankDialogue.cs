using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BankDialogue : MonoBehaviour {

    public static BankDialogue Instance;

    [SerializeField] private DialoguePopup _dialoguePopup;

    [SerializeField] private TMPro.TMP_Text _deposit;
    [SerializeField] private TMPro.TMP_Text _hellbucks;

    public BankDialogue() {
        Instance = this;
    }

    public void Deposit() {
        GameSession.Instance.BankDeposit += GameSession.Instance.HellBucks;
        GameSession.Instance.HellBucks = 0;
        UpdateText();
    }

    public void Withdraw() {
        GameSession.Instance.HellBucks += GameSession.Instance.BankDeposit;
        GameSession.Instance.BankDeposit = 0;
        UpdateText();
    }

    void OnDestroy() {
        Instance = null;
    }

    public void Hide() {
        _dialoguePopup.Hide();
    }

    public void Show() {
        _dialoguePopup.Show();
    }

    void OnEnable() {
        UpdateText();
    }

    private void UpdateText() {
        _deposit.SetText(GameSession.Instance.BankDeposit.ToString()); // TODO
        _hellbucks.SetText(GameSession.Instance.HellBucks.ToString()); // Instead of doing .ToString() add literal aka "K" "M" "T" "B".
    }
}
