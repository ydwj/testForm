using UnityEngine;
using System.Collections;

public class Debuger  
{
#if _RELEASE_
	static public bool EnableLog = false;
#else
	static public bool EnableLog = true;
#endif//_DEBUG_
	static public void Log(object message)
	{
        if (EnableLog)
        {
            Log(message, null);
        }
	}
	static public void Log(object message, Object context)
	{
		if(EnableLog)
		{
			Debug.Log(message,context);
		}
	}
	static public void LogError(object message)
	{
		LogError(message,null);
	}
	static public void LogError(object message, Object context)
	{
		if(EnableLog)
		{
			Debug.LogError(message,context);
		}
	}
	static public void LogWarning(object message)
	{
		LogWarning(message,null);
	}
	static public void LogWarning(object message, Object context)
	{
		if(EnableLog)
		{
			Debug.LogWarning(message,context);
		}
	}
	static public void Assert(bool condition)
	{
		if(EnableLog)
		{
			Debug.Assert(condition);
		}
	}
}
