using System;
using UnityEngine;

public class CreatureClass : MonoBehaviour {
    public enum CreatureClassType {
        Hellhound,
        Imp
    }
    public enum CreatureSubClassType {
        Melee
    }

    [SerializeField] private CreatureClassType _classType;
    public CreatureClassType ClassType {
        get { return _classType; }
        set { _classType = value; }
    }

    [SerializeField] private CreatureSubClassType _subClassType;
    public CreatureSubClassType SubClassType {
        get { return _subClassType; }
        set { _subClassType = value; }
    }

    public void UpdateClass(ClassTypeData classType) {
        _classType = (CreatureClassType)classType.ClassType;
        _subClassType = (CreatureSubClassType)classType.SubClassType;
    }
}

