using UnityEngine;

public class CounterTop : MonoBehaviour
{
    // 이 화구가 현재 직원에 의해 사용되고 있는지 여부
    public bool isBeingUsed = false;

    [Header("작업 위치")]
    public Transform interactionPoint;

    public Vector3 GetInteractionPosition()
    {
        return interactionPoint != null ? interactionPoint.position : transform.position;
    }
}
