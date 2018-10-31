using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWalker {

    LayerMask GetObstacleMask();
    Vector3 GetVelocity();
    void SetDestination(Vector3 destination, float stoppingDistance = 0f);
    void Pause();
    void Stop();
    void Resume();
}
