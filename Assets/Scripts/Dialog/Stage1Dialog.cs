using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1Dialog : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string[] stage1Dialog = {
            "B‚ğ‰Ÿ‚µ‚½‚ç“®‚«‚Ü‚·B.",
            "hahaha"
        };

        DialogManager dm = FindObjectOfType<DialogManager>();
        if (dm != null)
        {
            dm.StartDialog(stage1Dialog);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
