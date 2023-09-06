using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;

public class KeyboardManager : MonoBehaviour
{
    public OSCReceiver m_OSCReceiver;
    public float m_ValueMultiplier;
    public List<GameObject> m_SphereList = new List<GameObject>();
    public LesAlarmesManager m_AlarmesManager;

    public void Init()
    {
        for (int i = 1; i <= 10; i++)
        {
            ShowManager.m_Instance.OSCReceiver.Bind("/Note" + i.ToString(), OSCNote);
            ShowManager.m_Instance.OSCReceiver.Bind("/Velocity" + i.ToString(), OSCVelocity);
            //GameObject _NewSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //_NewSphere.transform.position = new Vector3(3 * i, 0, 0);
            //m_SphereList.Add(_NewSphere);
        }
    }

    void Update()
    {

    }

    void OSCNote(OSCMessage message)
    {
        string[] _SplitArray = message.Address.Split('e');
        bool _ParsingSuccess = int.TryParse(_SplitArray[1], out int _NoteNumber);

        if (_ParsingSuccess)
        {
            for (int i = 1; i <= 10; i++)
            {
                if (_NoteNumber == i)
                    //m_SphereList[i-1].transform.localPosition = new Vector3(m_SphereList[i-1].transform.localPosition.x, m_SphereList[i - 1].transform.localPosition.y + message.Values[0].IntValue * m_ValueMultiplier, m_SphereList[i-1].transform.localPosition.z);
                    m_SphereList[i - 1].transform.position += m_SphereList[i - 1].transform.forward * m_ValueMultiplier;

                if(message.Values[0].IntValue == 27)
                {
                    m_AlarmesManager.FadeOut();
                }
            }
        }
    }

    void OSCVelocity(OSCMessage message)
    {

    }
}