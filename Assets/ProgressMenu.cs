using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class ProgressMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI total;
    [Space]
    [SerializeField] TextMeshProUGUI worldOne;
    [SerializeField] TextMeshProUGUI oneDashOne;
    [SerializeField] TextMeshProUGUI oneDashTwo;
    [SerializeField] TextMeshProUGUI oneDashThree;
    [Space]
    [SerializeField] TextMeshProUGUI worldTwo;
    [SerializeField] TextMeshProUGUI twoDashOne;
    [SerializeField] TextMeshProUGUI twoDashTwo;
    [SerializeField] TextMeshProUGUI twoDashThree;
    [Space]
    [SerializeField] TextMeshProUGUI worldThree;
    [SerializeField] TextMeshProUGUI threeDashOne;
    [SerializeField] TextMeshProUGUI threeDashTwo;
    [SerializeField] TextMeshProUGUI threeDashThree;
    [Space]
    [SerializeField] TextMeshProUGUI worldFour;
    [SerializeField] TextMeshProUGUI fourDashOne;
    [SerializeField] TextMeshProUGUI fourDashTwo;
    [SerializeField] TextMeshProUGUI fourDashThree;

    public void UpdatePercents()
    {
        total.text =        $"{Mathf.RoundToInt(GetTotalPercentage())}%";

        //worldOne.text =     $"{GetWorldPercentage(1)}%";
        oneDashOne.text =   $"{Mathf.RoundToInt(GetLevelPercentage("1-1"))}%";
        oneDashTwo.text =   $"{Mathf.RoundToInt(GetLevelPercentage("1-2"))}%";
        oneDashThree.text = $"{Mathf.RoundToInt(GetLevelPercentage("1-3"))}%";

        //worldTwo.text =     $"{GetWorldPercentage(2)}%";
        twoDashOne.text =   $"{Mathf.RoundToInt(GetLevelPercentage("2-1"))}%";
        twoDashTwo.text =   $"{Mathf.RoundToInt(GetLevelPercentage("2-2"))}%";
        twoDashThree.text = $"{Mathf.RoundToInt(GetLevelPercentage("2-3"))}%";

        //worldThree.text =     $"{GetWorldPercentage(3)}%";
        threeDashOne.text =   $"{Mathf.RoundToInt(GetLevelPercentage("3-1"))}%";
        threeDashTwo.text =   $"{Mathf.RoundToInt(GetLevelPercentage("3-2"))}%";
        threeDashThree.text = $"{Mathf.RoundToInt(GetLevelPercentage("3-3"))}%";

        //worldFour.text =      $"{GetWorldPercentage(4)}%";
        fourDashOne.text =   $"{Mathf.RoundToInt(GetLevelPercentage("4-1"))}%";
        fourDashTwo.text =   $"{Mathf.RoundToInt(GetLevelPercentage("4-2"))}%";
        fourDashThree.text = $"{Mathf.RoundToInt(GetLevelPercentage("4-3"))}%";
    }

    public float GetLevelPercentage(string levelName)
    {
        StringLevelDictionary levels = LevelManager.Instance.Levels;

        if (!levels.ContainsKey(levelName)) return 0f;

        LevelData level = levels[levelName];

        float total = (float)level.CollectedPots.Where(p => p.Value == true).Count() / (float)level.TotalPots * 100f;


        if (level.SpecialPots.ContainsKey("Bronze Pot") && level.SpecialPots["Bronze Pot"]) total += 3.33f;
        if (level.SpecialPots.ContainsKey("Silver Pot") && level.SpecialPots["Silver Pot"]) total += 3.33f;
        if (level.SpecialPots.ContainsKey("Golden Pot") && level.SpecialPots["Golden Pot"]) total += 3.34f;
        //Crate should add an additional percent but can't be tracked currently

        return total;
    }

    public float GetWorldPercentage(int world)
    {
        float levelOne = GetLevelPercentage(world + "-1");
        float levelTwo = GetLevelPercentage(world + "-2");
        float levelThree = GetLevelPercentage(world + "-3");

        return (levelOne + levelTwo + levelThree) / 3f;
    }

    public float GetTotalPercentage()
    {
        float percent = 0;

        for(int i = 1; i < 5; i++)
        {
            percent += GetWorldPercentage(i);
        }

        percent /= 4;

        return percent;
    }
}
