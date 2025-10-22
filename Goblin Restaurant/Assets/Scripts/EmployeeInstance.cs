using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    /// �� ������ ���ΰ����� �����Դϴ�. (�ذ� ������)
    /// </summary>
    public bool isProtagonist { get; private set; }

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
    /// '������(GeneratedApplicant)' �����͸� �������� ���ο� ���� �ν��Ͻ��� �����մϴ�. (�Ϲ� ����)
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

        // �Ϲ� ������ false�� ����
        isProtagonist = false;
    }

    /// <summary>
    /// '���ΰ�'ó�� ������ ���ø�(EmployeeData)���� ���� ���� �ν��Ͻ��� �����մϴ�.
    /// </summary>
    public EmployeeInstance(EmployeeData baseData)
    {
        BaseData = baseData;
        // ��� ���� �ĺ��� ���� �⺻ �̸� ����
        firstName = "Goblin Chef";
        currentLevel = 1;
        currentExperience = 0;
        skillPoints = 5; // ���ΰ��� �⺻ ��ų ����Ʈ ���� (����)
        currentSalary = baseData.salary;
        currentCookingStat = baseData.baseCookingStat;
        currentServingStat = baseData.baseServingStat;
        currentCleaningStat = baseData.baseCleaningStat;
        currentTraits = new List<Trait>(); // ���ΰ��� �⺻ Ư�� �������� ���� (���� ����)

        // ���ΰ� ���θ� �����մϴ�.
        isProtagonist = true;
    }

    // --- �ٽ� ��� �Լ� ---

    // ************* [����ġ ���� �Լ��� ���� �������� �ʾ����Ƿ� ����] *************

    /// <summary>
    /// �丮 ���ȿ� ��ų ����Ʈ�� ����ϰ� ������ ������ŵ�ϴ�.
    /// </summary>
    /// <returns>���� ������ ���������� true�� ��ȯ�մϴ�.</returns>
    public bool SpendSkillPointOnCooking()
    {
        if (skillPoints > 0)
        {
            skillPoints--;
            currentCookingStat++;
            Debug.Log($"{firstName}: �丮 ������ {currentCookingStat}���� �����߽��ϴ�. ���� ����Ʈ: {skillPoints}");
            return true;
        }
        Debug.LogWarning($"{firstName}: ��ų ����Ʈ�� �����Ͽ� �丮 ������ �ø� �� �����ϴ�.");
        return false;
    }

    /// <summary>
    /// ���� ���ȿ� ��ų ����Ʈ�� ����ϰ� ������ ������ŵ�ϴ�.
    /// </summary>
    public bool SpendSkillPointOnServing()
    {
        if (skillPoints > 0)
        {
            skillPoints--;
            currentServingStat++;
            Debug.Log($"{firstName}: ���� ������ {currentServingStat}���� �����߽��ϴ�. ���� ����Ʈ: {skillPoints}");
            return true;
        }
        Debug.LogWarning($"{firstName}: ��ų ����Ʈ�� �����Ͽ� ���� ������ �ø� �� �����ϴ�.");
        return false;
    }

    /// <summary>
    /// ���� ���ȿ� ��ų ����Ʈ�� ����ϰ� ������ ������ŵ�ϴ�.
    /// </summary>
    public bool SpendSkillPointOnCleaning()
    {
        if (skillPoints > 0)
        {
            skillPoints--;
            currentCleaningStat++;
            Debug.Log($"{firstName}: ���� ������ {currentCleaningStat}���� �����߽��ϴ�. ���� ����Ʈ: {skillPoints}");
            return true;
        }
        Debug.LogWarning($"{firstName}: ��ų ����Ʈ�� �����Ͽ� ���� ������ �ø� �� �����ϴ�.");
        return false;
    }
}