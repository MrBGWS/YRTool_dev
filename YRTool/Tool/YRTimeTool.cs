using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class YRTimeTool
{
    public static TimeToolBehaviour timeHandler;
    static YRTimeTool()
    {
        GameObject go = new GameObject("#YRTimeTool#");
        Object.DontDestroyOnLoad(go);
        timeHandler = go.AddComponent<TimeToolBehaviour>();
    }
    /// <summary>
    /// ����x���ִ�лص�
    /// </summary>
    /// <param name="time"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static Coroutine WaitTime(float time,UnityAction action)
    {
        return timeHandler.StartCoroutine(Coroutine(time, action));
    }
    /// <summary>
    /// ȡ��Э��
    /// </summary>
    /// <param name="coroutine"></param>
    public static void CancelWait(ref Coroutine coroutine)
    {
        if (coroutine != null)
        {
            timeHandler.StopCoroutine(coroutine);
            coroutine = null;
        }
    }
    static IEnumerator Coroutine(float time,UnityAction callback)
    {
        yield return new WaitForSeconds(time);
        callback?.Invoke();
    }
    public class TimeToolBehaviour : MonoBehaviour { }
}
