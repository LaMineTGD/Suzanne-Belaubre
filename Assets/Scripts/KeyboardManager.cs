using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;

public class KeyboardManager : MonoBehaviour
{
    public OSCReceiver m_OSCReceiver;
    float _posX, posY;

    void Start()
    {
        m_OSCReceiver.Bind("/Note1", OSCNote);
        m_OSCReceiver.Bind("/Velocity1", OSCVelocity);
    }

    void Update()
    {
        
    }

    void MoveTo(float _X, float _Y)
    {
        transform.position = new Vector3(_X, _Y, transform.position.z);
    }

    void OSCNote(OSCMessage message)
    {
        Debug.Log(message.Values[0].IntValue);
        transform.position = new Vector3(message.Values[0].IntValue, transform.position.y, transform.position.z);
    }

    void OSCVelocity(OSCMessage message)
    {

    }
}