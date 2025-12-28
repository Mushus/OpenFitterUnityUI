using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using OpenFitter.Editor;

namespace OpenFitter.Editor.Services
{
    /// <summary>
    /// Holds and manages the application's runtime state and serves as the Single Source of Truth.
    /// Manages synchronization between memory and EditorPrefs persistence.
    /// </summary>
    public sealed class OpenFitterState : IDisposable
    {
        private const string PrefKeyPrefix = "OpenFitterWindow.";
        private const string WizardStepKey = "OpenFitter.WizardStep";
        private const string LastOutputPathKey = "LastOutputPath";

        // Backing Fields for Persistence
        private string _inputFbx = "";
        private string _sourceConfigPath = "";
        private string _targetConfigPath = "";
        private string _blendShapes = "";
        private string _blendShapeValues = "";
        private List<string> _blendShapeMappings = new();
        private string _clothMetadata = "";
        private string _meshMaterialData = "";
        private List<string> _targetMeshes = new();
        private List<string> _meshRenderers = new();
        private List<string> _nameConv = new();
        private bool _preserveBoneNames;
        private bool _subdivide;
        private bool _triangulate;

        // Runtime State
        private WizardStep _wizardStep = WizardStep.EnvironmentSetup;
        private string _lastOutputPath = "";
        public List<BlendShapeEntry> BlendShapeEntries { get; private set; } = new List<BlendShapeEntry>();

        // Events
        public event Action? OnInputFbxChanged;
        public event Action? OnSourceConfigPathChanged;
        public event Action? OnTargetConfigPathChanged;
        public event Action? OnBlendShapesChanged;
        public event Action? OnBlendShapeValuesChanged;
        public event Action? OnBlendShapeMappingsChanged;
        public event Action? OnClothMetadataChanged;
        public event Action? OnMeshMaterialDataChanged;
        public event Action? OnTargetMeshesChanged;
        public event Action? OnMeshRenderersChanged;
        public event Action? OnNameConvChanged;
        public event Action? OnPreserveBoneNamesChanged;
        public event Action? OnSubdivideChanged;
        public event Action? OnTriangulateChanged;
        public event Action? OnWizardStepChanged;
        public event Action? OnLastOutputPathChanged;

        public OpenFitterState()
        {
            Load();
        }

        public void Dispose()
        {
        }

        private List<string> LoadList(string keyName)
        {
            return EditorPrefs.GetString(PrefKeyPrefix + keyName, string.Empty)
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private void SaveList(string keyName, List<string> list)
        {
            EditorPrefs.SetString(PrefKeyPrefix + keyName, list != null ? string.Join(";", list) : string.Empty);
        }

        /// <summary>
        /// Loads all preferences from EditorPrefs into properties.
        /// </summary>
        private void Load()
        {
            _inputFbx = EditorPrefs.GetString(PrefKeyPrefix + "inputFbx", "");
            // _baseFbxList = LoadList("baseFbxList"); // Removed
            // _configList = LoadList("configList"); // Removed
            // _initPose = EditorPrefs.GetString(PrefKeyPrefix + "initPose", ""); // Removed
            _sourceConfigPath = EditorPrefs.GetString(PrefKeyPrefix + "sourceConfigPath", "");
            _targetConfigPath = EditorPrefs.GetString(PrefKeyPrefix + "targetConfigPath", "");
            // _hipsPosition = EditorPrefs.GetString(PrefKeyPrefix + "hipsPosition", ""); // Removed
            _blendShapes = EditorPrefs.GetString(PrefKeyPrefix + "blendShapes", "");
            _blendShapeValues = EditorPrefs.GetString(PrefKeyPrefix + "blendShapeValues", "");
            _blendShapeMappings = LoadList("blendShapeMappings");
            _clothMetadata = EditorPrefs.GetString(PrefKeyPrefix + "clothMetadata", "");
            _meshMaterialData = EditorPrefs.GetString(PrefKeyPrefix + "meshMaterialData", "");
            _targetMeshes = LoadList("targetMeshes");
            _meshRenderers = LoadList("meshRenderers");
            _nameConv = LoadList("nameConv");
            _preserveBoneNames = EditorPrefs.GetInt(PrefKeyPrefix + "preserveBoneNames", 0) == 1;
            _subdivide = EditorPrefs.GetInt(PrefKeyPrefix + "subdivide", 0) == 1;
            _triangulate = EditorPrefs.GetInt(PrefKeyPrefix + "triangulate", 0) == 1;

            _wizardStep = (WizardStep)EditorPrefs.GetInt(WizardStepKey, 0);
            _lastOutputPath = EditorPrefs.GetString(PrefKeyPrefix + LastOutputPathKey, "");
        }

        public void ClearPrefs()
        {
            InputFbx = "";
            SourceConfigPath = "";
            TargetConfigPath = "";
            BlendShapes = "";
            BlendShapeValues = "";
            BlendShapeMappings = new List<string>();
            ClothMetadata = "";
            MeshMaterialData = "";
            TargetMeshes = new List<string>();
            MeshRenderers = new List<string>();
            NameConv = new List<string>();
            PreserveBoneNames = false;
            Subdivide = false;
            Triangulate = false;

            WizardStep = WizardStep.EnvironmentSetup;
            LastOutputPath = "";
        }

        public void ResetEnvironmentState()
        {
            ClearPrefs();
        }

        public string InputFbx
        {
            get => _inputFbx;
            set
            {
                if (_inputFbx != value)
                {
                    _inputFbx = value;
                    EditorPrefs.SetString(PrefKeyPrefix + "inputFbx", value);
                    OnInputFbxChanged?.Invoke();
                }
            }
        }

        public GameObject? InputFbxObject
        {
            get => string.IsNullOrEmpty(InputFbx) ? null : AssetDatabase.LoadAssetAtPath<GameObject>(InputFbx);
            set
            {
                string path = string.Empty;
                if (value != null)
                {
                    path = OpenFitterPathUtility.ResolveFbxPath(value);
                }
                InputFbx = path ?? string.Empty; // Set the string property, which triggers events/save
            }
        }


        public string SourceConfigPath
        {
            get => _sourceConfigPath;
            set
            {
                if (_sourceConfigPath != value)
                {
                    _sourceConfigPath = value;
                    EditorPrefs.SetString(PrefKeyPrefix + "sourceConfigPath", value);
                    OnSourceConfigPathChanged?.Invoke();
                }
            }
        }

        public string TargetConfigPath
        {
            get => _targetConfigPath;
            set
            {
                if (_targetConfigPath != value)
                {
                    _targetConfigPath = value;
                    EditorPrefs.SetString(PrefKeyPrefix + "targetConfigPath", value);
                    OnTargetConfigPathChanged?.Invoke();
                }
            }
        }

        public string BlendShapes
        {
            get => _blendShapes;
            set
            {
                if (_blendShapes != value)
                {
                    _blendShapes = value;
                    EditorPrefs.SetString(PrefKeyPrefix + "blendShapes", value);
                    OnBlendShapesChanged?.Invoke();
                }
            }
        }

        public string BlendShapeValues
        {
            get => _blendShapeValues;
            set
            {
                if (_blendShapeValues != value)
                {
                    _blendShapeValues = value;
                    EditorPrefs.SetString(PrefKeyPrefix + "blendShapeValues", value);
                    OnBlendShapeValuesChanged?.Invoke();
                }
            }
        }

        public List<string> BlendShapeMappings
        {
            get => _blendShapeMappings;
            set
            {
                if (_blendShapeMappings != value)
                {
                    _blendShapeMappings = value;
                    SaveList("blendShapeMappings", value);
                    OnBlendShapeMappingsChanged?.Invoke();
                }
            }
        }

        public string ClothMetadata
        {
            get => _clothMetadata;
            set
            {
                if (_clothMetadata != value)
                {
                    _clothMetadata = value;
                    EditorPrefs.SetString(PrefKeyPrefix + "clothMetadata", value);
                    OnClothMetadataChanged?.Invoke();
                }
            }
        }

        public string MeshMaterialData
        {
            get => _meshMaterialData;
            set
            {
                if (_meshMaterialData != value)
                {
                    _meshMaterialData = value;
                    EditorPrefs.SetString(PrefKeyPrefix + "meshMaterialData", value);
                    OnMeshMaterialDataChanged?.Invoke();
                }
            }
        }

        public List<string> TargetMeshes
        {
            get => _targetMeshes;
            set
            {
                if (_targetMeshes != value)
                {
                    _targetMeshes = value;
                    SaveList("targetMeshes", value);
                    OnTargetMeshesChanged?.Invoke();
                }
            }
        }

        public List<string> MeshRenderers
        {
            get => _meshRenderers;
            set
            {
                if (_meshRenderers != value)
                {
                    _meshRenderers = value;
                    SaveList("meshRenderers", value);
                    OnMeshRenderersChanged?.Invoke();
                }
            }
        }

        public List<string> NameConv
        {
            get => _nameConv;
            set
            {
                if (_nameConv != value)
                {
                    _nameConv = value;
                    SaveList("nameConv", value);
                    OnNameConvChanged?.Invoke();
                }
            }
        }

        public bool PreserveBoneNames
        {
            get => _preserveBoneNames;
            set
            {
                if (_preserveBoneNames != value)
                {
                    _preserveBoneNames = value;
                    EditorPrefs.SetInt(PrefKeyPrefix + "preserveBoneNames", value ? 1 : 0);
                    OnPreserveBoneNamesChanged?.Invoke();
                }
            }
        }

        public bool Subdivide
        {
            get => _subdivide;
            set
            {
                if (_subdivide != value)
                {
                    _subdivide = value;
                    EditorPrefs.SetInt(PrefKeyPrefix + "subdivide", value ? 1 : 0);
                    OnSubdivideChanged?.Invoke();
                }
            }
        }

        public bool Triangulate
        {
            get => _triangulate;
            set
            {
                if (_triangulate != value)
                {
                    _triangulate = value;
                    EditorPrefs.SetInt(PrefKeyPrefix + "triangulate", value ? 1 : 0);
                    OnTriangulateChanged?.Invoke();
                }
            }
        }

        public WizardStep WizardStep
        {
            get => _wizardStep;
            set
            {
                if (_wizardStep != value)
                {
                    _wizardStep = value;
                    EditorPrefs.SetInt(WizardStepKey, (int)value);
                    OnWizardStepChanged?.Invoke();
                }
            }
        }

        public string LastOutputPath
        {
            get => _lastOutputPath;
            set
            {
                if (_lastOutputPath != value)
                {
                    _lastOutputPath = value;
                    EditorPrefs.SetString(PrefKeyPrefix + LastOutputPathKey, value);
                    OnLastOutputPathChanged?.Invoke();
                }
            }
        }

        public void SetBlendShapeEntries(List<BlendShapeEntry> entries)
        {
            BlendShapeEntries = entries ?? new List<BlendShapeEntry>();
        }
    }
}
