// https://www.ketra-games.com/2020/10/slow-motion-effect-unity-game-tutorial.html

using UnityEngine;

public class SlowMotion : MonoBehaviour
{
    public float slowMotionTimescale;

    private float startTimescale;
    private float startFixedDeltaTime;

    void Start()
    {
        startTimescale = Time.timeScale;
        startFixedDeltaTime = Time.fixedDeltaTime;
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     StartSlowMotion();
        // }

        // if (Input.GetKeyUp(KeyCode.Space))
        // {
        //     StopSlowMotion();
        // }
    }

    private void StartSlowMotion()
    {
        Time.timeScale = slowMotionTimescale;
        Time.fixedDeltaTime = startFixedDeltaTime * slowMotionTimescale;
    }

    private void StopSlowMotion()
    {
        Time.timeScale = startTimescale;
        Time.fixedDeltaTime = startFixedDeltaTime;
    }
}