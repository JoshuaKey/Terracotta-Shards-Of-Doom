using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProgression : MonoBehaviour {

    [Header("Enemies")]
    public bool UseAllEnemies = true;
    public Enemy[] Enemies;
    public int KillsNeeded;

    [Header("Object")]
    public GameObject ProgressionObject;
    public bool SetActive = true;
    public bool UseHud = false;

    private int currKills = 0;

    // Start is called before the first frame update
    void Start() {
        if (UseAllEnemies) {
            EnemyManager.Instance.OnEnemyDeath += OnEnemyDeath;
        } else {
            for (int i = 0; i < Enemies.Length; i++) {
                Enemies[i].health.OnDeath += OnEnemyDeath;
            }
        }

        if(ProgressionObject) {
            ProgressionObject.SetActive(!SetActive);
        }      

        if (UseHud) {
            PlayerHud.Instance.SetEnemyCount(currKills, KillsNeeded);
        }   
    }

    private void OnEnemyDeath() {
        if (!IsComplete()) {
            currKills++;
            Check();
        } else {
            currKills++;
        }

        if (UseHud) {
            PlayerHud.Instance.SetEnemyCount(currKills, KillsNeeded);
        }

        PlayerHud.Instance.PlayPotAnimation();
    }

    public void Check() {
        if(IsComplete()) {
            if (ProgressionObject) {
                ProgressionObject.SetActive(SetActive);
                
                //PortalTurnedOn.SetActive(true);
                //make this a corutine instead
            }
            if (UseHud) {
                PlayerHud.Instance.TurnOnPortalText();
            }
            Teleporter teleporter = FindObjectOfType<Teleporter>();
            if(teleporter)
            {
                teleporter.PlaySound();
            }
        }
    }

    public bool IsComplete() {
        return currKills >= KillsNeeded;
    }

}
