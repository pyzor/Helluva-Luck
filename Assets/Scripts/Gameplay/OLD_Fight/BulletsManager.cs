using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// keeps track of potential and available bullets
// also updates the corresponding text on the screen
// p.s. no physics involved
public class BulletsManager : MonoBehaviour {

    [SerializeField] private TMPro.TMP_Text _enemyPotentialBulletsText;
    [SerializeField] private TMPro.TMP_Text _enemyBulletsLeftToShootText;

    [SerializeField] private TMPro.TMP_Text _playerPotentialBulletsText;
    [SerializeField] private TMPro.TMP_Text _playerBulletsLeftToShootText;

    private uint _enemyPotentialBullets;
    private uint _enemyBulletsLeftToShoot;

    private uint _playerPotentialBullets;
    private uint _playerBulletsLeftToShoot;

    private uint _playerInitialPotential = 1;
    private uint _enemyInitialPotential = 1;

    public void Init() {
        _enemyPotentialBullets = _enemyInitialPotential;
        _enemyBulletsLeftToShoot = 0;
        UpdateEnemyBulletsLeftToShootText();
        UpdateEnemyPotentialText();

        _playerPotentialBullets = _playerInitialPotential;
        _playerBulletsLeftToShoot = 0;
        UpdatePlayerBulletsLeftToShootText();
        UpdatePlayerPotentialText();


    }

    public bool ConsumePlayerBullet() {
        if(_playerBulletsLeftToShoot > 0) {
            _playerBulletsLeftToShoot--;
            UpdatePlayerBulletsLeftToShootText();
            return true;
        }
        return false;
    }

    public bool ConsumeEnemyBullet() {
        if(_enemyBulletsLeftToShoot > 0) {
            _enemyBulletsLeftToShoot--;
            UpdateEnemyBulletsLeftToShootText();
            return true;
        }
        return false;
    }

    public void ConvertPlayerPotential(bool free = false) {
        _playerBulletsLeftToShoot += _playerPotentialBullets;
        UpdatePlayerBulletsLeftToShootText();
        if (!free) {
            _playerPotentialBullets = 1;
            UpdatePlayerPotentialText();
        }
    }
    public void ConvertEnemyPotential(bool free = false) {
        _enemyBulletsLeftToShoot += _enemyPotentialBullets;
        UpdateEnemyBulletsLeftToShootText();
        if (!free) {
            _enemyPotentialBullets = 1;
            UpdateEnemyPotentialText();
        }
    }

    public void AddToPlayerPotential(uint n) {
        _playerPotentialBullets += n;
        UpdatePlayerPotentialText();
    }

    public void AddToEnemyPotential(uint n) {
        _enemyPotentialBullets += n;
        UpdateEnemyPotentialText();
    }

    public void MultiplyPlayerPotential(float n) {
        _playerPotentialBullets = (uint)(_playerPotentialBullets* n);
        UpdatePlayerPotentialText();
    }

    public void MultiplyEnemyPotential(float n) {
        _enemyPotentialBullets = (uint)(_enemyPotentialBullets * n);
        UpdateEnemyPotentialText();
    }


    // TODO Setting Text for TMP_Text is probably slow
    // so multiple changes per frame might create some performance issue.
    private void UpdatePlayerBulletsLeftToShootText() {
        _playerBulletsLeftToShootText.SetText("" + _playerBulletsLeftToShoot);
    }
    private void UpdateEnemyBulletsLeftToShootText() {
        _enemyBulletsLeftToShootText.SetText("" + _enemyBulletsLeftToShoot);
    }

    private void UpdatePlayerPotentialText() {
        _playerPotentialBulletsText.SetText("" + _playerPotentialBullets);
    }
    private void UpdateEnemyPotentialText() {
        _enemyPotentialBulletsText.SetText("" + _enemyPotentialBullets);
    }


    void Start() {

    }

    void Update() {

    }
}
