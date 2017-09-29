using UnityEngine;
using System.Collections;
using System;

public class Checkpoint : MonoBehaviour, IComparable {

    public int checkpointNr;
    public bool isFinish;

    //for being able to sort the checkpoints by their nr
    public int CompareTo(System.Object o)
    {
        if (o is Checkpoint)
        {
            return this.checkpointNr.CompareTo((o as Checkpoint).checkpointNr);
        }
        return 0;
    }
}
