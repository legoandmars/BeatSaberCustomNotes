using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace CustomNotes
{
    public class CustomNote
    {

        public string path { get; private set; }
        public AssetBundle assetBundle { get; private set; }
        public NoteDescriptor noteDescriptor { get; private set; }
        public GameObject noteLeft { get; private set; }
        public GameObject noteRight { get; private set; }

        public CustomNote(string path)
        {
            this.path = path;

            if(path != "DefaultNotes")
            {
                assetBundle = AssetBundle.LoadFromFile(path);
                if (assetBundle == null)
                {
                    Logger.log.Warn("something went wrong getting the asset bundle!");
                    return;
                }
                GameObject note = assetBundle.LoadAsset<GameObject>("assets/_customnote.prefab");
                noteDescriptor = note?.GetComponent<NoteDescriptor>();
                //         Console.WriteLine("Succesfully obtained the asset bundle!");
                //            foreach (var c in assetBundle.AllAssetNames())
                //                 Logger.log.Info(c);
                noteLeft = note.transform.Find("NoteLeft").gameObject;
                noteRight = note.transform.Find("NoteRight").gameObject;

            }
            else
            {
                noteDescriptor = new NoteDescriptor { AuthorName = "Beat Saber", NoteName = "Default" };
            }
        }

    }
}
