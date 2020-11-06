using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;

public class SaveLoad_XML : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Save()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Foo));

        using (FileStream stream = new FileStream(fileName, FileMode.Create))
        {
            serializer.Serialize(stream, foo);
        }
    }

    public void Load()
    {

        XmlSerializer serializer = new XmlSerializer(typeof(Foo));

        using (FileStream stream = new FileStream(fileName, FileMode.Open))
        {
            var foo = serializer.Deserialize(stream);

            Debug.Log(foo);
        }
    }
}
