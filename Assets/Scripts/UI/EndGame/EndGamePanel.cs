using System.Threading.Tasks;
using Managers;
using UnityEngine;

namespace UI.EndGame
{
  public class EndGamePanel : MonoBehaviour
  {
    [SerializeField]
    private GameObject _successPanel;

    [SerializeField]
    private GameObject _failPanel;

    private void Awake()
    {
      GameManager.OnGameFinished += OnGameFinished;
      GameManager.OnGameStarted += OnGameStarted;
    }

    private void OnGameStarted()
    {
      gameObject.SetActive(false);
      _successPanel.SetActive(false);
      _failPanel.SetActive(false);
    }

    private async void OnGameFinished(bool success)
    {
      await Task.Delay(750);

      _successPanel.SetActive(false);
      _failPanel.SetActive(false);

      GameManager.Instance.SoundManager.PlaySound(success ? SoundKey.Win : SoundKey.Fail);

      if (success)
      {
        OpenSuccessPanel();
      }
      else
      {
        OpenFailPanel();
      }

      gameObject.SetActive(true);
    }

    private void OpenFailPanel()
    {
      _failPanel.SetActive(true);
    }

    private void OpenSuccessPanel()
    {
      _successPanel.SetActive(true);
    }

    public void GameStartButton()
    {
      GameManager.Instance.GameStarted();
    }
  }
}