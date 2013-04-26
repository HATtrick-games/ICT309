using System.Collections.Generic;
using DigitalRune.Graphics;
using DigitalRune.Graphics.SceneGraph;


namespace ICT309Game.Graphics
{
    // A scene query (see documentation of ISceneQuery) which sorts the queried
    // nodes into lists as required by the DeferredLightingScreen.
    public class SceneQuery : ISceneQuery
    {
        public SceneNode ReferenceNode { get; private set; }

        // All scene nodes where IsRenderable is true (e.g. MeshNodes).
        public List<SceneNode> RenderableNodes { get; private set; }

        public List<LightNode> AmbientLights { get; private set; }
        public List<LightNode> DirectionalLights { get; private set; }
        public List<LightNode> PointLights { get; private set; }
        public List<LightNode> Spotlights { get; private set; }
        public List<LightNode> ProjectorLights { get; private set; }

        public List<SceneNode> LensFlareNodes { get; private set; }

        public List<SceneNode> FogNodes { get; private set; }


        public SceneQuery()
        {
            RenderableNodes = new List<SceneNode>();
            AmbientLights = new List<LightNode>();
            DirectionalLights = new List<LightNode>();
            PointLights = new List<LightNode>();
            Spotlights = new List<LightNode>();
            ProjectorLights = new List<LightNode>();
            LensFlareNodes = new List<SceneNode>();
            FogNodes = new List<SceneNode>();
        }


        public void Reset()
        {
            ReferenceNode = null;

            RenderableNodes.Clear();
            AmbientLights.Clear();
            DirectionalLights.Clear();
            PointLights.Clear();
            Spotlights.Clear();
            ProjectorLights.Clear();
            LensFlareNodes.Clear();
            FogNodes.Clear();
        }


        public void Set(SceneNode referenceNode, IList<SceneNode> nodes)
        {
            Reset();
            ReferenceNode = referenceNode;

            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];

                if (node is LightNode)
                {
                    var lightNode = node as LightNode;
                    if (lightNode.Light is AmbientLight)
                        AmbientLights.Add(lightNode);
                    else if (lightNode.Light is DirectionalLight)
                        DirectionalLights.Add(lightNode);
                    else if (lightNode.Light is PointLight)
                        PointLights.Add(lightNode);
                    else if (lightNode.Light is Spotlight)
                        Spotlights.Add(lightNode);
                    else if (lightNode.Light is ProjectorLight)
                        ProjectorLights.Add(lightNode);
                }
                else if (node is LensFlareNode)
                {
                    LensFlareNodes.Add(node);
                }
                else if (node is FogNode)
                {
                    FogNodes.Add(node);
                }
                else if (node.IsRenderable)
                {
                    RenderableNodes.Add(node);
                }
            }
        }
    }
}
