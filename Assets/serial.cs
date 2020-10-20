using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.IO.Ports;

public class serial : MonoBehaviour
{
    private SerialPort _serialPort;
    private STMData sTMData;

    public GameObject Cube; //The Cube
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello World");

        _serialPort = new SerialPort();
        // Allow the user to set the appropriate properties.
        _serialPort.PortName = "COM3";
        _serialPort.BaudRate = 9600;
        _serialPort.Parity = Parity.None;
        _serialPort.DataBits = 8;
        _serialPort.StopBits = StopBits.One;
        _serialPort.Handshake = Handshake.None;

        if(!_serialPort.IsOpen)
        {
            try
            {
                Debug.Log("Open Serial");
                _serialPort.Open();
                Debug.Log(_serialPort.ReadLine());
            }
            catch (Exception ex)
            {
                
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        string data = _serialPort.ReadLine();
        Debug.Log(data);

        sTMData = STMData.CreateFromJSON(data);

        Quaternion rotation = Quaternion.Euler((float)sTMData.euler_roll / 16, (float)sTMData.euler_yaw / 16, (float)sTMData.euler_pitch / 16);

        Debug.Log((float)sTMData.euler_yaw / 16);
        
        Cube.transform.localRotation = rotation;
    }

    [System.Serializable]
    class STMData
    {
        public int id;
        public int calib_state;
        public double euler_roll;
        public double euler_pitch;
        public double euler_yaw;

        public static STMData CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<STMData>(jsonString);
        }
    }
}
