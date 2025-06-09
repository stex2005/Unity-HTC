using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class Device : MonoBehaviour {

    private enum Type
    {
        BASE_STATION,
        CONTROLLER,
        TRACKER,
        HMD,
        NONE
    }

    public Text idText;
    public Text typeText;
    public Text indexText;
    public InputField nameInput;

    public Sprite baseStationIcon;
    public Sprite controllerStationIcon;
    public Sprite trackerStationIcon;
    public Sprite hmdStationIcon;
    public Image icon;

    public Color inactive;
    public Color active;
    public Color moving;

    private int deviceIndex;
    private string deviceName;
    private string id;
    private Type type;
    private SteamVR_TrackedObject trackedObject;
    private SteamVR_PlayArea playArea;
    private Vector3 lastPosition = Vector3.zero;
    private float speed=0f;
    private const float MAX_SPEED=1.0f;// m/s

    public int DeviceIndex
    {
        set
        {
            this.deviceIndex = value;

            indexText.text = this.deviceIndex.ToString();

            ETrackedPropertyError error = new ETrackedPropertyError();
            StringBuilder result = new StringBuilder();
            OpenVR.System.GetStringTrackedDeviceProperty((uint)this.deviceIndex, ETrackedDeviceProperty.Prop_SerialNumber_String, result, OpenVR.k_unMaxPropertyStringSize, ref error);
            id = result.ToString();
            idText.text = id;

            result = new StringBuilder();
            OpenVR.System.GetStringTrackedDeviceProperty((uint)this.deviceIndex, ETrackedDeviceProperty.Prop_RenderModelName_String, result, 64, ref error);
            type = ExtractType(result.ToString());

            this.typeText.text = type.ToString();
            UpdateIcon();

            if(this.type == Type.CONTROLLER || this.type == Type.TRACKER)
            {
                nameInput.enabled = true;
                nameInput.placeholder.gameObject.SetActive(true);
                nameInput.textComponent.gameObject.SetActive(true);
            }
            else
            {
                nameInput.enabled = false;
                nameInput.placeholder.gameObject.SetActive(false);
                nameInput.textComponent.gameObject.SetActive(false);
            }

            Debug.Log(this.indexText.text + ": " + this.typeText.text);
        }
    }

    public string Name
    {
        set
        {
            this.deviceName = value;
            nameInput.text = this.deviceName;
        }
    }

    public string Id
    {
        get { return id; }
    }

    public string DeviceName
    {
        get { return this.deviceName; }
    }

    public string ToConfigurationFileLine()
    {
        return this.deviceName + ";" + this.id;
    }

	void Awake () {
        playArea = FindObjectOfType<SteamVR_PlayArea>();
    }

    void Start(){
        GameObject go=new GameObject("Device "+deviceIndex);
        trackedObject=go.AddComponent<SteamVR_TrackedObject>();
        trackedObject.index = (SteamVR_TrackedObject.EIndex) deviceIndex;
        go.transform.parent=playArea.transform;
        this.transform.localScale=new Vector3(1,1,1);
    }


    private Type ExtractType(string renderModelName)
    {
        if (renderModelName.Contains("controller"))
        {
            return Type.CONTROLLER;
        }
        if (renderModelName.Contains("tracker"))
        {
            return Type.TRACKER;
        }
        if (renderModelName.Contains("base"))
        {
            return Type.BASE_STATION;
        }
        if (renderModelName.Contains("hmd"))
        {
            return Type.HMD;
        }
        return Type.NONE;
    }

    private void FixedUpdate()
    {
        Vector3 pos = trackedObject.transform.position;
        Vector3 movement = pos - lastPosition;
        lastPosition = pos;
        
        speed=Mathf.Lerp(speed, movement.magnitude/Time.deltaTime, 0.1f);
    }

    public void Update()
    {
        switch (OpenVR.System.GetTrackedDeviceActivityLevel((uint)this.deviceIndex))
        {
            case EDeviceActivityLevel.k_EDeviceActivityLevel_UserInteraction:
                this.icon.color =  Color.Lerp(active, moving, speed/MAX_SPEED);
                break;
            case EDeviceActivityLevel.k_EDeviceActivityLevel_Idle:
                this.icon.color =  Color.Lerp(active, moving, speed/MAX_SPEED);
                //this.icon.color = active;
                break;
            case EDeviceActivityLevel.k_EDeviceActivityLevel_Standby:
            default:
                this.icon.color = inactive;
                break;
        }
    }

    private void UpdateIcon()
    {
        icon.enabled = true;
        switch (this.type)
        {
            case Type.HMD:
                icon.sprite = hmdStationIcon;
                break;
            case Type.CONTROLLER:
                icon.sprite = controllerStationIcon;
                break;
            case Type.TRACKER:
                icon.sprite = trackerStationIcon;
                break;
            case Type.BASE_STATION:
                icon.sprite = baseStationIcon;
                break;
            case Type.NONE:
                icon.enabled = false;
                break;
        }
    }
}
