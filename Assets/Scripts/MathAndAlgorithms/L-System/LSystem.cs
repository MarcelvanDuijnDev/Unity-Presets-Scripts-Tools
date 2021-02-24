using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSystem : MonoBehaviour 
{
    [Header("Settings")]
    [SerializeField] private int _MaxItterations = 5;

    private int _Type;
    private string _Axiom;
    private float _Angle;
    private string _CurrentString;
    private Dictionary<char,string> _Rules = new Dictionary<char,string>();
    private Stack<LSystem_TransformInfo> _TransformStack = new Stack<LSystem_TransformInfo>();

    private float _Length;
    private bool _IsgGenerating = false;

    [Header("Info")]
    [SerializeField] private int _Itterations = 0;

    void Start()
    {
        if (_Type == 0)
        {
            _Axiom = "F";
            _Rules.Add('F', "FF+[+F-F-F]-[-F+F+F]");
            _Angle = 25;
            _Length = 10f;
        }
        if (_Type == 1) //Needs fix
        {
            _Axiom = "B";
            _Rules.Add('A', "B-A-B");
            _Rules.Add('B', "A+B+A");
            _Angle = 60;
            _Length = 10f;
        }
        _CurrentString = _Axiom;
        StartCoroutine(GenerateSystem());
    }

    IEnumerator GenerateSystem()
    {
        int count = 0;

        while (count < 5)
        {
            if (!_IsgGenerating)
            {
                _IsgGenerating = true;
                StartCoroutine(Generate());
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
        
    IEnumerator Generate()
    {
        if (_Itterations < _MaxItterations)
        {
            _Itterations++;
            _Length = _Length / 2f;
            string newString = "";
            char[] stringCharacters = _CurrentString.ToCharArray();

            for (int i = 0; i < stringCharacters.Length; i++)
            {
                char currentCharacter = stringCharacters[i];

                if (_Rules.ContainsKey(currentCharacter))
                {
                    newString += _Rules[currentCharacter];
                }
                else
                {
                    newString += currentCharacter.ToString();
                }
            }
            _CurrentString = newString;

            for (int i = 0; i < stringCharacters.Length; i++)
            {
                char currentCharacter = stringCharacters[i];
                if (currentCharacter == 'F' || currentCharacter == 'G')
                {
                    Vector3 initialPosition = transform.position;
                    transform.Translate(Vector3.forward * _Length);
                    Debug.DrawLine(initialPosition, transform.position, Color.white, 10000f, false);
                    yield return null;
                }
                else if (currentCharacter == '+')
                {
                    transform.Rotate(Vector3.up * _Angle);
                }
                else if (currentCharacter == '-')
                {
                    transform.Rotate(Vector3.up * -_Angle);
                }
                else if (currentCharacter == '[')
                {
                    LSystem_TransformInfo ti = new LSystem_TransformInfo();
                    ti.Position = transform.position;
                    ti.Rotation = transform.rotation;
                    _TransformStack.Push(ti);
                }
                else if (currentCharacter == ']')
                {
                    LSystem_TransformInfo ti = _TransformStack.Pop();
                    transform.position = ti.Position;
                    transform.rotation = ti.Rotation;
                }
            }
            _IsgGenerating = false;
        }
    }
}

public class LSystem_TransformInfo
{
    public Vector3 Position;
    public Quaternion Rotation;
}