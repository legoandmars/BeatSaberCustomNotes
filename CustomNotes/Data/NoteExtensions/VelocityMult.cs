using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace CustomNotes.Data.NoteExtensions
{
    public class VelocityMult : MonoBehaviour
    {
        Rigidbody rigidbody;
        Vector3 oldVelocity;
        void Start()
        {
            rigidbody = this.GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            if ((rigidbody.velocity.x > 0.1 || rigidbody.velocity.y > 0.1 || rigidbody.velocity.z > 0.1) && oldVelocity == null) oldVelocity = rigidbody.velocity;
            float constFactor = 15;
            Vector3 newVelocity = new Vector3(
                Math.Min(oldVelocity.x * constFactor, constFactor),
                Math.Min(oldVelocity.y * constFactor, constFactor),
                Math.Min(oldVelocity.z * constFactor, constFactor)
            );
            rigidbody. = newVelocity;
        }
    }
}
