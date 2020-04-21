using TMPro;
using UnityEngine;

public class Highscore : MonoBehaviour
{

    private const string key = "HeartBit_Highscore";
    private void OnEnable()
    {
        float currentHighScore;
        if (PlayerPrefs.HasKey(key))
        {
            currentHighScore = PlayerPrefs.GetFloat(key);

            if (BeatLevelManager.instance.timeSinceRoundStarted > currentHighScore)
            {
                currentHighScore = BeatLevelManager.instance.timeSinceRoundStarted;
                PlayerPrefs.SetFloat(key, currentHighScore);
                GetComponent<TextMeshProUGUI>().text = "You Beat your old highscore! Your new highscore is " + ((int)currentHighScore).ToString() + " seconds";
            }

            else
            {
                GetComponent<TextMeshProUGUI>().text = "Your highscore is still " + ((int)currentHighScore).ToString() + " seconds";
            }
        }
        else
        {
            currentHighScore = BeatLevelManager.instance.timeSinceRoundStarted;
            PlayerPrefs.SetFloat(key, currentHighScore);
            GetComponent<TextMeshProUGUI>().text = "Not bad for first time! Your new highscore is " + ((int)currentHighScore).ToString() + " seconds";
        }
    }
}
