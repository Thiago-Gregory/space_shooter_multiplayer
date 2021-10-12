using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class SceneScript : NetworkBehaviour
{
    public GameObject playerUI;
    public GameObject gameOver;
    public GameObject textAmmo;
    public GameObject textScore;
    public GameObject sliderLifeBar;

    [SyncVar(hook = nameof(UIScoreText))]
    public int score;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
    }


    public void showPlayerUI()
    {
        playerUI.SetActive(true);
    }

    public void showGameOver()
    {
        playerUI.SetActive(false);
        gameOver.SetActive(true);
    }

    public void UIAmmoText(string value)
    {
        textAmmo.GetComponent<TMPro.TextMeshProUGUI>().text = value;
    }

    public void UILifeBar(float maxValue, float value)
    {
        sliderLifeBar.GetComponent<Slider>().maxValue = maxValue;
        sliderLifeBar.GetComponent<Slider>().value = value;

        if (value <= 0)
        {
            sliderLifeBar.GetComponent<Slider>().fillRect.gameObject.SetActive(false);
        }
    }

    void UIScoreText(int _Old, int _New)
    {
        textScore.GetComponent<TMPro.TextMeshProUGUI>().text = score.ToString();
    }

    //[Command]
    public void increaseScore(int value)
    {
        score += value;
        textScore.GetComponent<TMPro.TextMeshProUGUI>().text = score.ToString();
    }
}
