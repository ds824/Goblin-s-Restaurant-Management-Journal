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

    // �ڡڡ� [��� ����] �ڡڡ�
    private const int MAX_TOTAL_EMPLOYEES = 10; // ��� ���� ���� �� ���� �� ����
    private const int MAX_APPLICANTS = 10;      // ������ ��Ͽ� ǥ�õ� �ִ� ��
    // �ڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡ�

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // [�ٽ� �߰�] �� ������Ʈ�� ���� �ٲ� �ı����� �ʵ��� �����մϴ�.
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

        // 1. ������ ���� �ּ�/�ִ밪 ��� (���� ��)
        int minApplicantsRaw = 1 + (currentFame / 1500);
        int maxApplicantsRaw = 2 + (currentFame / 1000);

        // 2. ���� �ִ� ������ 10���� �����մϴ�. (maxApplicantsRaw�� 100�̵� 1000�̵� 10�� ��)
        int finalMaxLimit = Mathf.Min(maxApplicantsRaw, MAX_APPLICANTS);

        // 3. ���� �ּ� ������ ����մϴ�: 
        //    (minApplicantsRaw�� 10�� �Ѵ� ���, ������ finalMaxLimit(10)���� �����Ͽ� Random.Range ������ �����մϴ�.)
        int finalMinLimit = Mathf.Min(minApplicantsRaw, finalMaxLimit);

        // 4. ���� ������ ���� ����մϴ�. (min�� max ��� 10 �����̹Ƿ� ����� 10�� �ʰ����� �ʽ��ϴ�.)
        int applicantCount = Random.Range(finalMinLimit, finalMaxLimit + 1);

        // �ڡڡ� [�α�] ���� ������ ������ ���� Ȯ���մϴ�. (�� ���� 10�� ������ �� �˴ϴ�.)
        Debug.Log($"[������ �� ���� Ȯ��] ����: {currentFame}, ���� ���� ��: {applicantCount} (MAX: {MAX_APPLICANTS})");

        if (!allSpeciesTemplates.Any()) return;

        for (int i = 0; i < applicantCount; i++) // ���� ���ѵ� applicantCount�� ���
        {
            EmployeeData selectedSpecies = allSpeciesTemplates[Random.Range(0, allSpeciesTemplates.Count)];
            float fameMultiplier = (float)currentFame / 100f;
            int finalCook = Random.Range(selectedSpecies.baseCookingStat + (int)(fameMultiplier * selectedSpecies.cookingGrowthFactor * 0.8f), selectedSpecies.baseCookingStat + (int)(fameMultiplier * selectedSpecies.cookingGrowthFactor * 1.2f) + 1);
            int finalServe = Random.Range(selectedSpecies.baseServingStat + (int)(fameMultiplier * selectedSpecies.servingGrowthFactor * 0.8f), selectedSpecies.baseServingStat + (int)(fameMultiplier * selectedSpecies.servingGrowthFactor * 1.2f) + 1);
            int finalClean = Random.Range(selectedSpecies.baseCleaningStat + (int)(fameMultiplier * selectedSpecies.cleaningGrowthFactor * 0.8f), selectedSpecies.baseCleaningStat + (int)(fameMultiplier * selectedSpecies.cleaningGrowthFactor * 1.2f) + 1);
            string jobTitle = "����";
            if (finalCook >= finalServe && finalCook >= finalClean) { jobTitle = "�丮��"; } else if (finalServe > finalCook && finalServe >= finalClean) { jobTitle = "����"; } else { jobTitle = "�Ŵ���"; }
            string firstName = selectedSpecies.speciesName;
            if (selectedSpecies.possibleFirstNames.Any()) { firstName = selectedSpecies.possibleFirstNames[Random.Range(0, selectedSpecies.possibleFirstNames.Count)]; }
            List<Trait> finalTraits = new List<Trait>();
            if (selectedSpecies.possibleTraits.Any())
            {
                float traitChance = Mathf.Min(5 + (currentFame / 100f), 90);
                if (Random.Range(0, 100) < traitChance) { finalTraits.Add(selectedSpecies.possibleTraits[Random.Range(0, selectedSpecies.possibleTraits.Count)]); }
            }
            GeneratedApplicant newApplicant = new GeneratedApplicant(selectedSpecies, firstName, jobTitle, finalCook, finalServe, finalClean, finalTraits);
            applicants.Add(newApplicant);
        }

        if (UIManager.Instance != null) { UIManager.Instance.UpdateApplicantListUI(applicants); }
    }

    public void HireEmployee(GeneratedApplicant applicantToHire)
    {
        // �ڡڡ� [����] ��� �ο� Ȯ��: �� ���� ���� MAX_TOTAL_EMPLOYEES(10) �̻��̸� ��� �Ұ� �ڡڡ�
        if (hiredEmployees.Count >= MAX_TOTAL_EMPLOYEES)
        {
            Debug.LogWarning($"�ִ� ��� �ο�({MAX_TOTAL_EMPLOYEES}��)�� �����Ͽ� �� �̻� ������ ����� �� �����ϴ�.");
            return; // ��� ������ ���� �Լ� ����
        }
        // �ڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡڡ�

        if (applicants.Contains(applicantToHire))
        {
            int hiringCost = applicantToHire.BaseSpeciesData.salary;
            // TODO: ���� �ý��� ���� �� ���⿡ SpendMoney() üũ �߰�

            EmployeeInstance newEmployee = new EmployeeInstance(applicantToHire);
            hiredEmployees.Add(newEmployee);
            applicants.Remove(applicantToHire);
            Debug.Log($"{newEmployee.BaseData.speciesName} {newEmployee.firstName}(��)�� {hiringCost}���� ����߽��ϴ�.");

            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateApplicantListUI(applicants);
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
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateHiredEmployeeListUI();
            }
        }
    }
}