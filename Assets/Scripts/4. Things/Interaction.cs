using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 손님과 상호작용하는 가구
public abstract class Interaction : MonoBehaviour
{
    public abstract Vector3 PutInCustomer(GameObject customer);
    public abstract void PopOutCustomer(GameObject customer);
}
