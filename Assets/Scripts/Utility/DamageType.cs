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

// 1 & 2 == 
// 1 << 0 = '0000 0001'
// 1 << 1 = '0000 0010'
// 1 << 2 = '0000 0100'

// Swo BASIC + FIRE = '0000 0011'
// Bow BASIC + ICE = '0000 0101'
// Ham BASIC = '0000 0001'

// Res BASIC = '0000 0001'

// RES & SWO = '0000 0001'
// RES | SWO = '0000 0011'
// RES ^ SWO = '0000 0010 & SWO = '0000 0010'
// (RES ^ SWO) & SWO
// 