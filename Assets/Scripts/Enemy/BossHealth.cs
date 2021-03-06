﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BossHealth : MonoBehaviour {
    public GameObject enemy;
    public KillEnemy killEnemy;
    public GameObject theEnd;

    public Image[] healthImages;
    public Sprite[] healthSprites;
    
    public int startHearts;
    public int currHealth;

    private int healthPerHeart = 1;
    private bool gameEnd = false;

    void Start() {
        currHealth = startHearts * healthPerHeart;
        UpdateHearts();
    }

    void UpdateHearts()
    {

        if (!gameEnd)
        {
            bool empty = false;
            int i = 0;

            foreach (Image image in healthImages)
            {
                if (empty)
                {
                    image.sprite = healthSprites[0];
                }
                else
                {
                    i++;
                    if (currHealth >= i * healthPerHeart)
                    {
                        image.sprite = healthSprites[healthSprites.Length - 1];
                    }
                    else
                    {
                        int currentHeartHealth = (int)(healthPerHeart - (healthPerHeart * i - currHealth));
                        int healthPerImage = healthPerHeart / (healthSprites.Length - 1);
                        int imageIndex = currentHeartHealth / healthPerImage;

                        image.sprite = healthSprites[imageIndex];
                        image.GetComponent<Image>().color = new Color32(255, 90, 90, 100);
                        empty = true;
                    }
                }
            }
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.Return))             // return to start menu if user presses Enter after the game ends
            {
                SceneManager.LoadScene(0);
            }
        }

        checkIfBossKilled();

        if (currHealth != startHearts * healthPerHeart && currHealth % healthPerHeart == 0)
        {
            transform.Find("EvilPiano").GetComponent<BossMelody>().NextStage();
            BGM.PlayBossNextBGM();
        }
    }

    public void checkIfBossKilled()
    {
        if (currHealth == 0)
        {
            killEnemy.setEnemyDead(true);
            Destroy(enemy);
            theEnd.SetActive(true);
            this.gameEnd = true;
        }
    }
    public void TakeDamage(int amount)
    {
        if (currHealth > 0)
        {
            currHealth += amount;
            currHealth = Mathf.Clamp(currHealth, 0, startHearts * healthPerHeart);
            UpdateHearts();
        }
    }

}
