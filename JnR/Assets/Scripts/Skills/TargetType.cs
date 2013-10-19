using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum TargetType
{
    None,
    Myself,             // If it is a spell, which can be cast only on yourself, you don't have to choose a target
    Ally,
    Enemy
};