using UnityEngine;
using UnityEngine.UI;

public class UnlockedSoldier : MonoBehaviour
{
    public Image SoldierFace;
    public Image BackUnlocked { get; set; }
    public Image BackLocked;
    public Image UnlockedText;
    public Transform ProgressTransform;

    [Header("Features")]
    public GameObject Point;

    private void Awake()
    {
        BackUnlocked = GetComponent<Image>();
    }
}
