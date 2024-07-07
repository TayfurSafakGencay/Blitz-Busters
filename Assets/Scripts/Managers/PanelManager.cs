using UI.EndGame;
using UI.InGame;
using UnityEngine;

namespace Managers
{
  public class PanelManager : MonoBehaviour
  {
    public Collector Collector;

    public InGamePanel InGamePanel;

    public EndGamePanel EndGamePanel;

    private void Awake()
    {
      Collector.gameObject.SetActive(true);
      InGamePanel.gameObject.SetActive(true);
      EndGamePanel.gameObject.SetActive(true);
    }
  }
}