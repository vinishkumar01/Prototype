using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITriggerCheckable 
{
    bool isAggroed { get; set; }

    bool IsWithinStrikingDistance { get; set; }

    void SetAggroedStatus(bool aggroed);

    void SetStrikingDistance(bool isWithinStrikingDistance);
}
