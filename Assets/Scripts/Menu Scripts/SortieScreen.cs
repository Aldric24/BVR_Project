using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SortieScreen : MonoBehaviour
{
    [SerializeField]MenuController menuController;
    public List<Mission> missions;
    public GameObject missionButtonPrefab;
    public MissionButton SelectedMission;
    public GameObject brifingpanel;
    [SerializeField] Transform targetPosition;
    public Vector3 oldpos;
    public GameObject loadout;
    [SerializeField] TextMeshProUGUI bestmissionTime;
    [SerializeField ]GameController gameController;
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        foreach (Mission mission in missions)
        {
            GameObject missionButton = Instantiate(missionButtonPrefab, transform);
            missionButton.gameObject.name = mission.missionName;
            missionButton.GetComponent<MissionButton>().SetMission(mission);

            
        }
        loadout.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {

            gameController.setmission(SelectedMission.name);
        });
    }
    public void displaybriefing()
    {
        Debug.Log(gameController.bestMissionTimes.Count);
        if(gameController.bestMissionTimes.ContainsKey(SelectedMission.name))
        {
            bestmissionTime.text = "Best Time: " + gameController.bestMissionTimes[SelectedMission.name].ToString("F2") + "s";
        }
        else
        {
            bestmissionTime.text = "Best Time: 0.00s";
        }
       
        loadout.GetComponent<Button>().interactable = true;
        foreach (Transform t in transform)
        {
            if (t.gameObject.name != SelectedMission.name) // Check if not the selected mission
            {

                fadeOut(t.gameObject);
                
            }
        }
        this.gameObject.GetComponent<LayoutGroup>().enabled = false;
        MovetoBrifing(SelectedMission.gameObject);
        brifingpanel.GetComponentInChildren<TextMeshProUGUI>().text = SelectedMission.GetComponent<MissionButton>()._missionDescription;
        brifingpanel.SetActive(true);
    }
    void MovetoBrifing(GameObject mission)
    { 
        mission.GetComponent<Button>().interactable = false;
        LeanTween.move(mission, targetPosition, 1.5f).setEase(LeanTweenType.easeInOutSine);
        LeanTween.scale(mission, new Vector3(1.2f, 1.2f, 1.2f), 1.5f).setEase(LeanTweenType.easeInOutSine);
    }
    public void MovetoOldPos(GameObject mission)
    {
        if(SelectedMission!=null)
        {
            LeanTween.move(mission, oldpos, 1.5f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() => gameObject.GetComponent<LayoutGroup>().enabled = true );
            mission.GetComponent<Button>().interactable = true; 
            LeanTween.scale(mission, new Vector3(1f, 1f, 1f), 1.5f).setEase(LeanTweenType.easeInOutSine);
        }
      
    }
    public void back()
    {
        
        MovetoOldPos(SelectedMission.gameObject);
        foreach (Transform t in transform)
        {
            fadein(t.gameObject);
        }
        
        brifingpanel.SetActive(false);
        SelectedMission = null;
        
    }
    void fadeOut(GameObject mission)
    {
        //turn down the alpha of the mission using canvas group and then disable it
        LeanTween.alphaCanvas(mission.GetComponent<CanvasGroup>(), 0, 1.5f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() => mission.SetActive(false));
        
        
    }
    void fadein(GameObject mission)
    {
        //turn up the alpha of the mission and then enable it
        mission.SetActive(true);
        LeanTween.alphaCanvas(mission.GetComponent<CanvasGroup>(), 1, 1.5f).setEase(LeanTweenType.easeInOutSine);
    }
    public void backtomainmenu()
    {
        if(SelectedMission==null)
        {
            loadout.GetComponent<Button>().interactable = true;
            this.gameObject.transform.parent.gameObject.SetActive(false);
            menuController.menuScreens[0].SetActive(true);
        }
        else
        {
            loadout.GetComponent<Button>().interactable = false;
            back();
        }
    }
}

