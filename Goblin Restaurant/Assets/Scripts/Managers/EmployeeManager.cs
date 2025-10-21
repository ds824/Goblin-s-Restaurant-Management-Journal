using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EmployeeManager : MonoBehaviour
{
    public static EmployeeManager Instance { get; private set; }

    [Header("���� ���ø� ������")]
    public List<EmployeeData> allSpeciesTemplates;

    [Header("�ǽð� ������")]
    public List<EmployeeInstance> hiredEmployees = new List<EmployeeInstance>();
    public List<GeneratedApplicant> applicants = new List<GeneratedApplicant>();

    // ��� ����
    private const int MAX_TOTAL_EMPLOYEES = 10; // ��� ���� ���� �� ���� �� ����
    private const int MAX_APPLICANTS = 10;      // ������ ��Ͽ� ǥ�õ� �ִ� ��

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // �� ������Ʈ�� ���� �ٲ� �ı����� �ʵ��� �����մϴ�.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // �̹� �ٸ� EmployeeManager�� �ִٸ� �ڽ��� �ı��Ͽ� �ߺ��� �����ϴ�.
            Destroy(gameObject);
        }
    }

    public void GenerateApplicants(int currentFame)
    {
        applicants.Clear();

        // ���ø� ����Ʈ���� Null�� �ƴ� ��ȿ�� ���ø��� ���͸��մϴ�. (�� ���� ����)
        List<EmployeeData> validTemplates = allSpeciesTemplates
            .Where(t => t != null)
            .ToList();

        if (!validTemplates.Any())
        {
            Debug.LogError("GenerateApplicants ����: ��ȿ�� EmployeeData ���ø��� EmployeeManager�� ����Ǿ� ���� �ʽ��ϴ�!");
            return;
        }

        // 1. ������ ���� �ּ�/�ִ밪 ��� (���� ��)
        int minApplicantsRaw = 1 + (currentFame / 1500);
        int maxApplicantsRaw = 2 + (currentFame / 1000);

        // 2. ���� �ִ� ������ 10���� �����մϴ�.
        int finalMaxLimit = Mathf.Min(maxApplicantsRaw, MAX_APPLICANTS);

        // 3. ���� �ּ� ������ ����մϴ�: (Random.Range ���� ����)
        int finalMinLimit = Mathf.Min(minApplicantsRaw, finalMaxLimit);

        // 4. ���� ������ ���� ����մϴ�. 
        int applicantCount = Random.Range(finalMinLimit, finalMaxLimit + 1);

        // ���� ������ ������ ���� Ȯ���մϴ�.
        Debug.Log($"[������ �� ���� Ȯ��] ����: {currentFame}, ���� ���� ��: {applicantCount} (MAX: {MAX_APPLICANTS})");


        for (int i = 0; i < applicantCount; i++)
        {
            // ��ȿ�� ���ø� ����Ʈ �߿��� ���� ����
            EmployeeData selectedSpecies = validTemplates[Random.Range(0, validTemplates.Count)];

            float fameMultiplier = (float)currentFame / 100f;

            int finalCook = Random.Range(selectedSpecies.baseCookingStat + (int)(fameMultiplier * selectedSpecies.cookingGrowthFactor * 0.8f), selectedSpecies.baseCookingStat + (int)(fameMultiplier * selectedSpecies.cookingGrowthFactor * 1.2f) + 1);
            int finalServe = Random.Range(selectedSpecies.baseServingStat + (int)(fameMultiplier * selectedSpecies.servingGrowthFactor * 0.8f), selectedSpecies.baseServingStat + (int)(fameMultiplier * selectedSpecies.servingGrowthFactor * 1.2f) + 1);
            int finalClean = Random.Range(selectedSpecies.baseCleaningStat + (int)(fameMultiplier * selectedSpecies.cleaningGrowthFactor * 0.8f), selectedSpecies.baseCleaningStat + (int)(fameMultiplier * selectedSpecies.cleaningGrowthFactor * 1.2f) + 1);

            string jobTitle = "����";
            if (finalCook >= finalServe && finalCook >= finalClean) { jobTitle = "�丮��"; }
            else if (finalServe > finalCook && finalServe >= finalClean) { jobTitle = "����"; }
            else { jobTitle = "�Ŵ���"; }

            string firstName = selectedSpecies.speciesName;

            // Null üũ: possibleFirstNames ����Ʈ�� Null�� �ƴϸ�, �׸��� �ִ��� Ȯ��
            if (selectedSpecies.possibleFirstNames != null && selectedSpecies.possibleFirstNames.Any())
            {
                firstName = selectedSpecies.possibleFirstNames[Random.Range(0, selectedSpecies.possibleFirstNames.Count)];
            }

            List<Trait> finalTraits = new List<Trait>();
            // Null üũ: possibleTraits ����Ʈ�� Null�� �ƴϸ�, �׸��� �ִ��� Ȯ��
            if (selectedSpecies.possibleTraits != null && selectedSpecies.possibleTraits.Any())
            {
                float traitChance = Mathf.Min(5 + (currentFame / 100f), 90);
                if (Random.Range(0, 100) < traitChance)
                {
                    Trait selectedTrait = selectedSpecies.possibleTraits[Random.Range(0, selectedSpecies.possibleTraits.Count)];
                    if (selectedTrait != null) // ���õ� Ư�� ��ü ��ü�� Null���� üũ
                    {
                        finalTraits.Add(selectedTrait);
                    }
                }
            }

            GeneratedApplicant newApplicant = new GeneratedApplicant(selectedSpecies, firstName, jobTitle, finalCook, finalServe, finalClean, finalTraits);
            applicants.Add(newApplicant);
        }

        // ������ ��� UI ���� (Null üũ �߰�)
        if (EmployeeUI_Controller.Instance != null && EmployeeUI_Controller.Instance.employeeSubMenuPanel != null && EmployeeUI_Controller.Instance.employeeSubMenuPanel.activeSelf)
        {
            EmployeeUI_Controller.Instance.UpdateApplicantListUI(applicants);
        }
    }

    public void HireEmployee(GeneratedApplicant applicantToHire)
    {
        // ��� �ο� Ȯ��: �� ���� ���� MAX_TOTAL_EMPLOYEES(10) �̻��̸� ��� �Ұ�
        if (hiredEmployees.Count >= MAX_TOTAL_EMPLOYEES)
        {
            Debug.LogWarning($"�ִ� ��� �ο�({MAX_TOTAL_EMPLOYEES}��)�� �����Ͽ� �� �̻� ������ ����� �� �����ϴ�.");
            return; // ��� ������ ���� �Լ� ����
        }

        if (applicants.Contains(applicantToHire))
        {
            int hiringCost = applicantToHire.BaseSpeciesData.salary;
            // TODO: ���� �ý��� ���� �� ���⿡ SpendMoney() üũ �߰�

            EmployeeInstance newEmployee = new EmployeeInstance(applicantToHire);
            hiredEmployees.Add(newEmployee);
            applicants.Remove(applicantToHire);
            Debug.Log($"{newEmployee.BaseData.speciesName} {newEmployee.firstName}(��)�� {hiringCost}���� ����߽��ϴ�.");

            // UI ���� (Null üũ �߰�)
            if (EmployeeUI_Controller.Instance != null)
            {
                // ������ ��ϰ� ���� ���� ��� ��� ����
                EmployeeUI_Controller.Instance.UpdateApplicantListUI(applicants);
                EmployeeUI_Controller.Instance.UpdateHiredEmployeeListUI();
            }
        }
    }

    /// <summary>
    /// ���� ������ �ذ��մϴ�. �ذ� �� ��Ͽ��� �����ϰ� UI�� ������Ʈ�մϴ�.
    /// </summary>
    public void DismissEmployee(EmployeeInstance employeeToDismiss)
    {
        if (hiredEmployees.Contains(employeeToDismiss))
        {
            hiredEmployees.Remove(employeeToDismiss);
            Debug.Log($"{employeeToDismiss.firstName} ����({employeeToDismiss.BaseData.speciesName})�� �ذ��߽��ϴ�.");

            // �ذ� �� ���� ���� UI�� ���ΰ�ħ�մϴ�.
            if (EmployeeUI_Controller.Instance != null)
            {
                EmployeeUI_Controller.Instance.UpdateHiredEmployeeListUI();
            }
        }
    }
}
