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


namespace ICT309Game.Container
{
    class ModelContainer
    {
        public Model _model;
        public Pose _pose;
        public SkeletonPose _skeleton;
        public ITimeline[] _animations;
        
       

        public ModelContainer(Model modelpass, Pose posepass, SkeletonPose skeletonpass)
        {
            _model = modelpass;
            _pose = posepass;
            _skeleton = skeletonpass;


            var additionalData = (Dictionary<string,object>)_model.Tag;
            var animations = (Dictionary<string, SkeletonKeyFrameAnimation>)additionalData["Animations"];
            int index = 0;
            

            _animations = new ITimeline[animations.Count];
            foreach (var animation in animations.Values)
            {
                
                _animations[index] = new AnimationClip<SkeletonPose>(animations["First"])
                {
                    LoopBehavior = LoopBehavior.Cycle,
                    Duration = TimeSpan.MaxValue
                };
                index++;
                
            }
            
           
        }

        /*public void animate()
        {
            var additionalData = (Dictionary<string, object>)_model.Tag;
            var animations = (Dictionary<string, SkeletonKeyFrameAnimation>)additionalData["Animations"];
            walkAnimation = animations.Values.First();

            // Wrap the walk animation in an animation clip that loops the animation forever.
            loopingAnimation = new AnimationClip<SkeletonPose>(walkAnimation)
            {
                LoopBehavior = LoopBehavior.Cycle,
                Duration = TimeSpan.MaxValue,
            };
            ServiceLocator.Current.GetInstance<IAnimationService>().StartAnimation(loopingAnimation, (IAnimatableProperty)_skeleton).AutoRecycle();
            //AnimationService.StartAnimation(loopingAnimation, (IAnimatableProperty)_skeleton).AutoRecycle();
        }*/


    }
}
