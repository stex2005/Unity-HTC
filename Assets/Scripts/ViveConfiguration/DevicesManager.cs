using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevicesManager : MonoBehaviour {

    public GameObject deviceModel;
    
	void Awake () {
		for(int i=0; i<16; i++)
        {
            Device device=GameObject.Instantiate(deviceModel, this.transform).GetComponent<Device>();
            device.DeviceIndex = i;
            device.name = "Device" + i;
        }
	}
}
