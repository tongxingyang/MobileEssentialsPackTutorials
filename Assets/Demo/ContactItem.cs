using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VoxelBusters.NativePlugins;

public class ContactItem : MonoBehaviour {

	AddressBookContact contact;

	[SerializeField]
	Text fname;

	[SerializeField]
	Text lname;

	[SerializeField]
	Text phone;

	[SerializeField]
	Text email;

	// Use this for initialization
	 public void SetInfo ( AddressBookContact _C )
	{
		contact = _C;
		fname.text = contact.FirstName;
		lname.text = contact.LastName;
		if ( contact.PhoneNumberList.Length > 0 )
			phone.text = contact.PhoneNumberList[0];
		else
			phone.text = "---";
		if ( contact.EmailIDList.Length > 0 )
			email.text = contact.EmailIDList[0];
		else
			email.text = "---";


	}
	
}
