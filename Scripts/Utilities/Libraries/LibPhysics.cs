using UnityEngine;

namespace Armony.Utilities.Libraries
{
    namespace Armony.Utilities.Libraries
    {
        public static class LibPhysics
        {
            public static Vector3 GetGroundPoint(this Vector3 _startPosition, int _layerMask = Physics.DefaultRaycastLayers) =>
                Physics.Raycast(_startPosition, Vector3.down, out RaycastHit hit, Mathf.Infinity, _layerMask, QueryTriggerInteraction.Ignore) ? hit.point : _startPosition;
        }
    }
}