using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Assimp;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.WinUI;
using ModelViewer.Contracts.ViewModels;
using ModelViewer.Services;
using SharpDX;
using Windows.ApplicationModel;

namespace ModelViewer.ViewModels;

public partial class ModelViewModel : ObservableObject, INavigationAware
{
    public IEffectsManager EffectsManager
    {
        get;
    }

    public Camera Camera
    {
        get;
    } = new OrthographicCamera() { NearPlaneDistance = 1e-2, FarPlaneDistance = 1e4 };

    public SceneNodeGroupModel3D Root
    {
        get;
    } = new SceneNodeGroupModel3D();

    public Geometry3D Axis
    {
        private set;
        get;
    }
    public DiffuseMaterial Material
    {
        private set;
        get;
    }
    [ObservableProperty]
    private bool showAxis = true;

    [ObservableProperty]
    private Vector3 modelCentroid = default;
    [ObservableProperty]
    private bool showWireframe = false;
    [ObservableProperty]
    private BoundingBox boundingBox = default;

    private HelixToolkitScene scene = null;

    public List<BoundingBox> BoundingBoxList
    {
        get;
        set;
    } = new();

    public List<HelixToolkitScene> SceneList
    {
        get;
        set;
    } = new();

    public ModelViewModel(IEffectsManager effectsManager)
    {
        EffectsManager = effectsManager;
        Material = new DiffuseMaterial()
        {
            EnableUnLit = false,
            DiffuseMap = LoadTexture("紧闭眼.png")
        };

    }

    private TextureModel LoadTexture(string file)
    {
        var packageFolder = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
        return TextureModel.Create(packageFolder + @"\" + file);
    }

    [RelayCommand]
    private async Task<bool> LoadModel()
    {
        var path = await App.GetService<FilePickerService>().StartFilePicker(Importer.SupportedFormats);
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }
        //Root.Clear();
        scene = null;
        ResetSettings();
        var syncContext = SynchronizationContext.Current;
        return await Task.Factory.StartNew<HelixToolkitScene>(() =>
        {
            var importer = new Importer();
            var newScene = importer.Load(path);
            if (newScene != null)
            {
                /// Pre-attach and calculate all scene info in a separate task.
                newScene.Root.Attach(EffectsManager);
                newScene.Root.UpdateAllTransformMatrix();
                if (newScene.Root.TryGetBound(out var bound))
                {
                    /// Must use UI thread to set value back.
                    syncContext.Post((o) => { BoundingBox = bound; }, null);
                }
                if (newScene.Root.TryGetCentroid(out var centroid))
                {
                    /// Must use UI thread to set value back.
                    syncContext.Post((o) => { ModelCentroid = centroid; }, null);
                }
                foreach (var n in newScene.Root.Traverse())
                {
                    n.Tag = new AttachedNodeViewModel(n);
                }
                return newScene;
            }
            return null;
        }).ContinueWith((result) =>
        {
            scene = result.Result;
            if (result.IsCompleted && result.Result != null)
            {
                Root.AddNode(result.Result.Root);
                UpdateAxis();
                FocusCameraToScene();
            }
            return true;
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void ResetSettings()
    {
        ShowWireframe = false;
    }

    private void FocusCameraToScene()
    {
        var maxWidth = Math.Max(Math.Max(BoundingBox.Width, BoundingBox.Height), BoundingBox.Depth);
        var pos = BoundingBox.Center + new Vector3(0, 0, maxWidth);
        Camera.Position = pos;
        Camera.LookDirection = BoundingBox.Center - pos;
        Camera.UpDirection = Vector3.UnitY;
        if (Camera is OrthographicCamera orthCam)
        {
            orthCam.Width = maxWidth;
        }
    }

    partial void OnShowWireframeChanged(bool value)
    {
        if (scene != null && scene.Root != null)
        {
            foreach (var sceneItem in SceneList)
            {
                foreach (var node in sceneItem.Root.Traverse())
                {
                    if (node is MeshNode meshNode)
                    {
                        var list = BoundingBox.GetCorners();
                        var translationMatrix = Matrix.Translation(-list[1].X, -list[1].Y, -list[1].Z);
                        var tr2 = meshNode.ModelMatrix * translationMatrix;
                        var tr3 = tr2 * Matrix.RotationZ(MathUtil.DegreesToRadians(-30));
                        //var tr3 = tr2 * Matrix.RotationZ(30.0f * (float)Math.PI / 180.0f);
                        //var tr3 = tr2 * Matrix.RotationAxis(new Vector3(0, 0, 1), MathUtil.DegreesToRadians(30));
                        var tr4 = tr3 * Matrix.Translation(list[1].X, list[1].Y, list[1].Z);
                        meshNode.ModelMatrix = tr4;
                        meshNode.Material = Material;
                        meshNode.RenderWireframe = value;
                    }
                }
            }
            //foreach (var node in scene.Root.Traverse())
            //{
            //    if (node is MeshNode meshNode)
            //    {
            //        var list = BoundingBox.GetCorners();
            //        var translationMatrix = Matrix.Translation(-list[1].X, -list[1].Y, -list[1].Z);
            //        var tr2 = meshNode.ModelMatrix * translationMatrix;
            //        var tr3 = tr2 * Matrix.RotationZ(MathUtil.DegreesToRadians(-30));
            //        //var tr3 = tr2 * Matrix.RotationZ(30.0f * (float)Math.PI / 180.0f);
            //        //var tr3 = tr2 * Matrix.RotationAxis(new Vector3(0, 0, 1), MathUtil.DegreesToRadians(30));
            //        var tr4 = tr3 * Matrix.Translation(list[1].X, list[1].Y, list[1].Z);
            //        meshNode.ModelMatrix = tr4;
            //        meshNode.Material = Material;
            //        meshNode.RenderWireframe = value;
            //    }
            //}
        }

    }

    private void UpdateAxis()
    {
        float multiplier = 1.25f;
        var builder = new LineBuilder();
        builder.AddLine(ModelCentroid - new Vector3(BoundingBox.Width / 2 * multiplier, 0, 0), ModelCentroid + new Vector3(boundingBox.Width / 2 * multiplier, 0, 0));
        builder.AddLine(ModelCentroid - new Vector3(0, BoundingBox.Height / 2 * multiplier, 0), ModelCentroid + new Vector3(0, boundingBox.Height / 2 * multiplier, 0));
        builder.AddLine(ModelCentroid - new Vector3(0, 0, BoundingBox.Depth / 2 * multiplier), ModelCentroid + new Vector3(0, 0, boundingBox.Depth / 2 * multiplier));
        Axis = builder.ToLineGeometry3D();
        Axis.Colors = new Color4Collection();
        Axis.Colors.Resize(Axis.Positions.Count, true);
        Axis.Colors[0] = Axis.Colors[1] = Color.Red;
        Axis.Colors[2] = Axis.Colors[3] = Color.Green;
        Axis.Colors[4] = Axis.Colors[5] = Color.Blue;
        OnPropertyChanged(nameof(Axis));
    }

    public void OnNavigatedTo(object parameter)
    {

        var list = new List<string>()
        {
            "Base.obj",
            "Body1.obj",
            "Body2.obj",
            "Face.obj",
            "Head1.obj",
            "Head2.obj",
            "Head3.obj",
            "LeftArm1.obj",
            "LeftArm2.obj",
            "LeftShoulder.obj",
            "RightArm1.obj",
            "RightArm2.obj",
            "RightShoulder.obj"
        };

        foreach (var modelName in list)
        {
            var modelPath = Package.Current.InstalledLocation.Path + $"\\Assets\\ElectronBotModel\\{modelName}";

            var importer = new Importer();
            var newScene = importer.Load(modelPath);
            if (newScene != null)
            {
                /// Pre-attach and calculate all scene info in a separate task.
                newScene.Root.Attach(EffectsManager);
                newScene.Root.UpdateAllTransformMatrix();
                foreach (var n in newScene.Root.Traverse())
                {
                    n.Tag = new AttachedNodeViewModel(n);
                }

                Root.AddNode(newScene.Root);
                UpdateAxis();
                FocusCameraToScene();

                if (modelName == "Head3.obj")
                {
                    if (newScene != null && newScene.Root != null)
                    {
                        foreach (var node in newScene.Root.Traverse())
                        {
                            if (node is MeshNode meshNode)
                            {
                                meshNode.Material = Material;
                            }
                        }
                    }
                }

                if (modelName == "RightArm1.obj")
                {
                    if (newScene.Root.TryGetBound(out var bound))
                    {
                        /// Must use UI thread to set value back.
                        BoundingBox = bound;

                        BoundingBoxList.Add(bound);
                    }
                    if (newScene.Root.TryGetCentroid(out var centroid))
                    {
                        /// Must use UI thread to set value back.
                        ModelCentroid = centroid;
                    }
                    scene = newScene;

                    SceneList.Add(newScene);
                }

                if (modelName == "RightArm2.obj")
                {
                    if (newScene.Root.TryGetBound(out var bound))
                    {
                        /// Must use UI thread to set value back.
                        //BoundingBox = bound;

                        BoundingBoxList.Add(bound);
                    }
                    if (newScene.Root.TryGetCentroid(out var centroid))
                    {
                        /// Must use UI thread to set value back.
                        ModelCentroid = centroid;
                    }
                    //scene = newScene;

                    SceneList.Add(newScene);
                }
            }
        }


    }

    public void OnNavigatedFrom()
    {

    }
}

