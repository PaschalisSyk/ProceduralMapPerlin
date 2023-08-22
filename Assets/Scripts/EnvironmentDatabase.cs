using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnvironmentDatabase", menuName = "Custom/Environment Database")]
public class EnvironmentDatabase : ScriptableObject
{
    public EnvironmentProfile[] profiles;
}
