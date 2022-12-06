using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KneeSystems
{
    public class MainController : MonoBehaviour
    {
        ConsultAppController consultAppController;
        // Start is called before the first frame update
        void Start()
        {
            consultAppController = gameObject.AddComponent<ConsultAppController>();
            consultAppController.GenerateConsultApp();
        }
    }
}
