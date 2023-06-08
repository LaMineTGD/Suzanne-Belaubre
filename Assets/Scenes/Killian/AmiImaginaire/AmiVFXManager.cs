using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class AmiVFXManager : MonoBehaviour
{
    private const string DROP_SIGNAL = "DropSignalTime"; // Nom de la propriété exposée pour le rayon de la sphère de conformité.
    private const string Drop = "drop_"; // Nom de la propriété exposée pour le rayon de la sphère de conformité.
    private const string CONFORM_FIELD_FORCE = "ConformFieldForce"; // Nom de la propriété exposée pour la force du champ de conformité.

    [SerializeField]
    private VisualEffect vfx;
    // Start is called before the first frame update

    private void ChangeDropLocation(Vector2 location, int drop){
        string name = Drop+drop.ToString();
        vfx.SetVector2(name,location);
    }

    private void DropSignal(){
        float signalTime= Time.time-1.0f;
        vfx.SetFloat(DROP_SIGNAL,signalTime);
        for (int i = 1; i <= 3; i++)
        {
            float random_x = Random.Range(-2.2f, 2.2f);
            float random_y = Random.Range(-1.5f, 1.5f);
            ChangeDropLocation(new Vector2(random_x,random_y),i);
        }

    }

    void OnGouttedeau(InputValue _Value)
    {
        Debug.Log("test"+_Value);
        DropSignal();
    }

    void FixedUpdate() {

    }
}
