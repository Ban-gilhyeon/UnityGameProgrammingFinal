using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject gameCam;
    public Player player;
    public EnemyMelee boss;
    public float playTime;
    public bool isBattle;

    public GameObject menuPanel;
    public GameObject gamePanel;

    public Text maxScoreTxt;
    public Text scoreTxt;
    public Text playTimeTxt;
    public Text playerHealthTxt;

    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;

    // Start is called before the first frame update
    void Awake()
    {
        maxScoreTxt.text =string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));

    }

    public void GameStart()
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        isBattle = true;

        player.gameObject.SetActive(true);


    }

    void Update()
    {
        if (isBattle)
            playTime += Time.deltaTime;

    }
    void LateUpdate()
    {

        scoreTxt.text = string.Format("{0:n0}", player.score);
        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600)/ 60);
        int second = (int)(playTime % 60);

        playTimeTxt.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second);

        playerHealthTxt.text = player.curHealth + " / " + player.maxHealth;

        bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth,1 , 1);

    }
}
