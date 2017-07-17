using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssignShadowReceiver : MonoBehaviour {

	[MenuItem ( "Component/Unity/Assign Shadow Receiver to all childs" )]
	static void AssignChilds ()
	{
		Debug.Log ( "Selection :" + Selection.activeGameObject.name );

		MeshRenderer mr = Selection.activeGameObject.GetComponent<MeshRenderer> ( );

		if ( mr != null )
		{
			if ( mr.gameObject.GetComponent<ShadowReceiver> ( ) == null )
			{
				mr.gameObject.AddComponent<ShadowReceiver> ( );
			}
		}

		MeshRenderer [] mrs = Selection.activeGameObject.GetComponentsInChildren< MeshRenderer >();

		for ( int i = 0; i < mrs.Length; i++ )
		{
			if ( mrs[i].gameObject.GetComponent<ShadowReceiver> ( ) == null )
			{
				mrs[i].gameObject.AddComponent<ShadowReceiver> ( );
			}

		}

	}
	

}
