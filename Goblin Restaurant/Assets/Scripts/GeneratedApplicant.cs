using System.Collections.Generic;

// ����: ��� ����� ���� �������� ���� ������ ��� '�����' Ŭ����
public class GeneratedApplicant
{
    // � '����' ���ø����� �����Ǿ����� �����մϴ�.
    public EmployeeData BaseSpeciesData { get; private set; }

    // �������� ������ ������
    public string GeneratedFirstName { get; private set; }
    public string GeneratedJobTitle { get; private set; }

    // �������� ������ �ɷ�ġ �� Ư��
    public int GeneratedCookingStat { get; private set; }
    public int GeneratedServingStat { get; private set; }
    public int GeneratedCleaningStat { get; private set; }
    public List<Trait> GeneratedTraits { get; private set; }

    // ������: EmployeeManager�� ��� ����� ��ģ �� ������� ���޹޽��ϴ�.
    public GeneratedApplicant(EmployeeData speciesData, string firstName, string jobTitle, int cook, int serve, int clean, List<Trait> traits)
    {
        BaseSpeciesData = speciesData;
        GeneratedFirstName = firstName;
        GeneratedJobTitle = jobTitle;
        GeneratedCookingStat = cook;
        GeneratedServingStat = serve;
        GeneratedCleaningStat = clean;
        GeneratedTraits = traits;
    }
}

