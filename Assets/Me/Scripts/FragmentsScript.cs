using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentsScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(killMySelf());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator killMySelf() {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
