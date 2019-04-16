using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum DamageType {
    NONE = (1 << 0),
    BASIC = (1 << 1), // Pierce ?
    FIRE = (1 << 2),
    ICE = (1 << 3),
    LIGHTNING = (1 << 4),
    EXPLOSIVE = (1 << 5),
    TRUE = (1 << 6),
}
