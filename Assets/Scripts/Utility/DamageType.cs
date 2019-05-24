using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum DamageType {
    BASIC = (1 << 0), // Pierce ?
    FIRE = (1 << 1),
    ICE = (1 << 2),
    LIGHTNING = (1 << 3),
    EARTH = (1 << 4),
    EXPLOSIVE = (1 << 5),
    TRUE = (1 << 6),
}
