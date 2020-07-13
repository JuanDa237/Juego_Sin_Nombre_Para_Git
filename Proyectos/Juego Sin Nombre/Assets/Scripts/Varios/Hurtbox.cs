using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    public void añadirDañoAlPadre(int daño)
    {
        gameObject.SendMessageUpwards("añadirDaño", daño);
    }
}
