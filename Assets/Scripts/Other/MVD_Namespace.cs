using UnityEngine;
using System.Collections;

namespace MVD
{
    public class mvd : MonoBehaviour
    {

        //Movement
        public static void Movement_2D_Platformer(float speed, float shiftspeed, Rigidbody rigidbody, float jumpspeed, float groundcheck, LayerMask groundlayer, Transform playertransform)
        {
            float currentspeed;

            //Sprint
            if (Input.GetKey(KeyCode.LeftShift))
                currentspeed = shiftspeed;
            else
                currentspeed = speed;

            //Jumping
            if (Input.GetButtonDown("Jump") && IsGrounded(groundcheck, groundlayer, playertransform))
                rigidbody.AddForce(new Vector2(0, jumpspeed));

            //Apply Movement
            rigidbody.velocity = new Vector2(Input.GetAxis("Horizontal") * currentspeed, rigidbody.velocity.y);
        }
        static bool IsGrounded(float groundcheck, LayerMask groundlayer, Transform playertransform)
        {
            RaycastHit2D hit = Physics2D.Raycast(playertransform.position, Vector2.down, groundcheck, groundlayer);
            if (hit.collider != null)
            {
                return true;
            }
            return false;
        }
        public static void Movement_2D_TopDown(float speed, float shiftspeed, Rigidbody rigidbody)
        {
            float currentspeed;

            //Sprint
            if (Input.GetKey(KeyCode.LeftShift))
                currentspeed = shiftspeed;
            else
                currentspeed = speed;

            //Apply Movement
            rigidbody.velocity = new Vector2(Input.GetAxis("Horizontal") * currentspeed, Input.GetAxis("Vertical") * currentspeed);

        }
        public static void Movement_FreeCamera(float speed, float shiftspeed, Transform cameraobj)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            float _CurrentSpeed;

            if (Input.GetKey(KeyCode.LeftShift))
                _CurrentSpeed = shiftspeed;
            else
                _CurrentSpeed = speed;

            float xas = Input.GetAxis("Horizontal");
            float zas = Input.GetAxis("Vertical");

            cameraobj.Translate(new Vector3(xas, 0, zas) * _CurrentSpeed * Time.deltaTime);

            float mousex = Input.GetAxis("Mouse X");
            float mousey = Input.GetAxis("Mouse Y");
            cameraobj.transform.eulerAngles += new Vector3(-mousey, mousex, 0);
        }


        
    }
}