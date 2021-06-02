using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomNotes.Data
{
    public class ColliderEvent : MonoBehaviour
    {
        bool visualsSwitched = false;
        BoxCollider collider;
        void OnEnable()
        {
            collider = GetComponent<BoxCollider>();
            Logger.log.Info("HEY THERE COLLISION STARTING");
            Debug.Log(GetComponent<Collider>().isTrigger);
            visualsSwitched = false;

            foreach (Transform child in gameObject.transform.parent)
            {
                if (child.name.StartsWith("CustomNotes"))
                {
                    foreach (Transform subChild in child.GetChild(0))
                    {
                        if (subChild.name.StartsWith("RigidbodyNote"))
                        {
                            subChild.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }

        void Update()
        {
            if (gameObject.transform.position.z > 100) OnEnable();
            if (visualsSwitched) return;
            Collider[] _colliders = new Collider[4];
            Vector3 mult = new Vector3(
                (transform.lossyScale.x / transform.localScale.x) * collider.size.x / 2,
                (transform.lossyScale.y / transform.localScale.y) * collider.size.y / 2,
                (transform.lossyScale.z / transform.localScale.z) * collider.size.z / 2
            );
            LayerMask mask = ~0;
            mask = mask & ~(1 << 0);
            mask = mask & ~(1 << 1);
            mask = mask & ~(1 << 2);
            mask = mask & ~(1 << 3);
            mask = mask & ~(1 << 4);
            mask = mask & ~(1 << 5);
            mask = mask & ~(1 << 6);
            mask = mask & ~(1 << 7);
            mask = mask & ~(1 << 8);
            mask = mask & ~(1 << 0);
            mask = mask & ~(1 << 9);
            int num = Physics.OverlapBoxNonAlloc(transform.position, mult, _colliders, transform.rotation, mask);
            if(num > 0)
            {
                visualsSwitched = true;
                Logger.log.Info(_colliders[0].gameObject.name);
                Logger.log.Info(_colliders[0].gameObject.layer.ToString());
                Logger.log.Info("HOLY SHIT SABER");
                foreach(Transform child in gameObject.transform.parent)
                {
                    if (child.name.StartsWith("CustomNotes"))
                    {
                        foreach(Transform subChild in child.GetChild(0))
                        {
                            if (subChild.name.StartsWith("RigidbodyNote"))
                            {
                                subChild.localPosition = Vector3.zero;
                                subChild.localScale = new Vector3(2.6f, 2.6f, 2.6f);
                                subChild.rotation = Quaternion.identity;
                                subChild.Rotate(new Vector3(90, 0, 0));
                                GameObject newChild = Instantiate(subChild.gameObject);
                                newChild.transform.parent = null;
                                newChild.transform.position = subChild.transform.position - new Vector3(0, 0, 0.25f);
                                newChild.transform.localScale = subChild.transform.lossyScale;
                                newChild.transform.rotation = subChild.transform.rotation;
                                newChild.gameObject.SetActive(true);
                                newChild.AddComponent<NoteExtensions.VelocityMult>();
                                CustomNotes.Utilities.LayerUtils.SetLayer(newChild, 0);
                                // profit? should be done
                            }
                        }
                    }
                }

            }
        }

        void OnTriggerEnter(Collider e)
        {
            Logger.log.Info("ENTER COLLISSION");
            Logger.log.Info(gameObject.name);
            Logger.log.Info(gameObject.transform.parent.name);
        }
    }
}
