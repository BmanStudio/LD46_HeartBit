using TMPro;
using UnityEngine;

public class DeathText : MonoBehaviour
{
    private void OnEnable()
    {
        if (BeatLevelManager.instance == null) return;

        GetComponent<TextMeshProUGUI>().text = "This round you were kept alive for " + ((int)BeatLevelManager.instance.timeSinceRoundStarted).ToString() + " seconds";
    }
}
