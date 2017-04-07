using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ChecklistUI : MonoBehaviour {

  public RectTransform m_Frame;

  // Use this for initialization
  void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    Reshape();

  }

  public void Reshape()
  {
    RectTransform title = m_Frame.GetChild(0).GetComponent<RectTransform>();
    title.sizeDelta = new Vector2(m_Frame.sizeDelta.x, title.sizeDelta.y);

    RectTransform spacer = m_Frame.GetChild(m_Frame.childCount-1).GetComponent<RectTransform>();
    float height = m_Frame.sizeDelta.y - m_Frame.childCount - 1 * (title.sizeDelta.y + m_Frame.GetComponent<VerticalLayoutGroup>().spacing);
    spacer.sizeDelta = new Vector2(m_Frame.sizeDelta.x, height);

    for (int i = 1; i < m_Frame.childCount-1; i++)
    {
      RectTransform checkbox = m_Frame.GetChild(i).GetChild(0).GetComponent<RectTransform>();
      checkbox.sizeDelta = new Vector2(title.sizeDelta.y, title.sizeDelta.y);
      RectTransform item = m_Frame.GetChild(i).GetChild(1).GetComponent<RectTransform>();
      float spacing = m_Frame.GetChild(i).GetComponent<HorizontalLayoutGroup>().spacing;
      item.sizeDelta = new Vector2(m_Frame.sizeDelta.x - title.sizeDelta.y - spacing, title.sizeDelta.y);
    }


  }

  public void SetTitle(string title)
  {
    Text titleShadow = m_Frame.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
    Text titleText = titleShadow.transform.GetChild(0).GetComponent<Text>();
    titleShadow.text = title;
    titleText.text = title;
  }

  public string GetTitle()
  {
    Text titleShadow = m_Frame.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
    return titleShadow.text;
  }

  public void SetItemText(int index, string text)
  {
    if(index< GetItemCount())
    {
      Text textShadow = m_Frame.GetChild(index + 1).GetChild(1).GetChild(0).GetComponent<Text>();
      Text textItem = textShadow.transform.GetChild(0).GetComponent<Text>();

      textShadow.text = text;
      textItem.text = text;
    }
  }

  public string GetItemText(int index)
  {
    if (index < GetItemCount())
    {
      Text textShadow = m_Frame.GetChild(index + 1).GetChild(1).GetChild(0).GetComponent<Text>();
      return textShadow.text;
    }
    else
      return "";
  }

  public void SetItemCheck(int index, bool check)
  {
    if (index < GetItemCount())
    {
      GameObject checkmark = m_Frame.GetChild(index + 1).GetChild(0).GetChild(0).gameObject;
      checkmark.SetActive(check);
    }
  }

  public bool GetItemCheck(int index)
  {
    if (index < GetItemCount())
    {
      GameObject checkmark = m_Frame.GetChild(index + 1).GetChild(0).GetChild(0).gameObject;
      return checkmark.activeInHierarchy;
    }
    return false;
  }

  public GameObject AddItem()
  {
    Transform itemRef = m_Frame.GetChild(1);
    GameObject item = Object.Instantiate(itemRef.gameObject, m_Frame);

    item.transform.SetSiblingIndex(m_Frame.childCount - 2);
    item.name = "Item " + (m_Frame.childCount-3);

    SetItemText(GetItemCount() - 1, item.name);

    return item;
  }

  public int GetItemCount()
  {
    return m_Frame.childCount - 2;
  }


}
