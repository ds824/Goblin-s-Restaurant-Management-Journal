using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using System;
using Pathfinding;

public class Employee : MonoBehaviour
{
    private EmployeeInstance employeeData;

    [Header("직원 설정")]
    [Tooltip("음식 오브젝트가 붙을 손 위치")]
    public Transform handPosition;

    // A* 길찾기 관련 변수
    private Seeker seeker;
    private Path path;
    private int currentWaypoint = 0;
    private float nextWaypointDistance = 0.5f;
    private Vector3 lastDestination = Vector3.zero;
    private float repathRate = 0.5f;
    private float lastRepathTime = 0f;

    // 애니메이션 및 렌더링 변수
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    public enum EmployeeState
    {
        Idle, MovingToCounterTop, Cooking, MovingToServe, MovingToPickupFood,
        MovingToIdle, CheckingTable, MovingToTable, Cleaning
    }

    public EmployeeState currentState;

    [SerializeField] private float movespeed = 3f;
    [SerializeField] private Customer targetCustomer;
    [SerializeField] private CounterTop targetCountertop;
    [SerializeField] private Table targetTable;
    [SerializeField] private Transform idlePosition;
    private KitchenOrder targetOrder;

    public void Initialize(EmployeeInstance data, Transform defaultIdlePosition)
    {
        this.employeeData = data;
        this.idlePosition = defaultIdlePosition;

        if (data.BaseData != null)
        {
            this.movespeed = data.BaseData.baseMoveSpeed;
        }
        currentState = EmployeeState.Idle;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb == null) Debug.LogError("Employee 프리팹에 Rigidbody 2D가 없습니다");
        if (seeker == null) Debug.LogError("Employee 프리팹에 Seeker 컴포넌트가 없습니다");
        if (spriteRenderer == null) Debug.LogError("Employee 프리팹에 SpriteRenderer가 없습니다");

        if (handPosition == null) handPosition = this.transform;
        if (employeeData == null) currentState = EmployeeState.Idle;
    }

    void Update()
    {
        switch (currentState)
        {
            case EmployeeState.Idle:
                FindTask();
                break;
            case EmployeeState.MovingToIdle:
                FindTask();
                if (idlePosition != null)
                {
                    if (currentState == EmployeeState.MovingToIdle)
                        CheckArrived(idlePosition.position, () => { currentState = EmployeeState.Idle; });
                }
                else currentState = EmployeeState.Idle;
                break;
            case EmployeeState.MovingToCounterTop:
                // 이동 목표는 GetTargetPositionByState에서 가져오므로 여기서는 도착 체크만 수행
                CheckArrived(GetTargetPositionByState(), () => { StartCoroutine(CookFoodCoroutine()); });
                break;
            case EmployeeState.Cooking: break;
            case EmployeeState.MovingToPickupFood:
                CheckArrived(GetTargetPositionByState(), () => { StartCoroutine(PickupFoodCoroutine()); });
                break;
            case EmployeeState.MovingToServe:
                CheckArrived(GetTargetPositionByState(), ServeFood);
                break;
            case EmployeeState.CheckingTable:
                CheckTable();
                break;
            case EmployeeState.MovingToTable:
                CheckArrived(GetTargetPositionByState(), () => { StartCoroutine(CleaningTable()); });
                break;
            case EmployeeState.Cleaning: break;
        }
    }

    // 물리 이동 및 애니메이션 처리는 여기서 수행
    void FixedUpdate()
    {
        // 1. 이동 중지 상태
        if (currentState == EmployeeState.Idle ||
            currentState == EmployeeState.Cooking ||
            currentState == EmployeeState.Cleaning)
        {
            if (rb != null) rb.linearVelocity = Vector2.zero;
            ResetAnimation(); // 멈춤 애니메이션 재생
            return;
        }

        // 2. A* 경로 계산
        Vector3 targetDest = GetTargetPositionByState();
        if (targetDest == Vector3.zero) return;

        if (Vector3.Distance(targetDest, lastDestination) > 0.1f || Time.time > lastRepathTime + repathRate)
        {
            lastRepathTime = Time.time;
            lastDestination = targetDest;
            if (seeker.IsDone()) seeker.StartPath(rb.position, targetDest, OnPathComplete);
        }

        if (path == null) return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            if (rb != null) rb.linearVelocity = Vector2.zero;
            ResetAnimation(); // 도착 시 멈춤
            return;
        }

        // 3. 이동 처리
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        float finalMoveSpeed = CalculateFinalSpeed();

        if (rb != null)
        {
            rb.linearVelocity = direction * finalMoveSpeed;
        }

        // 4. 애니메이션 제어
        if (animator != null)
        {
            // 파라미터 초기화
            animator.SetBool("Up", false);
            animator.SetBool("Down", false);
            animator.SetBool("Idle", false);
            animator.SetFloat("InputX", 0f);

            if (direction.sqrMagnitude > 0.01f) // 움직이는 중일 때
            {
                // Y축 이동이 더 클 경우 위아래 애니메이션
                if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
                {
                    if (direction.y > 0) animator.SetBool("Up", true);
                    else animator.SetBool("Down", true);
                }
                else // X축 이동이 더 클 경우 좌우 애니메이션
                {
                    animator.SetFloat("InputX", Mathf.Abs(direction.x));
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.flipX = (direction.x < 0);
                    }
                }
            }
            else
            {
                ResetAnimation(); // 속도가 거의 없으면 멈춤 처리
            }
        }

        float distanceToWaypoint = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distanceToWaypoint < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    void ResetAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("Up", false);
            animator.SetBool("Down", false);
            animator.SetFloat("InputX", 0f);
            animator.SetBool("Idle", true);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void CheckArrived(Vector3 destination, Action onArrived)
    {
        float dist = Vector2.Distance(transform.position, destination);
        if (dist < 0.5f)
        {
            if (rb != null) rb.linearVelocity = Vector2.zero;
            ResetAnimation(); // 도착했으니 멈춤 애니메이션
            path = null;
            onArrived?.Invoke();
        }
    }

    // ▼▼▼ [수정됨] 상호작용 위치(InteractionPoint)를 반환하도록 변경 ▼▼▼
    Vector3 GetTargetPositionByState()
    {
        switch (currentState)
        {
            case EmployeeState.MovingToIdle:
                return idlePosition != null ? idlePosition.position : Vector3.zero;

            case EmployeeState.MovingToCounterTop:
            case EmployeeState.MovingToPickupFood:
                // targetCountertop.transform.position 대신 GetInteractionPosition() 사용
                return targetCountertop != null ? targetCountertop.GetInteractionPosition() : Vector3.zero;

            case EmployeeState.MovingToServe:
                // 손님 위치는 그대로 유지 (필요 시 손님 스크립트에도 interactionPoint 추가 가능)
                return targetCustomer != null ? targetCustomer.transform.position : Vector3.zero;

            case EmployeeState.MovingToTable:
                // targetTable.transform.position 대신 GetInteractionPosition() 사용
                return targetTable != null ? targetTable.GetInteractionPosition() : Vector3.zero;

            default:
                return Vector3.zero;
        }
    }
    // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

    float CalculateFinalSpeed()
    {
        float synergySpeedBonus = 0f;
        float traitSpeedBonus = 0f;

        if (SynergyManager.Instance != null) synergySpeedBonus = SynergyManager.Instance.GetMoveSpeedMultiplier();
        if (employeeData != null) traitSpeedBonus = employeeData.GetTraitMoveSpeedMultiplier();

        float finalSpeed = movespeed * (1.0f + synergySpeedBonus + traitSpeedBonus);
        return Mathf.Max(0.1f, finalSpeed);
    }

    void FindTask()
    {
        if (employeeData == null)
        {
            Debug.LogWarning("Employee.cs: employeeData가 null이라 FindTask를 실행할 수 없습니다");
            return;
        }

        if (employeeData.assignedRole == EmployeeRole.Unassigned)
        {
            currentState = EmployeeState.Idle;
            return;
        }

        // [홀 서빙]
        if (employeeData.assignedRole == EmployeeRole.Hall ||
            employeeData.assignedRole == EmployeeRole.AllRounder)
        {
            if (RestaurantManager.instance != null)
            {
                targetOrder = RestaurantManager.instance.OrderQueue.FirstOrDefault(o =>
                    o != null &&
                    o.status == OrderStatus.ReadyToServe &&
                    o.cookedOnCounterTop != null
                );
            }

            if (targetOrder != null)
            {
                Debug.Log($"{employeeData.firstName}: 홀 서빙 시작 (AllRounder/Hall)");
                targetOrder.status = OrderStatus.Completed;
                targetCustomer = targetOrder.customer;
                targetCountertop = targetOrder.cookedOnCounterTop;
                currentState = EmployeeState.MovingToPickupFood;
                return;
            }
        }

        // [주방 요리]
        if (employeeData.assignedRole == EmployeeRole.Kitchen ||
            employeeData.assignedRole == EmployeeRole.AllRounder)
        {
            if (RestaurantManager.instance != null)
            {
                targetOrder = RestaurantManager.instance.OrderQueue.FirstOrDefault(o => o != null && o.status == OrderStatus.Pending);
            }

            if (targetOrder != null)
            {
                targetCountertop = RestaurantManager.instance.counterTops.FirstOrDefault(s => s != null && !s.isBeingUsed);

                if (targetCountertop != null)
                {
                    Debug.Log($"{employeeData.firstName}: 요리 시작 (AllRounder/Kitchen)");
                    targetOrder.status = OrderStatus.Cooking;
                    targetCountertop.isBeingUsed = true;
                    currentState = EmployeeState.MovingToCounterTop;
                    return;
                }
            }
        }

        // [홀 청소]
        if (employeeData.assignedRole == EmployeeRole.Hall ||
            employeeData.assignedRole == EmployeeRole.AllRounder)
        {
            if (RestaurantManager.instance != null)
            {
                targetTable = RestaurantManager.instance.tables.FirstOrDefault(t =>
                    t != null && t.isDirty && !t.isBeingUsedForCleaning);
            }

            if (targetTable != null)
            {
                Debug.Log($"{employeeData.firstName}: 테이블 청소 시작 (AllRounder/Hall)");
                targetTable.isBeingUsedForCleaning = true;
                currentState = EmployeeState.MovingToTable;
                return;
            }
        }

        if (idlePosition != null && Vector2.Distance(transform.position, idlePosition.position) > 0.5f)
        {
            currentState = EmployeeState.MovingToIdle;
        }
    }

    IEnumerator CookFoodCoroutine()
    {
        currentState = EmployeeState.Cooking;
        // 요리할 때 직원을 화구 상호작용 위치로 정확히 이동시킴 (미세 보정)
        if (targetCountertop != null)
            transform.position = targetCountertop.GetInteractionPosition();

        Debug.Log($"{employeeData?.firstName ?? "Worker"} {targetOrder.recipe.data.recipeName} 요리 시작");

        if (targetOrder.foodObject == null)
        {
            Debug.LogError($"요리 오류 {targetOrder.recipe.data.recipeName}의 foodObject가 null입니다 작업을 중단합니다");
            if (targetCountertop != null) targetCountertop.isBeingUsed = false;
            currentState = EmployeeState.MovingToIdle;
            targetCustomer = null;
            targetCountertop = null;
            targetOrder = null;
            yield break;
        }

        // 음식 오브젝트 위치 설정
        targetOrder.foodObject.transform.position = targetCountertop.transform.position;
        targetOrder.foodObject.SetActive(true);

        float baseRecipeTime = 10f;
        if (targetOrder.recipe != null && targetOrder.recipe.data != null)
        {
            baseRecipeTime = targetOrder.recipe.data.baseCookTime;
        }

        int baseCookingStat = employeeData.currentCookingStat;
        int bonusCookingStat_Synergy = 0;
        float speedBonusPercent_Synergy = 0f;
        float specificStatMultiplier_Trait = 0f;
        float allStatMultiplier_Trait = 0f;
        float workSpeedMultiplier_Trait = 0f;

        if (SynergyManager.Instance != null)
        {
            var (cookBonus, serveBonus, charmBonus) = SynergyManager.Instance.GetStatBonuses(employeeData);
            bonusCookingStat_Synergy = cookBonus;
            speedBonusPercent_Synergy = SynergyManager.Instance.GetCookingSpeedBonus(employeeData);
        }

        if (employeeData != null)
        {
            specificStatMultiplier_Trait = employeeData.GetTraitCookingStatMultiplier();
            allStatMultiplier_Trait = employeeData.GetTraitAllStatMultiplier();
            workSpeedMultiplier_Trait = employeeData.GetTraitWorkSpeedMultiplier();
        }

        float totalMultiplier = 1.0f + specificStatMultiplier_Trait + allStatMultiplier_Trait;
        int finalCookingStat = (int)((baseCookingStat + bonusCookingStat_Synergy) * totalMultiplier);
        float finalCookTime = baseRecipeTime / (1 + (finalCookingStat * 0.008f));

        finalCookTime = finalCookTime * (1.0f - speedBonusPercent_Synergy);
        finalCookTime = finalCookTime * (1.0f - workSpeedMultiplier_Trait);
        finalCookTime = Mathf.Max(0.5f, finalCookTime);

        Debug.Log($"[{employeeData.firstName}] 요리 시간 {finalCookTime:F1}s");

        yield return new WaitForSeconds(finalCookTime);

        Debug.Log($"{employeeData?.firstName ?? "Worker"} 요리 완성");

        float totalSaveChance = 0f;
        if (SynergyManager.Instance != null) { totalSaveChance += SynergyManager.Instance.GetIngredientSaveChance(); }
        if (employeeData != null) { totalSaveChance += employeeData.GetTraitSaveChance(); }
        if (totalSaveChance > 0 && UnityEngine.Random.Range(0f, 1f) < totalSaveChance)
        {
            Debug.Log($"재료 절약 {employeeData.firstName} 재료 절약 성공");
            if (GameManager.instance != null && targetOrder.recipe != null)
            {
                GameManager.instance.RefundIngredients(targetOrder.recipe.data);
            }
        }

        targetOrder.status = OrderStatus.ReadyToServe;
        targetOrder.cookedOnCounterTop = this.targetCountertop;
        currentState = EmployeeState.MovingToIdle;

        if (targetCountertop != null) targetCountertop.isBeingUsed = false;

        targetCustomer = null;
        targetCountertop = null;
        targetOrder = null;
    }

    void ServeFood()
    {
        Debug.Log($"{employeeData?.firstName ?? "Worker"} 서빙 완료");
        if (targetCustomer != null)
        {
            targetCustomer.ReceiveFood(this.employeeData);
        }

        if (targetOrder.foodObject != null)
        {
            Destroy(targetOrder.foodObject);
        }

        if (RestaurantManager.instance != null && targetOrder != null)
        {
            RestaurantManager.instance.OrderQueue.Remove(targetOrder);
        }

        if (targetCountertop != null) targetCountertop.isBeingUsed = false;
        targetCustomer = null;
        targetCountertop = null;
        targetOrder = null;

        currentState = EmployeeState.MovingToIdle;
    }

    void CheckTable()
    {
        if (targetTable != null && targetTable.isDirty)
        {
            currentState = EmployeeState.MovingToTable;
        }
        else
        {
            currentState = EmployeeState.Idle;
        }
    }

    IEnumerator CleaningTable()
    {
        currentState = EmployeeState.Cleaning;
        
        // 청소할 때도 정확한 위치로 보정
        if(targetTable != null)
             transform.position = targetTable.GetInteractionPosition();

        Debug.Log($"{employeeData?.firstName ?? "Worker"} 테이블 청소 시작");

        float baseCleaningTime = 3f;
        int baseServingStat = employeeData.currentServingStat;
        int bonusServingStat = 0;
        float allStatMultiplier_Trait = 0f;
        float workSpeedMultiplier_Trait = 0f;

        if (SynergyManager.Instance != null)
        {
            var (cookBonus, serveBonus, charmBonus) = SynergyManager.Instance.GetStatBonuses(employeeData);
            bonusServingStat = serveBonus;
        }

        if (employeeData != null)
        {
            allStatMultiplier_Trait = employeeData.GetTraitAllStatMultiplier();
            workSpeedMultiplier_Trait = employeeData.GetTraitWorkSpeedMultiplier();
        }

        float totalMultiplier = 1.0f + allStatMultiplier_Trait;
        int finalServingStat = (int)((baseServingStat + bonusServingStat) * totalMultiplier);

        float finalCleaningTime = baseCleaningTime / (1 + (finalServingStat * 0.008f));
        finalCleaningTime = finalCleaningTime * (1.0f - workSpeedMultiplier_Trait);
        finalCleaningTime = Mathf.Max(0.5f, finalCleaningTime);

        Debug.Log($"[{employeeData.firstName}] 청소 시간 {finalCleaningTime:F1}s");

        yield return new WaitForSeconds(finalCleaningTime);

        Debug.Log($"{employeeData?.firstName ?? "Worker"} 테이블 청소 완료");
        if (targetTable != null)
        {
            targetTable.isDirty = false;
            targetTable.isBeingUsedForCleaning = false;
        }
        targetTable = null;
        currentState = EmployeeState.MovingToIdle;
    }

    IEnumerator PickupFoodCoroutine()
    {
        // 픽업 위치 보정
        if(targetCountertop != null)
             transform.position = targetCountertop.GetInteractionPosition();

        if (targetOrder.foodObject != null)
        {
            Debug.Log($"{employeeData?.firstName ?? "Worker"} 픽업 완료");

            targetOrder.foodObject.transform.SetParent(handPosition);
            targetOrder.foodObject.transform.localPosition = Vector3.zero;

            if (targetCountertop != null)
            {
                targetCountertop.isBeingUsed = false;
            }

            targetOrder.cookedOnCounterTop = null;
            currentState = EmployeeState.MovingToServe;

            yield return null;
        }
        else
        {
            Debug.LogError($"픽업 오류 foodObject가 null입니다");

            if (targetCountertop != null)
            {
                targetCountertop.isBeingUsed = false;
            }
            targetOrder.cookedOnCounterTop = null;

            currentState = EmployeeState.MovingToIdle;
            targetCustomer = null;
            targetCountertop = null;
            targetOrder = null;

            yield break;
        }
    }
}