using UnityEngine;

public class UpgradesMenu : MonoBehaviour
{
    [SerializeField] private RankMenu _rank;
    [SerializeField] private WeaponMenu _weapon;

    private Vector3 _startPosition;
    private Vector3 _nextPosition;

    private void Awake()
    {
        _startPosition = _rank.Price.rectTransform.localPosition;
        _nextPosition = new Vector3(0, _startPosition.y, _startPosition.z);
    }

    public void GetRank(Sprite sprite, string title, string price)
    {
        if (sprite != null) _rank.Image.sprite = sprite;
        if (title != null) _rank.Title.text = title;
        if (price != null) _rank.Price.text = price;

        if (price == "MAX. LV.")
        {
            _rank.PriceImage.enabled = false;
            _rank.Price.rectTransform.localPosition = _nextPosition;
        }
        else
        {
            _rank.Price.rectTransform.localPosition = _startPosition;
            _rank.PriceImage.enabled = true;
        }
    }

    public void GetWeapon(Sprite sprite, string title, string price)
    {
        if (sprite != null) _weapon.Image.sprite = sprite;
        if (title != null) _weapon.Title.text = title;
        if (price != null) _weapon.Price.text = price;

        if (price == "MAX. LV.")
        {
            _weapon.PriceImage.enabled = false;
            _weapon.Price.rectTransform.localPosition = _nextPosition;
        }
        else
        {
            _weapon.Price.rectTransform.localPosition = _startPosition;
            _weapon.PriceImage.enabled = true;
        }
    }
}
