using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProgressMenu : MonoBehaviour
{
#pragma warning disable 0649
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
#pragma warning restore 0649

    private readonly Color bronze = new Color(205f / 255f, 127f / 255f, 032f / 255f);
    private readonly Color silver = new Color(201f / 255f, 192f / 255f, 187f / 255f);
    private readonly Color golden = new Color(186f / 255f, 166f / 255f, 010f / 255f);
    private readonly Color black  = new Color(000f / 255f, 000f / 255f, 000f / 255f);

    public void UpdatePercents()
    {
        total.text = $"{Mathf.RoundToInt(GetTotalPercentage())}%";

        //worldOne.text =     $"{GetWorldPercentage(1)}%";
        oneDashOne.text = $"{Mathf.RoundToInt(GetLevelPercentage("1-1"))}%";
        Image[] pipsOneOne = oneDashOne.GetComponentsInChildren<Image>();
        SetSpecialPotPips(pipsOneOne, "1-1");

        oneDashTwo.text = $"{Mathf.RoundToInt(GetLevelPercentage("1-2"))}%";
        Image[] pipsOneTwo = oneDashTwo.GetComponentsInChildren<Image>();
        SetSpecialPotPips(pipsOneTwo, "1-2");

        oneDashThree.text = $"{Mathf.RoundToInt(GetLevelPercentage("1-3"))}%";

        //worldTwo.text =     $"{GetWorldPercentage(2)}%";
        twoDashOne.text = $"{Mathf.RoundToInt(GetLevelPercentage("2-1"))}%";
        Image[] pipsTwoOne = twoDashOne.GetComponentsInChildren<Image>();
        SetSpecialPotPips(pipsTwoOne, "2-1");

        twoDashTwo.text = $"{Mathf.RoundToInt(GetLevelPercentage("2-2"))}%";
        Image[] pipsTwoTwo = twoDashTwo.GetComponentsInChildren<Image>();
        SetSpecialPotPips(pipsTwoTwo, "2-2");

        twoDashThree.text = $"{Mathf.RoundToInt(GetLevelPercentage("2-3"))}%";

        //worldThree.text =     $"{GetWorldPercentage(3)}%";
        threeDashOne.text = $"{Mathf.RoundToInt(GetLevelPercentage("3-1"))}%";
        Image[] pipsThreeOne = threeDashOne.GetComponentsInChildren<Image>();
        SetSpecialPotPips(pipsThreeOne, "3-1");

        threeDashTwo.text = $"{Mathf.RoundToInt(GetLevelPercentage("3-2"))}%";
        Image[] pipsThreeTwo = threeDashTwo.GetComponentsInChildren<Image>();
        SetSpecialPotPips(pipsThreeTwo, "3-2");

        threeDashThree.text = $"{Mathf.RoundToInt(GetLevelPercentage("3-3"))}%";

        //worldFour.text =      $"{GetWorldPercentage(4)}%";
        fourDashOne.text = $"{Mathf.RoundToInt(GetLevelPercentage("4-1"))}%";
        Image[] pipsFourOne = fourDashOne.GetComponentsInChildren<Image>();
        SetSpecialPotPips(pipsFourOne, "4-1");

        fourDashTwo.text = $"{Mathf.RoundToInt(GetLevelPercentage("4-2"))}%";
        Image[] pipsFourTwo = fourDashTwo.GetComponentsInChildren<Image>();
        SetSpecialPotPips(pipsFourTwo, "4-2");

        fourDashThree.text = $"{Mathf.RoundToInt(GetLevelPercentage("4-3"))}%";
    }

    public void SetSpecialPotPips(Image[] pips, string levelName)
    {
        StringLevelDictionary levels = LevelManager.Instance.Levels;

        if (!levels.ContainsKey(levelName)) return;

        LevelData level = levels[levelName];

        if (level.SpecialPots.ContainsKey("Bronze Pot") && level.SpecialPots["Bronze Pot"]) pips.Single(i => i.name == "Bronze").color = bronze;
        if (level.SpecialPots.ContainsKey("Silver Pot") && level.SpecialPots["Silver Pot"]) pips.Single(i => i.name == "Silver").color = silver;
        if (level.SpecialPots.ContainsKey("Golden Pot") && level.SpecialPots["Golden Pot"]) pips.Single(i => i.name == "Golden").color = golden;
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
        if (level.SpecialPots.ContainsKey("Crate") && level.SpecialPots["Crate"]) total += 10f;
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
