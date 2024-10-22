using UnityEngine;

public class DDOL : MonoBehaviour
{

    public void Awake() 
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        DontDestroyOnLoad(gameObject);
    }

}
