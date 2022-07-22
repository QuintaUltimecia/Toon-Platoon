using UnityEngine;

[CreateAssetMenu(fileName = "RequirementSystem", menuName = "Requirement/Requirement", order = 1)]
public class RequirementSystemSO : ScriptableObject
{
    [SerializeField] internal Vector2 _levelRange;
    [SerializeField] internal Sprite _unitFace;
}
