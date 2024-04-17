using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class MissionButton : MonoBehaviour
{
    [SerializeField] private Sprite _sprite;
    [SerializeField] private string _missionName;
    public string _missionDescription;
    [SerializeField] private TextMeshProUGUI missionname;
    public AnimationClip anim;
    internal void SetMission(Mission mission)
    {
        _sprite = mission.missionImage;
        _missionName = mission.missionName;
        _missionDescription = mission.missionDescription;
    }

    // Start is called before the first frame update
    void Start()
    {
        missionname.text=_missionName;
        gameObject.GetComponent<UnityEngine.UI.Image>().sprite = _sprite;
        gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            gameObject.GetComponentInParent<SortieScreen>().SelectedMission = this;
            gameObject.GetComponentInParent<SortieScreen>().displaybriefing();
        });
    }

}
