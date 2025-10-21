using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class IngredientDataImporter
{
    [MenuItem("Tools/Import Ingredient Data from CSV")]
    public static void Import()
    {
        string path = EditorUtility.OpenFilePanel("Select Ingredient CSV", "", "csv");
        if (string.IsNullOrEmpty(path)) return;

        string[] allLines = File.ReadAllLines(path);

        for (int i = 1; i < allLines.Length; i++)
        {
            string[] cells = allLines[i].Split(',');
            if (cells.Length < 4) continue;

            IngredientData ingredient = ScriptableObject.CreateInstance<IngredientData>();

            ingredient.id = cells[0].Trim();
            ingredient.ingredientName = cells[1].Trim();

            ingredient.rarity = GetRarityFromString(cells[2].Trim());

            int.TryParse(cells[3].Trim(), out ingredient.buyPrice);

            AssetDatabase.CreateAsset(ingredient, $"Assets/Resources/Ingredients/{ingredient.id}.asset");
        }
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Import Complete", "Ingredient data imported successfully!", "OK");
    }

    private static Rarity GetRarityFromString(string rarityString)
    {
        switch (rarityString)
        {
            case "�Ϲ�":
                return Rarity.common;
            case "���":
                return Rarity.Uncommon;
            case "���":
                return Rarity.Rare;
            case "����":
                return Rarity.Legendary;
            default:
                Debug.LogWarning($"�� �� ���� ��͵� ���Դϴ�: '{rarityString}'. �⺻��(Common)���� �����˴ϴ�.");
                return Rarity.common;
        }
    }
}