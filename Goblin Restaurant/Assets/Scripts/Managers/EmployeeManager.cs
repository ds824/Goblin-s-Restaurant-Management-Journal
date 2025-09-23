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

    void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
    }

    public void GenerateApplicants(int currentFame)
    {
        // [���� �α� 1] �� �Լ��� ���۵Ǿ����� Ȯ��
        Debug.Log("--- GenerateApplicants �Լ� ���� ---");
        applicants.Clear();

        int minApplicants = 1 + (currentFame / 1500);
        int maxApplicants = 2 + (currentFame / 1000);
        int applicantCount = Random.Range(minApplicants, Mathf.Min(maxApplicants, 10) + 1);

        if (!allSpeciesTemplates.Any())
        {
            // [���� �α�] ���� ���� ���ø��� ������ �˷���
            Debug.LogWarning("EmployeeManager�� AllSpeciesTemplates ����Ʈ�� ����־� �����ڸ� ������ �� �����ϴ�!");
            return;
        }

        for (int i = 0; i < applicantCount; i++)
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

        Debug.Log($"[��: {currentFame}] ���� ������ ������ ��: {applicants.Count}��");

        if (UIManager.Instance != null)
        {
            // [���� �α� 2] UIManager���� ��ȣ�� �������� Ȯ��
            Debug.Log("UIManager���� UI ������Ʈ�� ��û�մϴ�.");
            UIManager.Instance.UpdateApplicantListUI(applicants);
        }
    }

    public void HireEmployee(GeneratedApplicant applicantToHire)
    {
        // [���� �α� 3] ��� ��ư�� ����� ���ȴ��� Ȯ��
        Debug.Log($"--- HireEmployee �Լ� ����: {applicantToHire.GeneratedFirstName} ��� �õ� ---");
        if (applicants.Contains(applicantToHire))
        {
            EmployeeInstance newEmployee = new EmployeeInstance(applicantToHire);
            hiredEmployees.Add(newEmployee);
            applicants.Remove(applicantToHire);

            Debug.Log($"{newEmployee.BaseData.speciesName} {newEmployee.firstName}(��)�� ���������� ����߽��ϴ�! ���� ������ ��: {applicants.Count}��");

            if (UIManager.Instance != null)
            {
                // [���� �α� 4] ��� �� UI ������Ʈ�� �ٽ� ��û�ϴ��� Ȯ��
                Debug.Log("����� �Ϸ�Ǿ� UIManager���� UI ������Ʈ�� �ٽ� ��û�մϴ�.");
                UIManager.Instance.UpdateApplicantListUI(applicants);
            }
        }
    }
}
