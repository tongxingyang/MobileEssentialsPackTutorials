using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.NativePlugins;

public class PluginDemo : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GetContacts()
	{
		NPBinding.AddressBook.ReadContacts ( OnReceivingContacts );
	}

	private     AddressBookContact[]    m_contactsInfo = null;
	private     Texture[]               m_contactPictures;

	[SerializeField]
	GameObject  PrefabContactItem;

	[SerializeField]
	GameObject  ContactsPanel;

	[SerializeField]
	GameObject  contentParent;
	private void OnReceivingContacts ( eABAuthorizationStatus _authorizationStatus , AddressBookContact[] _contactList )
	{

		if ( _contactList != null )
		{

			// Cache received contacts info
			m_contactsInfo = _contactList;

			// Start loading images
			int     _totalContacts      = _contactList.Length;
			m_contactPictures = new Texture[_totalContacts];

			for ( int _iter = 0; _iter < _totalContacts; _iter++ )
				LoadContactsImageAtIndex ( _iter );

			ContactsPanel.SetActive ( true );

			for ( int i = 0; i < _totalContacts; i++ )
			{
				GameObject contact = Instantiate( PrefabContactItem , contentParent.transform  );

				ContactItem cc = contact.GetComponent<ContactItem> ( );
				cc.SetInfo ( m_contactsInfo[i] );
			}

		}
		else
		{
			Debug.LogWarning ( "Addressbook : no contacts received" );
		}

		

		


	}

	private void LoadContactsImageAtIndex ( int _index )
	{
		AddressBookContact  _contactInfo    = m_contactsInfo[_index];

		_contactInfo.GetImageAsync ( ( Texture2D _texture , string _error ) => {
			if ( !string.IsNullOrEmpty ( _error ) )
			{
				Debug.LogError ( "[AddressBook] Contact Picture download failed " + _error );
				
				m_contactPictures[_index] = null;
			}
			else
			{
				m_contactPictures[_index] = _texture;
			}
		} );
	}


	[SerializeField, Header("Share Properties ")]
	private     string          m_shareMessage      = "share message, My awesome sharing of this game.";
	[SerializeField]
	private     string          m_shareURL          = "http://www.google.com";

	public void ShareFB ()
	{
		// Create composer
		FBShareComposer _composer   = new FBShareComposer();
		_composer.Text = m_shareMessage;
		_composer.URL = m_shareURL;

		// Show share view
		NPBinding.Sharing.ShowView ( _composer , FinishedSharing );
	}

	private void FinishedSharing ( eShareResult _result )
	{
		Debug.Log ( "Finished sharing" );
		Debug.Log ( "Share Result = " + _result );
	}

	public void ShareWhatsApp()
	{
		// Create composer
		WhatsAppShareComposer _composer = new WhatsAppShareComposer();
		_composer.Text = m_shareMessage;

		// Show share view
		NPBinding.Sharing.ShowView ( _composer , FinishedSharing );
	}


}
