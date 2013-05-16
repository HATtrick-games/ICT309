using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Graphics;
using DigitalRune.Geometry;
using DigitalRune.Animation.Character;
using DigitalRune.Graphics.SceneGraph;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using System.Collections.Generic;
using DigitalRune.Animation;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Practices.ServiceLocation;


namespace ICT309Game.Graphics
{
    class BasicScreen : DelegateGraphicsScreen
    {
        
        Dictionary<int,ICT309Game.Container.ModelContainer> Map = new Dictionary<int,ICT309Game.Container.ModelContainer>();
        private int NumModels;
        private List<Model> Models = new List<Model>();
        private List<Pose> Poses = new List<Pose>();
        private List<SkeletonPose> SkeletonPoses = new List<SkeletonPose>();
        private AnimationController _animationController;
        private List<int> DeadCharacters = new List<int>();
        

        public Scene Scene { get; set; }
        public CameraNode ActiveCamera { get; set; }

        private MeshRenderer _renderer;

        public BasicScreen(IGraphicsService graphics)
            : base(graphics)
        {
            var game = ServiceLocator.Current.GetInstance<Game>();
            
            
            Map = new Dictionary<int,ICT309Game.Container.ModelContainer>();
           // NumModels = 0;
            //Models = new List<Model>();
          // Poses = new List<Pose>();
          // SkeletonPoses = new List<SkeletonPose>();

            Scene = new Scene();
            ActiveCamera = null;
            _renderer = new MeshRenderer();
        }

        

        public void Rotation(int id, int rotate)
        {
            Map[id]._pose.Orientation = Matrix33F.CreateRotationY(MathHelper.ToRadians(rotate));
        }

        public void SetPos(int ID, Pose passed)
        {
            //Pose passpose = new Pose(new Vector3F(xposition, yposition, 0));
            //Poses.ElementAt<Pose>(i) = new Vector3F(0.5f, 10.0f, 0.0f);
            //Poses.ElementAt = new Vector3F(0.5f, 10.0f, 0.0f);
            // Console.WriteLine("PATH WAS FOUND");
            Map[ID]._pose = passed;
        }

        public void SetPos(int ID, Vector3F passed)
        {
            //Pose passpose = new Pose(new Vector3F(xposition, yposition, 0));
            //Poses.ElementAt<Pose>(i) = new Vector3F(0.5f, 10.0f, 0.0f);
            //Poses.ElementAt = new Vector3F(0.5f, 10.0f, 0.0f);
             // Console.WriteLine("PATH WAS FOUND");
            Map[ID]._pose.Position =  passed;
        }

        public void idle(int ID)
        {
            var AnimateService = ServiceLocator.Current.GetInstance<IAnimationService>();
            _animationController = AnimateService.StartAnimation(Map[ID]._animations[2], (IAnimatableProperty)Map[ID]._skeleton);
            _animationController.UpdateAndApply();
            _animationController.AutoRecycle();
            _animationController.Stop();

        }

        public void Attack(int ID)
        {
            var AnimateService = ServiceLocator.Current.GetInstance<IAnimationService>();
            _animationController = AnimateService.StartAnimation(Map[ID]._animations[1], (IAnimatableProperty)Map[ID]._skeleton);
            _animationController.UpdateAndApply();
            _animationController.AutoRecycle();
            
        }

        public void Walk(int ID)
        {
           // Map[0].animate();
            var AnimateService = ServiceLocator.Current.GetInstance<IAnimationService>();
          // AnimateService.StartAnimation(Map[ID]._animations[0], (IAnimatableProperty)Map[ID]._skeleton).AutoRecycle();
            _animationController = AnimateService.StartAnimation(Map[ID]._animations[0], (IAnimatableProperty)Map[ID]._skeleton);
            _animationController.Speed += 5;
           //ServiceLocator.Current.GetInstance<IAnimationService>().StartAnimation(Map[ID].loopingAnimation, (IAnimatableProperty)Map[ID]._skeleton).AutoRecycle();
           _animationController.UpdateAndApply();
           _animationController.AutoRecycle();
           
        }

        public void PauseWalk(int ID)
        {
            _animationController.Stop();
        }
        public int Add(Model _model,Pose _pose,SkeletonPose _skel )
        {
            Map.Add(NumModels, new ICT309Game.Container.ModelContainer(_model, _pose, _skel));
            Models.Add(_model);
            Poses.Add(_pose);
            SkeletonPoses.Add(_skel);
            NumModels++;
           // Map[0]._model = _model;
           // Walk(NumModels-1);
            idle(NumModels - 1);
            return NumModels - 1;
                       
        }

        public void Death(int ID)
        {
            DeadCharacters.Add(ID);
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            Scene.Update(deltaTime);

            base.OnUpdate(deltaTime);
        }

        protected override void OnRender(RenderContext context)
        {
            if (ActiveCamera == null)
                return;

            context.CameraNode = ActiveCamera;
            context.Scene = Scene;

            var graphicsDevice = context.GraphicsService.GraphicsDevice;

            graphicsDevice.Clear(Color.CornflowerBlue);

            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            int i = 0;
           // Map[0]._pose = new Pose(new Vector3F(10.5f, 0.0f, 0.0f));
            for (int z = 0; z < NumModels; z++ )
            {
                if (DeadCharacters.Contains(z))
                {
                    continue;
                }
                

                foreach (ModelMesh mesh in Map[z]._model.Meshes)
                {
                    foreach (SkinnedEffect effect in mesh.Effects)
                    {
                        // SkeletonPose.SkinningMatricesXna provides an array of transformations as needed
                        // by the SkinnedEffect.
                        effect.SetBoneTransforms(Map[z]._skeleton.SkinningMatricesXna);

                        // The world space transformation.
                        effect.World = Map[z]._pose;

                        // Camera transformation.
                        effect.View = ActiveCamera.View.ToXna();
                        effect.Projection = ActiveCamera.Camera.Projection.ToXna();

                        // Lighting.
                        effect.EnableDefaultLighting();
                        effect.SpecularColor = new Vector3(0.25f);
                        effect.SpecularPower = 16;
                    }

                    mesh.Draw();
                }
               
                i++;
            }
            var query = Scene.Query<CameraFrustumQuery>(ActiveCamera);

            context.RenderPass = "Default";
            _renderer.Render(query.SceneNodes, context);

            context.RenderPass = null;
            context.CameraNode = null;
            context.Scene = null;

            base.OnRender(context);
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                GraphicsService.Screens.Clear();
                Scene.Dispose();
            }
        }
    }
}
