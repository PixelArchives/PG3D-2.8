using System;
using System.Runtime.CompilerServices;
using Prime31;

public class FacebookManager : AbstractManager
{
	[method: MethodImpl(32)]
	public static event Action sessionOpenedEvent;

	[method: MethodImpl(32)]
	public static event Action preLoginSucceededEvent;

	[method: MethodImpl(32)]
	public static event Action<string> loginFailedEvent;

	[method: MethodImpl(32)]
	public static event Action<string> dialogCompletedWithUrlEvent;

	[method: MethodImpl(32)]
	public static event Action<string> dialogFailedEvent;

	[method: MethodImpl(32)]
	public static event Action<object> graphRequestCompletedEvent;

	[method: MethodImpl(32)]
	public static event Action<string> graphRequestFailedEvent;

	[method: MethodImpl(32)]
	public static event Action<object> restRequestCompletedEvent;

	[method: MethodImpl(32)]
	public static event Action<string> restRequestFailedEvent;

	[method: MethodImpl(32)]
	public static event Action<bool> facebookComposerCompletedEvent;

	[method: MethodImpl(32)]
	public static event Action reauthorizationSucceededEvent;

	[method: MethodImpl(32)]
	public static event Action<string> reauthorizationFailedEvent;

	static FacebookManager()
	{
		AbstractManager.initialize(typeof(FacebookManager));
	}

	public void sessionOpened(string accessToken)
	{
		FacebookManager.preLoginSucceededEvent.fire();
		Facebook.instance.accessToken = accessToken;
		FacebookManager.sessionOpenedEvent.fire();
	}

	public void loginFailed(string error)
	{
		FacebookManager.loginFailedEvent.fire(error);
	}

	public void dialogCompletedWithUrl(string url)
	{
		FacebookManager.dialogCompletedWithUrlEvent.fire(url);
	}

	public void dialogFailedWithError(string error)
	{
		FacebookManager.dialogFailedEvent.fire(error);
	}

	public void graphRequestCompleted(string json)
	{
		if (FacebookManager.graphRequestCompletedEvent != null)
		{
			object param = Json.jsonDecode(json);
			FacebookManager.graphRequestCompletedEvent.fire(param);
		}
	}

	public void graphRequestFailed(string error)
	{
		FacebookManager.graphRequestFailedEvent.fire(error);
	}

	public void restRequestCompleted(string json)
	{
		if (FacebookManager.restRequestCompletedEvent != null)
		{
			object param = Json.jsonDecode(json);
			FacebookManager.restRequestCompletedEvent.fire(param);
		}
	}

	public void restRequestFailed(string error)
	{
		FacebookManager.graphRequestFailedEvent.fire(error);
	}

	public void facebookComposerCompleted(string result)
	{
		FacebookManager.facebookComposerCompletedEvent.fire(result == "1");
	}

	public void reauthorizationSucceeded(string empty)
	{
		FacebookManager.reauthorizationSucceededEvent.fire();
	}

	public void reauthorizationFailed(string error)
	{
		FacebookManager.reauthorizationFailedEvent.fire(error);
	}
}
