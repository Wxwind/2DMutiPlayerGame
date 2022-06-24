
using Panel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameEndPanel : MonoBehaviour
{
    public TMP_Text winnerText;

    public Button backToRoomBtn;
    public GameObject cavans;

    private void Awake()
    {
        backToRoomBtn.onClick.AddListener(BackToRoom);
    }

    private void BackToRoom()
    {
        UIManeger.instance.Show();
        gameObject.SetActive(false);
        cavans.SetActive(false);
    }
    public void SetWinner(string winner)
    {
        winnerText.text = $"winner is {winner} !";
    }

    public void Show()
    {
        gameObject.SetActive(true);
        cavans.SetActive(true);
    }
}
