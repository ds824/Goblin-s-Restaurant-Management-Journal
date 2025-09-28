using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ���� ������ ���� ����(����, �ɷ�ġ, Ư�� ��)�� �����ϰ� �����ϴ� ������ Ŭ�����Դϴ�.
/// �� Ŭ������ �ν��Ͻ��� ���� ���� �� �ε��� ����� �˴ϴ�.
/// </summary>
[System.Serializable]
public class EmployeeInstance
{
    // --- ��� ���� ---

    /// <summary>
    /// �� ������ ������ �Ǵ� ���� ������(ScriptableObject)�Դϴ�. ������ �ʴ� �⺻ ������ ��� �ֽ��ϴ�.
    /// </summary>
    public EmployeeData BaseData { get; private set; }

    /// <summary>
    /// ���� �� �ο��� ������ �̸��Դϴ�.
    /// </summary>
    public string firstName;

    /// <summary>
    /// ������ ���� �����Դϴ�.
    /// </summary>
    public int currentLevel;

    /// <summary>
    /// ���� �������� ���� ����ġ�Դϴ�.
    /// </summary>
    public float currentExperience;

    /// <summary>
    /// ���� ������ ��ų ����Ʈ�Դϴ�.
    /// </summary>
    public int skillPoints;

    /// <summary>
    /// ������ ���� �޿��Դϴ�.
    /// </summary>
    public int currentSalary;

    /// <summary>
    /// ������ ���� �丮 �ɷ�ġ�Դϴ�.
    /// </summary>
    public int currentCookingStat;

    /// <summary>
    /// ������ ���� ���� �ɷ�ġ�Դϴ�.
    /// </summary>
    public int currentServingStat;

    /// <summary>
    /// ������ ���� ���� �ɷ�ġ�Դϴ�.
    /// </summary>
    public int currentCleaningStat;

    /// <summary>
    /// ������ ���� ������ Ư�� ����Դϴ�.
    /// </summary>
    public List<Trait> currentTraits;

    // --- ������ ---

    /// <summary>
    /// '������(GeneratedApplicant)' �����͸� �������� ���ο� ���� �ν��Ͻ��� �����մϴ�.
    /// </summary>
    public EmployeeInstance(GeneratedApplicant applicant)
    {
        BaseData = applicant.BaseSpeciesData;
        firstName = applicant.GeneratedFirstName;
        currentLevel = 1;
        currentExperience = 0;
        skillPoints = 0;
        currentSalary = applicant.BaseSpeciesData.salary;
        currentTraits = new List<Trait>(applicant.GeneratedTraits);
        currentCookingStat = applicant.GeneratedCookingStat;
        currentServingStat = applicant.GeneratedServingStat;
        currentCleaningStat = applicant.GeneratedCleaningStat;
    }

    /// <summary>
    /// '���ΰ�'ó�� ������ ���ø�(EmployeeData)���� ���� ���� �ν��Ͻ��� �����մϴ�.
    /// </summary>
    public EmployeeInstance(EmployeeData baseData)
    {
        BaseData = baseData;
        firstName = baseData.speciesName;
        currentLevel = 1;
        currentExperience = 0;
        skillPoints = 0;
        currentSalary = baseData.salary;
        currentCookingStat = baseData.baseCookingStat;
        currentServingStat = baseData.baseServingStat;
        currentCleaningStat = baseData.baseCleaningStat;
        currentTraits = new List<Trait>(baseData.possibleTraits);
    }

    // --- �ٽ� ��� �Լ� ---

    /// <summary>
    /// ����ġ�� �߰��ϰ�, �ʿ� ����ġ�� �����ϸ� �������� �����Ͽ� ��ų ����Ʈ�� ����ϴ�.
    /// </summary>
    /// <param name="amount">�߰��� ����ġ ��</param>
    public void AddExperience(float amount)
    {
        float requiredExp = currentLevel * 100;
        currentExperience += amount;

        while (currentExperience >= requiredExp)
        {
            currentLevel++;
            currentExperience -= requiredExp;
            skillPoints++;
            currentSalary += 10;
            Debug.Log($"�����մϴ�! {firstName}(��)�� {currentLevel}������ ����߽��ϴ�! (���� ��ų����Ʈ: {skillPoints})");
            requiredExp = currentLevel * 100;
        }
    }

    /// <summary>
    /// ��ų ����Ʈ�� 1 ����Ͽ� '�丮' �ɷ�ġ�� 1 �ø��ϴ�.
    /// </summary>
    /// <returns>�����ϸ� true, ��ų ����Ʈ�� �����ϸ� false�� ��ȯ�մϴ�.</returns>
    public bool SpendSkillPointOnCooking()
    {
        if (skillPoints > 0)
        {
            skillPoints--;
            currentCookingStat++;
            Debug.Log($"{firstName}�� �丮 �ɷ�ġ�� {currentCookingStat}(��)�� ����߽��ϴ�!");
            return true;
        }
        return false;
    }

    /// <summary>
    /// ��ų ����Ʈ�� 1 ����Ͽ� '����' �ɷ�ġ�� 1 �ø��ϴ�.
    /// </summary>
    /// <returns>�����ϸ� true, ��ų ����Ʈ�� �����ϸ� false�� ��ȯ�մϴ�.</returns>
    public bool SpendSkillPointOnServing()
    {
        if (skillPoints > 0)
        {
            skillPoints--;
            currentServingStat++;
            Debug.Log($"{firstName}�� ���� �ɷ�ġ�� {currentServingStat}(��)�� ����߽��ϴ�!");
            return true;
        }
        return false;
    }

    /// <summary>
    /// ��ų ����Ʈ�� 1 ����Ͽ� '����' �ɷ�ġ�� 1 �ø��ϴ�.
    /// </summary>
    /// <returns>�����ϸ� true, ��ų ����Ʈ�� �����ϸ� false�� ��ȯ�մϴ�.</returns>
    public bool SpendSkillPointOnCleaning()
    {
        if (skillPoints > 0)
        {
            skillPoints--;
            currentCleaningStat++;
            Debug.Log($"{firstName}�� ���� �ɷ�ġ�� {currentCleaningStat}(��)�� ����߽��ϴ�!");
            return true;
        }
        return false;
    }
}