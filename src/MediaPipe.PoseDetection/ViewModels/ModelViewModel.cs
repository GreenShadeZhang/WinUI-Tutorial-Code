using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Assimp;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.WinUI;
using Mediapipe.Net.Solutions;
using Microsoft.UI.Xaml;
using SharpDX;
using Windows.ApplicationModel;
using MediaPipe.PoseDetection.Extensions;

namespace MediaPipe.PoseDetection;

public partial class ModelViewModel : ObservableObject
{
    public IEffectsManager EffectsManager
    {
        get;
    } = new DefaultEffectsManager();

    public Camera Camera
    {
        get;
    } = new OrthographicCamera() { NearPlaneDistance = 1e-2, FarPlaneDistance = 1e4 };

    public SceneNodeGroupModel3D BodyModel
    {
        get;
    } = new SceneNodeGroupModel3D();

    public SceneNodeGroupModel3D LeftArmModel
    {
        get;
    } = new SceneNodeGroupModel3D();

    public SceneNodeGroupModel3D RightArmModel
    {
        get;
    } = new SceneNodeGroupModel3D();

    public SceneNodeGroupModel3D HeadModel
    {
        get;
    } = new SceneNodeGroupModel3D();

    public SceneNodeGroupModel3D BaseModel
    {
        get;
    } = new SceneNodeGroupModel3D();

    public Geometry3D Axis
    {
        private set;
        get;
    }

    public Geometry3D PointGeometry
    {
        private set; get;
    }

    public PhongMaterial FloorMaterial
    {
        private set; get;
    }

    public Geometry3D Sphere
    {
        private set; get;
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


    private Vector3 _average;

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

    public ModelViewModel()
    {
        Material = new DiffuseMaterial()
        {
            EnableUnLit = false,
            DiffuseMap = LoadTexture("紧闭眼.png")
        };

    }

    public void InitAsync(PoseOutput poseOutput, float x, float y, float z)
    {
        FocusCameraToScene();
        UpdateAxis(poseOutput, x, y, z);
    }


    private TextureModel LoadTexture(string file)
    {
        var packageFolder = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
        return TextureModel.Create(packageFolder + @"\" + file);
    }


    private void ResetSettings()
    {
        ShowWireframe = false;
    }

    private void FocusCameraToScene()
    {
        var maxWidth = Math.Max(Math.Max(BoundingBox.Width, BoundingBox.Height), BoundingBox.Depth) + 280;
        var pos = BoundingBox.Center + new Vector3(0, 0, maxWidth);
        Camera.Position = pos;
        Camera.LookDirection = BoundingBox.Center - pos;
        Camera.UpDirection = Vector3.UnitY;
        if (Camera is OrthographicCamera orthCam)
        {
            orthCam.Width = maxWidth;
        }
    }

    private void UpdateAxis(PoseOutput poseOutput, float x, float y, float z)
    {
        if (poseOutput != null)
        {
            var postLine3Ds = poseOutput.GetPose3DLines(x, y, z);

            var builder = new LineBuilder();

            foreach (var postLine3D in postLine3Ds)
            {
                builder.AddLine(new Vector3(postLine3D.StartVector3.X, -postLine3D.StartVector3.Y, -postLine3D.StartVector3.Z), new Vector3(postLine3D.EndVector3.X, -postLine3D.EndVector3.Y, -postLine3D.EndVector3.Z));
            }

            float multiplier = 1.25f;

            Axis = builder.ToLineGeometry3D();
            //Axis.Colors = new Color4Collection();
            //Axis.Colors.Resize(Axis.Positions.Count, true);
            //Axis.Colors[0] = Axis.Colors[1] = Color.Red;
            //Axis.Colors[2] = Axis.Colors[3] = Color.Green;
            //Axis.Colors[4] = Axis.Colors[5] = Color.Blue;
            OnPropertyChanged(nameof(Axis));

            var builder1 = new MeshBuilder();

            if (poseOutput.PoseWorldLandmarks != null)
            {
                foreach (var landmark in poseOutput.PoseWorldLandmarks.Landmark)
                {
                    builder1.AddSphere(new Vector3(landmark.X * x, -landmark.Y * y, -landmark.Z * z), 3);
                }
            }

            var mesh = builder1.ToMesh();

            mesh.UpdateOctree();

            PointGeometry = new PointGeometry3D() { Positions = mesh.Positions };
            OnPropertyChanged(nameof(PointGeometry));

            var builder2 = new MeshBuilder(true, true, true);
            builder2.AddSphere(new Vector3(0, 0, 0), 5);
            Sphere = builder2.ToMesh();
            Sphere.UpdateOctree();
            OnPropertyChanged(nameof(Sphere));
        }
    }
}

