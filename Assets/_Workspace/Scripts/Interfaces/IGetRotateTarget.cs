using UnityEngine;
using UnityEngine.Events;

public interface IGetRotateTarget
{
    public void AddRotateEvent(UnityAction<Transform> action);
    public void RemoveRotateEvent(UnityAction<Transform> action);
}
