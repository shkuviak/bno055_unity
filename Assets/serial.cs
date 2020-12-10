using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.IO.Ports;

public class serial : MonoBehaviour
{
    private SerialPort _serialPort;

    public GameObject Right_leg; //The Cube
    public Text Text_CAL_Sys;
    public Text Text_CAL_Gyr;
    public Text Text_CAL_Acc;
    public Text Text_CAL_Mag;

    private StreamWriter writer;
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

        // Write to disk
        writer = new StreamWriter("MyPath.csv", true);
        writer.WriteLine("pitch;roll;yaw");
    }

    // Update is called once per frame
    void Update()
    {
        string data = _serialPort.ReadLine();
        Debug.Log(data);

        BNOnodes bnos = BNOnodes.CreateFromJSON(data);
        STMData sTMData = bnos.nodes[0];

        // Update angle
        Quaternion rotation = Quaternion.Euler((float)sTMData.euler_yaw / 16, (float)sTMData.euler_roll / 16, (float)sTMData.euler_pitch / 16);

        // Debug.Log((float)sTMData.euler_yaw / 16);
        writer.WriteLine(String.Join(";", new List<double> { sTMData.euler_pitch, sTMData.euler_roll, sTMData.euler_yaw }));

        Right_leg.transform.localRotation = rotation;

        // Update calibration
        string binary = Convert.ToString(sTMData.calib_state, 2).PadLeft(8, '0');
        if(binary[7 - 7] == '1' && binary[7 - 6] == '1')
            Text_CAL_Sys.text = "Sys calibrated";
        else
            Text_CAL_Sys.text = "Sys not calibrated";
        if (binary[7 - 5] == '1' && binary[7 - 4] == '1')
            Text_CAL_Gyr.text = "Gyr calibrated";
        else
            Text_CAL_Gyr.text = "Gyr not calibrated";
        if (binary[7 - 3] == '1' && binary[7 - 2] == '1')
            Text_CAL_Acc.text = "Acc calibrated";
        else
            Text_CAL_Acc.text = "Acc not calibrated";
        if (binary[7 - 1] == '1' && binary[7 - 0] == '1')
            Text_CAL_Mag.text = "Mag calibrated";
        else
            Text_CAL_Mag.text = "Mag not calibrated";
    }

    void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
        writer.Close();
    }

    
}
