using Enum;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.InGame
{
  public class NecessaryItemInPanel : MonoBehaviour
  {
    public Image Image;

    public TextMeshProUGUI CountText;

    [HideInInspector]
    public ItemKey ItemKey;

    [HideInInspector]
    public int Count;

    [SerializeField]
    private Sprite _completedIcon;

    private void Awake()
    {
      GameManager.OnGameStarted += OnGameStarted;
      GameManager.Instance.LevelManager.OnDataChanged += OnDataChanged;
    }

    private void OnGameStarted()
    {
      Destroy(gameObject);
    }

    private void OnDataChanged(ItemKey itemKey, int count)
    {
      if (itemKey != ItemKey) return;
      if (Count <= 0) return;

      Count -= count;
      CountText.text = Count.ToString();

      if (Count > 0) return;
      CountText.gameObject.SetActive(false);
      Image.sprite = _completedIcon;
      Image.rectTransform.sizeDelta = new Vector2(100, 100);
    }

    public void SetData(Sprite sprite, int count, ItemKey itemKey)
    {
      ItemKey = itemKey;
      Image.sprite = sprite;

      Count = count;
      CountText.text = count.ToString();
    }

    private void OnDestroy()
    {
      GameManager.Instance.LevelManager.OnDataChanged -= OnDataChanged;
      GameManager.OnGameStarted -= OnGameStarted;
    }
  }
}