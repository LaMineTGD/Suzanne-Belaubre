using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;

public class KeyboardManager : MonoBehaviour
{
    public OSCReceiver m_OSCReceiver;
    List <GameObject> m_SphereList = new List<GameObject>();

    void Start()
    {
        for(int i = 1; i <= 10; i++)
        {
            m_OSCReceiver.Bind("/Note" +i.ToString(), OSCNote);
            m_OSCReceiver.Bind("/Velocity" +i.ToString(), OSCVelocity);
            GameObject _NewSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _NewSphere.transform.position = new Vector3(3*i, 0, 0);
            m_SphereList.Add(_NewSphere);
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
                    m_SphereList[i].transform.position = new Vector3(m_SphereList[i].transform.position.x, message.Values[0].IntValue, m_SphereList[i].transform.position.z);
            }
        }
    }

    void OSCVelocity(OSCMessage message)
    {

    }
}