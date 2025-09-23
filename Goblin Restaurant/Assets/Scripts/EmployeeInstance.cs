using System.Collections.Generic;

[System.Serializable]
public class EmployeeInstance
{
    public EmployeeData BaseData { get; private set; }
    public string firstName;

    public int currentLevel;
    public float currentExperience;
    public int skillPoints;
    public int currentSalary;

    public int currentCookingStat;
    public int currentServingStat;
    public int currentCleaningStat;
    public List<Trait> currentTraits;

    // ������: GeneratedApplicant(���� ������)�� �޾� �ν��Ͻ��� ����
    public EmployeeInstance(GeneratedApplicant applicant)
    {
        BaseData = applicant.BaseSpeciesData;
        firstName = applicant.GeneratedFirstName;

        currentLevel = 1;
        currentExperience = 0;
        skillPoints = 0; // ó�� ��� �� ��ų ����Ʈ�� 0�Դϴ�.
        currentSalary = applicant.BaseSpeciesData.salary;

        currentTraits = new List<Trait>(applicant.GeneratedTraits);
        currentCookingStat = applicant.GeneratedCookingStat;
        currentServingStat = applicant.GeneratedServingStat;
        currentCleaningStat = applicant.GeneratedCleaningStat;
    }

    // ���ΰ� ������ ���� Ư�� ������
    public EmployeeInstance(EmployeeData baseData)
    {
        BaseData = baseData;
        firstName = baseData.speciesName;

        currentLevel = 1;
        currentExperience = 0;
        skillPoints = 0; // ���ΰ��� ó���� ��ų ����Ʈ�� 0�Դϴ�.
        currentSalary = baseData.salary;

        currentCookingStat = baseData.baseCookingStat;
        currentServingStat = baseData.baseServingStat;
        currentCleaningStat = baseData.baseCleaningStat;
        currentTraits = new List<Trait>(baseData.possibleTraits);
    }

    // --- ������ �� ��ų ����Ʈ�� �����ϴ� ���� ---
    public void AddExperience(float amount)
    {
        float requiredExp = currentLevel * 100;
        currentExperience += amount;

        while (currentExperience >= requiredExp)
        {
            currentLevel++;
            currentExperience -= requiredExp;

            // �ɷ�ġ�� ���� �ø��� ���, ��ų ����Ʈ�� 1 �����մϴ�.
            skillPoints++;

            currentSalary += 10;
            UnityEngine.Debug.Log($"�����մϴ�! {firstName}(��)�� {currentLevel}������ ����߽��ϴ�! (���� ��ų����Ʈ: {skillPoints})");

            requiredExp = currentLevel * 100;
        }
    }

    // --- ��ų ����Ʈ�� ����Ͽ� �ɷ�ġ�� �ø��� �Լ��� ---

    public bool SpendSkillPointOnCooking()
    {
        if (skillPoints > 0)
        {
            skillPoints--;
            currentCookingStat++;
            return true;
        }
        return false;
    }

    public bool SpendSkillPointOnServing()
    {
        if (skillPoints > 0)
        {
            skillPoints--;
            currentServingStat++;
            return true;
        }
        return false;
    }

    public bool SpendSkillPointOnCleaning()
    {
        if (skillPoints > 0)
        {
            skillPoints--;
            currentCleaningStat++;
            return true;
        }
        return false;
    }
}
