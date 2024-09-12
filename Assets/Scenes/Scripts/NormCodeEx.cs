using UnityEngine;

public class NormCodeEx : MonoBehaviour
{
    public float VarName = 0f;

    [SerializeField] private char m_VarName = '_';

    [HideInInspector] private char m_VarName2 = '_';

    //[SerializeField] public char VarName = '_'; NOPE: SerializeField is useless on a public var

    public bool IsDirty = false;

    private bool m_IsDirty = false;

    void FunctionName(int varName)
    {  
    }
}
