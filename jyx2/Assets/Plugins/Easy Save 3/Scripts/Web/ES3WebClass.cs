using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ES3Internal
{
	public class ES3WebClass
	{
		protected string url;
		protected string apiKey;

		protected List<KeyValuePair<string,string>> formData = new List<KeyValuePair<string, string>>();
		protected UnityWebRequest _webRequest = null;


		public bool isDone = false;
		public float uploadProgress
		{
			get
			{ 
				if(_webRequest == null)
					return 0;
				else
					return _webRequest.uploadProgress;
			}
		}

		public float downloadProgress
		{
			get
			{ 
				if(_webRequest == null)
					return 0;
				else
					return _webRequest.downloadProgress;
			}
		}

		#region Error Handling

		/// <summary>An error message, if an error occurred.</summary>
		public string error = null;
		/// <summary>This is set to true if an error occurred while performing an operation.</summary>
		public bool isError{ get{ return !string.IsNullOrEmpty(error) || errorCode > 0; } }
		/// <summary>The error code relating to the error, if one occurred. If it's a server error, this will return the HTTP error code.</summary>
		public long errorCode = 0;

        public static bool IsNetworkError(UnityWebRequest www)
        {
#if UNITY_2020_1_OR_NEWER
            return www.result == UnityWebRequest.Result.ConnectionError;
#else
            return www.isNetworkError;
#endif
        }

#endregion

		protected ES3WebClass(string url, string apiKey)
		{
			this.url = url;
			this.apiKey = apiKey;
		}

#region Other Methods

		/// <summary>Adds POST data to any requests sent by this ES3Cloud object. Use this if you are sending data to a custom script on your server.</summary>
		/// <param name="fieldName">The name of the POST field we want to add.</param>
		/// <param name="value">The string value of the POST field we want to add.</param>
		public void AddPOSTField(string fieldName, string value)
		{
			formData.Add(new KeyValuePair<string, string>(fieldName, value));
		}

#endregion

#region Internal Methods

		protected string GetUser(string user, string password)
		{
			if(string.IsNullOrEmpty(user))
				return "";
			// Final user string is a combination of the username and password, and hashed if encryption is enabled.
			if(!string.IsNullOrEmpty(password))
				user += password;

#if !DISABLE_ENCRYPTION && !DISABLE_HASHING
			user = ES3Internal.ES3Hash.SHA1Hash(user);
#endif
			return user;
		}

		protected WWWForm CreateWWWForm()
		{
			var form = new WWWForm();
			foreach(var kvp in formData)
				form.AddField(kvp.Key, kvp.Value);
			return form;
		}

		/* Checks if an error occurred and sets relevant details, and returns true if an error did occur */
		protected bool HandleError(UnityWebRequest webRequest, bool errorIfDataIsDownloaded)
		{
			if(IsNetworkError(webRequest))
			{
				errorCode = 1;
				error = "Error: " + webRequest.error;
			}
			else if(webRequest.responseCode >= 400)
			{
				errorCode = webRequest.responseCode;
				if(string.IsNullOrEmpty(webRequest.downloadHandler.text))
					error = string.Format("Server returned {0} error with no message", webRequest.responseCode);
				else
					error = webRequest.downloadHandler.text;
			}
			else if(errorIfDataIsDownloaded && webRequest.downloadedBytes > 0)
			{
				errorCode = 2;
				error = "Server error: " + webRequest.downloadHandler.text;
			}
			else
				return false;
			return true;
		}

		protected IEnumerator SendWebRequest(UnityWebRequest webRequest)
		{
			_webRequest = webRequest;
#if !UNITY_2017_2_OR_NEWER
			yield return webRequest.Send();
#else
			yield return webRequest.SendWebRequest();
#endif
		}

		protected virtual void Reset()
		{
			error = null;
			errorCode = 0;
			isDone = false;
		}


#endregion
	}
}
