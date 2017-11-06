using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(KillMyself());

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator KillMyself() {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
