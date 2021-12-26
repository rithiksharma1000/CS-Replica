using UnityEngine;
using UnityEngine.UI;

public class KillfeedItem : MonoBehaviour {

    [SerializeField] Text KillerNameText;
    [SerializeField] Text KilledNameText;

    public void Setup(string player, string source)
    {
        KillerNameText.text = source;
        KilledNameText.text = player;
    }
}
