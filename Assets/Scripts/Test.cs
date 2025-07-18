using TMPro;
using UnityEngine;

public class Test : MonoBehaviour
{
    public TextMeshProUGUI TotalScoreValue;

    public TMP_InputField TotalCountField;
    public TMP_InputField ElectronicCountField;
    public TMP_InputField OrganicCountField;
    public TMP_InputField PlasticMetalCountField;
    public TMP_InputField GlassCountField;
    public TMP_InputField PaperCountField;
    public TMP_InputField ChangeBinsCountField;
    public TMP_InputField tutorialFinished;

    private void Start()
    {
        TotalScoreValue.text = SaveSystem.Instance.Player.GetTotalScore().ToString();

        TotalCountField.text = SaveSystem.Instance.Player.TotalCount.ToString();
        ElectronicCountField.text = SaveSystem.Instance.Player.ElectronicsBatteriesCount.ToString();
        OrganicCountField.text = SaveSystem.Instance.Player.OrganicCount.ToString();
        PlasticMetalCountField.text = SaveSystem.Instance.Player.PlasticMetalCount.ToString();
        GlassCountField.text = SaveSystem.Instance.Player.GlassCount.ToString();
        PaperCountField.text = SaveSystem.Instance.Player.PaperCount.ToString();
        ChangeBinsCountField.text = SaveSystem.Instance.Player.CountChangeBins.ToString();
        tutorialFinished.text = (BoolToInt(!SaveSystem.Instance.Player.TutorialFinished)).ToString();
    }

    public void SaveAndPlay()
    {
        SaveSystem.Instance.Player.PaperCount = int.Parse(PaperCountField.text);
        SaveSystem.Instance.Player.GlassCount = int.Parse(GlassCountField.text);
        SaveSystem.Instance.Player.PlasticMetalCount = int.Parse(PlasticMetalCountField.text);
        SaveSystem.Instance.Player.OrganicCount = int.Parse(OrganicCountField.text);
        SaveSystem.Instance.Player.ElectronicsBatteriesCount = int.Parse(ElectronicCountField.text);
        SaveSystem.Instance.Player.TotalCount = int.Parse(TotalCountField.text);
        SaveSystem.Instance.Player.CountChangeBins = int.Parse(ChangeBinsCountField.text);
        SaveSystem.Instance.Player.TutorialFinished = !IntToBool(int.Parse(tutorialFinished.text));

        BootMain.Instance.LoadSceneFromBoot(Scenes.Gameplay);
    }
    public bool IntToBool(int value)
    {
        return value == 1;
    }
    public int BoolToInt(bool value)
    {
        return value ? 1 : 0;
    }
    public void ClearPlayerData()
    {
        SaveSystem.Instance.Rebut();

        BootMain.Instance.LoadSceneFromBoot(Scenes.Gameplay);
    }
}
