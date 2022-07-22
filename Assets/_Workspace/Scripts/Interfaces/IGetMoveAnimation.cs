using UnityEngine.Events;

public interface IGetMoveAnimation
{
    public void AddMoveEvent(UnityAction<bool> action);
    public void RemoveMoveEvent(UnityAction<bool> action);
}
