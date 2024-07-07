using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Level;
using Managers;
using TMPro;
using UnityEngine;
using View;

namespace UI.InGame
{
  public class InGamePanel : MonoBehaviour
  {
    [SerializeField]
    private TextMeshProUGUI _timerText;

    [SerializeField]
    private Transform _necessaryItemsParent;

    [SerializeField]
    private NecessaryItemInPanel _necessaryItem;

    private int _timer;

    private bool _isGameEnded;

    private void Awake()
    {
      GameManager.OnGameStarted += OnGameStarted;
      GameManager.OnGameFinished += OnGameFinished;
    }

    private void OnGameFinished(bool obj)
    {
      _isGameEnded = true;
    }

    private void OnGameStarted()
    {
      _isGameEnded = false;
      
      LevelManager levelManager = GameManager.Instance.LevelManager;
      LevelVo levelVo = levelManager.GetLevelVo(GameManager.Level);
      
      SetTimer(levelVo.LevelTime);
    }

    private async void SetTimer(int initialTime)
    {
      _timer = initialTime;
      
      while (_timer > 0)
      {
        SetTimerText();
        await Task.Delay(1000);
        _timer--;

        if (_isGameEnded) return;
      }
      
      SetTimerText();
      GameFinished();
    }

    private void SetTimerText()
    {
      _timerText.text = _timer.ToString();
    }

    private void GameFinished()
    {
      GameManager.Instance.GameFinished(false);
    }

    public void SetNecessaryItemsInitial(Dictionary<Item, int> items)
    {
      for (int i = 0; i < items.Count; i++)
      {
        NecessaryItemInPanel necessaryItem = Instantiate(_necessaryItem, Vector3.zero, Quaternion.identity, _necessaryItemsParent);
        necessaryItem.transform.localPosition = Vector3.zero;
        necessaryItem.transform.localRotation = new Quaternion(0,0,0,0);
        
        KeyValuePair<Item, int> item = items.ElementAt(i);
        
        necessaryItem.SetData(item.Key.GetSprite(), item.Value, item.Key.GetItemKey());
      }
    }

    private void OnDestroy()
    {
      GameManager.OnGameStarted -= OnGameStarted;
      GameManager.OnGameFinished -= OnGameFinished;
    }
  }
}