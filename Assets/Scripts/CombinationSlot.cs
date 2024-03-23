using UnityEngine;
using UnityEngine.UI;

public class CombinationSlot : MonoBehaviour
{
    public Image icon;
    public Text environmentText;

    private Combination combination;

    public Color selectedColor, notSelectedColor;

    public void SetCombination(Combination newCombination)
    {
        combination = newCombination;
        icon.sprite = newCombination.requiredItems[0].icon;
        environmentText.text = newCombination.environmentType.ToString();
    }

    public void ClearSlot()
    {
        combination = null;
        icon.sprite = null;
        environmentText.text = "";
    }

    public Combination GetCombination()
    {
        return combination;
    }

    public void Select()
    {
        icon.color = selectedColor;
    }

    public void Deselect()
    {
        icon.color = notSelectedColor;
    }
}
