#define NAVMESHCOMPONENTS_SHOW_NAVMESHDATA_REF

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine.AI;
using UnityEngine;

namespace UnityEditor.AI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(NavMeshSurface))]
    class NavMeshSurfaceEditor : Editor
    {
        SerializedProperty m_AgentTypeID;
        SerializedProperty m_BuildHeightMesh;
        SerializedProperty m_Center;
        SerializedProperty m_CollectObjects;
        SerializedProperty m_DefaultArea;
        SerializedProperty m_LayerMask;
        SerializedProperty m_OverrideTileSize;
        SerializedProperty m_OverrideVoxelSize;
        SerializedProperty m_Size;
        SerializedProperty m_TileSize;
        SerializedProperty m_UseGeometry;
        SerializedProperty m_VoxelSize;

#if NAVMESHCOMPONENTS_SHOW_NAVMESHDATA_REF
        SerializedProperty m_NavMeshData;
#endif
        class Styles
        {
            public readonly GUIContent m_LayerMask = new GUIContent("Include Layers");

            public readonly GUIContent m_ShowInputGeom = new GUIContent("Show Input Geom");
            public readonly GUIContent m_ShowVoxels = new GUIContent("Show Voxels");
            public readonly GUIContent m_ShowRegions = new GUIContent("Show Regions");
            public readonly GUIContent m_ShowRawContours = new GUIContent("Show Raw Contours");
            public readonly GUIContent m_ShowContours = new GUIContent("Show Contours");
            public readonly GUIContent m_ShowPolyMesh = new GUIContent("Show Poly Mesh");
            public readonly GUIContent m_ShowPolyMeshDetail = new GUIContent("Show Poly Mesh Detail");
        }

        static Styles s_Styles;

        static bool s_ShowDebugOptions;

        static Color s_HandleColor = new Color(127f, 214f, 244f, 100f) / 255;
        static Color s_HandleColorSelected = new Color(127f, 214f, 244f, 210f) / 255;
        static Color s_HandleColorDisabled = new Color(127f * 0.75f, 214f * 0.75f, 244f * 0.75f, 100f) / 255;

        BoxBoundsHandle m_BoundsHandle = new BoxBoundsHandle();

        bool editingCollider
        {
            get { return EditMode.editMode == EditMode.SceneViewEditMode.Collider && EditMode.IsOwner(this); }
        }

        void OnEnable()
        {
            m_AgentTypeID = serializedObject.FindProperty("m_AgentTypeID");
            m_BuildHeightMesh = serializedObject.FindProperty("m_BuildHeightMesh");
            m_Center = serializedObject.FindProperty("m_Center");
            m_CollectObjects = serializedObject.FindProperty("m_CollectObjects");
            m_DefaultArea = serializedObject.FindProperty("m_DefaultArea");
            m_LayerMask = serializedObject.FindProperty("m_LayerMask");
            m_OverrideTileSize = serializedObject.FindProperty("m_OverrideTileSize");
            m_OverrideVoxelSize = serializedObject.FindProperty("m_OverrideVoxelSize");
            m_Size = serializedObject.FindProperty("m_Size");
            m_TileSize = serializedObject.FindProperty("m_TileSize");
            m_UseGeometry = serializedObject.FindProperty("m_UseGeometry");
            m_VoxelSize = serializedObject.FindProperty("m_VoxelSize");

#if NAVMESHCOMPONENTS_SHOW_NAVMESHDATA_REF
            m_NavMeshData = serializedObject.FindProperty("m_NavMeshData");
#endif
            NavMeshVisualizationSettings.showNavigation++;
        }

        void OnDisable()
        {
            NavMeshVisualizationSettings.showNavigation--;
        }

        Bounds GetBounds()
        {
            var navSurface = (NavMeshSurface)target;
            return new Bounds(navSurface.transform.position, navSurface.size);
        }

        // LÍNEA RARA / COMPLEJA: Renderizado del Inspector GUI mediante Serialización y Barra de Progreso de Hilos Asíncronos ('OnInspectorGUI').
        // Actualiza el búfer virtual mediante 'serializedObject.Update()' e intercepta la configuración física del agente para proyectar 
        // un diagrama de previsualización 2D ('DrawAgentDiagram'). Ejecuta bloques de control condicional complejos acoplados a la indentación 
        // del editor ('EditorGUI.indentLevel++') para desvelar menús plegables ('EditorGUILayout.Foldout') de manipulación de vóxeles y teselas (tiles). 
        // Tras aplicar las propiedades modificadas, procesa un reporte inverso recorriendo las operaciones activas de la GPU en hilos asíncronos 
        // extraídas de 'NavMeshAssetManager.instance.GetBakeOperations()'. Si la operación está en progreso, dibuja una barra interactiva de cancelación 
        // y renderiza una barra de carga procedural ('EditorGUI.ProgressBar') inyectando el flotante de ratio realizado, forzando el repintado del frame.
        public override void OnInspectorGUI()
        {
            if (s_Styles == null)
                s_Styles = new Styles();

            serializedObject.Update();

            var bs = NavMesh.GetSettingsByID(m_AgentTypeID.intValue);

            if (bs.agentTypeID != -1)
            {
                // Draw image
                const float diagramHeight = 80.0f;
                Rect agentDiagramRect = EditorGUILayout.GetControlRect(false, diagramHeight);
                NavMeshEditorHelpers.DrawAgentDiagram(agentDiagramRect, bs.agentRadius, bs.agentHeight, bs.agentClimb, bs.agentSlope);
            }
            NavMeshComponentsGUIUtility.AgentTypePopup("Agent Type", m_AgentTypeID);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_CollectObjects);
            if ((CollectObjects)m_CollectObjects.enumValueIndex == CollectObjects.Volume)
            {
                EditorGUI.indentLevel++;

                EditMode.DoEditModeInspectorModeButton(EditMode.SceneViewEditMode.Collider, "Edit Volume",
                    EditorGUIUtility.IconContent("EditCollider"), GetBounds, this);
                EditorGUILayout.PropertyField(m_Size);
                EditorGUILayout.PropertyField(m_Center);

                EditorGUI.indentLevel--;
            }
            else
            {
                if (editingCollider)
                    EditMode.QuitEditMode();
            }

            EditorGUILayout.PropertyField(m_LayerMask, s_Styles.m_LayerMask);
            EditorGUILayout.PropertyField(m_UseGeometry);

            EditorGUILayout.Space();

            m_OverrideVoxelSize.isExpanded = EditorGUILayout.Foldout(m_OverrideVoxelSize.isExpanded, "Advanced");
            if (m_OverrideVoxelSize.isExpanded)
            {
                EditorGUI.indentLevel++;

                NavMeshComponentsGUIUtility.AreaPopup("Default Area", m_DefaultArea);

                // Override voxel size.
                EditorGUILayout.PropertyField(m_OverrideVoxelSize);

                using (new EditorGUI.DisabledScope(!m_OverrideVoxelSize.boolValue || m_OverrideVoxelSize.hasMultipleDifferentValues))
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.PropertyField(m_VoxelSize);

                    if (!m_OverrideVoxelSize.hasMultipleDifferentValues)
                    {
                        if (!m_AgentTypeID.hasMultipleDifferentValues)
                        {
                            float voxelsPerRadius = m_VoxelSize.floatValue > 0.0f ? (bs.agentRadius / m_VoxelSize.floatValue) : 0.0f;
                            EditorGUILayout.LabelField(" ", voxelsPerRadius.ToString("0.00") + " voxels per agent radius", EditorStyles.miniLabel);
                        }
                        if (m_OverrideVoxelSize.boolValue)
                            EditorGUILayout.HelpBox("Voxel size controls how accurately the navigation mesh is generated from the level geometry. A good voxel size is 2-4 voxels per agent radius. Making voxel size smaller will increase build time.", MessageType.None);
                    }
                    EditorGUI.indentLevel--;
                }

                // Override tile size
                EditorGUILayout.PropertyField(m_OverrideTileSize);

                using (new EditorGUI.DisabledScope(!m_OverrideTileSize.boolValue || m_OverrideTileSize.hasMultipleDifferentValues))
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.PropertyField(m_TileSize);

                    if (!m_TileSize.hasMultipleDifferentValues && !m_VoxelSize.hasMultipleDifferentValues)
                    {
                        float tileWorldSize = m_TileSize.intValue * m_VoxelSize.floatValue;
                        EditorGUILayout.LabelField(" ", tileWorldSize.ToString("0.00") + " world units", EditorStyles.miniLabel);
                    }

                    if (!m_OverrideTileSize.hasMultipleDifferentValues)
                    {
                        if (m_OverrideTileSize.boolValue)
                            EditorGUILayout.HelpBox("Tile size controls the how local the changes to the world are (rebuild or carve). Small tile size allows more local changes, while potentially generating more data overall.", MessageType.None);
                    }
                    EditorGUI.indentLevel--;
                }


                // Height mesh
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.PropertyField(m_BuildHeightMesh);
                }

                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();

            var hadError = false;
            var multipleTargets = targets.Length > 1;
            foreach (NavMeshSurface navSurface in targets)
            {
                var settings = navSurface.GetBuildSettings();
                // Calculating bounds is potentially expensive when unbounded - so here we just use the center/size.
                // It means the validation is not checking vertical voxel limit correctly when the surface is set to something else than "in volume".
                var bounds = new Bounds(Vector3.zero, Vector3.zero);
                if (navSurface.collectObjects == CollectObjects.Volume)
                {
                    bounds = new Bounds(navSurface.center, navSurface.size);
                }

                var errors = settings.ValidationReport(bounds);
                if (errors.Length > 0)
                {
                    if (multipleTargets)
                        EditorGUILayout.LabelField(navSurface.name);
                    foreach (var err in errors)
                    {
                        EditorGUILayout.HelpBox(err, MessageType.Warning);
                    }
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(EditorGUIUtility.labelWidth);
                    if (GUILayout.Button("Open Agent Settings...", EditorStyles.miniButton))
                        NavMeshEditorHelpers.OpenAgentSettings(navSurface.agentTypeID);
                    GUILayout.EndHorizontal();
                    hadError = true;
                }
            }

            if (hadError)
                EditorGUILayout.Space();

#if NAVMESHCOMPONENTS_SHOW_NAVMESHDATA_REF
            var nmdRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginProperty(nmdRect, GUIContent.none, m_NavMeshData);
            var rectLabel = EditorGUI.PrefixLabel(nmdRect, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(m_NavMeshData.displayName));
            EditorGUI.EndProperty();

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUI.BeginProperty(nmdRect, GUIContent.none, m_NavMeshData);
                EditorGUI.ObjectField(rectLabel, m_NavMeshData, GUIContent.none);
                EditorGUI.EndProperty();
            }
#endif
            using (new EditorGUI.DisabledScope(Application.isPlaying || m_AgentTypeID.intValue == -1))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUIUtility.labelWidth);
                if (GUILayout.Button("Clear"))
                {
                    NavMeshAssetManager.instance.ClearSurfaces(targets);
                    SceneView.RepaintAll();
                }

                if (GUILayout.Button("Bake"))
                {
                    NavMeshAssetManager.instance.StartBakingSurfaces(targets);
                }

                GUILayout.EndHorizontal();
            }

            // Show progress for the selected targets
            var bakeOperations = NavMeshAssetManager.instance.GetBakeOperations();
            for (int i = bakeOperations.Count - 1; i >= 0; --i)
            {
                if (!targets.Contains(bakeOperations[i].surface))
                    continue;

                var oper = bakeOperations[i].bakeOperation;
                if (oper == null)
                    continue;

                var p = oper.progress;
                if (oper.isDone)
                {
                    SceneView.RepaintAll();
                    continue;
                }

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Cancel", EditorStyles.miniButton))
                {
                    var bakeData = bakeOperations[i].bakeData;
                    UnityEngine.AI.NavMeshBuilder.Cancel(bakeData);
                    bakeOperations.RemoveAt(i);
                }

                EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(), p, "Baking: " + (int)(100 * p) + "%");
                if (p <= 1)
                    Repaint();

                GUILayout.EndHorizontal();
            }
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.Active | GizmoType.Pickable)]
        static void RenderBoxGizmoSelected(NavMeshSurface navSurface, GizmoType gizmoType)
        {
            RenderBoxGizmo(navSurface, gizmoType, true);
        }

        [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Pickable)]
        static void RenderBoxGizmoNotSelected(NavMeshSurface navSurface, GizmoType gizmoType)
        {
            if (NavMeshVisualizationSettings.showNavigation > 0)
                RenderBoxGizmo(navSurface, gizmoType, false);
            else
                Gizmos.DrawIcon(navSurface.transform.position, "NavMeshSurface Icon", true);
        }

        // LÍNEA RARA / COMPLEJA: Inyección de Matrices TRS no Escaladas para el Dibujo Tridimensional de Contenedores Volumétricos ('RenderBoxGizmo').
        // Coordina el renderizado de Gizmos en la vista de escena de Unity aplicando colores con opacidad alfa controlada basados en la selección. 
        // Almacena en memoria temporal el color y la matriz previa del motor. Acto seguido, calcula una matriz afín de transformación de coordenadas 
        // locales a mundiales ('Matrix4x4.TRS') utilizando la posición y rotación del componente, pero forzando un vector de escala unitaria 
        // stática ('Vector3.one'). Al sobreescribir 'Gizmos.matrix', desvincula el volumen de renderizado de la escala del transformador del GameObject, 
        // dibujando de forma matemática una caja alámbrica ('DrawWireCube') y un cubo sólido traslúcido perfectos que delimitan la zona de cálculo de IA.
        static void RenderBoxGizmo(NavMeshSurface navSurface, GizmoType gizmoType, bool selected)
        {
            var color = selected ? s_HandleColorSelected : s_HandleColor;
            if (!navSurface.enabled)
                color = s_HandleColorDisabled;

            var oldColor = Gizmos.color;
            var oldMatrix = Gizmos.matrix;

            // Use the unscaled matrix for the NavMeshSurface
            var localToWorld = Matrix4x4.TRS(navSurface.transform.position, navSurface.transform.rotation, Vector3.one);
            Gizmos.matrix = localToWorld;

            if (navSurface.collectObjects == CollectObjects.Volume)
            {
                Gizmos.color = color;
                Gizmos.DrawWireCube(navSurface.center, navSurface.size);

                if (selected && navSurface.enabled)
                {
                    var colorTrans = new Color(color.r * 0.75f, color.g * 0.75f, color.b * 0.75f, color.a * 0.15f);
                    Gizmos.color = colorTrans;
                    Gizmos.DrawCube(navSurface.center, navSurface.size);
                }
            }
            else
            {
                if (navSurface.navMeshData != null)
                {
                    var bounds = navSurface.navMeshData.sourceBounds;
                    Gizmos.color = Color.grey;
                    Gizmos.DrawWireCube(bounds.center, bounds.size);
                }
            }

            Gizmos.matrix = oldMatrix;
            Gizmos.color = oldColor;

            Gizmos.DrawIcon(navSurface.transform.position, "NavMeshSurface Icon", true);
        }

        // LÍNEA RARA / COMPLEJA: Manipulación del Ámbito de Manejadores de Escena e Intercepción de Cambios para Historial de Deshacer ('OnSceneGUI').
        // Activa la proyección del manipulador del cuadro tridimensional ('BoxBoundsHandle') en el espacio de la escena si el modo de edición de 
        // colisionadores está habilitado. Establece una directiva 'Handles.DrawingScope' inyectando la matriz TRS pura. Abre un bloque detector 
        // 'EditorGUI.BeginChangeCheck()' y procesa la proyección interactiva del objeto ('m_BoundsHandle.DrawHandle()'). Si el diseñador arrastra 
        // las aristas en la vista de escena, captura el delta físico, inyecta la firma en el búfer de operaciones reversibles del editor mediante 
        // 'Undo.RecordObject' (habilitando el Ctrl+Z), sobrescribe los vectores 'center' y 'size' y marca el archivo de escena como sucio ('SetDirty').
        void OnSceneGUI()
        {
            if (!editingCollider)
                return;

            var navSurface = (NavMeshSurface)target;
            var color = navSurface.enabled ? s_HandleColor : s_HandleColorDisabled;
            var localToWorld = Matrix4x4.TRS(navSurface.transform.position, navSurface.transform.rotation, Vector3.one);
            using (new Handles.DrawingScope(color, localToWorld))
            {
                m_BoundsHandle.center = navSurface.center;
                m_BoundsHandle.size = navSurface.size;

                EditorGUI.BeginChangeCheck();
                m_BoundsHandle.DrawHandle();
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(navSurface, "Modified NavMesh Surface");
                    Vector3 center = m_BoundsHandle.center;
                    Vector3 size = m_BoundsHandle.size;
                    navSurface.center = center;
                    navSurface.size = size;
                    EditorUtility.SetDirty(target);
                }
            }
        }

        [MenuItem("GameObject/AI/NavMesh Surface", false, 2000)]
        public static void CreateNavMeshSurface(MenuCommand menuCommand)
        {
            var parent = menuCommand.context as GameObject;
            var go = NavMeshComponentsGUIUtility.CreateAndSelectGameObject("NavMesh Surface", parent);
            go.AddComponent<NavMeshSurface>();
            var view = SceneView.lastActiveSceneView;
            if (view != null)
                view.MoveToView(go.transform);
        }
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el **Inspector Personalizado y Módulo de Extensión del Editor para Superficies de Navegación** (`NavMeshSurfaceEditor`). Pertenece de forma exclusiva al ecosistema de desarrollo de Unity (`UnityEditor`). Su responsabilidad 
   arquitectónica es rediseñar por completo la interfaz gráfica del componente `NavMeshSurface` dentro del Inspector de Unity, 
   proporcionando herramientas visuales avanzadas para configurar, validar, depurar y precalcular (bake) mallas de navegación para IA.

   Características arquitectónicas clave:
   1. Interfaz Gráfica Avanzada Basada en Datos de Agentes (Custom Inspector Layout): Utiliza las APIs de maquetación de Unity 
      (`EditorGUILayout`) para dibujar diagramas interactivos que representan las dimensiones físicas del agente de navegación actual 
      (radio, pendiente máxima de escalada, altura). Adapta dinámicamente las secciones de configuración basándose en si la recopilación 
      de objetos se realiza por volumen geométrico o de forma global.
   2. Manipulación Espacial Directa en la Escena (SceneView Handles): Sobrescribe `OnSceneGUI` implementando un manejador volumétrico 
      `BoxBoundsHandle`. Esto permite a los diseñadores de niveles arrastrar y redimensionar interactivamente las fronteras de cálculo 
      de la IA de forma tridimensional directamente en la Scene View de Unity, integrándose limpiamente con el búfer de deshacer (`Undo`).
   3. Monitoreo Asíncrono del Proceso de Horneado (Bake Process Pooling): El inspector interactúa directamente con hilos de cómputo en 
      segundo plano controlados por el `NavMeshAssetManager`. Captura la progresión fraccionada del cálculo geométrico de la malla de 
      navegación y proyecta de forma procedural barras de carga nativas, permitiendo cancelar asertivamente los subprocesos de la CPU 
      sin congelar la interfaz general del motor de videojuegos.
   ========================================================================================================
*/