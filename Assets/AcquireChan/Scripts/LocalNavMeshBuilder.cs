using System.Collections.Generic;
using System.Linq;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;

// Build and update a localized navmesh from the sources marked by NavMeshSourceTag
namespace AcquireChan.Scripts {
    using SpatialAwarenessHandler = IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessMeshObject>;

    public class LocalNavMeshBuilder : MonoBehaviour, SpatialAwarenessHandler {
        // The center of the build
        public Transform m_Tracked;

        // The size of the build bounds
        public Vector3 m_Size = new Vector3(80.0f, 20.0f, 80.0f);

        NavMeshData m_NavMesh;
        NavMeshDataInstance m_Instance;
        private Dictionary<int, NavMeshBuildSource> sources = new Dictionary<int, NavMeshBuildSource>();

        private void Start() {
            UpdateNavMesh(true);
            RegisterEventHandlers(this);
        }

        private void OnEnable() {
            // Construct and add navmesh
            m_NavMesh = new NavMeshData();
            m_Instance = NavMesh.AddNavMeshData(m_NavMesh);
            if (m_Tracked == null)
                m_Tracked = transform;
            UpdateNavMesh();
            RegisterEventHandlers(this);
        }

        #region register_event

        private bool isRegistered;

        private void RegisterEventHandlers(IEventSystemHandler handler) {
            if (isRegistered || CoreServices.SpatialAwarenessSystem == null) return;
            CoreServices.SpatialAwarenessSystem.RegisterHandler<SpatialAwarenessHandler>(handler);
            isRegistered = true;
            Debug.Log("RegisterEventHandlers: ");
        }

        private void UnregisterEventHandlers(IEventSystemHandler handler) {
            if (!isRegistered || CoreServices.SpatialAwarenessSystem == null) return;
            CoreServices.SpatialAwarenessSystem.UnregisterHandler<SpatialAwarenessHandler>(handler);
            isRegistered = false;
            Debug.Log("UnregisterEventHandlers: ");
        }

        #endregion

        private void OnDisable() {
            m_Instance.Remove();
            UnregisterEventHandlers(this);
        }

        private void OnDestroy() {
            UnregisterEventHandlers(this);
        }

        private void UpdateNavMesh(bool asyncUpdate = false) {
            var m_Sources = sources.Values.ToList();
            var defaultBuildSettings = NavMesh.GetSettingsByID(0);
            var bounds = QuantizedBounds();

            if (asyncUpdate)
                NavMeshBuilder.UpdateNavMeshDataAsync(m_NavMesh, defaultBuildSettings, m_Sources, bounds);
            else
                NavMeshBuilder.UpdateNavMeshData(m_NavMesh, defaultBuildSettings, m_Sources, bounds);
        }

        private static Vector3 Quantize(Vector3 v, Vector3 quant) {
            var x = quant.x * Mathf.Floor(v.x / quant.x);
            var y = quant.y * Mathf.Floor(v.y / quant.y);
            var z = quant.z * Mathf.Floor(v.z / quant.z);
            return new Vector3(x, y, z);
        }

        private Bounds QuantizedBounds() {
            // Quantize the bounds to update only when theres a 10% change in size
            var center = m_Tracked ? m_Tracked.position : transform.position;
            return new Bounds(Quantize(center, 0.1f * m_Size), m_Size);
        }

        public void OnObservationAdded(MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject> eventData) {
            var result = MeshFilter2NavMeshBuildSource(eventData.SpatialObject.Filter);
            var id = eventData.Id;
            sources.Add(id, result);
            UpdateNavMesh();
        }

        public void OnObservationUpdated(
            MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject> eventData) {
            var result = MeshFilter2NavMeshBuildSource(eventData.SpatialObject.Filter);
            var id = eventData.Id;
            sources.Add(id, result);
            UpdateNavMesh();
        }

        public void OnObservationRemoved(
            MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject> eventData) {
            var id = eventData.Id;
            sources.Remove(id);
            UpdateNavMesh();
        }

        private static NavMeshBuildSource MeshFilter2NavMeshBuildSource(MeshFilter meshFilter) {
            var mesh = meshFilter.mesh;
            var navMeshBuildSource = new NavMeshBuildSource {
                shape = NavMeshBuildSourceShape.Mesh,
                sourceObject = mesh,
                transform = meshFilter.transform.localToWorldMatrix,
                area = 0
            };
            return navMeshBuildSource;
        }
        
    }
}
